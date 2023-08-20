using Mercedes.API.Data;
using Mercedes.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Mercedes.API.Controllers
{
    
    [ApiController]
    public class UrlController : ControllerBase
    {
        private readonly DataStore _urlStore;

        public UrlController(DataStore urlStore)
        {
            _urlStore = urlStore;
        }

        [HttpPost]
        [Route("api/[controller]/ShortenUrl")]
        public IActionResult ShortenUrl(ShortenedUrl shortenedUrl)
        {
            if (!IsValidUrl(shortenedUrl.OriginalUrl))
            {
                return BadRequest("Invalid URL format");
            }

            Uri uri = new Uri(shortenedUrl.OriginalUrl);
            string domain = $"{uri.Scheme}://{uri.Host}";

            string hash = GenerateShortUrlHash();

            string shortUrl = $"{domain}/{hash}/";

            shortenedUrl.ShortUrl = shortUrl;

            _urlStore.Add(shortenedUrl);

            return Ok(shortenedUrl.ShortUrl);
        }

        [HttpGet]
        [Route("api/[controller]/RedirectShortUrl")]
        public IActionResult RedirectShortUrl(string url)
        {
            var shortenedUrl = _urlStore.GetByShortUrl(url);

            if (shortenedUrl == null)
            {
                return NotFound();
            }

            return Ok(shortenedUrl.OriginalUrl);
        }

        private string GenerateShortUrlHash()
        {
            using (var md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()));
                return BitConverter.ToString(hash).Replace("-", "").Substring(0, 6);
            }
        }

        private bool IsValidUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return false;
            }

            string pattern = @"^(https?|ftp)://[^\s/$.?#].[^\s]*$";
            return Regex.IsMatch(url, pattern, RegexOptions.IgnoreCase);
        }
    }
}
