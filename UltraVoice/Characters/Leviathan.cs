using HarmonyLib;
using UnityEngine;
using UltraVoice.Utilities;

namespace UltraVoice.Characters
{
    public static class LeviathanCharacter
    {
        public static AudioClip[] ChatterClips;
        public static AudioClip BiteClip;
        public static AudioClip BiteWindupClip;
        public static AudioClip ParryClip;
        public static AudioClip RoarClip;
        public static AudioClip IntroRoarClip;
        public static AudioClip DeathClip;
        public static AudioClip DeathEndClip;

        public static readonly string[] ChatterSubs =
        {
            "DEVOUR IT",
            "TEAR IT APART",
            "SINK IN DESPAIR",
            "THE STYX WILL CLAIM YOU",
            "WE ARE HUNGRY",
        };

        public static void LoadVoiceLines(BepInEx.Logging.ManualLogSource logger)
        {
            ChatterClips = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip("Leviathan.lev_Chatter1.wav"),
                UltraVoicePlugin.LoadClip("Leviathan.lev_Chatter2.wav"),
                UltraVoicePlugin.LoadClip("Leviathan.lev_Chatter3.wav"),
                UltraVoicePlugin.LoadClip("Leviathan.lev_Chatter4.wav"),
                UltraVoicePlugin.LoadClip("Leviathan.lev_Chatter5.wav"),
            };

            BiteClip = UltraVoicePlugin.LoadClip("Leviathan.lev_Bite.wav");
            BiteWindupClip = UltraVoicePlugin.LoadClip("Leviathan.lev_BiteWindup.wav");
            ParryClip = UltraVoicePlugin.LoadClip("Leviathan.lev_Parried.wav");
            RoarClip = UltraVoicePlugin.LoadClip("Leviathan.lev_Roar.wav");
            IntroRoarClip = UltraVoicePlugin.LoadClip("Leviathan.lev_IntroRoar.wav");
            DeathClip = UltraVoicePlugin.LoadClip("Leviathan.lev_Death.wav");
            DeathEndClip = UltraVoicePlugin.LoadClip("Leviathan.lev_DeathEnd.wav");

            logger.LogInfo("Leviathan voice lines loaded successfully!");
        }

        [HarmonyPatch(typeof(LeviathanHead), nameof(LeviathanHead.StopAction))]
        class LeviathanChatterPatch
        {
            static void Prefix(LeviathanHead __instance)
            {
                if (!UltraVoicePlugin.LeviathanVoiceEnabled.value)
                    return;

                if (!VoiceManager.CheckCooldown(__instance, 15f))
                    return;

                if (!__instance.active)
                    return;

                __instance.attackCooldown = 3.5f;

                VoiceManager.PlayRandomVoice(__instance, "Leviathan",
                    ChatterClips,
                    ChatterSubs,
                    true,
                    volumeMult: 3f
                );

            }
        }

        [HarmonyPatch(typeof(LeviathanHead), nameof(LeviathanHead.Bite))]
        class LeviathanBiteWindupPatch
        {
            static void Postfix(LeviathanHead __instance)
            {
                if (!UltraVoicePlugin.LeviathanVoiceEnabled.value)
                    return;

                VoiceManager.CreateVoiceSource(__instance, "Leviathan",
                    BiteWindupClip,
                    null,
                    true,
                    volumeMult: 3f
                );
            }
        }

        [HarmonyPatch(typeof(LeviathanHead), nameof(LeviathanHead.Roar))]
        class LeviathanRoarPatch
        {
            static void Postfix(LeviathanHead __instance)
            {
                if (!UltraVoicePlugin.LeviathanVoiceEnabled.value)
                    return;

                VoiceManager.CreateVoiceSource(__instance, "Leviathan",
                    IntroRoarClip,
                    null,
                    true,
                    volumeMult: 3f
                );
            }
        }

        [HarmonyPatch(typeof(LeviathanHead), nameof(LeviathanHead.Ascend))]
        class LeviathanAscendPatch
        {
            static void Postfix(LeviathanHead __instance)
            {
                if (!UltraVoicePlugin.LeviathanVoiceEnabled.value)
                    return;

                VoiceManager.CreateVoiceSource(__instance, "Leviathan",
                    IntroRoarClip,
                    null,
                    true,
                    volumeMult: 3f
                );
            }
        }

        [HarmonyPatch(typeof(LeviathanHead), nameof(LeviathanHead.BiteDamageStart))]
        class LeviathanBitePatch
        {
            static void Postfix(LeviathanHead __instance)
            {
                if (!UltraVoicePlugin.LeviathanVoiceEnabled.value)
                    return;

                VoiceManager.CreateVoiceSource(__instance, "Leviathan",
                    BiteClip,
                    null,
                    true,
                    volumeMult: 3f
                );
            }
        }

        [HarmonyPatch(typeof(LeviathanHead), nameof(LeviathanHead.GotParried))]
        class LeviathanParryPatch
        {
            static void Postfix(LeviathanHead __instance)
            {
                if (!UltraVoicePlugin.LeviathanVoiceEnabled.value)
                    return;

                if (!__instance.active)
                    return;

                VoiceManager.CreateVoiceSource(__instance, "Leviathan",
                    ParryClip,
                    null,
                    true,
                    volumeMult: 3f
                );
            }
        }

        [HarmonyPatch(typeof(LeviathanHead), nameof(LeviathanHead.ProjectileBurst))]
        class LeviathanBarragePatch
        {
            static void Postfix(LeviathanHead __instance)
            {
                if (!UltraVoicePlugin.LeviathanVoiceEnabled.value)
                    return;

                VoiceManager.CreateVoiceSource(__instance, "Leviathan",
                    RoarClip,
                    null,
                    true,
                    volumeMult: 3f
                );
            }
        }

        [HarmonyPatch(typeof(LeviathanHead), nameof(LeviathanHead.BeamAttack))]
        class LeviathanBeamPatch
        {
            static void Postfix(LeviathanHead __instance)
            {
                if (!UltraVoicePlugin.LeviathanVoiceEnabled.value)
                    return;

                VoiceManager.CreateVoiceSource(__instance, "Leviathan",
                    RoarClip,
                    null,
                    true,
                    volumeMult: 3f
                );
            }
        }

        [HarmonyPatch(typeof(LeviathanHead), nameof(LeviathanHead.Death))]
        class LeviathanDeathPatch
        {
            static void Postfix(LeviathanHead __instance)
            {
                if (!UltraVoicePlugin.LeviathanVoiceEnabled.value)
                    return;

                VoiceManager.CreateVoiceSource(__instance, "Leviathan",
                    DeathClip,
                    null,
                    true,
                    volumeMult: 3f
                );
            }
        }

        [HarmonyPatch(typeof(LeviathanHead), nameof(LeviathanHead.HeadExplode))]
        class LeviathanDeathEndPatch
        {
            static void Postfix(LeviathanHead __instance)
            {
                if (!UltraVoicePlugin.LeviathanVoiceEnabled.value)
                    return;

                VoiceManager.CreateVoiceSource(__instance, "Leviathan",
                    DeathEndClip,
                    null,
                    true,
                    volumeMult: 3f
                );
            }
        }
    }
}