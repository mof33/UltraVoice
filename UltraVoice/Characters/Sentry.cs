using HarmonyLib;
using System.Collections;
using UnityEngine;
using UltraVoice.Utilities;
using System;

namespace UltraVoice.Characters
{
    public class SentryCharacter
    {
        // Voice line storage
        public static AudioClip[] DigInClips;
        public static AudioClip[] ShootClips;
        public static AudioClip[] InterruptPainClips;
        public static AudioClip[] InterruptClips;
        public static AudioClip[] KickClips;
        public static AudioClip[] DeathClips;

        public static AudioClip[] DigInClipsGoob;
        public static AudioClip[] ShootClipsGoob;
        public static AudioClip[] InterruptPainClipsGoob;
        public static AudioClip[] InterruptClipsGoob;
        public static AudioClip[] KickClipsGoob;
        public static AudioClip[] DeathClipsGoob;

        public static readonly string[] DigInSubs =
        {
            "This'll do!",
            "This spot's perfect!",
            "Now… to wait.",
            "Let's make this quick.",
            "Lockin' in.",
            "Holding here!",
        };

        public static readonly string[] ShootSubs =
        {
            "Dodge this!",
            "Now!",
            "Fire!"
        };

        public static readonly string[] KickSubs =
        {
            "Outta my face!",
            "Back off!",
            "Shoo!",
        };

        public static readonly string[] InterruptSubs =
        {
            "What the- hey!",
            "The hell!?",
            "Oh, you little...",
            "Oh, come on!",
            "Seriously!?"
        };

        public static AudioClip[] UseSentryClips(AudioClip[] notoClips, AudioClip[] goobClips)
        {
            return UltraVoicePlugin.SentryVoiceActorField != null && UltraVoicePlugin.SentryVoiceActorField.value == UltraVoicePlugin.SentryVoiceActor.Goober
                ? goobClips
                : notoClips;
        }

        public static void LoadVoiceLines(BepInEx.Logging.ManualLogSource logger)
        {
            DigInClips = new[]
            {
                UltraVoicePlugin.LoadClip("Sentry.tur_DigIn1.wav"),
                UltraVoicePlugin.LoadClip("Sentry.tur_DigIn2.wav"),
                UltraVoicePlugin.LoadClip("Sentry.tur_DigIn3.wav"),
                UltraVoicePlugin.LoadClip("Sentry.tur_DigIn4.wav"),
                UltraVoicePlugin.LoadClip("Sentry.tur_DigIn5.wav"),
                UltraVoicePlugin.LoadClip("Sentry.tur_DigIn6.wav")
            };

            InterruptPainClips = new[]
            {
                UltraVoicePlugin.LoadClip("Sentry.tur_Pain1.wav"),
                UltraVoicePlugin.LoadClip("Sentry.tur_Pain2.wav"),
            };

            InterruptClips = new[]
            {
                UltraVoicePlugin.LoadClip("Sentry.tur_Interrupt1.wav"),
                UltraVoicePlugin.LoadClip("Sentry.tur_Interrupt2.wav"),
                UltraVoicePlugin.LoadClip("Sentry.tur_Interrupt3.wav"),
                UltraVoicePlugin.LoadClip("Sentry.tur_Interrupt4.wav"),
                UltraVoicePlugin.LoadClip("Sentry.tur_Interrupt5.wav")
            };

            ShootClips = new[]
            {
                UltraVoicePlugin.LoadClip("Sentry.tur_Fire1.wav"),
                UltraVoicePlugin.LoadClip("Sentry.tur_Fire2.wav"),
                UltraVoicePlugin.LoadClip("Sentry.tur_Fire3.wav")
            };

            KickClips = new[]
            {
                UltraVoicePlugin.LoadClip("Sentry.tur_Kick1.wav"),
                UltraVoicePlugin.LoadClip("Sentry.tur_Kick2.wav"),
                UltraVoicePlugin.LoadClip("Sentry.tur_Kick3.wav")
            };

            DeathClips = new[]
            {
                UltraVoicePlugin.LoadClip("Sentry.tur_Death1.wav"),
                UltraVoicePlugin.LoadClip("Sentry.tur_Death2.wav"),
            };

            DigInClipsGoob = new[]
            {
                UltraVoicePlugin.LoadClip("Sentry.tur_DigIn1Goob.wav"),
                UltraVoicePlugin.LoadClip("Sentry.tur_DigIn2Goob.wav"),
                UltraVoicePlugin.LoadClip("Sentry.tur_DigIn3Goob.wav"),
                UltraVoicePlugin.LoadClip("Sentry.tur_DigIn4Goob.wav"),
                UltraVoicePlugin.LoadClip("Sentry.tur_DigIn5Goob.wav"),
                UltraVoicePlugin.LoadClip("Sentry.tur_DigIn6Goob.wav")
            };

            ShootClipsGoob = new[]
            {
                UltraVoicePlugin.LoadClip("Sentry.tur_Fire1Goob.wav"),
                UltraVoicePlugin.LoadClip("Sentry.tur_Fire2Goob.wav"),
                UltraVoicePlugin.LoadClip("Sentry.tur_Fire3Goob.wav")
            };

            InterruptPainClipsGoob = new[]
            {
                UltraVoicePlugin.LoadClip("Sentry.tur_Pain1Goob.wav"),
                UltraVoicePlugin.LoadClip("Sentry.tur_Pain2Goob.wav"),
            };

            InterruptClipsGoob = new[]
            {
                UltraVoicePlugin.LoadClip("Sentry.tur_Interrupt1Goob.wav"),
                UltraVoicePlugin.LoadClip("Sentry.tur_Interrupt2Goob.wav"),
                UltraVoicePlugin.LoadClip("Sentry.tur_Interrupt3Goob.wav"),
                UltraVoicePlugin.LoadClip("Sentry.tur_Interrupt4Goob.wav"),
                UltraVoicePlugin.LoadClip("Sentry.tur_Interrupt5Goob.wav")
            };

            KickClipsGoob = new[]
            {
                UltraVoicePlugin.LoadClip("Sentry.tur_Kick1Goob.wav"),
                UltraVoicePlugin.LoadClip("Sentry.tur_Kick2Goob.wav"),
                UltraVoicePlugin.LoadClip("Sentry.tur_Kick3Goob.wav")
            };

            DeathClipsGoob = new[]
            {
                UltraVoicePlugin.LoadClip("Sentry.tur_Death1Goob.wav"),
                UltraVoicePlugin.LoadClip("Sentry.tur_Death2Goob.wav"),
            };

            logger.LogInfo("Sentry voice lines loaded successfully!");
        }
    }

