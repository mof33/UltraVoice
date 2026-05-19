using HarmonyLib;
using System.Collections;
using UnityEngine;
using UltraVoice.Utilities;

namespace UltraVoice.Characters
{
    public class MindflayerCharacter
    {
        // Voice line storage
        public static AudioClip[] SpawnClips;
        public static AudioClip[] ChatterClips;
        public static AudioClip[] MeleeClips;
        public static AudioClip[] EnrageClips;

        public static AudioClip[] SpawnClipsMasc;
        public static AudioClip[] ChatterClipsMasc;
        public static AudioClip[] MeleeClipsMasc;
        public static AudioClip[] EnrageClipsMasc;

        // Subtitle storage
        public static readonly string[] SpawnSubs =
        {
            "I require your blood.",
            "You appear to contain blood.",
            "You are suitable for blood extraction.",
            "Please donate your blood to me.",
            "Thank you for your imminent blood donation."
        };

        public static readonly string[] ChatterSubs =
        {
            "This process will be brief.",
            "Your cooperation is appreciated.",
            "Please do not resist.",
            "Continued resistance is unnecessary."
        };

        public static readonly string[] MeleeSubs =
        {
            "Please maintain distance.",
            "Kindly step back.",
            "You are too close."
        };

        public static readonly string[] EnrageSubs =
        {
            "You have made a very unwise choice.",
            "Your behavior is unacceptable.",
            "This is your final warning.",
            "I will correct you by force."
        };

        public static bool IsMascMindflayer(Mindflayer mf)
        {
            var smr = mf.GetComponentInChildren<SkinnedMeshRenderer>();
            if (smr == null) return false;
            return smr.sharedMesh == mf.maleMesh;
        }

        public static void LoadVoiceLines(BepInEx.Logging.ManualLogSource logger)
        {
            SpawnClips = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip("Mindflayer.mf_Spawn1.wav"),
                UltraVoicePlugin.LoadClip("Mindflayer.mf_Spawn2.wav"),
                UltraVoicePlugin.LoadClip("Mindflayer.mf_Spawn3.wav"),
                UltraVoicePlugin.LoadClip("Mindflayer.mf_Spawn4.wav"),
                UltraVoicePlugin.LoadClip("Mindflayer.mf_Spawn5.wav"),
            };

            ChatterClips = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip("Mindflayer.mf_Chatter1.wav"),
                UltraVoicePlugin.LoadClip("Mindflayer.mf_Chatter2.wav"),
                UltraVoicePlugin.LoadClip("Mindflayer.mf_Chatter3.wav"),
                UltraVoicePlugin.LoadClip("Mindflayer.mf_Chatter4.wav"),
            };

