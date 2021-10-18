using System.Threading.Tasks;

namespace MusicBotV2.Services.Interfaces
{
	public interface IMusicService
	{
		Task AuthenticationFromRawTokenAsync(string rawToken, long chatId);

		Task<bool> AddToQueueAsync(string correctMusicLink, long chatId);
	}
}
