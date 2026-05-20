using BepInEx;
using UnityEngine;
using HarmonyLib;
using BepInEx.Configuration;
using PluginConfig.API;
using System.Reflection;
using PluginConfig.API.Fields;
using PluginConfig;
using PluginConfig.API.Decorators;

[BepInPlugin("me.NFC","No Feedbacker Cooldown","1.0")]
public class Plugin:BaseUnityPlugin{
    static public float PunchCd=0;
    static public int cdPerHit=5;
    static public int punched=0;
    public static ConfigEntry<bool> Fb;
    public static ConfigEntry<float> cd;
    public static PluginConfigurator config;
    void Awake()
    {
        config = PluginConfigurator.
            Create("No Feedbacker Cooldown", "me.NFC");
        Fb=Config.Bind(
            "General", 
            "Enabled", true, 
            "Whether the mod is enabled or not."
        );
        cd=Config.Bind(
            "General", 
            "Cooldown Per Hit", 0f, 
            "Cooldown per hit in seconds."
        );
        FloatField cdField=new FloatField(
            config.rootPanel, "Cooldown per hit in seconds.",
            "field.cdperhit",
            0f,true);
        cdField.onValueChange+=(v)=>{
            cd.Value=v.value;
        };
        cd.Value=cdField.value;
        BoolField f=new BoolField(
            config.rootPanel, "Whether the mod is enabled or not.",
            "field.isenabled",
            true,true);
        f.onValueChange += (v) =>{
            Fb.Value=v.value;
        };
        Logger.LogInfo("Mod loaded!");
        var harmony = new Harmony("me.NFC");
        harmony.PatchAll();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Logger.LogInfo("Called "+punched+" times");
        }
        PunchCd-=Time.deltaTime;
        if (PunchCd<0) PunchCd=0;
    }
};

[HarmonyPatch(typeof(FistControl), "Update")]
class FeedbackerPatch
{
    static void Prefix(FistControl __instance)
    { 
        if (!Plugin.Fb.Value) return;
        Plugin.punched++;
        MonoSingleton<WeaponCharges>.
            Instance.punchStamina = 3f;
        __instance.fistCooldown = 0;
    }
};

[HarmonyPatch(typeof(FistControl), "ForceArm")]
class AnotherPatch
{
    static void Prefix(ref bool __result)
    { 
        if (!Plugin.Fb.Value) return;
        __result = true;
    }
}

[HarmonyPatch(typeof(Punch), "PunchStart")]
class PunchStartPatch
{
    static void Prefix(Punch __instance)
    {
        if (Plugin.PunchCd>0) {
            __instance.ready=false;
            return;
        }
        if (!Plugin.Fb.Value) return;
        Plugin.punched++;
        __instance.ready=true;
        Plugin.PunchCd=Plugin.cd.Value;
    }
}
