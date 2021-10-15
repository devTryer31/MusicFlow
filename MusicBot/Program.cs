using System;
using System.Threading;
using System.Threading.Tasks;

namespace MusicBot
{
    internal static class Program
    {
        private static async Task Main()
        {
            // load data from file
            Configuration.Initialize();
            

            #region Start Telegram Bot

            try
            {
                // bot launch
                Bot.StartBot();
                await SpotifyManager.Launch();
                Console.ReadKey();
                // bot stop
                Bot.StopBot();
                SpotifyManager.Stop();
            }
            catch (Exception e)
            {
                // printing error information to the console
                Console.WriteLine(e.Message);
            }

            #endregion
        }
    }
}