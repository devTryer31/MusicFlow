#nullable enable
using System;
using System.IO;
using System.Text.Json;

namespace MusicBotV2.Services.Static
{
    public static class ConfigurationService
    { 
	    public const string ServerUri = @"https://musicbotv220211102154604.azurewebsites.net";
        public const string ServerAuthStrUri = ServerUri + "/Authentication";
        public static readonly Uri ServerAuthUri = new(ServerAuthStrUri);
        
        private const string FileName = "config.json";
        private static Configuration? _configuration;

        #region Properties for secret data

        public static string? Token
        {
            get
            {
                if (_configuration?.Token == null)
                {
                    throw new ArgumentNullException(Token);
                }

                return _configuration.Token;
            }
        }

        public static string? ClientID
        {
            get
            {
                if (_configuration?.ClientId == null)
                {
                    throw new ArgumentNullException(ClientID);
                }

                return _configuration.ClientId;
            }
        }

        public static string? ClientSecret
        {
            get
            {
                if (_configuration?.ClientSecret is null)
                    throw new ArgumentNullException(ClientSecret);

                return _configuration.ClientSecret;
            }
        }

        public static string? IFAPIKey
        {
            get
            {
                if (_configuration?.IFAPIKey is null)
                    throw new ArgumentNullException(IFAPIKey);

                return _configuration.IFAPIKey;
            }
        }

        #endregion

        /// <summary>
        /// Load configuration data from json file or initiate manual input
        /// </summary>
        public static void Initialize()
        {
            if (File.Exists(FileName))
            {
                // read from file
                string data = File.ReadAllText(FileName);
                _configuration = JsonSerializer.Deserialize<Configuration>(data);

                if (_configuration is not null &&
                    _configuration.Token is not null &&
                    _configuration.ClientId is not null &&
                    _configuration.ClientSecret is not null &&
                    _configuration.IFAPIKey is not null)
                {
                    // If all fields are loaded — loading is successful
                    return;
                }
            }

            // manual input
            _configuration = InputConfigFromConsole();

            SaveToFile();
        }

        /// <summary>
        /// Read config data from console
        /// </summary>
        /// <returns>New configuration</returns>
        private static Configuration InputConfigFromConsole()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("File not found or values are missing");
            Console.ResetColor();

            string? token = null,
                clientID = null,
                clientSecret = null,
                ifApiKey = null;

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

            return new Configuration
            {
                Token = token,
                ClientId = clientID,
                ClientSecret = clientSecret,
                IFAPIKey = ifApiKey
            };
        }


        /// <summary>
        /// Save configuration data to json file
        /// </summary>
        private static void SaveToFile()
        {
            JsonSerializerOptions jsp = new()
            {
                WriteIndented = true
            };
            
            string data = JsonSerializer.Serialize(_configuration, jsp);
            File.WriteAllText(FileName, data);
        }
    }
}