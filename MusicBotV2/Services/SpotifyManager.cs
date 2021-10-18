using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MusicBotV2.Data;
using MusicBotV2.Services.Interfaces;
using MusicBotV2.Services.Static;
using SpotifyAPI.Web;
using static SpotifyAPI.Web.Scopes;

namespace MusicBotV2.Services
{
	public class SpotifyManager : IMusicService
	{
		public static SpotifyClient VerifyClient { get; } = new(
			SpotifyClientConfig
				.CreateDefault()
				.WithAuthenticator(
					new ClientCredentialsAuthenticator(
						ConfigurationService.ClientID!,
						ConfigurationService.ClientSecret!)
				)
		);


		public async Task AuthenticationFromRawTokenAsync(string rawToken, long chatId)
		{
			//var (verifier, challenge) = PKCEUtil.GenerateCodes();

			PKCETokenResponse token = await new OAuthClient().RequestToken(
				new PKCETokenRequest(ConfigurationService.ClientID!, rawToken, ConfigurationService.ServerAuthUri, _verifier.verifier)
			);

			var authenticator = new PKCEAuthenticator(ConfigurationService.ClientID!, token!);
			//authenticator.TokenRefreshed += (sender, t) =>
			//	token = t; // TODO: database adding.

			var config = SpotifyClientConfig.CreateDefault()
				.WithAuthenticator(authenticator);

			var spotify = new SpotifyClient(config);

			InMemoryDatabaseTest.Data.Add(chatId, spotify); //TODO: move it to database reflection.
		}

		public async Task<bool> AddToQueueAsync(string correctMusicLink, long chatId)
		{
			var db = InMemoryDatabaseTest.Data;
			if (!db.ContainsKey(chatId))
				return false;

			var spotify = InMemoryDatabaseTest.Data[chatId];

			return await spotify.Player.AddToQueue(new PlayerAddToQueueRequest(correctMusicLink));
		}

		private static (string verifier, string challenge) _verifier;

		public static string BuildAuthenticationLink(long chatId)
		{
			_verifier = PKCEUtil.GenerateCodes();

			var request = new LoginRequest(ConfigurationService.ServerAuthUri,
				ConfigurationService.ClientID!,
				LoginRequest.ResponseType.Code)
			{
				CodeChallenge = _verifier.challenge,
				CodeChallengeMethod = "S256",
				Scope = new List<string> { UserModifyPlaybackState },
				State = chatId.ToString()
			};
			
			
			//return SpotifyManager.AuthenticationLink + $"&state={chat_id}";
			return request.ToUri().ToString();
		}
	}
}
