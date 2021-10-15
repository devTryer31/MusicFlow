using System;
using System.Threading.Tasks;
using MusicBotV2.Data;
using MusicBotV2.Services.Interfaces;
using MusicBotV2.Services.Static;
using SpotifyAPI.Web;

namespace MusicBotV2.Services
{
	public class SpotifyManager : IMusicService
	{
		public const string AuthenticationLink = "Localhost:5000/";

		public static SpotifyClient VerifyClient { get; } = new(
			SpotifyClientConfig
				.CreateDefault()
				.WithAuthenticator(
					new ClientCredentialsAuthenticator(
						ConfigurationService.ClientID!,
						ConfigurationService.ClientSecret!)
				)
		);

		public async Task AuthenticationFromRawTokenAsync(string rawToken, string chatId)
		{
			var (verifier, _) = PKCEUtil.GenerateCodes();

			PKCETokenResponse token = await new OAuthClient().RequestToken(
				new PKCETokenRequest(ConfigurationService.ClientID!, "200", new Uri(ConfigurationService.ServerUri), verifier)
			);

			var authenticator = new PKCEAuthenticator(ConfigurationService.ClientID!, token!);
			//authenticator.TokenRefreshed += (sender, t) =>
			//	token = t; // TODO: database adding.

			var config = SpotifyClientConfig.CreateDefault()
				.WithAuthenticator(authenticator);

			var spotify = new SpotifyClient(config);

			InMemoryDatabaseTest.Data.Add(chatId, spotify); //TODO: move it to database reflection.
		}

		public async Task<bool> AddToQueueAsync(string correctMusicLink, string chatId)
		{
			var db = InMemoryDatabaseTest.Data;
			if (!db.ContainsKey(chatId))
				return false;

			var spotify = InMemoryDatabaseTest.Data[chatId];
			
			return await spotify.Player.AddToQueue(new PlayerAddToQueueRequest(correctMusicLink));
		}
	}
}
