namespace CustomEmojiWebAPI.Source.Provider
{
    using CustomEmojiWebAPI.Source.CustomEmojiInterface;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    interface ICustomEmojiProvider
    {
        /// <summary>
        /// Try to add new custom emoji
        /// </summary>
        /// <param name="entry">custom emoji entry</param>
        /// <returns>true if emoji added successfully, otherwise false</returns>
        public Task<bool> TryAddCustomEmojiEntry(CustomEmojiRequest entry);

        /// <summary>
        /// Try to add new custom emoji or update existing emoji entry if entry Id is already existing in database
        /// </summary>
        /// <param name="entry">custom emoji entry</param>
        /// <returns>true if emoji updated successfully, otherwise false</returns>
        public Task<bool> TryAddOrUpdateCustomEmojiEntry(CustomEmojiRequest entry);

        /// <summary>
        /// Try to inactive existing emoji entry
        /// </summary>
        /// <param name="id">id of emoji to be inactived</param>
        /// <param name="userId">user id of this action</param>
        /// <returns>true if emoji state is updated successfully, otherwise false</returns>
        public Task<bool> TryInactiveExistingCustomEmojiEntry(string id, string userId);

        /// <summary>
        /// Get list of CustomEmojiEntry with State Public
        /// </summary>
        /// <returns>list of CustomEmojiEntry</returns>
        public Task<IList<CustomEmojiResponse>> GetPublicCustomEmojiEntries();

        /// <summary>
        /// Get list of CustomEmojiEntry including public custom emoji or prive emoji which only can accessed by input user
        /// </summary>
        /// <param name="userId">user id</param>
        /// <returns>list of CustomEmojiEntry</returns>
        public Task<IList<CustomEmojiResponse>> GetCustomEmojiEntriesByUserId(string userId);
    }
}
