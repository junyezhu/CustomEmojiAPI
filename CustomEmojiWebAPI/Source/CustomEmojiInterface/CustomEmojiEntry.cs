namespace CustomEmojiWebAPI.Source.CustomEmojiInterface
{
    using System;

    /// <summary>
    /// Custom Emoji Entry
    /// </summary>
    public class CustomEmojiEntry
    {
        /// <summary>
        /// Custom Emoji Guid
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Custom Emoji Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Custom Emoji Display Name
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Custom Emoji Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The user Guid of Custom Emoji's author
        /// </summary>
        public string AuthorUserObjectId { get; set; }

        /// <summary>
        /// Entry's ingested timestamp
        /// </summary>
        public DateTime IngestedTime { get; set; }

        /// <summary>
        /// Custom Emoji State, Public/Private/Inactive
        /// </summary>
        public CustomEmojiEntryState State { get; set; }

        /// <summary>
        /// Custom Emoji url
        /// </summary>
        public string EmojiUrl { get; set; }
    }
}
