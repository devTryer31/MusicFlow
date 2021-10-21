using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using MusicFlow.Domain.Entities.Base;

namespace MusicFlow.Domain.Entities
{
	public class Chat : Entity
	{
		public long TelegramChatId { get; set; }
		
		public string Token { get; set; }

		public long? HostUserId { get; set; }

		public string RefreshToken { get; set; }
	}
}
