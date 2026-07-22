using System;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.IO;

[BepInPlugin(PluginGUID, PluginName, PluginVersion)]
[BepInDependency("sparroh.uilibrary")]
[MycoMod(null, ModFlags.IsClientSide)]
public class SparrohPlugin : BaseUnityPlugin

{
    public const string PluginGUID = "sparroh.altimeter";
    public const string PluginName = "Altimeter";
    public const string PluginVersion = "1.0.1";

    internal static new ManualLogSource Logger;

    private Harmony harmony;
    private AltimeterMod altimeter;

    private void Awake()
    {
        Logger = base.Logger;

        try
        {
            harmony = new Harmony(PluginGUID);
        }
        catch (Exception ex)
        {
            Logger.LogError($"Failed to create Harmony instance: {ex.Message}");
            return;
        }

        var configFile = Config;
        try
        {
            var watcher = new FileSystemWatcher(Paths.ConfigPath, "sparroh.altimeter.cfg");
            watcher.Changed += (s, e) =>
            {
                configFile.Reload();
            };
            watcher.EnableRaisingEvents = true;
        }
        catch (Exception ex)
        {
            Logger.LogWarning($"Failed to set up config watcher: {ex.Message}");
        }

        try
        {
            altimeter = new AltimeterMod(configFile, harmony);
        }
        catch (Exception ex)
        {
            Logger.LogError($"Failed to initialize Altimeter: {ex.Message}");
        }

        try
        {
            harmony.PatchAll();
        }
        catch (Exception ex)
        {
            Logger.LogError($"Failed to apply Harmony patches: {ex.Message}");
        }

        Logger.LogInfo($"{PluginName} loaded successfully.");
    }

    private void Update()
    {
        try
        {
            if (altimeter != null) altimeter.UpdateHudVisibility();
        }
        catch (Exception ex)
        {
            Logger.LogError($"Error in Altimeter.UpdateHudVisibility(): {ex.Message}");
        }

        try
        {
            if (altimeter != null) altimeter.Update();
        }
        catch (Exception ex)
        {
            Logger.LogError($"Error in Altimeter.Update(): {ex.Message}");
        }
    }

    private void OnDestroy()
    {
        try
        {
            if (altimeter != null) altimeter.OnDestroy();
        }
        catch (Exception ex)
        {
            Logger.LogError($"Error in Altimeter.OnDestroy(): {ex.Message}");
        }

        try
        {
            if (harmony != null) harmony.UnpatchSelf();
        }
        catch (Exception ex)
        {
            Logger.LogError($"Error unpatching Harmony: {ex.Message}");
        }
    }
}
