using System.Collections.Generic;
using SpotifyAPI.Web;

namespace MusicBotV2.Data
{
    public static class InMemoryDatabaseTest //TODO: delete this.
    {
	    public static Dictionary<long, SpotifyClient> Data { get; set; } = new();
    }
}
