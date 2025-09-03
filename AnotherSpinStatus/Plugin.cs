using AnotherSpinStatus.Patches;
using AnotherSpinStatus.Services;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace AnotherSpinStatus;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public partial class Plugin : BaseUnityPlugin
{
    internal static string CustomDataPath => CustomAssetLoadingHelper.CUSTOM_DATA_PATH;
    
    internal static ManualLogSource Log = null!;
    private static Harmony? _harmony;

    private void Awake()
    {
        Log = Logger;

        RegisterConfigEntries();
        
        SocketApi socketApi = new();
        socketApi.Initialize();
        
        _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
        _harmony.PatchAll();
        
        Events.Initialize();

        Log.LogInfo("Plugin loaded");
    }

    private void OnDestroy()
    {
        _harmony?.UnpatchSelf();
    }
}