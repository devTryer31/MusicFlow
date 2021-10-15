using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpotifyAPI.Web;

namespace MusicBotV2.Data
{
    public static class InMemoryDatabaseTest //TODO: delete this.
    {
	    public static Dictionary<string, SpotifyClient> Data { get; set; }
    }
}
