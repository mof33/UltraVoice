using HarmonyLib;
using System.Collections;
using UnityEngine;
using UltraVoice.Utilities;

namespace UltraVoice.Characters
{
    public class GuttertankCharacter
    {
        public static AudioClip[] SpawnClips;
        public static AudioClip AttackClip;
        public static AudioClip[] PunchHitClips;
        public static AudioClip[] FrustratedClips;
        public static AudioClip[] DeathClips;
        public static AudioClip[] TripPainClips;

        public static readonly string[] SpawnSubs =
        {
            "WEAPONS ONLINE.",
            "READY FOR COMBAT.",
            "ENGAGING PROTOCOLS.",
            "ALL SYSTEMS OPERATIONAL."
        };

        public static readonly string[] PunchHitSubs =
        {
            "DIRECT HIT.",
            "HIT CONFIRMED.",
            "TRY THAT AGAIN."
        };

        public static readonly string[] FrustratedSubs =
        {
            "DAMN IT!",
            "VERDAMMT!",
            "UGH, MY HEAD…",
            "SCHEIẞE!"
        };

        public static bool GuttertankSpawnInMirror = false;

        public static void LoadVoiceLines(AssetBundle bundle, BepInEx.Logging.ManualLogSource logger)
        {
            SpawnClips = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip(bundle, "gt_Spawn1"),
                UltraVoicePlugin.LoadClip(bundle, "gt_Spawn2"),
                UltraVoicePlugin.LoadClip(bundle, "gt_Spawn3"),
                UltraVoicePlugin.LoadClip(bundle, "gt_Spawn4")
            };

            AttackClip = UltraVoicePlugin.LoadClip(bundle, "gt_Attack");

            PunchHitClips = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip(bundle, "gt_PunchHit1"),
                UltraVoicePlugin.LoadClip(bundle, "gt_PunchHit2"),
                UltraVoicePlugin.LoadClip(bundle, "gt_PunchHit3")
            };

            FrustratedClips = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip(bundle, "gt_PunchTrip1"),
                UltraVoicePlugin.LoadClip(bundle, "gt_PunchTrip2"),
                UltraVoicePlugin.LoadClip(bundle, "gt_PunchTrip3"),
                UltraVoicePlugin.LoadClip(bundle, "gt_PunchTrip4")
            };

            DeathClips = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip(bundle, "gt_Death1"),
                UltraVoicePlugin.LoadClip(bundle, "gt_Death2"),
                UltraVoicePlugin.LoadClip(bundle, "gt_Death3")
            };

            TripPainClips = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip(bundle, "gt_TripPain1"),
                UltraVoicePlugin.LoadClip(bundle, "gt_TripPain2"),
                UltraVoicePlugin.LoadClip(bundle, "gt_TripPain3"),
            };

            logger.LogInfo("Guttertank voice lines loaded successfully!");
        }

}

    // GUTTERTANK PATCHES

    [HarmonyPatch(typeof(Guttertank), "Start")]
    class GuttertankSpawnPatch
    {
        static void Postfix(Guttertank __instance)
        {
            if (!UltraVoicePlugin.GuttertankVoiceEnabled.value) return;

            if (__instance == null)
                return;

            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "45addc6c3730dae418321e00af1116c5" && !GuttertankCharacter.GuttertankSpawnInMirror) // Fraud Second Scene
            {
                GuttertankCharacter.GuttertankSpawnInMirror = true;
                VoiceManager.enemySpawnTimes[__instance] = Time.time;
                return;
            }

            VoiceManager.enemySpawnTimes[__instance] = Time.time;

            UltraVoicePlugin.Instance.StartCoroutine(UltraVoicePlugin.DelayedVox(() =>
                VoiceManager.PlayRandomVoice(__instance, "Guttertank",
                    GuttertankCharacter.SpawnClips,
                    GuttertankCharacter.SpawnSubs,
                    false
                ),
                () => GuttertankCharacter.SpawnClips != null && GuttertankCharacter.SpawnClips.Length > 0,
                __instance
            ));
        }
    }

    [HarmonyPatch(typeof(Guttertank), "TargetBeenHit")]
    class GuttertankPunchHitPatch
    {
        static void Postfix(Guttertank __instance)
        {
            if (!UltraVoicePlugin.GuttertankVoiceEnabled.value) return;

            if (__instance == null)
                return;

            UltraVoicePlugin.Instance.StartCoroutine(UltraVoicePlugin.DelayedVox(() =>
                        VoiceManager.PlayRandomVoice(__instance, "Guttertank",
                            GuttertankCharacter.PunchHitClips,
                            GuttertankCharacter.PunchHitSubs
                        ),
                    () => GuttertankCharacter.PunchHitClips != null && GuttertankCharacter.PunchHitClips.Length > 0,
                    __instance
                ));
        }
    }

    [HarmonyPatch(typeof(Guttertank), "FallImpact")]
    class GuttertankFallImpactPatch
    {
        static void Postfix(Guttertank __instance)
        {
            if (!UltraVoicePlugin.GuttertankVoiceEnabled.value) return;

            UltraVoicePlugin.Instance.StartCoroutine(UltraVoicePlugin.DelayedVox(() =>
                        VoiceManager.PlayRandomVoice(__instance, "Guttertank",
                            GuttertankCharacter.TripPainClips,
                            null
                        ),
                    () => GuttertankCharacter.TripPainClips != null && GuttertankCharacter.TripPainClips.Length > 0,
                    __instance
                ));

            if (__instance.dead || __instance == null) return;

            UltraVoicePlugin.Instance.StartCoroutine(DelayedPunchTripVox(__instance));
        }

        static IEnumerator DelayedPunchTripVox(Guttertank tank)
        {
            if (tank.dead || tank == null) yield break;

            yield return new WaitForSeconds(0.75f);

            VoiceManager.PlayRandomVoice(tank, "Guttertank",
                GuttertankCharacter.FrustratedClips,
                GuttertankCharacter.FrustratedSubs,
                true
            );
        }
    }

    [HarmonyPatch(typeof(Guttertank), "Death")]
    class GuttertankDeathPatch
    {
        static void Postfix(Guttertank __instance)
        {
            if (!UltraVoicePlugin.GuttertankVoiceEnabled.value) return;

            if (__instance == null)
                return;

            UltraVoicePlugin.Instance.StartCoroutine(UltraVoicePlugin.DelayedVox(() =>
                        VoiceManager.PlayRandomVoice(__instance, "Guttertank",
                            GuttertankCharacter.DeathClips,
                            null,
                            true
                        ),
                    () => GuttertankCharacter.DeathClips != null && GuttertankCharacter.DeathClips.Length > 0,
                    __instance
                ));
        }
    }
}