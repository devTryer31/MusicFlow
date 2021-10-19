using System.ComponentModel.DataAnnotations;
using MusicFlow.Domain.Entities.Base;

namespace MusicFlow.Domain.Entities
{
	public class Chat : Entity
	{
		public long ChatId { get; set; }
		
		public string Token { get; set; }
	}
}
