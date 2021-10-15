using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicBotV2.Services.Interfaces
{
	public interface IMusicService
	{
		Task AuthenticationFromRawTokenAsync(string rawToken, string chatId);

		Task<bool> AddToQueueAsync(string correctMusicLink, string chatId);
	}
}
