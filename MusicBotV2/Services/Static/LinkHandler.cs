using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json.Linq;
using SpotifyAPI.Web;

namespace MusicBotV2.Services.Static
{
    public static class LinkHandler
    {
        private const string PatternSpotify = @"https:\/\/open\.spotify\.com\/track\/(.{22})\?";
        private const string PatternAppleMusic = @"https:\/\/music\.apple\.com\/.+album\/.+\?i=(\d{9})";
        private const string PatternIFramelySongInfo = @"\<(.+)\>.?\((.+)\)";
        private const string SpotifyPrefix = "spotify:track:";


        /// <param name="url">Apple music or Spotify link or wrong.</param>
        /// <returns>Correct Spotify track Id or null if track not found.</returns>
        public static async Task<string> HandleLinkOrDefaultAsync(string url)
        {
            //If Spotify.
            var match = Regex.Match(url, PatternSpotify);
            if (match.Success)
            {
                var id = match.Groups[1].Value;

                try //Is track id existed?
                {
                    await SpotifyManager.VerifyClient.Tracks.Get(id).ConfigureAwait(false);
                }
                catch (Exception)
                {
                    return null;
                }

                return SpotifyPrefix + id;
            }

            //If Apple music
            match = Regex.Match(url, PatternAppleMusic);
            if (match.Success)
            {
                return AppleMusicToSpotifyId(url);
            }

            return null;
        }

        /// <param name="trackLink">Apple Music link(without changes) to the song</param>
        /// <returns>
        /// Spotify track id already with prefix,
        /// null if link is wrong you can't find song in the Spotify.
        /// </returns>
        private static string AppleMusicToSpotifyId(string trackLink)
        {
            // don't know is it necessarily
            trackLink = HttpUtility.UrlDecode(trackLink);

            const string tempIFLink = @"http://iframe.ly/api/oembed?url={URL}&api_key={API_KEY}";

            // create link for GET request to the iframe.ly
            var IFLink = tempIFLink
                .Replace("{URL}", trackLink)
                .Replace("{API_KEY}", ConfigurationService.IFAPIKey);

            // create GET request and transform it to JSON
            var request = WebRequest.Create(IFLink);
            var response = request.GetResponse();
            var sr = new StreamReader(response.GetResponseStream()!);
            var res = sr.ReadToEnd();
            var obj = JObject.Parse(res);

            // get track info from request
            string trackInfo = null;
            if (obj.ContainsKey("title"))
            {
                var title = obj["title"]!
                    .ToString()
                    .Replace("«", "<")
                    .Replace("»", ">");

                var match = Regex.Match(title, PatternIFramelySongInfo);
                if (match.Success)
                {
                    var songName = match.Groups[1].ToString();
                    var groupName = match.Groups[2].ToString();
                    trackInfo = $"{songName} {groupName}";

                    /* Search song in the Spotify
                     NOTICE: this method can return null */
                    return FindSongInSpotify(trackInfo);
                }
                else
                {
                    //iframe.ly return response that has "title" key,
                    //but it has wrong format, so can't search song in the spotify
                    return null;
                }
            }
            else
            {
                // Apple Music match the pattern but the link has wrong parameters (track id)
                return null;
            }
        }

        /// <summary>
        /// Find song in the spotify (by GET request to the Spotify API) by song name and artist
        /// </summary>
        /// <param name="songInfo">Song info contains song name and artist</param>
        /// <returns>
        /// Spotify track ID already with prefix,
        /// null if Spotify can't find this song
        /// </returns>
        private static string FindSongInSpotify(string songInfo)
        {
            var res =
                SpotifyManager.VerifyClient.Search.Item(new SearchRequest(SearchRequest.Types.Track, songInfo));

            return res.Result.Tracks.Items[0].Uri;
        }
    }
}