namespace CustomEmojiWebAPI.Source.CustomEmojiInterface
{
    /// <summary>
    /// State of Custom Emoji
    /// </summary>
    public enum CustomEmojiEntryState
    {
        /// <summary>
        /// Unknown
        /// </summary>
        Unknown,

        /// <summary>
        /// Public
        /// </summary>
        Public,

        /// <summary>
        /// Private, can only be used by the emoji's author
        /// </summary>
        Private,

        /// <summary>
        /// Inactive, will not be returned to user for selection
        /// </summary>
        Inactive
    }
}
