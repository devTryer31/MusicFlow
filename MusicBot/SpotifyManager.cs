using System.IO;
using System.Threading.Tasks;
using System;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web;
using System.Collections.Generic;
using Newtonsoft.Json;
using static SpotifyAPI.Web.Scopes;


namespace MusicBot
{
    public static class SpotifyManager
    {
        private const string CredentialsPath = "credentials.json";
        private static readonly string? clientId = Configuration.ClientID;
        private static readonly EmbedIOAuthServer _server = new EmbedIOAuthServer(
            new Uri("http://localhost:5000/callback"), 5000);

        private static SpotifyClient _spotifyClient;

        private static void Exiting() => Console.CursorVisible = true;

        public static async Task Launch()
        {
            // This is a bug in the SWAN Logging library, need this hack to bring back the cursor
            AppDomain.CurrentDomain.ProcessExit += (sender, e) => Exiting();


            if (File.Exists(CredentialsPath))
            {
                await Start();
            }
            else
            {
                await StartAuthentication();
            }
        }
        
        private static async Task Start()
        {
            var json = await File.ReadAllTextAsync(CredentialsPath);
            var token = JsonConvert.DeserializeObject<PKCETokenResponse>(json);

            var authenticator = new PKCEAuthenticator(clientId!, token!);
            authenticator.TokenRefreshed += (sender, token) => 
                File.WriteAllText(CredentialsPath, JsonConvert.SerializeObject(token));

            var config = SpotifyClientConfig.CreateDefault()
                .WithAuthenticator(authenticator);

            var spotify = new SpotifyClient(config);
            _spotifyClient = spotify;

            var me = await spotify.UserProfile.Current();
            Console.WriteLine($"Welcome {me.DisplayName} ({me.Id}), you're authenticated!");
        }
        
        private static async Task StartAuthentication()
        {
            var (verifier, challenge) = PKCEUtil.GenerateCodes();

            await _server.Start();
            _server.AuthorizationCodeReceived += async (sender, response) =>
            {
                await _server.Stop();
                PKCETokenResponse token = await new OAuthClient().RequestToken(
                    new PKCETokenRequest(clientId!, response.Code, _server.BaseUri, verifier)
                );

                await File.WriteAllTextAsync(CredentialsPath, JsonConvert.SerializeObject(token));
                await Start();
            };

            var request = new LoginRequest(_server.BaseUri, clientId!, LoginRequest.ResponseType.Code)
            {
                CodeChallenge = challenge,
                CodeChallengeMethod = "S256",
                Scope = new List<string> { UserReadEmail, UserReadPrivate, PlaylistReadPrivate, PlaylistReadCollaborative, AppRemoteControl, Streaming }
            };

            Uri uri = request.ToUri();
            try
            {
                BrowserUtil.Open(uri);
            }
            catch (Exception)
            {
                Console.WriteLine("Unable to open URL, manually open: {0}", uri);
            }
        }

        public static async Task<bool> AddToQueue(string uri)
        {
            try
            {
                await _spotifyClient.Player.AddToQueue(new PlayerAddToQueueRequest(uri));
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return false;


        }

        public static void Stop()
        {
            _server.Dispose();
        }
    }
}