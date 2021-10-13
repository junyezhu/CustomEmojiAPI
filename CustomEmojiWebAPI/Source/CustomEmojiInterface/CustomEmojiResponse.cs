namespace CustomEmojiWebAPI.Source.CustomEmojiInterface
{
    using System;

    /// <summary>
    /// Custom Emoji Response
    /// </summary>
    public class CustomEmojiResponse
    {
        /// <summary>
        /// Custom Emoji Guid
        /// </summary>
        public string emoji_id { get; set; }

        /// <summary>
        /// Custom Emoji Display Name
        /// </summary>
        public string emoji_display_name { get; set; }

        /// <summary>
        /// Custom Emoji url
        /// </summary>
        public string emoji_url { get; set; }
    }
}
