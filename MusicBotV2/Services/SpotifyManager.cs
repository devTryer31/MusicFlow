using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MusicBotV2.Services.Interfaces;
using MusicBotV2.Services.Loggers;
using MusicBotV2.Services.Static;
using MusicFlow.DAL.Context;
using MusicFlow.Domain.Entities;
using SpotifyAPI.Web;
using static SpotifyAPI.Web.Scopes;

namespace MusicBotV2.Services
{
	public class SpotifyManager : IMusicService
	{
		private static readonly SpotifyClientConfig _DefaultSpotifyConfig = SpotifyClientConfig.CreateDefault();//.WithHTTPLogger(new SpotifyHTTPLogger());

		public static SpotifyClient VerifyClient { get; } = new(
			_DefaultSpotifyConfig
				.WithAuthenticator(
					new ClientCredentialsAuthenticator(
						ConfigurationService.ClientID!,
						ConfigurationService.ClientSecret!)
				)
		);

		public SpotifyManager(MusicFlowDb db)
		{
			_Db = db;
		}

		/// <summary> PKCE Spotify is used. </summary>
		/// <param name="rawToken">Raw token from Spotify authentication.</param>
		/// <param name="chatId">Telegram chat id.</param>
		/// <param name="hostId">Telegram chat member id for host definition.</param>
		/// <returns>True if authentication passed else false.</returns>
		public async Task<bool> AuthenticationFromRawTokenAsync(string rawToken, long chatId, long hostId)
		{
			var chat = await _Db.Chats.FirstOrDefaultAsync(ch => ch.TelegramChatId == chatId);

			if (chat is not null && chat.IsHostEstablished)
				return false;

			PKCETokenResponse response = await new OAuthClient().RequestToken(
				new PKCETokenRequest(ConfigurationService.ClientID!, rawToken, ConfigurationService.ServerAuthUri, _verifier.verifier)
			).ConfigureAwait(false);

			if (chat is null) {//Create new db item.
				chat = new Chat {
					TelegramChatId = chatId,
				};
				chat.SetHost(hostId, response.RefreshToken, response.ExpiresIn, response.AccessToken, response.CreatedAt);
				await _Db.Chats.AddAsync(chat).ConfigureAwait(false);
			}
			else {//If db item exist, but host unset.
				chat.SetHost(hostId, response.RefreshToken, response.ExpiresIn, response.AccessToken, response.CreatedAt);
				_Db.Chats.Update(chat);
			}

			await _Db.SaveChangesAsync();
			return true;
		}

		/// <returns>Return null if thrown APIException with message "Player command failed: No active device found"</returns>
		public async Task<bool?> AddToQueueAsync(string correctMusicLink, long chatId)
		{
			var chat = await GetChatWithUpdatedTokenAsync(chatId).ConfigureAwait(false);
			if (chat?.HostUserId is null)
				return false;

			var spotify = new SpotifyClient(_DefaultSpotifyConfig.WithToken(chat.Token));
			try {
				return await spotify.Player.AddToQueue(new PlayerAddToQueueRequest(correctMusicLink));
			}
			catch (APIException e) {
				if (e.Message == "Player command failed: No active device found")
					return null;
			}

			return false;
		}

		private static (string verifier, string challenge) _verifier;
		private readonly MusicFlowDb _Db;

		public static string BuildAuthenticationLink(long chatId, long hostId)
		{
			_verifier = PKCEUtil.GenerateCodes();

			var request = new LoginRequest(ConfigurationService.ServerAuthUri,
				ConfigurationService.ClientID!,
				LoginRequest.ResponseType.Code) {
				CodeChallenge = _verifier.challenge,
				CodeChallengeMethod = "S256",
				Scope = new List<string> { UserModifyPlaybackState },
				State = chatId + "+" + hostId
			};
			
			return request.ToUri().ToString();
		}

		/// <param name="chatId">Telegram chat id</param>
		/// <returns>Founded chat if it has a null fields. Null if not found. Chat with updated host spotify token.</returns>
		public async Task<Chat> GetChatWithUpdatedTokenAsync(long chatId)
		{
			var chat = await _Db.Chats.FirstOrDefaultAsync(ch => ch.TelegramChatId == chatId)
				.ConfigureAwait(false);

			if (chat is null || !chat.IsHostEstablished)
				return chat;

			if (chat.CreatedAt!.Value.AddSeconds(chat.ExpiresIn!.Value) > DateTime.UtcNow)
				return chat;

			var response = await new OAuthClient().RequestToken(
				new PKCETokenRefreshRequest(ConfigurationService.ClientID!, chat.RefreshToken)
			);

			chat.UpdateHost(response.RefreshToken, response.ExpiresIn, response.AccessToken, response.CreatedAt);

			_Db.Chats.Update(chat);
			await _Db.SaveChangesAsync();

			return chat;
		}
	}
}
