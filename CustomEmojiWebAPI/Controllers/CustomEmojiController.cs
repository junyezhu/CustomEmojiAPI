namespace CustomEmojiWebAPI.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using CustomEmojiWebAPI.Source.CustomEmojiInterface;
    using CustomEmojiWebAPI.Source.Provider;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("[controller]")]
    public class CustomEmojiController : ControllerBase
    {
        private ICustomEmojiProvider provider = new CustomEmojiProvider();

        [HttpGet]
        [OverrideResponseHeader]
        [EnableCors]
        public async Task<IList<CustomEmojiResponse>> GetPublicCustomEmojis()
        {
            return await this.provider.GetPublicCustomEmojiEntries();
        }

        [HttpGet]
        [Route("{userId}")]
        [OverrideResponseHeader]
        [EnableCors]
        public async Task<IList<CustomEmojiResponse>> GetCustomEmojisByUserId([FromRoute] string userId)
        {
            return await this.provider.GetCustomEmojiEntriesByUserId(userId);
        }

        [HttpPost]
        [OverrideResponseHeader]
        [EnableCors]
        public async Task CreateCustomEmoji([FromBody] CustomEmojiRequest emoji)
        {
            await this.provider.TryAddCustomEmojiEntry(emoji);
        }

        [HttpPut]
        [OverrideResponseHeader]
        [EnableCors]
        public async Task UpdateCustomEmoji([FromBody] CustomEmojiRequest emoji)
        {
            await this.provider.TryAddOrUpdateCustomEmojiEntry(emoji);
        }

        [HttpDelete]
        [Route("{userId}/customEmoji/{emojiId}")]
        [OverrideResponseHeader]
        [EnableCors]
        public async Task DeleteAppEntitlement([FromRoute] string userId, [FromRoute] string emojiId)
        {
            await this.provider.TryInactiveExistingCustomEmojiEntry(emojiId, userId);
        }
    }
}
