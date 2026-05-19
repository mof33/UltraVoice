using HarmonyLib;
using UnityEngine;
using UltraVoice.Utilities;

namespace UltraVoice.Characters
{
    public class MannequinCharacter
    {
        // Voice line storage
        public static AudioClip[] ChatterClips;
        public static AudioClip[] DeathClips;

        public static void LoadVoiceLines(BepInEx.Logging.ManualLogSource logger)
        {
            ChatterClips = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip("Mannequin.mq_Laugh1.wav"),
                UltraVoicePlugin.LoadClip("Mannequin.mq_Laugh2.wav"),
                UltraVoicePlugin.LoadClip("Mannequin.mq_Laugh3.wav"),
                UltraVoicePlugin.LoadClip("Mannequin.mq_Laugh4.wav"),
                UltraVoicePlugin.LoadClip("Mannequin.mq_Laugh5.wav")
            };

            DeathClips = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip("Mannequin.mq_Death1.wav"),
                UltraVoicePlugin.LoadClip("Mannequin.mq_Death2.wav"),
            };

            logger.LogInfo("Mannequin voice lines loaded successfully!");
        }

}

    // MANNEQUIN PATCHES

    [HarmonyPatch(typeof(Mannequin), "Update")]
    class MannequinChatterPatch
    {
        static void Postfix(Mannequin __instance)
        {
            if (!UltraVoicePlugin.MannequinVoiceEnabled.value) return;

            if (ULTRAKILL.Cheats.BlindEnemies.Blind)
                return;

            if (!VoiceManager.CheckCooldown(__instance, 4f))
                return;

            if (Random.Range(0f, 1f) < 0.75f)
                return;

            VoiceManager.PlayRandomVoice(__instance, "Mannequin",
                MannequinCharacter.ChatterClips,
                null,
                randomPitch: true
            );
        }
    }

    [HarmonyPatch(typeof(Mannequin), "MeleeAttack")]
    class MannequinSwingPatch
    {
        static void Postfix(Mannequin __instance)
        {
            if (!UltraVoicePlugin.MannequinVoiceEnabled.value) return;

            VoiceManager.PlayRandomVoice(__instance, "Mannequin",
                MannequinCharacter.ChatterClips,
                null,
                randomPitch: true
            );
        }
    }

    [HarmonyPatch(typeof(Mannequin), "OnDeath")]
    class MannequinDeathPatch
    {
        static void Postfix(Mannequin __instance)
        {
            if (!UltraVoicePlugin.MannequinVoiceEnabled.value) return;

            VoiceManager.PlayRandomVoice(__instance, "Mannequin",
                MannequinCharacter.DeathClips,
                null,
                true,
                randomPitch: true
            );
        }
    }
}