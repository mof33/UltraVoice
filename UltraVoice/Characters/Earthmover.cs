using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UltraVoice.Utilities;

namespace UltraVoice.Characters
{
    public class EarthmoverCharacter
    {
        // Voice line storage
        public static AudioClip IntroClip;
        public static AudioClip BloodVentClip;
        public static AudioClip DefenseSystemClip;
        public static AudioClip BloodFloodClip;
        public static AudioClip BrainBattleClip;
        public static AudioClip BrainKilledClip;

        public static void LoadVoiceLines(BepInEx.Logging.ManualLogSource logger)
        {
            IntroClip = UltraVoicePlugin.LoadClip("Earthmover.em_Intro.wav");
            BloodVentClip = UltraVoicePlugin.LoadClip("Earthmover.em_BloodVent.wav");
            DefenseSystemClip = UltraVoicePlugin.LoadClip("Earthmover.em_DefenseSystem.wav");
            BloodFloodClip = UltraVoicePlugin.LoadClip("Earthmover.em_BloodFlood.wav");
            BrainBattleClip = UltraVoicePlugin.LoadClip("Earthmover.em_BrainBattle.wav");
            BrainKilledClip = UltraVoicePlugin.LoadClip("Earthmover.em_BrainKilled.wav");

            logger.LogInfo("Earthmover voice lines loaded successfully!");
        }

        public static float lastBloodFloodTime;
        public static float lastBrainBattleTime;
        public static float lastSelfDestructTime;
    }

    [HarmonyPatch(typeof(AudioSource))]
    class EarthmoverPlayerDetectedPatch
    {
        static IEnumerable<MethodBase> TargetMethods()
        {
            return AccessTools
                .GetDeclaredMethods(typeof(AudioSource))
                .Where(method => !method.IsStatic && method.Name.Contains("Play"));
        }

        static void Postfix(AudioSource __instance)
        {
            if (!UltraVoicePlugin.EarthmoverVoiceEnabled.value)
                return;

            if (!VoiceManager.CheckCooldown(__instance, 0.1f))
                return;

            if (__instance == null || __instance.clip == null)
                return;

            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "6e981b1865c649749a610aafc471e198")
                return;

            if (__instance.clip.name == "Centaur A-1")
            {
                UltraVoicePlugin.Instance.StartCoroutine(Detected());
            }

