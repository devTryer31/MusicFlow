#nullable enable
using System;

namespace MusicBotV2
{
    [Serializable]
    public class Configuration
    {
        public string? Token { get; init; }
        public string? ClientId { get; init; }
        public string? ClientSecret { get; init; }
        public string? IFAPIKey { get; init; }
    }
}