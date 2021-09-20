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
                Bot.StartBot();
                await SpotifyManager.Launch();
                Console.ReadKey();
                Bot.StopBot();
                SpotifyManager.Stop();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            #endregion
        }
    }
}