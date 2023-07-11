using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;
using TestWebAPI.Model;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TestWebAPI.Controllers
{
    public class EmailController : Controller
    {
        private IMemoryCache _cache;
        private const string receiveCacheKey = "receiveCacheKey";

        public EmailController(IMemoryCache cache)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }


        [HttpPost]
        [Route("Receive")]
        public async Task<ActionResult> Receive([FromBody] EmailEntity email)
        {
            try
            {
                if (!ValidateUsingEmailAddressAttribute(email.Emailaddress))
                    return BadRequest("Email not valid");

                if (_cache.TryGetValue(receiveCacheKey, out EmailEntity emailEntity))
                {
                    var secondsExp = System.Math.Abs((email.DateReceive - emailEntity.DateReceive).TotalSeconds);
                    if (secondsExp < 3)
                        return new StatusCodeResult(429);
                    AddToCache(email);
                }
                else
                    emailEntity = AddToCache(email);


            }
            catch (Exception)
            {
                return new StatusCodeResult(429);
            }

            return Ok(DateTime.Now);
        }
        //Need to create extantion helper class class for 
        private EmailEntity AddToCache(EmailEntity email)
        {
            EmailEntity emailEntity = email;
            var cachemailEntity = emailEntity;
            var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromSeconds(60))
            .SetAbsoluteExpiration(TimeSpan.FromSeconds(3600))
            .SetPriority(CacheItemPriority.Normal)
            .SetSize(1024);
            _cache.Set(receiveCacheKey, cachemailEntity, cacheEntryOptions);
            return emailEntity;
        }
        public bool ValidateUsingEmailAddressAttribute(string emailAddress)
        {
            var emailValidation = new EmailAddressAttribute();
            return emailValidation.IsValid(emailAddress);
        }
    }
}
