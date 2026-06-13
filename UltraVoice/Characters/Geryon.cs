using HarmonyLib;
using System.Collections;
using UltraVoice.Utilities;
using UnityEngine;

namespace UltraVoice.Characters
{
    public class GeryonCharacter
    {
        public static AudioClip IntroClip;
        public static AudioClip RestartClip;
        public static AudioClip EnrageClip;
        public static AudioClip DeathClip;
        public static AudioClip BigPainClip;
        public static AudioClip RecoverClip;

        public static AudioClip[] ChatterClips;
        public static AudioClip[] OverheatClips;

        public static readonly string[] ChatterSubs =
        {
            "IT MUST NOT PREVAIL!",
            "IT CANNOT ESCAPE US!",
            "NO ONE WILL SAVE YOU!",
            "RIP IT TO PIECES!",
            "CRUSH IT TO DUST!",
            "MORE BLOOD FOR US!",
            "WHY DO YOU PERSIST?",
            "ACCEPT YOUR END.",
            "SUCH POINTLESS RESISTANCE."
        };

        public static readonly string[] OverheatSubs =
        {
            "HOLD TOGETHER! HOLD TOGETHER!",
            "TOO MUCH! TOO MUCH!",
            "CALM DOWN! CALM DOWN!",
        };

        public static void LoadVoiceLines(BepInEx.Logging.ManualLogSource logger)
        {
            IntroClip = UltraVoicePlugin.LoadClip("Geryon.ger_Intro.wav");
            RestartClip = UltraVoicePlugin.LoadClip("Geryon.ger_Respawn.wav");
            EnrageClip = UltraVoicePlugin.LoadClip("Geryon.ger_Enrage.wav");
            DeathClip = UltraVoicePlugin.LoadClip("Geryon.ger_Death.wav");
            BigPainClip = UltraVoicePlugin.LoadClip("Geryon.ger_BigPain.wav");
            RecoverClip = UltraVoicePlugin.LoadClip("Geryon.ger_Recover.wav");

            ChatterClips = new[]
            {
                UltraVoicePlugin.LoadClip("Geryon.ger_Chatter1.wav"),
                UltraVoicePlugin.LoadClip("Geryon.ger_Chatter2.wav"),
                UltraVoicePlugin.LoadClip("Geryon.ger_Chatter3.wav"),
                UltraVoicePlugin.LoadClip("Geryon.ger_Chatter4.wav"),
                UltraVoicePlugin.LoadClip("Geryon.ger_Chatter5.wav"),
                UltraVoicePlugin.LoadClip("Geryon.ger_Chatter6.wav"),
                UltraVoicePlugin.LoadClip("Geryon.ger_Chatter7.wav"),
                UltraVoicePlugin.LoadClip("Geryon.ger_Chatter8.wav"),
                UltraVoicePlugin.LoadClip("Geryon.ger_Chatter9.wav"),
            };

            OverheatClips = new[]
            {
                UltraVoicePlugin.LoadClip("Geryon.ger_Overheat1.wav"),
                UltraVoicePlugin.LoadClip("Geryon.ger_Overheat2.wav"),
                UltraVoicePlugin.LoadClip("Geryon.ger_Overheat3.wav")
            };

            logger.LogInfo("Geryon voice lines loaded successfully!");
        }

        public static bool EnrageLinePlayed = false;
        public static bool RestartedFight = false;
        public static bool CanRestartFight = false;
    }

    [HarmonyPatch(typeof(Geryon), "Start")]
    class GeryonIntroPatch
    {
        static void Postfix(Geryon __instance)
        {
            if (!UltraVoicePlugin.GeryonVoiceEnabled.value)
                return;

            GeryonCharacter.EnrageLinePlayed = false;
            GeryonCharacter.CanRestartFight = true;

            VoiceManager.enemySpawnTimes[__instance] = Time.time;

            if (!GeryonCharacter.RestartedFight)
                UltraVoicePlugin.Instance.StartCoroutine(PlayIntro(__instance));
            else
                UltraVoicePlugin.Instance.StartCoroutine(PlayRestart(__instance));

            static IEnumerator PlayIntro(Geryon geryon)
            {
                if (!geryon.active)
                    yield break;

                var src = VoiceManager.CreateVoiceSource(
                    geryon,
                    "Geryon",
                    GeryonCharacter.IntroClip,
                    "FOOLISH...",
                    false
                );

                if (!geryon.active)
                    yield break;

                yield return new WaitForSeconds(1f);
                VoiceManager.ShowSubtitle(
                    "OR BRAVE?",
                    src
                );

                if (!geryon.active)
                    yield break;

                yield return new WaitForSeconds(1.25f);
                VoiceManager.ShowSubtitle(
                    "IT MATTERS NOT...",
                    src
                );

                if (!geryon.active)
                    yield break;

                yield return new WaitForSeconds(1.25f);
                VoiceManager.ShowSubtitle(
                    "FOR WE SHALL STRIKE YOU DOWN!",
                    src
                );
            }

            static IEnumerator PlayRestart(Geryon geryon)
            {
                if (!geryon.active)
                    yield break;

                var src = VoiceManager.CreateVoiceSource(
                    geryon,
                    "Geryon",
                    GeryonCharacter.RestartClip,
                    "IT REAPPEARS...",
                    false
                );

                if (!geryon.active)
                    yield break;

                yield return new WaitForSeconds(1.25f);
                VoiceManager.ShowSubtitle(
                    "IT WANTS MORE?",
                    src
                );

                if (!geryon.active)
                    yield break;

                yield return new WaitForSeconds(1.5f);
                VoiceManager.ShowSubtitle(
                    "THEN MORE IT SHALL RECEIVE!",
                    src
                );
            }
        }
    }

