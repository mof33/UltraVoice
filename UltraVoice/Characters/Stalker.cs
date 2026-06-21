using HarmonyLib;
using UnityEngine;
using UltraVoice.Utilities;

namespace UltraVoice.Characters
{
    public class StalkerCharacter
    {
        public static AudioClip[] ChatterClips;
        public static AudioClip AttackClip;

        public static void LoadVoiceLines(BepInEx.Logging.ManualLogSource logger)
        {
            ChatterClips = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip("Stalker.stalker_Chatter1.wav"),
                UltraVoicePlugin.LoadClip("Stalker.stalker_Chatter1.wav"),
                UltraVoicePlugin.LoadClip("Stalker.stalker_Chatter1.wav"),
            };

            AttackClip = UltraVoicePlugin.LoadClip("Stalker.stalker_Explode.wav");

            logger.LogInfo("Stalker voice lines loaded successfully!");
        }
    }

    [HarmonyPatch(typeof(Stalker), "Update")]
    class StalkerChatterPatch
    {
        static void Postfix(Stalker __instance)
        {
            if (!UltraVoicePlugin.StalkerVoiceEnabled.value) return;

            if (ULTRAKILL.Cheats.BlindEnemies.Blind)
                return;

            if (!VoiceManager.CheckCooldown(__instance, 4f))
                return;

            if (UnityEngine.Random.Range(0f, 1f) < 0.75f)
                return;

            VoiceManager.PlayRandomVoice(__instance, "Stalker",
                StalkerCharacter.ChatterClips,
                null,
                randomPitch: true
            );
        }
    }

    [HarmonyPatch(typeof(Stalker), nameof(Stalker.Countdown))]
    class StalkerExplodePatch
    {
        static void Postfix(Stalker __instance)
        {
            if (!UltraVoicePlugin.StalkerVoiceEnabled.value) return;

            VoiceManager.CreateVoiceSource(__instance, "Stalker",
                StalkerCharacter.AttackClip,
                null,
                randomPitch: true
            );
        }
    }
}
