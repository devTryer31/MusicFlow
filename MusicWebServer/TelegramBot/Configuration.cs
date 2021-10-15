using System;
using System.IO;

namespace MusicBot
{
    public static class Configuration
    {
        private static string? _token;
        private static string? _clientID;
        private const string FileName = "config.txt";

        public static string? Token
        {
            get
            {
                if (_token == null)
                {
                    throw new ArgumentNullException(Token);
                }

                return _token;
            }
            private set => _token = value;
        }
        public static string? ClientID
        {
            get
            {
                if (_clientID == null)
                {
                    throw new ArgumentNullException(ClientID);
                }

                return _clientID;
            }
            
            private set => _clientID = value;
        }
        
        public static void Initialize()
        {
            string? token = null, clientID = null;
            if (File.Exists(FileName))
            {
                // read from file
                using StreamReader sr = new StreamReader(FileName);
                token = sr.ReadLine();
                clientID = sr.ReadLine();
                sr.Close();

                if (token != null && clientID != null)
                {
                    Token = token;
                    ClientID = clientID;
                    return;
                }

            }

            // manual input
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("File not found or values are missing");
            Console.ResetColor();

            while (token == null || clientID == null)
            {
                Console.WriteLine("Enter your Telegram Bot API key:");
                token = Console.ReadLine();

                Console.WriteLine("Enter your Spotify App client ID:");
                clientID = Console.ReadLine();
            }

            Token = token;
            ClientID = clientID;
            SaveToFile();
        }
        private static void SaveToFile()
        {
            using StreamWriter sw = new StreamWriter(FileName);
            sw.WriteLine(Token);
            sw.WriteLine(ClientID);
            sw.Close();
        }
    }
}