    [HarmonyPatch(typeof(StatsManager), nameof(StatsManager.Restart))]
    class GeryonRestartPatch
    {
        static void Postfix(StatsManager __instance)
        {
            if (!UltraVoicePlugin.GeryonVoiceEnabled.value)
                return;

            if (GeryonCharacter.CanRestartFight)
            {
                GeryonCharacter.RestartedFight = true;
            }
        }
    }

    [HarmonyPatch(typeof(Geryon), "Stun")]
    class GeryonOverheatPatch
    {
        static void Postfix(Geryon __instance)
        {
            if (!UltraVoicePlugin.GeryonVoiceEnabled.value)
                return;

            VoiceManager.InterruptVoices(__instance);
            UltraVoicePlugin.Instance.StartCoroutine(PlayOverheat(__instance));

            static IEnumerator PlayOverheat(Geryon geryon)
            {
                VoiceManager.CreateVoiceSource(
                    geryon,
                    "Geryon",
                    GeryonCharacter.BigPainClip,
                    null,
                    true
                );

                yield return new WaitForSeconds(1f);

                VoiceManager.PlayRandomVoice(
                    geryon, 
                    "Geryon",
                    GeryonCharacter.OverheatClips,
                    GeryonCharacter.OverheatSubs,
                    true
                );
            }
        }
    }


    [HarmonyPatch(typeof(Geryon), "Update")]
    class GeryonChatterPatch
    {
        static void Postfix(Geryon __instance)
        {
            if (!UltraVoicePlugin.GeryonVoiceEnabled.value)
                return;

            if (VoiceManager.CheckCooldown(__instance, 6f) && !VoiceManager.TooSoonAfterSpawn(__instance, 6f) && !__instance.stunned && __instance.active)

            if (Random.Range(0f, 1f) < 0.75f)
                VoiceManager.PlayRandomVoice(
                    __instance,
                    "Geryon",
                    GeryonCharacter.ChatterClips,
                    GeryonCharacter.ChatterSubs,
                    false
                );
        }
    }

    [HarmonyPatch(typeof(GameObject), "SetActive")]
    class GeryonDeathPatch
    {
        static void Postfix(GameObject __instance, bool value)
        {
            if (!UltraVoicePlugin.GeryonVoiceEnabled.value)
                return;

            if (__instance.name != "Geryon_Rig" || __instance.transform.parent.name != "Theatre (1)")
                return;

            UltraVoicePlugin.Instance.StartCoroutine(PlayDeath(__instance));

            static IEnumerator PlayDeath(GameObject geryon)
            {
                yield return new WaitForSeconds(0.2f);

                VoiceManager.CreateVoiceSource(
                        geryon.GetComponent(typeof(Animator)),
                        "Geryon",
                        GeryonCharacter.DeathClip,
                        null,
                        true
                    );
            }
        }
    }

    [HarmonyPatch(typeof(Geryon), "Death")]
    class GeryonFallPatch
    {
        static void Postfix(Geryon __instance)
        {
            if (!UltraVoicePlugin.GeryonVoiceEnabled.value)
                return;

            VoiceManager.CreateVoiceSource(
                __instance,
                "Geryon",
                GeryonCharacter.BigPainClip,
                null,
                true
            );
        }
    }

    [HarmonyPatch(typeof(Geryon), "Unstun")]
    class GeryonEnragePatch
    {
        static void Postfix(Geryon __instance)
        {
            if (!UltraVoicePlugin.GeryonVoiceEnabled.value)
                return;

            if (!__instance.secondPhase)
                VoiceManager.CreateVoiceSource(
                    __instance,
                    "Geryon",
                    GeryonCharacter.RecoverClip,
                    null,
                    true
                );

            else if (__instance.secondPhase && !GeryonCharacter.EnrageLinePlayed)
                UltraVoicePlugin.Instance.StartCoroutine(PlayEnrage(__instance));

            else
                VoiceManager.CreateVoiceSource(
                    __instance,
                    "Geryon",
                    GeryonCharacter.RecoverClip,
                    null,
                    true
                );

            static IEnumerator PlayEnrage(Geryon geryon)
            {
                VoiceManager.CreateVoiceSource(
                    geryon,
                    "Geryon",
                    GeryonCharacter.RecoverClip,
                    null,
                    true
                );

                yield return new WaitForSeconds(0.75f);

                if (!geryon.active)
                    yield break;

                var src = VoiceManager.CreateVoiceSource(
                    geryon,
                    "Geryon",
                    GeryonCharacter.EnrageClip,
                    "INSUFFERABLE...",
                    true
                );

                GeryonCharacter.EnrageLinePlayed = true;

                if (!geryon.active)
                    yield break;

                yield return new WaitForSeconds(1.25f);
                VoiceManager.ShowSubtitle(
                    "INFURIATING!",
                    src
                );

                if (!geryon.active)
                    yield break;

                yield return new WaitForSeconds(1.25f);
                VoiceManager.ShowSubtitle(
                    "DIE!",
                    src
                );
            }
        }
    }
}
