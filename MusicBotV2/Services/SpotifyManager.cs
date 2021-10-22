using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MusicBotV2.Services.Interfaces;
using MusicBotV2.Services.Static;
using MusicFlow.DAL.Context;
using MusicFlow.Domain.Entities;
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

		public SpotifyManager(MusicFlowDb db)
		{
			_Db = db;
		}

		public async Task AuthenticationFromRawTokenAsync(string rawToken, long chatId, long hostId)
		{
			PKCETokenResponse response = await new OAuthClient().RequestToken(
				new PKCETokenRequest(ConfigurationService.ClientID!, rawToken, ConfigurationService.ServerAuthUri, _verifier.verifier)
			).ConfigureAwait(false);
			//var authenticator = new PKCEAuthenticator(ConfigurationService.ClientID!, token!);
			//authenticator.TokenRefreshed += (sender, t) =>
			//	token = t; // TODO: database adding.

			//var config = SpotifyClientConfig.CreateDefault()
			//	.WithAuthenticator(authenticator);

			//var spotify = new SpotifyClient(config);
			var chat = await _Db.Chats.FirstOrDefaultAsync(ch => ch.TelegramChatId == chatId);
			if (chat is null)
			{
				await _Db.Chats.AddAsync(new Chat
				{
					TelegramChatId = chatId,
					Token = response.AccessToken,
					HostUserId = hostId,
					RefreshToken = response.RefreshToken
				}).ConfigureAwait(false);
			}
			else
			{
				chat.HostUserId = hostId;
				chat.Token = response.AccessToken;
				chat.RefreshToken = response.RefreshToken;
				_Db.Chats.Update(chat);
			}
			
			await _Db.SaveChangesAsync();
		}
		
		/// <returns>Return null if thrown APIException with message "Player command failed: No active device found"</returns>
		public async Task<bool?> AddToQueueAsync(string correctMusicLink, long chatId)
		{
			var chat = await GetChatWithUpdatedTokenAsync(chatId).ConfigureAwait(false);
			if (chat?.HostUserId is null)
				return false;

			var spotify = new SpotifyClient(chat.Token);
			try
			{
				return await spotify.Player.AddToQueue(new PlayerAddToQueueRequest(correctMusicLink));
			}
			catch (APIException e)
			{
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
				LoginRequest.ResponseType.Code)
			{
				CodeChallenge = _verifier.challenge,
				CodeChallengeMethod = "S256",
				Scope = new List<string> { UserModifyPlaybackState },
				State = chatId + "+" + hostId
			};
			
			
			//return SpotifyManager.AuthenticationLink + $"&state={chat_id}";
			return request.ToUri().ToString();
		}

		/// <param name="chatId">Telegram chat id</param>
		/// <returns>Founded chat if it has a null fields. Null if not found. Chat with updated host spotify token.</returns>
		public async Task<Chat> GetChatWithUpdatedTokenAsync(long chatId)
		{
			var chat = await _Db.Chats.FirstOrDefaultAsync(ch => ch.TelegramChatId == chatId)
				.ConfigureAwait(false);
			
			if (chat?.HostUserId is null || string.IsNullOrWhiteSpace(chat.Token) || string.IsNullOrWhiteSpace(chat.RefreshToken))
				return chat;

			//TODO:Need update token.
			//var response = await new OAuthClient().RequestToken(
			//	new PKCETokenRefreshRequest(ConfigurationService.ClientID!, chat.RefreshToken)
			//);
			
			//if(response.IsExpired)
			//	chat.Token = response.AccessToken;

			//_Db.Chats.Update(chat);
			//await _Db.SaveChangesAsync();

			return chat;
		}
	}
}
