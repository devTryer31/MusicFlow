using MusicBotV2.Services.Static;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    public class ConverterTests
    {
        
        [SetUp]
        public void Init()
        {
            ConfigurationService.Initialize();
        }
        
        
        [TestCase(@"https://open.spotify.com/track/1wsEluhFNYKR5YIBVqQO?si=3deb30f303d14b99", ExpectedResult = null)]
        [TestCase(@"https://oen.spotify.com/track/1wsEluhF9GNYKR5YIBVqQO?si=3deb30f303d14b99", ExpectedResult = null)]
        [TestCase(@"https://open.spoify.com/track/1wsEluhF9GNYKR5YIBVqQO?si=3deb30f303d14b99", ExpectedResult = null)]
        [TestCase(@"https://open.spotify.cm/track/1wsEluhF9GNYKR5YIBVqQO?si=3deb30f303d14b99", ExpectedResult = null)]
        [TestCase(@"https://open.spotify.com/tack/1wsEluhF9GNYKR5YIBVqQO?si=3deb30f303d14b99", ExpectedResult = null)]
        [TestCase(@"https://open.spotify.com/track/1wsEluhF9GNYK0000R5YIBVqQO?si=3deb30f303d14b99", ExpectedResult = null)]
        public string TestWrongSpotifyLink(string testLink)
        {
            return LinkHandler.HandleLinkOrDefaultAsync(testLink).Result;
        }
        
        [TestCase(@"https://open.spotify.com/track/1wsEluhF9GNYKR5YIBVqQO?si=3deb30f303d14b99", 
            ExpectedResult = "spotify:track:1wsEluhF9GNYKR5YIBVqQO")]
        [TestCase(@"https://open.spotify.com/track/5FfvntTwp2ndEOxv4bY6ng?si=bda43a4441a04023", 
            ExpectedResult = "spotify:track:5FfvntTwp2ndEOxv4bY6ng")]
        [TestCase(@"https://open.spotify.com/track/6xeECelOBEsSko9y9iVPsY?si=85vh9whMQ_uPYn4Zpd0RyQ&utm_source=native-share-menu&dl_branch=1",
            ExpectedResult = "spotify:track:6xeECelOBEsSko9y9iVPsY")]
        [TestCase(@"https://open.spotify.com/track/0P4ImoUrjOjtlz7dmWCp9g?si=OAYb5mG2QJKo1LXN7JGo8w&utm_source=native-share-menu",
            ExpectedResult = "spotify:track:0P4ImoUrjOjtlz7dmWCp9g")]
        public string TestCorrectSpotifyLinkWithCorrectID(string testLink)
        {
            return LinkHandler.HandleLinkOrDefaultAsync(testLink).Result;
        }
        
        [TestCase(@"https://open.spotify.com/track/1wsEluhF9GNYKR5YIBV000?si=3deb30f303d14b99", ExpectedResult = null)]
        [TestCase(@"https://open.spotify.com/track/0001wsEluhF9GNYKR5Y000?si=3deb30f303d14b99", ExpectedResult = null)]
        public string TestCorrectSpotifyLinkWithWrongID(string testLink)
        {
            return LinkHandler.HandleLinkOrDefaultAsync(testLink).Result;
        }

        [TestCase(@"https://music.apple.com/ru/album/someone-like-you/403037872?i=403037927",
            ExpectedResult = "spotify:track:1zwMYTA5nlNjZxYrvBB2pV")]
        public string TestCorrectAppleMusicLinkWithCorrectID(string testLink)
        {
            return LinkHandler.HandleLinkOrDefaultAsync(testLink).Result;
        }

        [TestCase(@"https://music.apple.com/ru/album/wrong-song-name/000000000?i=000000000", ExpectedResult = null)]
        public string TestCorrectAppleMusicLinkWithWrongID(string testLink)
        {
            return LinkHandler.HandleLinkOrDefaultAsync(testLink).Result; 
        }

        [TestCase(@"https://music.aple.com/ru/album/someone-like-you/403037872?i=403037927", ExpectedResult = null)]
        [TestCase(@"https://msic.apple.com/ru/album/someone-like-you/403037872?i=403037927", ExpectedResult = null)]
        [TestCase(@"https://music.apple.om/ru/album/someone-like-you/403037872?i=403037927", ExpectedResult = null)]
        public string TestWrongAppleMusicLink(string testLink)
        {
            return LinkHandler.HandleLinkOrDefaultAsync(testLink).Result; 
        }

        [TestCase(@"https://music.yandex.ru/album/3015939/track/19641303", 
            ExpectedResult = "spotify:track:1lgN0A2Vki2FTON5PYq42m")]
        public string TestCorrectYandexMusicLink(string testLink)
        {
            return LinkHandler.HandleLinkOrDefaultAsync(testLink).Result; 
        }
        
        [TestCase(@"https://music.yandex.ru/album/3015939/track/00000000", ExpectedResult = null)]
        public string TestCorrectYandexMusicLinkWithWrongTrackID(string testLink)
        {
            return LinkHandler.HandleLinkOrDefaultAsync(testLink).Result; 
        }
        
        [TestCase(@"https://msic.yandex.ru/album/3015939/track/19641303", ExpectedResult = null)]
        [TestCase(@"https://music.yadex.ru/album/3015939/track/19641303", ExpectedResult = null)]
        [TestCase(@"https://music.yandex.r/album/3015939/track/19641303", ExpectedResult = null)]
        [TestCase(@"https://music.yandex.ru/alum/3015939/track/19641303", ExpectedResult = null)]
        [TestCase(@"https://music.yandex.ru/album/3015939/trck/19641303", ExpectedResult = null)]
        public string TestWrongYandexMusicLink(string testLink)
        {
            return LinkHandler.HandleLinkOrDefaultAsync(testLink).Result;
        }
        
    }
}