            static IEnumerator Detected()
            {
                yield return new WaitForSeconds(1f);

                var src = VoiceManager.CreateVoiceSource(
                      MonoSingleton<NewMovement>.Instance,
                      "Earthmover_Intro",
                      EarthmoverCharacter.IntroClip,
                      "MULTIPLE MACHINE SIGNATURES PRESENT ON RADAR",
                      true,
                      volumeMult: 3f,
                      spatialBlend: 0f
                );

                yield return new WaitForSeconds(3f);

                VoiceManager.ShowSubtitle("THREAT LEVEL: MAXIMAL", src);

                yield return new WaitForSeconds(1.5f);

                VoiceManager.ShowSubtitle("READYING WEAPONS", src);
            }
        }
    }

    [HarmonyPatch(typeof(Crossfade), "StartFade")]
    class EarthmoverBloodVentPatch
    {
        static void Postfix(Crossfade __instance)
        {
            if (!UltraVoicePlugin.EarthmoverVoiceEnabled.value)
                return;

            if (!VoiceManager.CheckCooldown(__instance, 0.1f))
                return;

            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "6e981b1865c649749a610aafc471e198")
                return;

            if (__instance.to.clip.name == "Centaur A-2")
            {
                UltraVoicePlugin.Instance.StartCoroutine(BloodVent());
            }

            static IEnumerator BloodVent()
            {
                yield return new WaitForSeconds(0.5f);

                var src = VoiceManager.CreateVoiceSource(
                      MonoSingleton<NewMovement>.Instance,
                      "Earthmover_BloodVent",
                      EarthmoverCharacter.BloodVentClip,
                      "CONTAMINATED BLOOD DETECTED IN FUEL TANK B",
                      true,
                      volumeMult: 3f,
                      spatialBlend: 0f
                );

                yield return new WaitForSeconds(3.25f);

                VoiceManager.ShowSubtitle("OPENING VENTS", src);
            }
        }
    }

    [HarmonyPatch(typeof(CombinedBossBar), "OnEnable")]
    class EarthmoverDefenseSystemPatch
    {
        static void Postfix(CombinedBossBar __instance)
        {
            if (!UltraVoicePlugin.EarthmoverVoiceEnabled.value)
                return;

            if (!VoiceManager.CheckCooldown(__instance, 0.1f))
                return;

            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "6e981b1865c649749a610aafc471e198")
                return;

            if (__instance.name == "SecuritySystem")
            {
                UltraVoicePlugin.Instance.StartCoroutine(DefenseSystem());
            }

            static IEnumerator DefenseSystem()
            {
                yield return new WaitForSeconds(0.1f);

                var src = VoiceManager.CreateVoiceSource(
                      MonoSingleton<NewMovement>.Instance,
                      "Earthmover_DefenseSystem",
                      EarthmoverCharacter.DefenseSystemClip,
                      "UNAUTHORIZED PERSONNEL DETECTED AT INTERIOR ENTRANCE",
                      true,
                      volumeMult: 3f,
                      spatialBlend: 0f
                );

                yield return new WaitForSeconds(3.75f);

                VoiceManager.ShowSubtitle("ENGAGING DEFENSE SYSTEM", src);
            }
        }
    }

    [HarmonyPatch(typeof(GameObject), "SetActive")]
    class EarthmoverBloodFloodPatch
    {
        static void Postfix(GameObject __instance)
        {
            if (!UltraVoicePlugin.EarthmoverVoiceEnabled.value)
                return;

            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "6e981b1865c649749a610aafc471e198")
                return;

            if (__instance == null)
                return;

            if (__instance.name == "SecondBeats")
            {
                if (Time.time - EarthmoverCharacter.lastBloodFloodTime < 0.1f)
                    return;

                Transform alarmTransform = __instance.transform.Find("Alarm");
                if (alarmTransform == null)
                    return;

                AudioSource alarmSource = alarmTransform.GetComponent<AudioSource>();
                if (alarmSource == null || !alarmSource.isPlaying)
                    return;

                EarthmoverCharacter.lastBloodFloodTime = Time.time;
                UltraVoicePlugin.Instance.StartCoroutine(BloodFlood());
            }

            static IEnumerator BloodFlood()
            {
                var src = VoiceManager.CreateVoiceSource(
                      MonoSingleton<NewMovement>.Instance,
                      "Earthmover_BloodFlood",
                      EarthmoverCharacter.BloodFloodClip,
                      "UNAUTHORIZED PERSONNEL HAS BREACHED INTERIOR ENTRANCE",
                      true,
                      volumeMult: 3f,
                      spatialBlend: 0f
                );

                yield return new WaitForSeconds(2.75f);

                VoiceManager.ShowSubtitle("FLUSHING INTERIOR", src);
            }
        }
    }

    [HarmonyPatch(typeof(GameObject), "SetActive")]
    class EarthmoverBrainBattlePatch
    {
        static void Postfix(GameObject __instance)
        {
            if (!UltraVoicePlugin.EarthmoverVoiceEnabled.value)
                return;

            if (__instance == null || !__instance.activeSelf)
                return;

            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "6e981b1865c649749a610aafc471e198")
                return;

            if (__instance.name == "Beams" && __instance.transform.parent.parent.name == "LaserRing")
            {
                if (Time.time - EarthmoverCharacter.lastBrainBattleTime < 0.1f)
                    return;

                EarthmoverCharacter.lastBrainBattleTime = Time.time;
                UltraVoicePlugin.Instance.StartCoroutine(BrainBattle());
            }

            static IEnumerator BrainBattle()
            {
                var src = VoiceManager.CreateVoiceSource(
                      MonoSingleton<NewMovement>.Instance,
                      "Earthmover_BrainBattle",
                      EarthmoverCharacter.BrainBattleClip,
                      "UNAUTHORIZED PERSONNEL HAS BREACHED CENTRAL CONTROL TOWER",
                      true,
                      volumeMult: 3f,
                      spatialBlend: 0f
                );

                yield return new WaitForSeconds(3f);

                VoiceManager.ShowSubtitle("ENGAGING DEFENSE PROTOCOLS", src);
            }
        }
    }

    [HarmonyPatch(typeof(GameObject), "SetActive")]
    class EarthmoverSelfDestructPatch
    {
        static void Postfix(GameObject __instance)
        {
            if (__instance == null || !__instance.activeSelf)
                return;

            if (!UltraVoicePlugin.EarthmoverVoiceEnabled.value)
                return;

            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "6e981b1865c649749a610aafc471e198")
                return;

            if (__instance.name == "Countdown")
            {
                if (Time.time - EarthmoverCharacter.lastSelfDestructTime < 0.1f)
                    return;

                EarthmoverCharacter.lastSelfDestructTime = Time.time;
                UltraVoicePlugin.Instance.StartCoroutine(SelfDestruct());
            }

            static IEnumerator SelfDestruct()
            {
                yield return new WaitForSeconds(0.25f);

                var src = VoiceManager.CreateVoiceSource(
                      MonoSingleton<NewMovement>.Instance,
                      "Earthmover_SelfDestruct",
                      EarthmoverCharacter.BrainKilledClip,
                      "CRITICAL DAMAGE DETECTED IN CENTRAL CONTROL TOWER!",
                      true,
                      volumeMult: 3f,
                      spatialBlend: 0f
                );

                yield return new WaitForSeconds(3.25f);

                VoiceManager.ShowSubtitle("SELF-DESTRUCT SEQUENCE INITIATED!", src);

                yield return new WaitForSeconds(2.25f);

                VoiceManager.ShowSubtitle("ALL PERSONNEL EVACUATE IMMEDIATELY!", src);
            }
        }
    }
}