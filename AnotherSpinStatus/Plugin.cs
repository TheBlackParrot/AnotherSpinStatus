using AnotherSpinStatus.Patches;
using AnotherSpinStatus.Services;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace AnotherSpinStatus;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("srxd.raoul1808.spincore", "1.1.2")]
public partial class Plugin : BaseUnityPlugin
{
    internal static string CustomDataPath => CustomAssetLoadingHelper.CUSTOM_DATA_PATH;
    
    internal static ManualLogSource Log = null!;
    private static readonly Harmony HarmonyInstance = new(MyPluginInfo.PLUGIN_GUID);
    private static readonly SocketApi SocketApiInstance = new();

    private void Awake()
    {
        Log = Logger;

        RegisterConfigEntries();

        Log.LogInfo("Plugin loaded");
    }

    private void OnEnable()
    {
        SocketApiInstance.Initialize();
        HarmonyInstance.PatchAll();
        Events.Initialize();
    }

    private void OnDisable()
    {
        SocketApiInstance.Dispose();
        HarmonyInstance.UnpatchSelf();
        Events.Dispose();
    }
}