using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using MoviesApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi.Controllers
{
    [ApiController]
    [Route("api/security")]
    public class SecurityController : ControllerBase
    {
        private readonly IDataProtector _protector;
        private readonly HashService _hashService;

        public SecurityController(IDataProtectionProvider protectionProvider, HashService hashService)
        {
            _protector = protectionProvider.CreateProtector("value_secret_and_unique");
            _hashService = hashService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            string plainText = "Elliot Beckett";
            string encryptedText = _protector.Protect(plainText);
            string decryptedText = _protector.Unprotect(encryptedText);

            return Ok(new { plainText, encryptedText, decryptedText });
        }

        [HttpGet("TimeBound")]
        // Add encyrption that can only be decrypted before a certain time limit. e.g This text can only be decrypted for 5 seconds after creation.
        public async Task<IActionResult> GetTimeBound()
        {
            var protectorTimeBound = _protector.ToTimeLimitedDataProtector();
            string plainText = "Elliot Beckett";
            string encryptedText = protectorTimeBound.Protect(plainText, lifetime: TimeSpan.FromSeconds(5));

            await Task.Delay(6000);

            string decryptedText = protectorTimeBound.Unprotect(encryptedText);

            return Ok(new { plainText, encryptedText, decryptedText });
        }

        [HttpGet("Hash")]
        public IActionResult GetHash()
        {
            var plainText = "Elliot Beckett";

            var hashResult1 = _hashService.Hash(plainText);
            var hashResult2 = _hashService.Hash(plainText);

            return Ok(new { plainText, hashResult1, hashResult2 });
        }
    }
}