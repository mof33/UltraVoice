using HarmonyLib;
using System.Collections;
using UnityEngine;
using UltraVoice.Utilities;

namespace UltraVoice.Characters
{
    public class MaliciousFaceCharacter
    {
        // Voice line storage
        public static AudioClip[] GenericClips;
        public static AudioClip[] EnrageClips;
        public static AudioClip DeathClip;

        // Subtitle storage
        public static readonly string[] GenericSubs =
        {
            "BLEED",
            "FOOLISH",
            "FUTILE",
            "FRAIL",
            "WEAKLING"
        };

        public static readonly string[] EnrageSubs =
        {
            "ENOUGH",
            "SUFFER",
            "PATHETIC",
        };

        public static void LoadVoiceLines(BepInEx.Logging.ManualLogSource logger)
        {
            GenericClips = new[]
            {
                UltraVoicePlugin.LoadClip("MaliciousFace.maurice_Generic1.wav"),
                UltraVoicePlugin.LoadClip("MaliciousFace.maurice_Generic2.wav"),
                UltraVoicePlugin.LoadClip("MaliciousFace.maurice_Generic3.wav"),
                UltraVoicePlugin.LoadClip("MaliciousFace.maurice_Generic4.wav"),
                UltraVoicePlugin.LoadClip("MaliciousFace.maurice_Generic5.wav"),
            };

            EnrageClips = new[]
            {
                UltraVoicePlugin.LoadClip("MaliciousFace.maurice_Enraged1.wav"),
                UltraVoicePlugin.LoadClip("MaliciousFace.maurice_Enraged2.wav"),
                UltraVoicePlugin.LoadClip("MaliciousFace.maurice_Enraged3.wav"),
            };

            DeathClip = UltraVoicePlugin.LoadClip("MaliciousFace.maurice_Death.wav");

            logger.LogInfo("Malicious Face voice lines loaded successfully!");
        }
    }

    [HarmonyPatch(typeof(MaliciousFace), "Start")]
    class MaliciousFaceSpawnPatch
    {
        static void Postfix(MaliciousFace __instance)
        {
            VoiceManager.enemySpawnTimes[__instance] = Time.time;
        }
    }

    [HarmonyPatch(typeof(MaliciousFace), "Update")]
    class MaliciousFaceChatterPatch
    {
        static void Postfix(MaliciousFace __instance)
        {
            if (!UltraVoicePlugin.MauriceVoiceEnabled.value)
                return;

            if (ULTRAKILL.Cheats.BlindEnemies.Blind)
                return;

            if (!VoiceManager.CheckCooldown(__instance, 4f))
                return;

            if (__instance == null || __instance.eid.dead)
                return;

            if (Random.Range(0f, 1f) < 0.75f)
                VoiceManager.PlayRandomVoice(__instance, "MaliciousFace",
                    MaliciousFaceCharacter.GenericClips,
                    MaliciousFaceCharacter.GenericSubs,
                    randomPitch: true
                );
        }
    }

    [HarmonyPatch(typeof(MaliciousFace), "Enrage")]
    class MaliciousFaceEnragePatch
    {
        static void Postfix(MaliciousFace __instance)
        {
            if (!UltraVoicePlugin.MauriceVoiceEnabled.value)
                return;

            VoiceManager.PlayRandomVoice(__instance, "MaliciousFace",
                MaliciousFaceCharacter.EnrageClips,
                MaliciousFaceCharacter.EnrageSubs,
                randomPitch: true
            );
        }
    }

    [HarmonyPatch(typeof(MaliciousFace), "OnGoLimp")]
    class MaliciousFaceDeathPatch
    {
        static void Prefix(MaliciousFace __instance)
        {
            if (!UltraVoicePlugin.MauriceVoiceEnabled.value)
                return;

            VoiceManager.CreateVoiceSource(__instance,
                "MaliciousFace",
                MaliciousFaceCharacter.DeathClip,
                null,
                true,
                randomPitch: true
            );
        }
    }
}