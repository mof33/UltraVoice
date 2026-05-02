using HarmonyLib;
using System.Collections;
using UnityEngine;
using UltraVoice.Utilities;

namespace UltraVoice.Characters
{
    public class MirrorReaperCharacter
    {
        public static AudioClip BossSpawnClip;
        public static AudioClip[] SpawnClips;
        public static AudioClip[] LaughClips;
        public static AudioClip[] MirrorTauntClips;
        public static AudioClip[] PuppetHandClips;
        public static AudioClip[] DeathClips;

        public static readonly string[] SpawnSubs =
        {
            "I FOUND YOU!",
            "THERE YOU ARE",
            "FOUND YOU! FOUND YOU!",
            "YOU CAN'T HIDE!"
        };

        public static readonly string[] MirrorTauntSubs =
        {
            "TURN AROUND",
            "LOOK IN THE MIRROR",
            "DO YOU SEE ME? DO YOU SEE ME?!"
        };

        public static readonly string[] PuppetHandSubs =
        {
            "TAKE HOLD",
            "NO ESCAPE",
            "FIND THEM",
            "SEEK THEM OUT"
        };

        public static bool Spawned = false;

        public static void LoadVoiceLines(AssetBundle bundle, BepInEx.Logging.ManualLogSource logger)
        {
            SpawnClips = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip(bundle, "mr_SpawnSpecial"),
                UltraVoicePlugin.LoadClip(bundle, "mr_Spawn1"),
                UltraVoicePlugin.LoadClip(bundle, "mr_Spawn2"),
                UltraVoicePlugin.LoadClip(bundle, "mr_Spawn3"),
            };
            LaughClips = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip(bundle, "mr_Laugh1"),
                UltraVoicePlugin.LoadClip(bundle, "mr_Laugh2"),
                UltraVoicePlugin.LoadClip(bundle, "mr_Laugh3"),
            };
            MirrorTauntClips = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip(bundle, "mr_MirrorTaunt1"),
                UltraVoicePlugin.LoadClip(bundle, "mr_MirrorTaunt2"),
                UltraVoicePlugin.LoadClip(bundle, "mr_MirrorTaunt3"),
            };
            PuppetHandClips = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip(bundle, "mr_PuppetHand1"),
                UltraVoicePlugin.LoadClip(bundle, "mr_PuppetHand2"),
                UltraVoicePlugin.LoadClip(bundle, "mr_PuppetHand3"),
                UltraVoicePlugin.LoadClip(bundle, "mr_PuppetHand4")
            };
            DeathClips = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip(bundle, "mr_Death1"),
                UltraVoicePlugin.LoadClip(bundle, "mr_Death2"),
                UltraVoicePlugin.LoadClip(bundle, "mr_Death3"),
            };

            logger.LogInfo("Mirror Reaper voice lines loaded successfully!");
        }
    }

    [HarmonyPatch(typeof(BossHealthBar), "OnEnable")]
    class MirrorReaperBossSpawnPatch
    {
        static void Postfix(BossHealthBar __instance)
        {
            if (!UltraVoicePlugin.MirrorReaperVoiceEnabled.value)
                return;

            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "45addc6c3730dae418321e00af1116c5")

            VoiceManager.enemySpawnTimes[__instance.GetComponentInParent<MirrorReaper>()] = Time.time;

            VoiceManager.PlayRandomVoice(__instance.GetComponentInParent<MirrorReaper>(), "MirrorReaper",
                MirrorReaperCharacter.SpawnClips,
                MirrorReaperCharacter.SpawnSubs
            );

            MirrorReaperCharacter.Spawned = true;
        }
    }

    [HarmonyPatch(typeof(MirrorReaper), "Start")]
    class MirrorReaperCommonSpawnPatch
    {
        static void Postfix(MirrorReaper __instance)
        {
            if (!UltraVoicePlugin.MirrorReaperVoiceEnabled.value) 
                return;

            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "45addc6c3730dae418321e00af1116c5")
                return;

            VoiceManager.enemySpawnTimes[__instance] = Time.time;

            VoiceManager.PlayRandomVoice(__instance, "MirrorReaper",
                MirrorReaperCharacter.SpawnClips,
                MirrorReaperCharacter.SpawnSubs
            );

            MirrorReaperCharacter.Spawned = true;
        }
    }

    [HarmonyPatch(typeof(MirrorReaper), "Update")]
    class MirrorReaperChatterPatch
    {
        static void Postfix(MirrorReaper __instance)
        {
            if (!UltraVoicePlugin.MirrorReaperVoiceEnabled.value) return;

            if (ULTRAKILL.Cheats.BlindEnemies.Blind)
                return;

            if (!MirrorReaperCharacter.Spawned)
                return;

            if (!VoiceManager.CheckCooldown(__instance, 6f))
                return;

            if (Random.Range(0f, 1f) < 0.75f)
                return;

            if (__instance == null || __instance.eid.dead)
                return;

            if (__instance.inMirrorPhase)
                VoiceManager.PlayRandomVoice(__instance, "MirrorReaper",
                    MirrorReaperCharacter.MirrorTauntClips,
                    MirrorReaperCharacter.MirrorTauntSubs
                );
            else
                VoiceManager.PlayRandomVoice(__instance, "MirrorReaper",
                    MirrorReaperCharacter.LaughClips,
                    null
                );
        }
    }

    [HarmonyPatch(typeof(MirrorReaper), "SwingTriple")]
    class MirrorReaperSwingTriplePatch
    {
        static void Postfix(MirrorReaper __instance)
        {
            if (!UltraVoicePlugin.MirrorReaperVoiceEnabled.value) return;

            if (!MirrorReaperCharacter.Spawned)
                return;

            if (!VoiceManager.CheckCooldown(__instance, 4f))
                return;

            if (Random.Range(0f, 1f) < 0.75f)
                return;

            if (__instance == null || __instance.eid.dead)
                return;

            VoiceManager.PlayRandomVoice(__instance, "MirrorReaper",
                MirrorReaperCharacter.LaughClips,
                null
            );
        }
    }

    [HarmonyPatch(typeof(MirrorReaper), "SwingVertical")]
    class MirrorReaperSwingVerticalPatch
    {
        static void Postfix(MirrorReaper __instance)
        {
            if (!UltraVoicePlugin.MirrorReaperVoiceEnabled.value) return;

            if (!MirrorReaperCharacter.Spawned)
                return;

            if (!VoiceManager.CheckCooldown(__instance, 4f))
                return;

            if (Random.Range(0f, 1f) < 0.75f)
                return;

            if (__instance == null || __instance.eid.dead)
                return;

            VoiceManager.PlayRandomVoice(__instance, "MirrorReaper",
                MirrorReaperCharacter.LaughClips,
                null
            );
        }
    }

    [HarmonyPatch(typeof(MirrorReaper), "SwingSpree")]
    class MirrorReaperSwingSpreePatch
    {
        static void Postfix(MirrorReaper __instance)
        {
            if (!UltraVoicePlugin.MirrorReaperVoiceEnabled.value) return;

            if (!MirrorReaperCharacter.Spawned)
                return;

            if (!VoiceManager.CheckCooldown(__instance, 4f))
                return;

            if (Random.Range(0f, 1f) < 0.75f)
                return;

            if (__instance == null || __instance.eid.dead)
                return;

            VoiceManager.PlayRandomVoice(__instance, "MirrorReaper",
                MirrorReaperCharacter.LaughClips,
                null
            );
        }
    }

    [HarmonyPatch(typeof(MirrorReaper), "GroundWave")]
    class MirrorReaperHandPatch
    {
        static void Postfix(MirrorReaper __instance)
        {
            if (!UltraVoicePlugin.MirrorReaperVoiceEnabled.value) return;

            if (!MirrorReaperCharacter.Spawned)
                return;

            if (__instance == null || __instance.eid.dead)
                return;

            VoiceManager.PlayRandomVoice(__instance, "MirrorReaper",
                MirrorReaperCharacter.PuppetHandClips,
                MirrorReaperCharacter.PuppetHandSubs
            );
        }
    }

    [HarmonyPatch(typeof(MirrorReaper), "Death")]
    class MirrorReaperDeathPatch
    {
        static void Postfix(MirrorReaper __instance)
        {
            if (!UltraVoicePlugin.MirrorReaperVoiceEnabled.value) return;

            VoiceManager.PlayRandomVoice(__instance, "MirrorReaper",
                MirrorReaperCharacter.DeathClips,
                null,
                true
            );
        }
    }
}