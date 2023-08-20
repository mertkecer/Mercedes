using Mercedes.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mercedes.API.Data
{
    public class DataStore
    {
        private readonly List<ShortenedUrl> _urls = new List<ShortenedUrl>();

        public ShortenedUrl GetByShortUrl(string shortUrl)
        {
            return _urls.FirstOrDefault(url => url.ShortUrl == shortUrl);
        }

        public ShortenedUrl Add(ShortenedUrl shortenedUrl)
        {
            _urls.Add(shortenedUrl);
            return shortenedUrl;
        }
    }
}
