using System;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;
using Pigeon.Movement;
using Sparroh.UI;

public class AltimeterMod
{
    private ConfigEntry<bool> enableAltimeterHUD;
    private ConfigEntry<float> altimeterAnchorX;
    private ConfigEntry<float> altimeterAnchorY;
    private ConfigColor valueColor;
    private HudHandle hud;

    private const float RaycastMaxDistance = 1000f;

    private readonly ConfigFile configFile;
    private readonly Harmony harmony;

    public static AltimeterMod Instance { get; private set; }

    public AltimeterMod(ConfigFile configFile, Harmony harmony)
    {
        this.configFile = configFile;
        this.harmony = harmony;

        Instance = this;

        enableAltimeterHUD = configFile.Bind("General", "EnableAltimeterHUD", true, "Enables the Altimeter HUD display showing player altitude above ground.");
        enableAltimeterHUD.SettingChanged += OnEnableAltimeterHUDChanged;

        altimeterAnchorX = configFile.Bind("HUD Positioning", "AltimeterAnchorX", 0.06355292f, "X anchor position for Altimeter (0-1).");
        altimeterAnchorY = configFile.Bind("HUD Positioning", "AltimeterAnchorY", 0.2589327f, "Y anchor position for Altimeter (0-1).");
        altimeterAnchorX.SettingChanged += OnAnchorChanged;
        altimeterAnchorY.SettingChanged += OnAnchorChanged;

        valueColor = ConfigColor.Bind(configFile, "Colors", "ValueColor", UIColors.Shamrock,
            "Rich-text value color for altitude (hex RRGGBB or #RRGGBB).");
    }


    public bool IsActive => hud != null && hud.IsActive;
    public Vector2 GetSize => hud?.Size ?? Vector2.zero;

    public void UpdateHudVisibility()
    {
        if (hud != null)
            hud.SetActive(enableAltimeterHUD.Value);
    }

    private void OnEnableAltimeterHUDChanged(object sender, EventArgs e)
    {
        if (enableAltimeterHUD.Value == false && hud != null)
            DestroyHud();
        UpdateHudVisibility();
    }

    private void OnAnchorChanged(object sender, EventArgs e)
    {
        UpdateAnchors();
    }

    private void UpdateAnchors()
    {
        if (hud != null)
            hud.SetAnchor(altimeterAnchorX.Value, altimeterAnchorY.Value);
    }

    private void CreateAltimeterHUD()
    {
        if (hud != null) return;

        hud = HudBuilder.Create("AltimeterHUD")
            .ParentToReticle()
            .Anchor(altimeterAnchorX.Value, altimeterAnchorY.Value)
            .Pivot(new Vector2(0f, 0.5f))
            .Size(300f, 25f)
            .AddText("AltitudeText")
            .Build();

        if (hud == null)
            return;

        HudRepositionClient.Register(
            SparrohPlugin.PluginGUID,
            "Altimeter",
            hud.Rect,
            altimeterAnchorX,
            altimeterAnchorY);

        UpdateHudVisibility();
    }

    private void DestroyHud()
    {
        HudRepositionClient.Unregister(SparrohPlugin.PluginGUID);
        if (hud != null)
        {
            hud.Destroy();
            hud = null;
        }
    }

    public void Update()
    {
        if (!enableAltimeterHUD.Value) return;

        if (hud == null)
        {
            CreateAltimeterHUD();
            return;
        }

        if (hud.Primary == null || Player.LocalPlayer == null) return;

        float altitude = CalculateAltitude();

        if (altitude < 1.3f)
            hud.Primary.Text = "On Ground";
        else if (altitude >= RaycastMaxDistance)
            hud.Primary.Text = "Too High";
        else
            hud.Primary.SetRich("Altitude", altitude, valueColor.Value, "m");

    }

    private float CalculateAltitude()
    {
        if (Player.LocalPlayer == null) return 0f;

        Vector3 start = Player.LocalPlayer.transform.position + Vector3.up * 0.1f;
        Vector3 direction = Vector3.down;

        RaycastHit[] hits = Physics.RaycastAll(start, direction, RaycastMaxDistance);

        float minDistance = float.MaxValue;
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.gameObject != Player.LocalPlayer.gameObject && !hit.collider.isTrigger)
                minDistance = Mathf.Min(minDistance, hit.distance);
        }

        return minDistance < float.MaxValue ? minDistance : RaycastMaxDistance;
    }

    public void OnDestroy()
    {
        DestroyHud();
    }
}
