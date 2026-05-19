using BepInEx;
using UnityEngine;
using HarmonyLib;
using BepInEx.Configuration;

[BepInPlugin("me.NFC","No Feedbacker Cooldown","1.0")]
public class Plugin:BaseUnityPlugin{
    public static ConfigEntry<bool> Fb;
    void Awake()
    {
        Fb=Config.Bind(
            "General", 
            "Enabled", true, 
            "Whether the mod is enabled or not."
        );
        Logger.LogInfo("Mod loaded!");
        var harmony = new Harmony("me.NFC");
        harmony.PatchAll();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Logger.LogInfo("Pressed F1");
        }
    }
};

[HarmonyPatch(typeof(FistControl),"Update")]
public class FeedbackerPatch{
    static void Postfix(FistControl __instance){
        __instance.fistCooldown=0;
    }
}
