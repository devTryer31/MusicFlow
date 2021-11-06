using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using MusicFlow.Domain.Entities.Base;

namespace MusicFlow.Domain.Entities
{
	public class Chat : Entity
	{
		public long TelegramChatId { get; set; }

		public string Token { get; private set; }

		public long? HostUserId { get; private set; }

		public string RefreshToken { get; private set; }

		public int? ExpiresIn { get; private set; }

		[Column(TypeName = "long")]
		public DateTime? CreatedAt { get; private set; }

		[NotMapped] 
		public bool IsHostEstablished { get; private set; }

		public void RemoveHost()
		{
			HostUserId = null;
			RefreshToken = null;
			ExpiresIn = null;
			Token = null;
			IsHostEstablished = false;
		}

		public void SetHost(long? hostUserID, string refreshToken, int? expiresIn, string token, DateTime? createdAt)
		{
			HostUserId = hostUserID;
			RefreshToken = refreshToken;
			ExpiresIn = expiresIn;
			Token = token;
			CreatedAt = createdAt;
			IsHostEstablished = true;
		}

		public void UpdateHost(string refreshToken, int? expiresIn, string token, DateTime? updatedAt)
		{
			if (!IsHostEstablished)
				throw new InvalidOperationException("The host cannot be updated if it is not set.");
			RefreshToken = refreshToken;
			ExpiresIn = expiresIn;
			Token = token;
			CreatedAt = updatedAt;
		}
	}
}
