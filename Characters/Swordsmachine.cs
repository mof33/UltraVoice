using HarmonyLib;
using System.Collections;
using UnityEngine;
using UltraVoice.Utilities;

namespace UltraVoice.Characters
{
    public class SwordsmachineCharacter
    {
        // Voice line storage
        public static AudioClip IntroClip;
        public static AudioClip IntroClipSecond;
        public static AudioClip BigPainClip;
        public static AudioClip LungeClip;
        public static AudioClip ComboClip;
        public static AudioClip DeathClip;
        public static AudioClip KnockdownClipSpecial;

        public static AudioClip AgonySpawnClip;
        public static AudioClip TundraSpawnClip;
        public static AudioClip AgonyKnockdownClip;
        public static AudioClip TundraKnockdownClip;

        public static AudioClip[] SpawnClips;
        public static AudioClip[] EnrageClips;
        public static AudioClip[] KnockdownClips;
        public static AudioClip[] RangedClips;

        public static AudioClip IntroClipNoto;
        public static AudioClip IntroClipSecondNoto;
        public static AudioClip BigPainClipNoto;
        public static AudioClip LungeClipNoto;
        public static AudioClip ComboClipNoto;
        public static AudioClip DeathClipNoto;
        public static AudioClip KnockdownClipSpecialNoto;

        public static AudioClip AgonySpawnClipNoto;
        public static AudioClip TundraSpawnClipNoto;
        public static AudioClip AgonyKnockdownClipNoto;
        public static AudioClip TundraKnockdownClipNoto;

        public static AudioClip[] SpawnClipsNoto;
        public static AudioClip[] EnrageClipsNoto;
        public static AudioClip[] KnockdownClipsNoto;
        public static AudioClip[] RangedClipsNoto;

        // Subtitle storage
        public static readonly string[] EnrageSubs =
        {
            "GRRR",
            "GRRR",
            "MOTHERFUCKER",
            "OHOHO, YOU ARE SO DEAD"
        };

        public static readonly string[] EnrageSubs2 =
        {
            "I'LL KICK YOUR ASS!",
            "I'LL KILL YOU!",
            null,
            null
        };

        public static readonly string[] SpawnSubs =
        {
            "COME ON, COME ON",
            "LET'S SEE SOME BLOOD",
            "WHO'S READY TO FIGHT",
            "I SMELL BLOOD",
            "I'LL CUT YOU ALL DOWN",
            "FRESH BLOOD",
            "MORE MEAT FOR THE SLAUGHTER",
        };

        public static readonly string[] KnockdownSubs =
        {
            "IS THAT ALL YOU GOT?",
            "I AIN'T DONE WITH YOU YET!",
            "I'M JUST GETTING STARTED"
        };

        public static readonly string[] RangedSubs =
        {
            "CATCH THIS",
            "TAKE THIS",
        };

        public static UnityEngine.Color SwordsmachineColor = new UnityEngine.Color(0.91f, 0.6f, 0.05f);
        public static UnityEngine.Color AgonyColor = new UnityEngine.Color(0.79f, 0.17f, 0.17f);
        public static UnityEngine.Color TundraColor = new UnityEngine.Color(0.2f, 0.73f, 0.87f);

        public static bool FirstFightDone = false;
        public static bool FirstFightLinePlayed = false;

        public static bool IsAgony(SwordsMachine sm)
        {
            if (sm == null) return false;
            string n = sm.gameObject.name;
            return n.Contains("Agony");
        }

        public static bool IsTundra(SwordsMachine sm)
        {
            if (sm == null) return false;
            string n = sm.gameObject.name;
            return n.Contains("Tundra");
        }

        public static bool IsAgonyOrTundra(SwordsMachine sm)
        {
            return IsAgony(sm) || IsTundra(sm);
        }

        public static UnityEngine.Color? GetColorOverride(SwordsMachine sm)
        {
            if (IsAgony(sm))
                return AgonyColor;
            if (IsTundra(sm))
                return TundraColor;
            else return SwordsmachineColor;
        }

        public static AudioClip UseSwordsmachineClip(AudioClip mofClip, AudioClip notoClip)
        {
            return UltraVoicePlugin.SwordsmachineVoiceActorField != null && UltraVoicePlugin.SwordsmachineVoiceActorField.value == UltraVoicePlugin.SwordsmachineVoiceActor.Noto
                ? notoClip
                : mofClip;
        }

