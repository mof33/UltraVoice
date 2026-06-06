using HarmonyLib;
using System.Collections;
using UltraVoice.Utilities;
using UnityEngine;

namespace UltraVoice.Characters
{
    public class ProvidenceCharacter
    {
        // Voice line storage
        public static AudioClip[] SpawnClips;
        public static AudioClip[] AttackClips;
        public static AudioClip[] DodgeClips;

        // Subtitle storage
        public static readonly string[] SpawnSubs =
        {
            "The light descends",
            "The light finds you",
            "You cannot hide from the light",
            "He must be found",
            "His silence calls us here"
        };

        public static readonly string[] AttackSubs =
        {
            "Your end is written",
            "You will answer",
            "You chose this",
            "This ends here",
            "I will unmake you"
        };

        public static readonly string[] DodgeSubs =
        {
            "Foolish",
            "Unwise",
            "You know nothing"
        };

        public static bool IsProvidence(Drone d)
        {
            if (d == null || d.eid == null)
                return false;

            if (d.GetComponent<DroneFlesh>() != null)
                return false;

            return d.eid.enemyType == EnemyType.Providence;
        }

        public static void LoadVoiceLines(BepInEx.Logging.ManualLogSource logger)
        {
            SpawnClips = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip("Providence.prov_Spawn1.wav"),
                UltraVoicePlugin.LoadClip("Providence.prov_Spawn2.wav"),
                UltraVoicePlugin.LoadClip("Providence.prov_Spawn3.wav"),
                UltraVoicePlugin.LoadClip("Providence.prov_Spawn4.wav"),
                UltraVoicePlugin.LoadClip("Providence.prov_Spawn5.wav")
            };

            AttackClips = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip("Providence.prov_Chatter1.wav"),
                UltraVoicePlugin.LoadClip("Providence.prov_Chatter2.wav"),
                UltraVoicePlugin.LoadClip("Providence.prov_Chatter3.wav"),
                UltraVoicePlugin.LoadClip("Providence.prov_Chatter4.wav"),
                UltraVoicePlugin.LoadClip("Providence.prov_Chatter5.wav")
            };

            DodgeClips = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip("Providence.prov_Dodge1.wav"),
                UltraVoicePlugin.LoadClip("Providence.prov_Dodge2.wav"),
                UltraVoicePlugin.LoadClip("Providence.prov_Dodge3.wav"),
            };

            logger.LogInfo("Providence voice lines loaded successfully!");
        }

    }

    [HarmonyPatch(typeof(Drone), "Start")]
    class ProvidenceSpawnPatch
    {
        static void Postfix(Drone __instance)
        {
            if (!UltraVoicePlugin.ProvidenceVoiceEnabled.value) return;

            if (!ProvidenceCharacter.IsProvidence(__instance))
                return;

            VoiceManager.enemySpawnTimes[__instance] = Time.time;

            VoiceManager.PlayRandomVoice(__instance, "Providence",
                ProvidenceCharacter.SpawnClips,
                ProvidenceCharacter.SpawnSubs,
                randomPitch: true
            );
        }
    }


    [HarmonyPatch(typeof(Drone), "Shoot")]
    class ProvidenceShootPatch
    {
        static void Postfix(Drone __instance)
        {
            if (!UltraVoicePlugin.ProvidenceVoiceEnabled.value) return;

            if (!ProvidenceCharacter.IsProvidence(__instance))
                return;

            if (!VoiceManager.CheckCooldown(__instance, 5f))
                return;

            if (VoiceManager.TooSoonAfterSpawn(__instance, 2f))
                return;

            VoiceManager.PlayRandomVoice(__instance, "Providence",
                ProvidenceCharacter.AttackClips,
                ProvidenceCharacter.AttackSubs,
                randomPitch: true
            );
        }
    }

    [HarmonyPatch(typeof(Drone), "ShootSecondary")]
    class ProvidencePincerPatch
    {
        static void Postfix(Drone __instance)
        {
            if (!UltraVoicePlugin.ProvidenceVoiceEnabled.value) return;

            if (!ProvidenceCharacter.IsProvidence(__instance))
                return;

            if (!VoiceManager.CheckCooldown(__instance, 5f))
                return;

            if (VoiceManager.TooSoonAfterSpawn(__instance, 2f))
                return;

            VoiceManager.PlayRandomVoice(__instance, "Providence",
                ProvidenceCharacter.AttackClips,
                ProvidenceCharacter.AttackSubs,
                randomPitch: true
            );
        }
    }

    [HarmonyPatch(typeof(Drone), "DodgeLaugh")]
    class ProvidenceDodgePatch
    {
        static void Postfix(Drone __instance)
        {
            if (!UltraVoicePlugin.ProvidenceVoiceEnabled.value) return;

            if (!ProvidenceCharacter.IsProvidence(__instance))
                return;

            if (!VoiceManager.CheckCooldown(__instance, 4f))
                return;

            if (VoiceManager.TooSoonAfterSpawn(__instance, 4f))
                return;

            VoiceManager.PlayRandomVoice(__instance, "Providence",
                ProvidenceCharacter.DodgeClips,
                ProvidenceCharacter.DodgeSubs,
                randomPitch: true
            );
        }
    }
}