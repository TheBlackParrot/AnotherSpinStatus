using BepInEx.Configuration;

namespace AnotherSpinStatus;

public partial class Plugin
{
    internal static ConfigEntry<int> SocketPort = null!;
    internal static ConfigEntry<string> SocketAddress = null!;
    
    internal static ConfigEntry<int> CoverArtSize = null!;
    internal static ConfigEntry<int> CoverArtQuality = null!;

    private void RegisterConfigEntries()
    {
        SocketAddress = Config.Bind("WebSocket", nameof(SocketAddress), "127.0.0.1",
            "IP address for the WebSocket firehose to listen on");
        
        SocketPort = Config.Bind("WebSocket", nameof(SocketPort), 6971,
            "Port for the WebSocket firehose to listen on");

        CoverArtSize = Config.Bind("CoverArt", nameof(CoverArtSize), 80,
            "Size of base64 encoded cover art in MapData events");
        CoverArtQuality = Config.Bind("CoverArt", nameof(CoverArtQuality), 70,
            "JPEG quality of base64 encoded cover art in MapData events");
    }
}