        public static AudioClip[] UseSwordsmachineClips(AudioClip[] mofClips, AudioClip[] notoClips)
        {
            return UltraVoicePlugin.SwordsmachineVoiceActorField != null && UltraVoicePlugin.SwordsmachineVoiceActorField.value == UltraVoicePlugin.SwordsmachineVoiceActor.Noto
                ? notoClips
                : mofClips;
        }

        public static void LoadVoiceLines(AssetBundle bundle, BepInEx.Logging.ManualLogSource logger)
        {
            IntroClip = UltraVoicePlugin.LoadClip(bundle, "sm_SpawnSpecial");
            IntroClipSecond = UltraVoicePlugin.LoadClip(bundle, "sm_SpawnSpecial2");
            BigPainClip = UltraVoicePlugin.LoadClip(bundle, "sm_BigPain");
            LungeClip = UltraVoicePlugin.LoadClip(bundle, "sm_Lunge");
            ComboClip = UltraVoicePlugin.LoadClip(bundle, "sm_Combo");
            DeathClip = UltraVoicePlugin.LoadClip(bundle, "sm_Death");
            KnockdownClipSpecial = UltraVoicePlugin.LoadClip(bundle, "sm_KnockdownSpecial");

            AgonySpawnClip = UltraVoicePlugin.LoadClip(bundle, "sm_SpawnSpecialAgony");
            TundraSpawnClip = UltraVoicePlugin.LoadClip(bundle, "sm_SpawnSpecialTundra");
            AgonyKnockdownClip = UltraVoicePlugin.LoadClip(bundle, "sm_DownedAgony");
            TundraKnockdownClip = UltraVoicePlugin.LoadClip(bundle, "sm_DownedTundra");

            SpawnClips = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip(bundle, "sm_Spawn1"),
                UltraVoicePlugin.LoadClip(bundle, "sm_Spawn2"),
                UltraVoicePlugin.LoadClip(bundle, "sm_Spawn3"),
                UltraVoicePlugin.LoadClip(bundle, "sm_Spawn4"),
                UltraVoicePlugin.LoadClip(bundle, "sm_Spawn5"),
                UltraVoicePlugin.LoadClip(bundle, "sm_Spawn6"),
                UltraVoicePlugin.LoadClip(bundle, "sm_Spawn7"),
            };

            EnrageClips = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip(bundle, "sm_Enrage1"),
                UltraVoicePlugin.LoadClip(bundle, "sm_Enrage2"),
                UltraVoicePlugin.LoadClip(bundle, "sm_Enrage3"),
                UltraVoicePlugin.LoadClip(bundle, "sm_Enrage4")
            };