            MeleeClips = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip("Mindflayer.mf_Melee1.wav"),
                UltraVoicePlugin.LoadClip("Mindflayer.mf_Melee2.wav"),
                UltraVoicePlugin.LoadClip("Mindflayer.mf_Melee3.wav"),
            };

            EnrageClips = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip("Mindflayer.mf_Enrage1.wav"),
                UltraVoicePlugin.LoadClip("Mindflayer.mf_Enrage2.wav"),
                UltraVoicePlugin.LoadClip("Mindflayer.mf_Enrage3.wav"),
                UltraVoicePlugin.LoadClip("Mindflayer.mf_Enrage4.wav"),
            };

            SpawnClipsMasc = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip("Mindflayer.mf_Spawn1Masc.wav"),
                UltraVoicePlugin.LoadClip("Mindflayer.mf_Spawn2Masc.wav"),
                UltraVoicePlugin.LoadClip("Mindflayer.mf_Spawn3Masc.wav"),
                UltraVoicePlugin.LoadClip("Mindflayer.mf_Spawn4Masc.wav"),
                UltraVoicePlugin.LoadClip("Mindflayer.mf_Spawn5Masc.wav"),
            };

            ChatterClipsMasc = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip("Mindflayer.mf_Chatter1Masc.wav"),
                UltraVoicePlugin.LoadClip("Mindflayer.mf_Chatter2Masc.wav"),
                UltraVoicePlugin.LoadClip("Mindflayer.mf_Chatter3Masc.wav"),
                UltraVoicePlugin.LoadClip("Mindflayer.mf_Chatter4Masc.wav"),
            };

            MeleeClipsMasc = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip("Mindflayer.mf_Melee1Masc.wav"),
                UltraVoicePlugin.LoadClip("Mindflayer.mf_Melee2Masc.wav"),
                UltraVoicePlugin.LoadClip("Mindflayer.mf_Melee3Masc.wav"),
            };

            EnrageClipsMasc = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip("Mindflayer.mf_Enrage1Masc.wav"),
                UltraVoicePlugin.LoadClip("Mindflayer.mf_Enrage2Masc.wav"),
                UltraVoicePlugin.LoadClip("Mindflayer.mf_Enrage3Masc.wav"),
                UltraVoicePlugin.LoadClip("Mindflayer.mf_Enrage4Masc.wav"),
            };

            logger.LogInfo("Mindflayer voice lines loaded successfully!");
        }

    }

    // MINDFLAYER PATCHES

    [HarmonyPatch(typeof(Mindflayer), "Start")]
    class MindflayerSpawnPatch
    {
        static void Postfix(Mindflayer __instance)
        {
            if (!UltraVoicePlugin.MindflayerVoiceEnabled.value) return;

            if (__instance.dying == true) return;

            VoiceManager.enemySpawnTimes[__instance] = Time.time;

            var clips = MindflayerCharacter.IsMascMindflayer(__instance)
                ? MindflayerCharacter.SpawnClipsMasc
                : MindflayerCharacter.SpawnClips;

            VoiceManager.PlayRandomVoice(__instance, "Mindflayer",
                clips,
                MindflayerCharacter.SpawnSubs,
                randomPitch: true
            );
        }
    }

    [HarmonyPatch(typeof(Mindflayer), "Update")]
    class MindflayerChatterPatch
    {
        static void Postfix(Mindflayer __instance)
        {
            if (!UltraVoicePlugin.MindflayerVoiceEnabled.value) return;

            if (ULTRAKILL.Cheats.BlindEnemies.Blind)
                return;

            if (__instance == null)
                return;

            if (__instance.dying == true) return;

            if (!VoiceManager.CheckCooldown(__instance, 5f))
                return;

            if (VoiceManager.TooSoonAfterSpawn(__instance, 3f))
                return;

            if (__instance.dying)
                return;

            if (Random.Range(0f, 1f) < 0.75f)
            {
                var clips = MindflayerCharacter.IsMascMindflayer(__instance)
                    ? MindflayerCharacter.ChatterClipsMasc
                    : MindflayerCharacter.ChatterClips;

                VoiceManager.PlayRandomVoice(__instance, "Mindflayer",
                    clips,
                    MindflayerCharacter.ChatterSubs,
                    randomPitch: true
                );
            }
        }
    }

    [HarmonyPatch(typeof(Mindflayer), "MeleeAttack")]
    class MindflayerMeleePatch
    {
        static void Postfix(Mindflayer __instance)
        {
            if (!UltraVoicePlugin.MindflayerVoiceEnabled.value) return;

            if (__instance.dying == true) return;

            if (!VoiceManager.CheckCooldown(__instance, 2f))
                return;

            UltraVoicePlugin.Instance.StartCoroutine(DelayedMeleeVoice(__instance));
        }

        static IEnumerator DelayedMeleeVoice(Mindflayer mf)
        {
            yield return new WaitForSeconds(0.5f);

            if (mf == null) yield break;

            var clips = MindflayerCharacter.IsMascMindflayer(mf)
                ? MindflayerCharacter.MeleeClipsMasc
                : MindflayerCharacter.MeleeClips;

            VoiceManager.PlayRandomVoice(mf, "Mindflayer",
                clips,
                MindflayerCharacter.MeleeSubs,
                randomPitch: true
            );
        }
    }

    [HarmonyPatch(typeof(Mindflayer), "Enrage")]
    class MindflayerEnragePatch
    {
        static void Postfix(Mindflayer __instance)
        {
            if (!UltraVoicePlugin.MindflayerVoiceEnabled.value) return;

            if (__instance.dying == true) return;

            var clips = MindflayerCharacter.IsMascMindflayer(__instance)
                ? MindflayerCharacter.EnrageClipsMasc
                : MindflayerCharacter.EnrageClips;

            VoiceManager.PlayRandomVoice(__instance, "Mindflayer",
                clips,
                MindflayerCharacter.EnrageSubs,
                interrupt: true,
                randomPitch: true
            );
        }
    }

    [HarmonyPatch(typeof(Mindflayer), "Death")]
    class MindflayerDeathPatch
    {
        static void Postfix(Mindflayer __instance)
        {
            VoiceManager.InterruptVoices(__instance);
        }
    }
}