using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MusicBotV2.Services.Static
{
	public static class LinkHandler
	{
		private const string PatternSpotify = @"https:\/\/open\.spotify\.com\/track\/(.{22})\?";

		//TODO: Rebuild.
		private const string PatternAppleMusic = @"https:\/\/open\.spotify\.com\/track\/(.{22})\?";


		private const string SpotifyPrefix = "spotify:track:";
		/// <param name="url">Apple music or Spotify link or wrong.</param>
		/// <returns>Correct Spotify track Id or null if track not found.</returns>
		public static async Task<string> HandleLinkOrDefaultAsync(string url)
		{

			//If Spotify.
			var match = Regex.Match(url, PatternSpotify);
			if (match.Success) {

				var id = match.Groups[1].Value;

				try { //Is track id existed?
					await SpotifyManager.VerifyClient.Tracks.Get(id).ConfigureAwait(false);
				}
				catch (Exception) {
					//if (e.Message == "non existing id")
						return null;
				}

				return SpotifyPrefix + id;
			}

			//If Apple music
			match = Regex.Match(url, PatternAppleMusic);
			if (match.Success) {
				return SpotifyPrefix + _AppleMusicToSpotifyId(match.Groups[1].Value);
			}

			return null;
		}

		private static string _AppleMusicToSpotifyId(string trackName)
		{
			//TODO: Create converter.
			return string.Empty;
		}

	}
}