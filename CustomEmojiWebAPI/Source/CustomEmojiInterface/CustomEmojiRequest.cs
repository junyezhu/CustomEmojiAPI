namespace CustomEmojiWebAPI.Source.CustomEmojiInterface
{
    using System;

    /// <summary>
    /// Custom Emoji Response
    /// </summary>
    public class CustomEmojiRequest
    {
        /// <summary>
        /// Custom Emoji Guid
        /// </summary>
        public string emoji_id { get; set; }

        /// <summary>
        /// Custom Emoji Name
        /// </summary>
        public string emoji_name { get; set; }

        /// <summary>
        /// Custom Emoji Display Name
        /// </summary>
        public string emoji_display_name { get; set; }

        /// <summary>
        /// Custom Emoji Author's user id
        /// </summary>
        public string emoji_autho_userId { get; set; }

        /// <summary>
        /// Custom Emoji Description
        /// </summary>
        public string emoji_description { get; set; }

        /// <summary>
        /// Custom Emoji State, Public/Private/Inactive
        /// </summary>
        public CustomEmojiEntryState emoji_state { get; set; }

        /// <summary>
        /// Custom Emoji content in base64 string
        /// </summary>
        public string emoji_base64_content { get; set; }
    }
}
