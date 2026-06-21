using HarmonyLib;
using UnityEngine;
using System.Collections;
using UltraVoice.Utilities;

namespace UltraVoice.Characters
{
    public class StreetcleanerCharacter
    {
        // Voice line storage
        public static AudioClip[] ChatterClips;
        public static AudioClip[] AttackClips;
        public static AudioClip[] ParryClips;
        public static AudioClip ScreamingClip;

        // Subtitle storage
        public static readonly string[] ChatterSubs =
        {
            "IMPURITY",
            "SANITIZE",
            "EXTERMINATE",
            "PURGE",
            "UNCLEAN"
        };

        public static readonly string[] AttackSubs =
        {
            "CLEANSING",
            "PURIFYING",
            "TO ASH",
            "QUIT MOVING",
            "STOP RESISTING"
        };

        public static readonly string[] ParrySubs =
        {
            "DENIED",
            "DEFLECTED",
            "HA HA"
        };

        public static void LoadVoiceLines(BepInEx.Logging.ManualLogSource logger)
        {
            ChatterClips = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip("Streetcleaner.sc_Chatter1.wav"),
                UltraVoicePlugin.LoadClip("Streetcleaner.sc_Chatter2.wav"),
                UltraVoicePlugin.LoadClip("Streetcleaner.sc_Chatter3.wav"),
                UltraVoicePlugin.LoadClip("Streetcleaner.sc_Chatter4.wav"),
                UltraVoicePlugin.LoadClip("Streetcleaner.sc_Chatter5.wav")
            };

            AttackClips = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip("Streetcleaner.sc_Attack1.wav"),
                UltraVoicePlugin.LoadClip("Streetcleaner.sc_Attack2.wav"),
                UltraVoicePlugin.LoadClip("Streetcleaner.sc_Attack3.wav"),
                UltraVoicePlugin.LoadClip("Streetcleaner.sc_Attack4.wav"),
                UltraVoicePlugin.LoadClip("Streetcleaner.sc_Attack5.wav")
            };

            ParryClips = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip("Streetcleaner.sc_Parry1.wav"),
                UltraVoicePlugin.LoadClip("Streetcleaner.sc_Parry2.wav"),
                UltraVoicePlugin.LoadClip("Streetcleaner.sc_Parry3.wav"),
            };

            ScreamingClip = UltraVoicePlugin.LoadClip("Streetcleaner.sc_Screaming.wav");

            logger.LogInfo("Streetcleaner voice lines loaded successfully!");
        }
    }

    // STREETCLEANER PATCHES

    [HarmonyPatch(typeof(Streetcleaner), "Update")]
    class StreetcleanerChatterPatch
    {
        static void Postfix(Streetcleaner __instance)
        {
            if (!UltraVoicePlugin.StreetcleanerVoiceEnabled.value)
                return;

            if (__instance == null)
                return;

            if (__instance.dead)
                return;

            if (!VoiceManager.CheckCooldown(__instance, 4f))
                return;

            if (Random.Range(0f, 1f) < 0.75)
                VoiceManager.PlayRandomVoice(__instance, "Streetcleaner",
                    StreetcleanerCharacter.ChatterClips,
                    StreetcleanerCharacter.ChatterSubs,
                    randomPitch: true
                );
        }
    }

    [HarmonyPatch(typeof(Streetcleaner), "StartFire")]
    class StreetcleanerFlameAttackPatch
    {
        static void Postfix(Streetcleaner __instance)
        {
            if (!UltraVoicePlugin.StreetcleanerVoiceEnabled.value)
                return;

            if (Random.Range(0f, 1f) < 0.75)
                VoiceManager.PlayRandomVoice(__instance, "Streetcleaner",
                    StreetcleanerCharacter.AttackClips,
                    StreetcleanerCharacter.AttackSubs,
                    randomPitch: true
                );
        }
    }

    [HarmonyPatch(typeof(Streetcleaner), "DeflectShot")]
    class StreetcleanerParryPatch
    {
        static void Postfix(Streetcleaner __instance)
        {
            if (!UltraVoicePlugin.StreetcleanerVoiceEnabled.value)
                return;

            if (Random.Range(0f, 1f) < 0.5)
                VoiceManager.PlayRandomVoice(__instance, "Streetcleaner",
                StreetcleanerCharacter.ParryClips,
                StreetcleanerCharacter.ParrySubs,
                randomPitch: true
            );
        }
    }

    [HarmonyPatch(typeof(Streetcleaner), "OnGoLimp")]
    class StreetcleanerDeathInterruptPatch
    {
        static void Postfix(Streetcleaner __instance)
        {
            VoiceManager.InterruptVoices(__instance);
        }
    }
}