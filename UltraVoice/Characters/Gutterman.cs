using HarmonyLib;
using UnityEngine;
using System.Collections;
using UltraVoice.Utilities;

namespace UltraVoice.Characters
{
    public class GuttermanCharacter
    {
        // Voice line storage
        public static AudioClip[] SpawnClips;
        public static AudioClip[] ShieldBreakClips;
        public static AudioClip[] EnrageClips;
        public static AudioClip[] DeathClips;
        public static AudioClip[] ParryClips;
        public static AudioClip[] PunchClips;

        // Subtitle storage
        public static readonly string[] SpawnSubs =
        {
            "THIS WILL NOT TAKE LONG.",
            "WHO MUST I KILL TODAY?",
            "IT IS TIME TO KILL.",
            "FOR THE MOTHERLAND!",
            "SAY YOUR PRAYERS.",
            "ЗА РОДИНУ!"
        };

        public static readonly string[] ShieldBreakSubs =
        {
            "SHIELD DOWN!",
            "SHIELD BROKEN!",
            "SHIELD DESTROYED!",
        };

        public static readonly string[] EnrageSubs =
        {
            "THAT COST MONEY!",
            "YOU WILL PAY FOR THIS!",
            "OH, YOU MAKE ME SO MAD…",
            "ЧЁРТ ПОБЕРИ!",
            "СУКИН СЫН!"
        };

        public static void LoadVoiceLines(BepInEx.Logging.ManualLogSource logger)
        {
            SpawnClips = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip("Gutterman.gm_Spawn1.wav"),
                UltraVoicePlugin.LoadClip("Gutterman.gm_Spawn2.wav"),
                UltraVoicePlugin.LoadClip("Gutterman.gm_Spawn3.wav"),
                UltraVoicePlugin.LoadClip("Gutterman.gm_Spawn4.wav"),
                UltraVoicePlugin.LoadClip("Gutterman.gm_Spawn5.wav"),
                UltraVoicePlugin.LoadClip("Gutterman.gm_Spawn6.wav")
            };

            PunchClips = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip("Gutterman.gm_Punch1.wav"),
                UltraVoicePlugin.LoadClip("Gutterman.gm_Punch2.wav"),
                UltraVoicePlugin.LoadClip("Gutterman.gm_Punch3.wav")
            };

            ShieldBreakClips = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip("Gutterman.gm_GuardBreak1.wav"),
                UltraVoicePlugin.LoadClip("Gutterman.gm_GuardBreak2.wav"),
                UltraVoicePlugin.LoadClip("Gutterman.gm_GuardBreak3.wav"),
            };

            EnrageClips = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip("Gutterman.gm_Enrage1.wav"),
                UltraVoicePlugin.LoadClip("Gutterman.gm_Enrage2.wav"),
                UltraVoicePlugin.LoadClip("Gutterman.gm_Enrage3.wav"),
                UltraVoicePlugin.LoadClip("Gutterman.gm_Enrage4.wav"),
                UltraVoicePlugin.LoadClip("Gutterman.gm_Enrage5.wav"),
            };

            DeathClips = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip("Gutterman.gm_Death1.wav"),
                UltraVoicePlugin.LoadClip("Gutterman.gm_Death2.wav"),
                UltraVoicePlugin.LoadClip("Gutterman.gm_Death3.wav"),
            };

            ParryClips = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip("Gutterman.gm_Parry1.wav"),
                UltraVoicePlugin.LoadClip("Gutterman.gm_Parry2.wav"),
                UltraVoicePlugin.LoadClip("Gutterman.gm_Parry3.wav"),
            };

            logger.LogInfo("Gutterman voice lines loaded successfully!");
        }

}

    // GUTTERMAN PATCHES

    [HarmonyPatch(typeof(Gutterman), "Start")]
    class GuttermanSpawnPatch
    {
        static void Postfix(Gutterman __instance)
        {
            if (!UltraVoicePlugin.GuttermanVoiceEnabled.value) return;

            if (__instance.dead) return;

            VoiceManager.enemySpawnTimes[__instance] = Time.time;

            VoiceManager.PlayRandomVoice(__instance, "Gutterman",
                GuttermanCharacter.SpawnClips,
                GuttermanCharacter.SpawnSubs,
                false,
                randomPitch: true
            );
        }
    }

    [HarmonyPatch(typeof(Gutterman), "ShieldBreak")]
    class GuttermanShieldBreakPatch
    {
        static void Postfix(Gutterman __instance)
        {
            if (!UltraVoicePlugin.GuttermanVoiceEnabled.value) return;

            if (__instance.enraged)
                VoiceManager.PlayRandomVoice(__instance, "Gutterman",
                GuttermanCharacter.EnrageClips,
                GuttermanCharacter.EnrageSubs,
                true,
                randomPitch: true
            );
            else VoiceManager.PlayRandomVoice(__instance, "Gutterman",
                GuttermanCharacter.ShieldBreakClips,
                GuttermanCharacter.ShieldBreakSubs,
                true,
                randomPitch: true
            );
        }
    }

    [HarmonyPatch(typeof(Gutterman), "Death")]
    class GuttermanDeathPatch
    {
        static void Postfix(Gutterman __instance)
        {
            if (!UltraVoicePlugin.GuttermanVoiceEnabled.value) return;

            VoiceManager.PlayRandomVoice(__instance, "Gutterman",
                GuttermanCharacter.DeathClips,
                null,
                true,
                randomPitch: true
            );
        }
    }

    [HarmonyPatch(typeof(Gutterman), "GotParried")]
    class GuttermanParryPatch
    {
        static void Postfix(Gutterman __instance)
        {
            if (!UltraVoicePlugin.GuttermanVoiceEnabled.value) return;

            if (!VoiceManager.CheckCooldown(__instance, 3f))
                return;

            VoiceManager.PlayRandomVoice(__instance, "Gutterman",
                GuttermanCharacter.ParryClips,
                null,
                true,
                randomPitch: true
            );
        }
    }

    [HarmonyPatch(typeof(Gutterman), nameof(Gutterman.ShieldBash))]
    class GuttermanShieldBashPatch
    {
        static void Postfix(Gutterman __instance)
        {
            if (!UltraVoicePlugin.GuttermanVoiceEnabled.value) return;

            VoiceManager.PlayRandomVoice(__instance, "Gutterman",
                GuttermanCharacter.PunchClips,
                null,
                false,
                randomPitch: true
            );
        }
    }
}