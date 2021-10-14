namespace CustomEmojiWebAPI.Source.Provider
{
    using Azure.Storage.Blobs;
    using Azure.Storage.Blobs.Specialized;
    using CustomEmojiWebAPI.Source.CustomEmojiInterface;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    public class CustomEmojiProvider : ICustomEmojiProvider
    {
        private const string databaseName = "CustomEmojiDatabase";
        private const string tableName = "CustomEmojiEntryTable";
        private static string xmlFilepath = "EmojiDatabase.xml";
        private static string imgFolderpath = "Emojis/";
        private readonly PropertyInfo[] customEmojiEntryProperties = typeof(CustomEmojiEntry).GetProperties();
        private DataSet dataSet;
        private DataTable emojiEntriesTable;
        private BlobContainerClient containerClient;
        private BlobClient blobClient;

        private const string connectionString = "DefaultEndpointsProtocol=https;AccountName=juztest;AccountKey=IAB7nS0LUzkHFFALHXqMGTM7h/F3hX2syL3oajniwTTkNX42ul0eYIYntq27fAdBnjd7s//Pg/XJbnCaVj5Row==;EndpointSuffix=core.windows.net";

        public CustomEmojiProvider()
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            containerClient = blobServiceClient.GetBlobContainerClient("juztest");
            blobClient = containerClient.GetBlobClient(xmlFilepath);
            dataSet = null;
        }

        ///<inheritdoc/>
        public async Task<bool> TryAddCustomEmojiEntry(CustomEmojiRequest request)
        {
            if (dataSet == null)
            {
                await InitializeDataSet();
            }

            if (string.IsNullOrEmpty(request?.emoji_id))
            {
                // Log input entry Id can't be empty.
                return false;
            }

            if (emojiEntriesTable.AsEnumerable().Any(row => string.Equals(row.Field<String>("Id"), request.emoji_id, StringComparison.OrdinalIgnoreCase)))
            {
                // Log entry with same Id already exists.
                return false;
            }

            try
            {
                var entry = await GetCustomEmojiEntryAsync(request);
                AddCustomEmojiEntryIntoTable(entry);

                emojiEntriesTable.AcceptChanges();
                dataSet.AcceptChanges();
                var blockBlobClient = containerClient.GetBlockBlobClient(xmlFilepath);
                dataSet.WriteXml(await blockBlobClient.OpenWriteAsync(true));
                return true;
            }
            catch (Exception)
            {
                // Log exception
                return false;
            }
        }

        ///<inheritdoc/>
        public async Task<bool> TryAddOrUpdateCustomEmojiEntry(CustomEmojiRequest request)
        {
            if (dataSet == null)
            {
                await InitializeDataSet();
            }

            if (string.IsNullOrEmpty(request?.emoji_id))
            {
                // Log input entry Id can't be empty.
                return false;
            }

            try
            {
                var entry = await GetCustomEmojiEntryAsync(request);
                var row = emojiEntriesTable.AsEnumerable().FirstOrDefault(row => string.Equals(row.Field<String>("Id"), entry.Id, StringComparison.OrdinalIgnoreCase));
                if (row == null)
                {
                    AddCustomEmojiEntryIntoTable(entry);
                }
                else
                {
                    UpdateCustomEmojiEntry(row, entry);
                }

                emojiEntriesTable.AcceptChanges();
                dataSet.AcceptChanges();
                var blockBlobClient = containerClient.GetBlockBlobClient(xmlFilepath);
                dataSet.WriteXml(await blockBlobClient.OpenWriteAsync(true));
                return true;
            }
            catch (Exception)
            {
                // Log exception
                return false;
            }
        }

        ///<inheritdoc/>
        public async Task<IList<CustomEmojiResponse>> GetPublicCustomEmojiEntries()
        {
            if (dataSet == null)
            {
                await InitializeDataSet();
            }

            return GetCustomEmojiResponses(emojiEntriesTable?.AsEnumerable()?.Select(row => GetCustomEmojiEntry(row)).Where(entry => entry.State == CustomEmojiEntryState.Public));
        }

        ///<inheritdoc/>
        public async Task<IList<CustomEmojiResponse>> GetCustomEmojiEntriesByUserId(string userId)
        {
            if (dataSet == null)
            {
                await InitializeDataSet();
            }

            if (string.IsNullOrEmpty(userId))
            {
                throw new Exception($"Input user object id can't be empty.");
            }

            return GetCustomEmojiResponses(emojiEntriesTable?.AsEnumerable()?.Select(row => GetCustomEmojiEntry(row))
                .Where(entry => entry.State == CustomEmojiEntryState.Public || (entry.State == CustomEmojiEntryState.Private && string.Equals(entry.AuthorUserObjectId, userId, StringComparison.OrdinalIgnoreCase))));
        }

        ///<inheritdoc/>
        public async Task<bool> TryInactiveExistingCustomEmojiEntry(string id, string userId)
        {
            if (dataSet == null)
            {
                await InitializeDataSet();
            }

            if (string.IsNullOrEmpty(id))
            {
                // Log input emoji entry Id can't be empty.
                return false;
            }

            var entry = emojiEntriesTable.AsEnumerable().FirstOrDefault(row => string.Equals(row.Field<String>("Id"), id, StringComparison.OrdinalIgnoreCase));
            if (entry == null)
            {
                // Log unable to find exisiting record with Id.
                return false;
            }
            else if (!string.Equals((string)entry["AuthorUserObjectId"], userId, StringComparison.OrdinalIgnoreCase))
            {
                // Provided user doesn't have permission to inactive this custom emoji.
                return false;
            }

            try
            {
                entry.BeginEdit();
                entry["State"] = CustomEmojiEntryState.Inactive;
                entry.EndEdit();

                emojiEntriesTable.AcceptChanges();
                dataSet.AcceptChanges();
                var blockBlobClient = containerClient.GetBlockBlobClient(xmlFilepath);
                dataSet.WriteXml(await blockBlobClient.OpenWriteAsync(true));
                return true;
            }
            catch (Exception)
            {
                // Log exception
                return false;
            }
        }

        /// <summary>
        /// Load custom emoji entry data from Xml
        /// </summary>
        private async Task LoadDataFromXmlIntoDataSet()
        {
            if (!await blobClient.ExistsAsync())
            {
                // Log Failed to read xml file using path xmlFilepath, will create new xml file.
                return;
            }

            var response = await blobClient.DownloadAsync();
            using (var streamReader = new StreamReader(response.Value.Content))
            {
                dataSet.ReadXml(streamReader);
                emojiEntriesTable = dataSet.Tables[tableName];
            }
        }

        /// <summary>
        /// Initialize database
        /// </summary>
        private async Task InitializeDataSet()
        {
            dataSet = new DataSet(databaseName);
            emojiEntriesTable = new DataTable(tableName);
            foreach (PropertyInfo info in customEmojiEntryProperties)
            {
                emojiEntriesTable.Columns.Add(new DataColumn(info.Name, Nullable.GetUnderlyingType(info.PropertyType) ?? info.PropertyType));
            }

            dataSet.Tables.Add(emojiEntriesTable);
            dataSet.AcceptChanges();

            await LoadDataFromXmlIntoDataSet();
        }

        /// <summary>
        /// Insert new CustomEmojiEntry entry into emojiEntriesTable
        /// </summary>
        /// <param name="entry">CustomEmojiEntry entry to be inserted</param>
        private void AddCustomEmojiEntryIntoTable(CustomEmojiEntry entry)
        {
            object[] values = new object[customEmojiEntryProperties.Length];
            for (int i = 0; i < customEmojiEntryProperties.Length; i++)
            {
                values[i] = customEmojiEntryProperties[i].GetValue(entry);
            }

            emojiEntriesTable.Rows.Add(values);
        }

        /// <summary>
        /// Convert DataRow in emojiEntriesTable into CustomEmojiEntry object
        /// </summary>
        /// <param name="dr">DataRow in table emojiEntriesTable</param>
        /// <returns>CustomEmojiEntry object</returns>
        private CustomEmojiEntry GetCustomEmojiEntry(DataRow dr)
        {
            CustomEmojiEntry entry = new CustomEmojiEntry();

            foreach (DataColumn column in dr.Table.Columns)
            {
                PropertyInfo property = customEmojiEntryProperties.FirstOrDefault(pro => string.Equals(pro.Name, column.ColumnName, StringComparison.OrdinalIgnoreCase));
                if (property != null)
                    property.SetValue(entry, dr[column.ColumnName], null);
            }

            return entry;
        }

        /// <summary>
        /// Update exisiting DataRowin emojiEntriesTable with input CustomEmojiEntry object
        /// </summary>
        /// <param name="dr">DataRow in table emojiEntriesTable</param>
        /// <param name="entry">CustomEmojiEntry object</param>
        private void UpdateCustomEmojiEntry(DataRow dr, CustomEmojiEntry entry)
        {
            dr.BeginEdit();
            foreach (DataColumn column in dr.Table.Columns)
            {
                PropertyInfo property = customEmojiEntryProperties.FirstOrDefault(pro => string.Equals(pro.Name, column.ColumnName, StringComparison.OrdinalIgnoreCase));
                if (property != null)
                {
                    dr[column.ColumnName] = property.GetValue(entry);
                }
            }

            dr.EndEdit();
        }

        /// <summary>
        /// Upload input gif base64 string to Azure blob, e.g https://juztest.blob.core.windows.net/juztest/Emojis/089d1244-2474-44c6-9a71-96582add9ac9.gif
        /// </summary>
        /// <param name="emojiId">emoji id</param>
        /// <param name="base64">gif base64 string</param>
        /// <returns>azure blob Url</returns>
        private async Task<string> UploadImgToAzureBlobAsync(string emojiId, string base64)
        {
            byte[] bytes = Convert.FromBase64String(base64);
            MemoryStream ms = new MemoryStream(bytes);
            ms.Position = 0;
            string emojiUrl = imgFolderpath + $"{emojiId}.gif";
            var blockBlobClient = containerClient.GetBlockBlobClient(emojiUrl);
            await blockBlobClient.UploadAsync(ms);

            return "https://juztest.blob.core.windows.net/juztest/" + emojiUrl;
        }

        private IList<CustomEmojiResponse> GetCustomEmojiResponses(IEnumerable<CustomEmojiEntry> entries)
        {
            if (entries == null)
            {
                return null;
            }

            return entries.Select(e => new CustomEmojiResponse
            {
                emoji_id = e.Id,
                emoji_display_name = e.DisplayName,
                emoji_url = e.EmojiUrl
            }).ToList();
        }

        private async Task<CustomEmojiEntry> GetCustomEmojiEntryAsync(CustomEmojiRequest request)
        {
            if (request == null)
                return null;

            CustomEmojiEntry entry = new CustomEmojiEntry
            {
                Id = request.emoji_id,
                Name = request.emoji_name,
                DisplayName = request.emoji_display_name,
                Description = request.emoji_description,
                AuthorUserObjectId = request.emoji_autho_userId,
                State = request.emoji_state,
                IngestedTime = DateTime.UtcNow
            };

            entry.EmojiUrl = await UploadImgToAzureBlobAsync(entry.Id, request.emoji_base64_content);
            return entry;
        }
    }
}
