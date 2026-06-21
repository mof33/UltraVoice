using HarmonyLib;
using System.Collections;
using UnityEngine;
using UltraVoice.Utilities;

namespace UltraVoice.Characters
{
    public class HideousMassCharacter
    {
        public static AudioClip AwakenClip;
        public static AudioClip[] ChatterClips;
        public static AudioClip[] EnrageClips;
        public static AudioClip[] HarpoonClips;
        public static AudioClip[] ParryClips;
        public static AudioClip[] DeathClips;

        public static readonly string[] ChatterSubs =
        {
            "KILL, KILL",
            "HURT, HURT",
            "DIE, DIE",
            "NO MERCY",
            "CRUSH, CRUSH"
        };

        public static readonly string[] EnrageSubs =
        {
            "HATE, HATE",
            "JUST DIE",
            null,
            null
        };

        public static readonly string[] HarpoonSubs =
        {
            "STAY STILL",
            "STOP MOVING",
            "DON'T MOVE"
        };

        public static readonly string[] ParrySubs =
        {
            null,
            null,
            "PAIN"
        };

        public static void LoadVoiceLines(BepInEx.Logging.ManualLogSource logger)
        {
            AwakenClip = UltraVoicePlugin.LoadClip("HideousMass.mass_SpawnSpecial.wav");

            ChatterClips = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip("HideousMass.mass_Generic1.wav"),
                UltraVoicePlugin.LoadClip("HideousMass.mass_Generic2.wav"),
                UltraVoicePlugin.LoadClip("HideousMass.mass_Generic3.wav"),
                UltraVoicePlugin.LoadClip("HideousMass.mass_Generic4.wav"),
                UltraVoicePlugin.LoadClip("HideousMass.mass_Generic5.wav")
            };

            EnrageClips = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip("HideousMass.mass_Enrage1.wav"),
                UltraVoicePlugin.LoadClip("HideousMass.mass_Enrage2.wav"),
                UltraVoicePlugin.LoadClip("HideousMass.mass_Enrage3.wav"),
                UltraVoicePlugin.LoadClip("HideousMass.mass_Enrage4.wav"),
            };

            HarpoonClips = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip("HideousMass.mass_Harpoon1.wav"),
                UltraVoicePlugin.LoadClip("HideousMass.mass_Harpoon2.wav"),
                UltraVoicePlugin.LoadClip("HideousMass.mass_Harpoon3.wav"),
            };

            ParryClips = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip("HideousMass.mass_Parried1.wav"),
                UltraVoicePlugin.LoadClip("HideousMass.mass_Parried2.wav"),
                UltraVoicePlugin.LoadClip("HideousMass.mass_Parried3.wav"),
            };

            DeathClips = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip("HideousMass.mass_Death1.wav"),
                UltraVoicePlugin.LoadClip("HideousMass.mass_Death2.wav"),
            };

            logger.LogInfo("Hideous Mass voice lines loaded successfully!");
        }
    }

    [HarmonyPatch(typeof(Mass), "Update")]
    class MassChatterPatch
    {
        static void Postfix(Mass __instance)
        {
            if (!UltraVoicePlugin.MassVoiceEnabled.value)
                return;

            if (ULTRAKILL.Cheats.BlindEnemies.Blind)
                return;

            if (!VoiceManager.CheckCooldown(__instance, 6f))
                return;

            if (__instance == null || __instance.isDead)
                return;

            if (UnityEngine.Random.Range(0f, 1f) < 0.75f)
                VoiceManager.PlayRandomVoice(__instance, "HideousMass",
                    HideousMassCharacter.ChatterClips,
                    HideousMassCharacter.ChatterSubs,
                    randomPitch: true
                );
        }
    }

    [HarmonyPatch(typeof(Mass), nameof(Mass.ReadySpear))]
    class MassHarpoonPatch
    {
        static void Postfix(Mass __instance)
        {
            if (!UltraVoicePlugin.MassVoiceEnabled.value)
                return;

            if (__instance == null || __instance.eid.dead)
                return;

            VoiceManager.PlayRandomVoice(__instance, "HideousMass",
                    HideousMassCharacter.HarpoonClips,
                    HideousMassCharacter.HarpoonSubs,
                    randomPitch: true
            );
        }
    }

    [HarmonyPatch(typeof(FakeMassActivator), nameof(FakeMassActivator.OnEnable))]
    class MassActivatePatch
    {
        static void Postfix(Mass __instance)
        {
            if (!UltraVoicePlugin.MassVoiceEnabled.value)
                return;

            if (__instance == null || __instance.eid.dead)
                return;

            UltraVoicePlugin.Instance.StartCoroutine(PlayAwaken(__instance));

            static IEnumerator PlayAwaken(Mass mass)
            {
                yield return new WaitForSeconds(0.75f);

                VoiceManager.CreateVoiceSource(mass, "HideousMass",
                        HideousMassCharacter.AwakenClip,
                        "WHO DARES DISTURB ME",
                        subtitleColor: new UnityEngine.Color(0.67f, 0.57f, 0.57f),
                        randomPitch: true
                );
            }
        }
    }

    [HarmonyPatch(typeof(Mass), nameof(Mass.SpearParried))]
    class MassSpearParryPatch
    {
        static void Postfix(Mass __instance)
        {
            if (!UltraVoicePlugin.MassVoiceEnabled.value)
                return;

            if (__instance == null || __instance.eid.dead)
                return;

            VoiceManager.PlayRandomVoice(__instance, "HideousMass",
                    HideousMassCharacter.ParryClips,
                    HideousMassCharacter.ParrySubs,
                    true,
                    randomPitch: true
            );
        }
    }

    [HarmonyPatch(typeof(Mass), nameof(Mass.Enrage))]
    class MassEnragePatch
    {
        static void Postfix(Mass __instance)
        {
            if (!UltraVoicePlugin.MassVoiceEnabled.value)
                return;

            if (__instance == null || __instance.eid.dead)
                return;

            VoiceManager.PlayRandomVoice(__instance, "HideousMass",
                    HideousMassCharacter.EnrageClips,
                    HideousMassCharacter.EnrageSubs,
                    randomPitch: true
            );
        }
    }

    [HarmonyPatch(typeof(Mass), nameof(Mass.OnGoLimp))]
    class MassDeathPatch
    {
        static void Postfix(Mass __instance)
        {
            if (!UltraVoicePlugin.MassVoiceEnabled.value)
                return;

            if (__instance == null || __instance.eid.dead)
                return;

            UltraVoicePlugin.Instance.StartCoroutine(PlayDeath(__instance));

            static IEnumerator PlayDeath(Mass sm)
            {
                yield return new WaitForSeconds(1f);

                VoiceManager.PlayRandomVoice(sm, "HideousMass",
                        HideousMassCharacter.DeathClips,
                        null,
                        true,
                        randomPitch: true
                );
            }
        }
    }
}
