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

        public static void LoadVoiceLines(AssetBundle bundle, BepInEx.Logging.ManualLogSource logger)
        {
            SpawnClips = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip(bundle, "prov_Spawn1"),
                UltraVoicePlugin.LoadClip(bundle, "prov_Spawn2"),
                UltraVoicePlugin.LoadClip(bundle, "prov_Spawn3"),
                UltraVoicePlugin.LoadClip(bundle, "prov_Spawn4"),
                UltraVoicePlugin.LoadClip(bundle, "prov_Spawn5")
            };

            AttackClips = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip(bundle, "prov_Chatter1"),
                UltraVoicePlugin.LoadClip(bundle, "prov_Chatter2"),
                UltraVoicePlugin.LoadClip(bundle, "prov_Chatter3"),
                UltraVoicePlugin.LoadClip(bundle, "prov_Chatter4"),
                UltraVoicePlugin.LoadClip(bundle, "prov_Chatter5")
            };

            DodgeClips = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip(bundle, "prov_Dodge1"),
                UltraVoicePlugin.LoadClip(bundle, "prov_Dodge2"),
                UltraVoicePlugin.LoadClip(bundle, "prov_Dodge3"),
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
                ProvidenceCharacter.SpawnSubs
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
                ProvidenceCharacter.AttackSubs
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
                ProvidenceCharacter.AttackSubs
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
                ProvidenceCharacter.DodgeSubs
            );
        }
    }
}