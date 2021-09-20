using System;
using System.Text.RegularExpressions;

namespace MusicBot
{
    public static class LinkHandler
    {
        private const string PatternSpotify = @"https:\/\/open\.spotify\.com\/track\/(.{22})\?";
        
        public static bool HandleSpotifyLink(string url, out string link)
        {
            var match  = Regex.Match(url, PatternSpotify);
            if (match.Success)
            {
                link = match.Groups[1].Value;
                return true;
            }

            link = string.Empty;
            return false;
            
        }
        
    }
}