            KnockdownClips = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip(bundle, "sm_Knockdown1"),
                UltraVoicePlugin.LoadClip(bundle, "sm_Knockdown2"),
                UltraVoicePlugin.LoadClip(bundle, "sm_Knockdown3")
            };

            RangedClips = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip(bundle, "sm_Ranged1"),
                UltraVoicePlugin.LoadClip(bundle, "sm_Ranged2"),
            };

            IntroClipNoto = UltraVoicePlugin.LoadClip(bundle, "sm_SpawnSpecialNoto");
            IntroClipSecondNoto = UltraVoicePlugin.LoadClip(bundle, "sm_SpawnSpecial2Noto");
            BigPainClipNoto = UltraVoicePlugin.LoadClip(bundle, "sm_BigPainNoto");
            LungeClipNoto = UltraVoicePlugin.LoadClip(bundle, "sm_LungeNoto");
            ComboClipNoto = UltraVoicePlugin.LoadClip(bundle, "sm_ComboNoto");
            DeathClipNoto = UltraVoicePlugin.LoadClip(bundle, "sm_DeathNoto");
            KnockdownClipSpecialNoto = UltraVoicePlugin.LoadClip(bundle, "sm_KnockdownSpecialNoto");

            AgonySpawnClipNoto = UltraVoicePlugin.LoadClip(bundle, "sm_SpawnSpecialAgonyNoto");
            TundraSpawnClipNoto = UltraVoicePlugin.LoadClip(bundle, "sm_SpawnSpecialTundraNoto");
            AgonyKnockdownClipNoto = UltraVoicePlugin.LoadClip(bundle, "sm_DownedAgonyNoto");
            TundraKnockdownClipNoto = UltraVoicePlugin.LoadClip(bundle, "sm_DownedTundraNoto");

            SpawnClipsNoto = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip(bundle, "sm_Spawn1Noto"),
                UltraVoicePlugin.LoadClip(bundle, "sm_Spawn2Noto"),
                UltraVoicePlugin.LoadClip(bundle, "sm_Spawn3Noto"),
                UltraVoicePlugin.LoadClip(bundle, "sm_Spawn4Noto"),
                UltraVoicePlugin.LoadClip(bundle, "sm_Spawn5Noto"),
                UltraVoicePlugin.LoadClip(bundle, "sm_Spawn6Noto"),
                UltraVoicePlugin.LoadClip(bundle, "sm_Spawn7Noto")
            };

            EnrageClipsNoto = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip(bundle, "sm_Enrage1Noto"),
                UltraVoicePlugin.LoadClip(bundle, "sm_Enrage2Noto"),
                UltraVoicePlugin.LoadClip(bundle, "sm_Enrage3Noto"),
                UltraVoicePlugin.LoadClip(bundle, "sm_Enrage4Noto")
            };

            KnockdownClipsNoto = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip(bundle, "sm_Knockdown1Noto"),
                UltraVoicePlugin.LoadClip(bundle, "sm_Knockdown2Noto"),
                UltraVoicePlugin.LoadClip(bundle, "sm_Knockdown3Noto")
            };

            RangedClipsNoto = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip(bundle, "sm_Ranged1Noto"),
                UltraVoicePlugin.LoadClip(bundle, "sm_Ranged2Noto"),
            };

            logger.LogInfo("Swordsmachine voice lines loaded successfully!");
        }

}

    // SWORDSMACHINE PATCHES

    [HarmonyPatch(typeof(SwordsMachine), "Start")]
    class SwordsmachineSpawnPatch
    {
        static void Postfix(SwordsMachine __instance)
        {
            VoiceManager.enemySpawnTimes[__instance] = Time.time;

            if (!UltraVoicePlugin.SwordsmachineVoiceEnabled.value)
                return;

            if (SwordsmachineCharacter.IsAgonyOrTundra(__instance))
                return;

            if (__instance.bossVersion)
            {
                if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "5bcb2e0461e7fce408badfcb6778c271") // Prelude Third Scene
                    return;

                UltraVoicePlugin.Instance.StartCoroutine(PlayBossIntro(__instance));
                return;
            }

            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "7927c42db92e4164cae682a55e6b7725") // Prelude Second Scene
                return;

            VoiceManager.PlayRandomVoice(__instance, "Swordsmachine",
                SwordsmachineCharacter.UseSwordsmachineClips(SwordsmachineCharacter.SpawnClips, SwordsmachineCharacter.SpawnClipsNoto),
                SwordsmachineCharacter.SpawnSubs,
                true
            );
        }

        static IEnumerator PlayBossIntro(SwordsMachine sm)
        {
            yield return null;

            AudioClip clip;
            string subtitle;

            if (!SwordsmachineCharacter.FirstFightDone)
            {
                clip = SwordsmachineCharacter.UseSwordsmachineClip(SwordsmachineCharacter.IntroClip, SwordsmachineCharacter.IntroClipNoto);
                subtitle = "YOU WANT A FIGHT? LET'S FIGHT";
            }
            else
            {
                clip = SwordsmachineCharacter.UseSwordsmachineClip(SwordsmachineCharacter.IntroClipSecond, SwordsmachineCharacter.IntroClipSecondNoto);
                subtitle = "DID YOU THINK I FORGOT ABOUT YOU?";
            }

            var src = VoiceManager.CreateVoiceSource(sm, "SwordsmachineIntro", clip, subtitle, true);
            if (src != null)
            {
                VoiceManager.spawnVoiceEndTimes[sm] = Time.time + clip.length;
            }
        }
    }

    [HarmonyPatch(typeof(SwordsMachine), "Start")]
    class SwordsmachineSpecialSpawnPatch
    {
        static void Postfix(SwordsMachine __instance)
        {
            if (!UltraVoicePlugin.SwordsmachineVoiceEnabled.value)
                return;

            if (!SwordsmachineCharacter.IsAgonyOrTundra(__instance))
                return;

            VoiceManager.enemySpawnTimes[__instance] = Time.time;

            if (SwordsmachineCharacter.IsAgony(__instance))
            {
                VoiceManager.CreateVoiceSource(
                    __instance,
                    "AgonySpawn",
                    SwordsmachineCharacter.UseSwordsmachineClip(SwordsmachineCharacter.AgonySpawnClip, SwordsmachineCharacter.AgonySpawnClipNoto),
                    "JUMP 'EM!",
                    true,
                    SwordsmachineCharacter.AgonyColor
                );
            }
            else if (SwordsmachineCharacter.IsTundra(__instance))
            {
                VoiceManager.CreateVoiceSource(
                    __instance,
                    "TundraSpawn",
                    SwordsmachineCharacter.UseSwordsmachineClip(SwordsmachineCharacter.TundraSpawnClip, SwordsmachineCharacter.TundraSpawnClipNoto),
                    "THERE THEY ARE!",
                    true,
                    SwordsmachineCharacter.TundraColor
                );
            }
        }
    }

    [HarmonyPatch(typeof(SwordsMachine), "Enrage")]
    class SwordsmachineEnragePatch
    {
        static void Prefix(SwordsMachine __instance)
        {
            if (!UltraVoicePlugin.SwordsmachineVoiceEnabled.value)
                return;

            if (SwordsmachineCharacter.IsAgonyOrTundra(__instance))
                return;

            if (__instance.enraged)
                return;

            UltraVoicePlugin.Instance.StartCoroutine(PlayEnrage(__instance));
        }

        static IEnumerator PlayEnrage(SwordsMachine sm)
        {
            yield return new WaitForSeconds(0.75f);

            int i = UnityEngine.Random.Range(0, SwordsmachineCharacter.EnrageClips.Length);

            if (!sm.enraged)
                yield break;

            var src = VoiceManager.CreateVoiceSource(
                sm,
                "SwordsmachineEnrage",
                SwordsmachineCharacter.UseSwordsmachineClip(SwordsmachineCharacter.EnrageClips[i], SwordsmachineCharacter.EnrageClipsNoto[i]),
                SwordsmachineCharacter.EnrageSubs[i],
                true,
                SwordsmachineCharacter.GetColorOverride(sm)
            );

            if (src == null)
                yield break;

            if (!string.IsNullOrEmpty(SwordsmachineCharacter.EnrageSubs2[i]))
            {
                yield return new WaitForSeconds(0.75f);

                VoiceManager.ShowSubtitle(
                    SwordsmachineCharacter.EnrageSubs2[i],
                    src,
                    SwordsmachineCharacter.GetColorOverride(sm)
                );
            }
        }
    }

    [HarmonyPatch(typeof(SwordsMachine), "EndFirstPhase")]
    class SwordsmachinePhaseChangePatch
    {
        static void Postfix(SwordsMachine __instance)
        {
            if (!UltraVoicePlugin.SwordsmachineVoiceEnabled.value)
                return;

            if (SwordsmachineCharacter.IsAgonyOrTundra(__instance))
                return;

            VoiceManager.CreateVoiceSource(
                __instance,
                "SwordsmachineBigPain",
                SwordsmachineCharacter.UseSwordsmachineClip(SwordsmachineCharacter.BigPainClip, SwordsmachineCharacter.BigPainClipNoto),
                null,
                true
            );

            if (__instance.bossVersion && UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "5bcb2e0461e7fce408badfcb6778c271" && __instance.difficulty <= 2 && !SwordsmachineCharacter.FirstFightLinePlayed)
                UltraVoicePlugin.Instance.StartCoroutine(PlayKnockdownSpecial(__instance));
            else
                UltraVoicePlugin.Instance.StartCoroutine(PlayKnockdown(__instance));
        }

        static IEnumerator PlayKnockdown(SwordsMachine sm)
        {
            yield return new WaitForSeconds(0.85f);

            int i = UnityEngine.Random.Range(0, SwordsmachineCharacter.KnockdownClips.Length);

            var src = VoiceManager.CreateVoiceSource(
                sm,
                "SwordsmachineKnockdown",
                SwordsmachineCharacter.UseSwordsmachineClip(SwordsmachineCharacter.KnockdownClips[i], SwordsmachineCharacter.KnockdownClipsNoto[i]),
                SwordsmachineCharacter.KnockdownSubs[i],
                true,
                SwordsmachineCharacter.GetColorOverride(sm)
            );
        }

        static IEnumerator PlayKnockdownSpecial(SwordsMachine sm)
        {
            SwordsmachineCharacter.FirstFightLinePlayed = true;
            yield return new WaitForSeconds(0.85f);

            var src = VoiceManager.CreateVoiceSource(
                sm,
                "SwordsmachineKnockdownSpecial",
                SwordsmachineCharacter.UseSwordsmachineClip(SwordsmachineCharacter.KnockdownClipSpecial, SwordsmachineCharacter.KnockdownClipSpecialNoto),
                "YOU’RE NOT OFF THE HOOK YET, YOU HEAR ME",
                true,
                SwordsmachineCharacter.GetColorOverride(sm)
            );

            yield return new WaitForSeconds(2f);

            if (src == null)
                yield break;

            VoiceManager.ShowSubtitle(
                "I'LL",
                src,
                SwordsmachineCharacter.GetColorOverride(sm)
            );

            if (src == null)
                yield break;

            yield return new WaitForSeconds(0.5f);

            VoiceManager.ShowSubtitle(
                "BE",
                src,
                SwordsmachineCharacter.GetColorOverride(sm)
            );

            if (src == null)
                yield break;

            yield return new WaitForSeconds(0.5f);

            VoiceManager.ShowSubtitle(
                "BACK!",
                src,
                SwordsmachineCharacter.GetColorOverride(sm)
            );
        }
    }

    [HarmonyPatch(typeof(SwordsMachine), "Knockdown")]
    class SwordsmachineKnockdownPatch
    {
        static void Postfix(SwordsMachine __instance)
        {
            if (!UltraVoicePlugin.SwordsmachineVoiceEnabled.value)
                return;

            if (!VoiceManager.CheckCooldown(__instance, 0.1f))
                return;

            VoiceManager.CreateVoiceSource(
                __instance,
                "SwordsmachineBigPain",
                SwordsmachineCharacter.UseSwordsmachineClip(SwordsmachineCharacter.BigPainClip, SwordsmachineCharacter.BigPainClipNoto),
                null,
                true
            );

            UltraVoicePlugin.Instance.StartCoroutine(PlaySpecialKnockdown(__instance));
        }

        static IEnumerator PlaySpecialKnockdown(SwordsMachine sm)
        {
            yield return new WaitForSeconds(0.75f);

            if (SwordsmachineCharacter.IsAgony(sm))
            {
                VoiceManager.CreateVoiceSource(
                    sm,
                    "AgonyKnockdown",
                    SwordsmachineCharacter.UseSwordsmachineClip(SwordsmachineCharacter.AgonyKnockdownClip, SwordsmachineCharacter.AgonyKnockdownClipNoto),
                    "DAMMIT!",
                    true,
                    SwordsmachineCharacter.AgonyColor
                );
            }
            else if (SwordsmachineCharacter.IsTundra(sm))
            {
                VoiceManager.CreateVoiceSource(
                    sm,
                    "TundraKnockdown",
                    SwordsmachineCharacter.UseSwordsmachineClip(SwordsmachineCharacter.TundraKnockdownClip, SwordsmachineCharacter.TundraKnockdownClipNoto),
                    "COVER ME!",
                    true,
                    SwordsmachineCharacter.TundraColor
                );
            }
        }
    }

    [HarmonyPatch(typeof(SwordsMachine), "TeleportAway")]
    class SwordsmachineTeleportPatch
    {
        static void Postfix(SwordsMachine __instance)
        {
            SwordsmachineCharacter.FirstFightDone = true;
            VoiceManager.InterruptVoices(__instance);
        }
    }

    [HarmonyPatch(typeof(SwordsMachine), "ShootGun")]
    class SwordsmachineShotgunPatch
    {
        static void Postfix(SwordsMachine __instance)
        {
            if (!UltraVoicePlugin.SwordsmachineVoiceEnabled.value)
                return;

            if (!VoiceManager.CheckCooldown(__instance, 3f))
                return;

            if (VoiceManager.IsEnemyVoicePlaying(__instance))
                return;

            if (VoiceManager.TooSoonAfterSpawn(__instance, 2f))
                return;

            VoiceManager.PlayRandomVoice(__instance, "Swordsmachine",
                SwordsmachineCharacter.UseSwordsmachineClips(SwordsmachineCharacter.RangedClips, SwordsmachineCharacter.RangedClipsNoto),
                SwordsmachineCharacter.RangedSubs,
                colorOverride: SwordsmachineCharacter.GetColorOverride(__instance)
            );
        }
    }

    [HarmonyPatch(typeof(SwordsMachine), "SwordThrow")]
    class SwordsmachineSwordThrowPatch
    {
        static void Postfix(SwordsMachine __instance)
        {
            if (!UltraVoicePlugin.SwordsmachineVoiceEnabled.value)
                return;

            if (!VoiceManager.CheckCooldown(__instance, 3f))
                return;

            if (VoiceManager.IsEnemyVoicePlaying(__instance))
                return;

            if (VoiceManager.TooSoonAfterSpawn(__instance, 2f))
                return;

            VoiceManager.PlayRandomVoice(__instance, "Swordsmachine",
                SwordsmachineCharacter.UseSwordsmachineClips(SwordsmachineCharacter.RangedClips, SwordsmachineCharacter.RangedClipsNoto),
                SwordsmachineCharacter.RangedSubs,
                colorOverride: SwordsmachineCharacter.GetColorOverride(__instance)
            );
        }
    }

    [HarmonyPatch(typeof(SwordsMachine), "SwordSpiral")]
    class SwordsmachineSwordSpiralPatch
    {
        static void Postfix(SwordsMachine __instance)
        {
            if (!UltraVoicePlugin.SwordsmachineVoiceEnabled.value)
                return;

            if (!VoiceManager.CheckCooldown(__instance, 3f))
                return;

            if (VoiceManager.IsEnemyVoicePlaying(__instance))
                return;

            if (VoiceManager.TooSoonAfterSpawn(__instance, 2f))
                return;

            VoiceManager.PlayRandomVoice(__instance, "Swordsmachine",
                SwordsmachineCharacter.UseSwordsmachineClips(SwordsmachineCharacter.RangedClips, SwordsmachineCharacter.RangedClipsNoto),
                SwordsmachineCharacter.RangedSubs,
                colorOverride: SwordsmachineCharacter.GetColorOverride(__instance)
            );
        }
    }

    [HarmonyPatch(typeof(SwordsMachine), "Combo")]
    class SwordsmachineComboPatc
    {
        static void Postfix(SwordsMachine __instance)
        {
            if (!UltraVoicePlugin.SwordsmachineVoiceEnabled.value)
                return;

            if (!VoiceManager.CheckCooldown(__instance, 3f))
                return;

            if (VoiceManager.IsEnemyVoicePlaying(__instance))
                return;

            if (VoiceManager.TooSoonAfterSpawn(__instance, 2f))
                return;

            var src = VoiceManager.CreateVoiceSource(
                __instance,
                "SwordsmachineCombo",
                SwordsmachineCharacter.UseSwordsmachineClip(SwordsmachineCharacter.ComboClip, SwordsmachineCharacter.ComboClipNoto),
                "DIE, DIE, DIE",
                subtitleColor: SwordsmachineCharacter.GetColorOverride(__instance)
            );
        }
    }

    [HarmonyPatch(typeof(SwordsMachine), "RunningSwing")]
    class SwordsmachineLungePatch
    {
        static void Postfix(SwordsMachine __instance)
        {
            if (!UltraVoicePlugin.SwordsmachineVoiceEnabled.value)
                return;

            if (!VoiceManager.CheckCooldown(__instance, 3f))
                return;

            if (VoiceManager.IsEnemyVoicePlaying(__instance))
                return;

            if (VoiceManager.TooSoonAfterSpawn(__instance, 2f))
                return;

            var src = VoiceManager.CreateVoiceSource(
                __instance,
                "SwordsmachineLunge",
                SwordsmachineCharacter.UseSwordsmachineClip(SwordsmachineCharacter.LungeClip, SwordsmachineCharacter.LungeClipNoto),
                "DIE",
                subtitleColor: SwordsmachineCharacter.GetColorOverride(__instance)
            );
        }
    }

    [HarmonyPatch(typeof(SwordsMachine), "OnGoLimp")]
    class SwordsmachineDeathPatch
    {
        static void Postfix(SwordsMachine __instance)
        {
            if (!UltraVoicePlugin.SwordsmachineVoiceEnabled.value)
                return;

            VoiceManager.InterruptVoices(__instance);

            var src = VoiceManager.CreateVoiceSource(
                __instance,
                "SwordsmachineDeath",
                SwordsmachineCharacter.UseSwordsmachineClip(SwordsmachineCharacter.DeathClip, SwordsmachineCharacter.DeathClipNoto),
                null,
                true,
                subtitleColor: SwordsmachineCharacter.GetColorOverride(__instance)
            );
        }
    }
}