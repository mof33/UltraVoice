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
        public static AudioClip KnockdownClipSpecialBrutal;

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
        public static AudioClip KnockdownClipSpecialBrutalNoto;

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
            "LET'S SEE SOME BLOOD",
            "COME ON, COME ON",
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
            "EAT THIS",
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

        public static AudioClip UseSwordsmachineClip(AudioClip mofClip, AudioClip notoClip, bool randomPitch = false)
        {
            return UltraVoicePlugin.SwordsmachineVoiceActorField != null && UltraVoicePlugin.SwordsmachineVoiceActorField.value == UltraVoicePlugin.SwordsmachineVoiceActor.Noto
                ? notoClip
                : mofClip;
        }

        public static AudioClip[] UseSwordsmachineClips(AudioClip[] mofClips, AudioClip[] notoClips, bool randomPitch = false)
        {
            return UltraVoicePlugin.SwordsmachineVoiceActorField != null && UltraVoicePlugin.SwordsmachineVoiceActorField.value == UltraVoicePlugin.SwordsmachineVoiceActor.Noto
                ? notoClips
                : mofClips;
        }

        public static void LoadVoiceLines(BepInEx.Logging.ManualLogSource logger)
        {
            IntroClip = UltraVoicePlugin.LoadClip("Swordsmachine.sm_SpawnSpecial.wav");
            IntroClipSecond = UltraVoicePlugin.LoadClip("Swordsmachine.sm_SpawnSpecial2.wav");
            BigPainClip = UltraVoicePlugin.LoadClip("Swordsmachine.sm_BigPain.wav");
            LungeClip = UltraVoicePlugin.LoadClip("Swordsmachine.sm_Lunge.wav");
            ComboClip = UltraVoicePlugin.LoadClip("Swordsmachine.sm_Combo.wav");
            DeathClip = UltraVoicePlugin.LoadClip("Swordsmachine.sm_Death.wav");
            KnockdownClipSpecial = UltraVoicePlugin.LoadClip("Swordsmachine.sm_KnockdownSpecial.wav");
            KnockdownClipSpecialBrutal = UltraVoicePlugin.LoadClip("Swordsmachine.sm_PieceOfShit.wav");
            KnockdownClipSpecialBrutalNoto = UltraVoicePlugin.LoadClip("Swordsmachine.sm_PieceOfShitNoto.wav");

            AgonySpawnClip = UltraVoicePlugin.LoadClip("Swordsmachine.sm_SpawnSpecialAgony.wav");
            TundraSpawnClip = UltraVoicePlugin.LoadClip("Swordsmachine.sm_SpawnSpecialTundra.wav");
            AgonyKnockdownClip = UltraVoicePlugin.LoadClip("Swordsmachine.sm_DownedAgony.wav");
            TundraKnockdownClip = UltraVoicePlugin.LoadClip("Swordsmachine.sm_DownedTundra.wav");

            SpawnClips = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip("Swordsmachine.sm_Spawn1.wav"),
                UltraVoicePlugin.LoadClip("Swordsmachine.sm_Spawn2.wav"),
                UltraVoicePlugin.LoadClip("Swordsmachine.sm_Spawn3.wav"),
                UltraVoicePlugin.LoadClip("Swordsmachine.sm_Spawn4.wav"),
                UltraVoicePlugin.LoadClip("Swordsmachine.sm_Spawn5.wav"),
                UltraVoicePlugin.LoadClip("Swordsmachine.sm_Spawn6.wav"),
                UltraVoicePlugin.LoadClip("Swordsmachine.sm_Spawn7.wav"),
            };

            EnrageClips = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip("Swordsmachine.sm_Enrage1.wav"),
                UltraVoicePlugin.LoadClip("Swordsmachine.sm_Enrage2.wav"),
                UltraVoicePlugin.LoadClip("Swordsmachine.sm_Enrage3.wav"),
                UltraVoicePlugin.LoadClip("Swordsmachine.sm_Enrage4.wav")
            };

            KnockdownClips = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip("Swordsmachine.sm_Knockdown1.wav"),
                UltraVoicePlugin.LoadClip("Swordsmachine.sm_Knockdown2.wav"),
                UltraVoicePlugin.LoadClip("Swordsmachine.sm_Knockdown3.wav")
            };

            RangedClips = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip("Swordsmachine.sm_Ranged1.wav"),
                UltraVoicePlugin.LoadClip("Swordsmachine.sm_Ranged2.wav"),
            };

            IntroClipNoto = UltraVoicePlugin.LoadClip("Swordsmachine.sm_SpawnSpecialNoto.wav");
            IntroClipSecondNoto = UltraVoicePlugin.LoadClip("Swordsmachine.sm_SpawnSpecial2Noto.wav");
            BigPainClipNoto = UltraVoicePlugin.LoadClip("Swordsmachine.sm_BigPainNoto.wav");
            LungeClipNoto = UltraVoicePlugin.LoadClip("Swordsmachine.sm_LungeNoto.wav");
            ComboClipNoto = UltraVoicePlugin.LoadClip("Swordsmachine.sm_ComboNoto.wav");
            DeathClipNoto = UltraVoicePlugin.LoadClip("Swordsmachine.sm_DeathNoto.wav");
            KnockdownClipSpecialNoto = UltraVoicePlugin.LoadClip("Swordsmachine.sm_KnockdownSpecialNoto.wav");

            AgonySpawnClipNoto = UltraVoicePlugin.LoadClip("Swordsmachine.sm_SpawnSpecialAgonyNoto.wav");
            TundraSpawnClipNoto = UltraVoicePlugin.LoadClip("Swordsmachine.sm_SpawnSpecialTundraNoto.wav");
            AgonyKnockdownClipNoto = UltraVoicePlugin.LoadClip("Swordsmachine.sm_DownedAgonyNoto.wav");
            TundraKnockdownClipNoto = UltraVoicePlugin.LoadClip("Swordsmachine.sm_DownedTundraNoto.wav");

            SpawnClipsNoto = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip("Swordsmachine.sm_Spawn1Noto.wav"),
                UltraVoicePlugin.LoadClip("Swordsmachine.sm_Spawn2Noto.wav"),
                UltraVoicePlugin.LoadClip("Swordsmachine.sm_Spawn3Noto.wav"),
                UltraVoicePlugin.LoadClip("Swordsmachine.sm_Spawn4Noto.wav"),
                UltraVoicePlugin.LoadClip("Swordsmachine.sm_Spawn5Noto.wav"),
                UltraVoicePlugin.LoadClip("Swordsmachine.sm_Spawn6Noto.wav"),
                UltraVoicePlugin.LoadClip("Swordsmachine.sm_Spawn7Noto.wav")
            };

            EnrageClipsNoto = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip("Swordsmachine.sm_Enrage1Noto.wav"),
                UltraVoicePlugin.LoadClip("Swordsmachine.sm_Enrage2Noto.wav"),
                UltraVoicePlugin.LoadClip("Swordsmachine.sm_Enrage3Noto.wav"),
                UltraVoicePlugin.LoadClip("Swordsmachine.sm_Enrage4Noto.wav")
            };

            KnockdownClipsNoto = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip("Swordsmachine.sm_Knockdown1Noto.wav"),
                UltraVoicePlugin.LoadClip("Swordsmachine.sm_Knockdown2Noto.wav"),
                UltraVoicePlugin.LoadClip("Swordsmachine.sm_Knockdown3Noto.wav")
            };

            RangedClipsNoto = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip("Swordsmachine.sm_Ranged1Noto.wav"),
                UltraVoicePlugin.LoadClip("Swordsmachine.sm_Ranged2Noto.wav"),
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

            VoiceManager.PlayRandomVoice(__instance, "Virtue",
            SwordsmachineCharacter.UseSwordsmachineClips(SwordsmachineCharacter.SpawnClips, SwordsmachineCharacter.SpawnClipsNoto, randomPitch: true),
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
                SwordsmachineCharacter.GetColorOverride(sm),
                randomPitch: true
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
            else if (__instance.bossVersion && UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "5bcb2e0461e7fce408badfcb6778c271" && __instance.difficulty == 3 && !SwordsmachineCharacter.FirstFightLinePlayed)
                UltraVoicePlugin.Instance.StartCoroutine(PlayKnockdownSpecialFasterDelay(__instance));
            else if (__instance.bossVersion && UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "5bcb2e0461e7fce408badfcb6778c271" && __instance.difficulty == 4 && !SwordsmachineCharacter.FirstFightLinePlayed)
                UltraVoicePlugin.Instance.StartCoroutine(PieceOfShit(__instance));
            else
                UltraVoicePlugin.Instance.StartCoroutine(PlayKnockdown(__instance));
        }

        static IEnumerator PlayKnockdown(SwordsMachine sm)
        {
           yield return new WaitForSeconds(0.75f);

           VoiceManager.PlayRandomVoice(
                sm,
                "SwordsmachineKnockdownSpecial",
                SwordsmachineCharacter.UseSwordsmachineClips(SwordsmachineCharacter.KnockdownClips, SwordsmachineCharacter.KnockdownClips),
                SwordsmachineCharacter.KnockdownSubs,
                true,
                true,
                colorOverride: SwordsmachineCharacter.GetColorOverride(sm)
            );
        }

        static IEnumerator PlayKnockdownSpecialFasterDelay(SwordsMachine sm)
        {
            yield return new WaitForSeconds(0.5f);

            var src = VoiceManager.CreateVoiceSource(
                sm,
                "SwordsmachineKnockdownSpecial",
                SwordsmachineCharacter.UseSwordsmachineClip(SwordsmachineCharacter.KnockdownClipSpecial, SwordsmachineCharacter.KnockdownClipSpecialNoto),
                null,
                true,
                SwordsmachineCharacter.GetColorOverride(sm)
            );
            SwordsmachineCharacter.FirstFightLinePlayed = true;

            yield return new WaitForSeconds(1f);

            if (src == null)
                yield break;

            VoiceManager.ShowSubtitle(
                "YOU THINK YOU'RE SO MUCH BETTER THAN ME, HUH?!",
                src,
                SwordsmachineCharacter.GetColorOverride(sm)
            );
        }

        static IEnumerator PieceOfShit(SwordsMachine sm)
        {
            yield return new WaitForSeconds(0.85f);

            var src = VoiceManager.CreateVoiceSource(
                sm,
                "SwordsmachineKnockdown",
                SwordsmachineCharacter.UseSwordsmachineClip(SwordsmachineCharacter.KnockdownClipSpecialBrutal, SwordsmachineCharacter.KnockdownClipSpecialBrutalNoto),
                "PIECE OF SHIT!",
                true,
                SwordsmachineCharacter.GetColorOverride(sm)
            );
            SwordsmachineCharacter.FirstFightLinePlayed = true;
        }

        static IEnumerator PlayKnockdownSpecial(SwordsMachine sm)
        {
            yield return new WaitForSeconds(0.75f);

            var src = VoiceManager.CreateVoiceSource(
                sm,
                "SwordsmachineKnockdownSpecial",
                SwordsmachineCharacter.UseSwordsmachineClip(SwordsmachineCharacter.KnockdownClipSpecial, SwordsmachineCharacter.KnockdownClipSpecialNoto),
                null,
                true,
                SwordsmachineCharacter.GetColorOverride(sm)
            );
            SwordsmachineCharacter.FirstFightLinePlayed = true;

            yield return new WaitForSeconds(1f);

            if (src == null)
                yield break;

            VoiceManager.ShowSubtitle(
                "YOU THINK YOU'RE SO MUCH BETTER THAN ME, HUH?!",
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
                true,
                randomPitch: true
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
                colorOverride: SwordsmachineCharacter.GetColorOverride(__instance),
                randomPitch: true
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
                colorOverride: SwordsmachineCharacter.GetColorOverride(__instance),
                randomPitch: true
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
                colorOverride: SwordsmachineCharacter.GetColorOverride(__instance),
                randomPitch: true
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

            var src = VoiceManager.CreateVoiceSource(
                __instance,
                "SwordsmachineCombo",
                SwordsmachineCharacter.UseSwordsmachineClip(SwordsmachineCharacter.ComboClip, SwordsmachineCharacter.ComboClipNoto),
                "DIE, DIE, DIE",
                subtitleColor: SwordsmachineCharacter.GetColorOverride(__instance),
                randomPitch: true
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

            var src = VoiceManager.CreateVoiceSource(
                __instance,
                "SwordsmachineLunge",
                SwordsmachineCharacter.UseSwordsmachineClip(SwordsmachineCharacter.LungeClip, SwordsmachineCharacter.LungeClipNoto),
                "DIE",
                subtitleColor: SwordsmachineCharacter.GetColorOverride(__instance),
                randomPitch: true
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
                subtitleColor: SwordsmachineCharacter.GetColorOverride(__instance),
                randomPitch: true
            );
        }
    }
}