    [HarmonyPatch(typeof(Turret), "StartAiming")]
    class SentryStartAimingPatch
    {
        static void Postfix(Turret __instance)
        {
            if (!UltraVoicePlugin.SentryVoiceEnabled.value) return;

            VoiceManager.PlayRandomVoice(__instance, "Sentry",
                SentryCharacter.UseSentryClips(SentryCharacter.DigInClips, SentryCharacter.DigInClipsGoob),
                SentryCharacter.DigInSubs,
                false,
                randomPitch: true
            );
        }
    }

    [HarmonyPatch(typeof(Turret), "Shoot")]
    class SentryShootPatch
    {
        static void Postfix(Turret __instance)
        {
            if (!UltraVoicePlugin.SentryVoiceEnabled.value) return;

            VoiceManager.PlayRandomVoice(__instance, "Sentry",
                SentryCharacter.UseSentryClips(SentryCharacter.ShootClips, SentryCharacter.ShootClipsGoob),
                SentryCharacter.ShootSubs,
                false,
                randomPitch: true
            );
        }
    }

    [HarmonyPatch(typeof(Turret), "Kick")]
    class SentryKickPatch
    {
        static void Postfix(Turret __instance)
        {
            if (!UltraVoicePlugin.SentryVoiceEnabled.value) return;

            VoiceManager.PlayRandomVoice(__instance, "Sentry",
                SentryCharacter.UseSentryClips(SentryCharacter.KickClips, SentryCharacter.KickClipsGoob),
                SentryCharacter.KickSubs,
                false,
                randomPitch: true
            );
        }
    }

    [HarmonyPatch(typeof(Turret), "Interrupt")]
    class SentryInterruptPatch
    {
        static void Postfix(Turret __instance)
        {
            if (!UltraVoicePlugin.SentryVoiceEnabled.value) return;

            VoiceManager.PlayRandomVoice(__instance, "Sentry",
                SentryCharacter.UseSentryClips(SentryCharacter.InterruptPainClips, SentryCharacter.InterruptPainClipsGoob),
                null,
                true,
                randomPitch: true
            );
            
            UltraVoicePlugin.Instance.StartCoroutine(Interrupt(__instance));

            static IEnumerator Interrupt(Turret turret)
            {
                yield return new WaitForSeconds(0.75f);

                if (turret == null)
                    yield break;

                VoiceManager.PlayRandomVoice(turret, "Sentry",
                    SentryCharacter.UseSentryClips(SentryCharacter.InterruptClips, SentryCharacter.InterruptClipsGoob),
                    SentryCharacter.InterruptSubs,
                    true,
                    randomPitch: true
                );
            }
        }
    }

    [HarmonyPatch(typeof(Turret), nameof(Turret.ShouldKnockback))]
    class SentryShouldKnockbackPatch
    {
        static void Postfix(Turret __instance)
        {
            if (!UltraVoicePlugin.SentryVoiceEnabled.value) return;

            if (__instance.aiming) return;

            if (!VoiceManager.CheckCooldown(__instance, 2f))
                return;

            VoiceManager.PlayRandomVoice(
                __instance,
                "Sentry",
                SentryCharacter.UseSentryClips(
                    SentryCharacter.InterruptPainClips,
                    SentryCharacter.InterruptPainClipsGoob
                ),
                null,
                true,
                randomPitch: true
            );
        }
    }

    [HarmonyPatch(typeof(Turret), "OnDeath")]
    class SentryDeathPatch
    {
        static void Postfix(Turret __instance)
        {
            if (!UltraVoicePlugin.SentryVoiceEnabled.value) return;

            VoiceManager.PlayRandomVoice(__instance, "Sentry",
                SentryCharacter.UseSentryClips(SentryCharacter.DeathClips, SentryCharacter.DeathClipsGoob),
                null,
                true,
                randomPitch: true
            );
        }
    }
}
