#nullable enable
using System;
using System.IO;

namespace MusicBotV2.Services.Static
{
    public static class ConfigurationService
    {
        public const string ServerAuthStrUri = ServerUri + "/Authentication";
        public static readonly Uri ServerAuthUri = new(ServerAuthStrUri);
        public const string ServerUri = "http://localhost:5000";
        private const string FileName = "config.txt";

        #region Fields for secret data

        private static string? _token;
        private static string? _clientID;
        private static string? _clientSecret;
        private static string? _IFAPIKey;

        #endregion

        #region Getters for secret data

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

        public static string? ClientSecret
        {
            get
            {
                if (_clientSecret is null)
                    throw new ArgumentNullException(_clientSecret);

                return _clientSecret;
            }

            private set => _clientSecret = value;
        }

        public static string? IFAPIKey
        {
            get
            {
                if (_IFAPIKey is null)
                    throw new ArgumentNullException(_IFAPIKey);

                return _IFAPIKey;
            }
            private set => _IFAPIKey = value;
        }

        #endregion

        public static void Initialize()
        {
            string? token = null,
                clientID = null,
                clientSecret = null,
                ifApiKey = null;

            if (File.Exists(FileName))
            {
                // read from file
                using StreamReader sr = new StreamReader(FileName);
                token = sr.ReadLine();
                clientID = sr.ReadLine();
                clientSecret = sr.ReadLine();
                ifApiKey = sr.ReadLine();
                sr.Close();

                if (token is not null &&
                    clientID is not null &&
                    clientSecret is not null &&
                    ifApiKey is not null)
                {
                    Token = token;
                    ClientID = clientID;
                    ClientSecret = clientSecret;
                    IFAPIKey = ifApiKey;

                    return;
                }
            }

            // manual input
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("File not found or values are missing");
            Console.ResetColor();

            while (token is null || clientID is null || clientSecret is null || ifApiKey is null)
            {
                Console.WriteLine("Enter your Telegram Bot API key:");
                token = Console.ReadLine();

                Console.WriteLine("Enter your Spotify App Client ID:");
                clientID = Console.ReadLine();

                Console.WriteLine("Enter your Spotify App Client Secret");
                clientSecret = Console.ReadLine();

                Console.WriteLine("Enter your iframe.ly account API key:");
                ifApiKey = Console.ReadLine();
            }

            Token = token;
            ClientID = clientID;
            ClientSecret = clientSecret;
            IFAPIKey = ifApiKey;
            SaveToFile();
        }

        private static void SaveToFile()
        {
            using StreamWriter sw = new(FileName);
            sw.WriteLine(Token);
            sw.WriteLine(ClientID);
            sw.WriteLine(ClientSecret);
            sw.Close();
        }
    }
}