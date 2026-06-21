using HarmonyLib;
using System;
using System.Collections.Generic;
using UltraVoice.Utilities;
using UnityEngine;

namespace UltraVoice.Characters
{
    public class InsurrectionistCharacter
    {
        public static AudioClip[] SpawnClips;
        public static AudioClip[] AttackClips;
        public static AudioClip[] AttackSpecialClips;
        public static AudioClip[] StunClips;
        public static AudioClip[] FrustratedClips;
        public static AudioClip[] DeathClips;

        public static AudioClip AngrySpawnClip;
        public static AudioClip RudeSpawnClip;
        public static AudioClip AngryKnockdownClip;
        public static AudioClip RudeKnockdownClip;

        public static readonly string[] SpawnSubs =
        {
            "YOU OR ME",
            "YOU SHALL FALL",
            "MY DEATH OR YOURS",
            "PROVE YOURSELF"
        };

        public static readonly string[] FrustratedSubs =
        {
            "YOU WILL PAY",
            "I AM NOT DONE",
            "STILL STANDING",
        };

        public static readonly string[] AttackSpecialSubs =
        {
            "YOU CANNOT ESCAPE",
            "THIS WILL HURT",
            "BE GONE",
        };

        public static UnityEngine.Color InsurrectionistColor = new UnityEngine.Color(0.79f, 0.58f, 0.49f);
        public static UnityEngine.Color AngryColor = new UnityEngine.Color(0.81f, 0.14f, 0.16f);
        public static UnityEngine.Color RudeColor = new UnityEngine.Color(0f, 0.26f, 0.43f);

        public static bool IsAngryOrRude(Sisyphus sisyphus)
        {
            Transform t = sisyphus.transform;

            while (t != null)
            {
                if (t.name.Contains("10B - Chapel Roof"))
                    return true;

                t = t.parent;
            }

            return false;
        }

        public static bool IsAngry(Sisyphus sisyphus)
        {
            if (IsAngryOrRude(sisyphus)) {
                string n = sisyphus.gameObject.name;
                return n.Contains("Angry");
            }
            else return false;
        }

        public static bool IsRude(Sisyphus sisyphus)
        {
            if (IsAngryOrRude(sisyphus))
            {
                string n = sisyphus.gameObject.name;
                return n.Contains("Rude");
            }
            else return false;
        }

        public static UnityEngine.Color? GetColorOverride(Sisyphus insur)
        {
            if (IsAngry(insur))
                return AngryColor;
            if (IsRude(insur))
                return RudeColor;
            else return InsurrectionistColor;
        }

        public static void LoadVoiceLines(BepInEx.Logging.ManualLogSource logger)
        {
            SpawnClips = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip("Insurrectionist.insur_Spawn1.wav"),
                UltraVoicePlugin.LoadClip("Insurrectionist.insur_Spawn2.wav"),
                UltraVoicePlugin.LoadClip("Insurrectionist.insur_Spawn3.wav"),
                UltraVoicePlugin.LoadClip("Insurrectionist.insur_Spawn4.wav")
            };

            AttackClips = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip("Insurrectionist.insur_Attack1.wav"),
                UltraVoicePlugin.LoadClip("Insurrectionist.insur_Attack2.wav"),
                UltraVoicePlugin.LoadClip("Insurrectionist.insur_Attack3.wav"),
                UltraVoicePlugin.LoadClip("Insurrectionist.insur_Attack4.wav")
            };

            AttackSpecialClips = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip("Insurrectionist.insur_AttackSpecial1.wav"),
                UltraVoicePlugin.LoadClip("Insurrectionist.insur_AttackSpecial2.wav"),
                UltraVoicePlugin.LoadClip("Insurrectionist.insur_AttackSpecial3.wav"),
            };

            StunClips = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip("Insurrectionist.insur_Stun1.wav"),
                UltraVoicePlugin.LoadClip("Insurrectionist.insur_Stun2.wav"),
            };

            FrustratedClips = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip("Insurrectionist.insur_Frustration1.wav"),
                UltraVoicePlugin.LoadClip("Insurrectionist.insur_Frustration2.wav"),
                UltraVoicePlugin.LoadClip("Insurrectionist.insur_Frustration3.wav"),
            };

            DeathClips = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip("Insurrectionist.insur_Death1.wav"),
                UltraVoicePlugin.LoadClip("Insurrectionist.insur_Death2.wav"),
            };

            AngrySpawnClip = UltraVoicePlugin.LoadClip("Insurrectionist.insur_SpawnSpecialAngry.wav");
            RudeSpawnClip = UltraVoicePlugin.LoadClip("Insurrectionist.insur_SpawnSpecialRude.wav");
            AngryKnockdownClip = UltraVoicePlugin.LoadClip("Insurrectionist.insur_DownedAngry.wav");
            RudeKnockdownClip = UltraVoicePlugin.LoadClip("Insurrectionist.insur_DownedRude.wav");

            logger.LogInfo("Insurrectionist voice lines loaded successfully!");
        }
    }

    [HarmonyPatch(typeof(Sisyphus), nameof(Sisyphus.Start))]
    class InsurrectionistSpawnPatch
    {
        static void Postfix(Sisyphus __instance)
        {
            if (!UltraVoicePlugin.InsurrectionistVoiceEnabled.value) return;

            if (InsurrectionistCharacter.IsAngryOrRude(__instance)) return;

            VoiceManager.PlayRandomVoice(__instance, "Insurrectionist",
                InsurrectionistCharacter.SpawnClips,
                InsurrectionistCharacter.SpawnSubs,
                true,
                colorOverride: InsurrectionistCharacter.GetColorOverride(__instance),
                randomPitch: true
            );
        }
    }

    [HarmonyPatch(typeof(Sisyphus), nameof(Sisyphus.Start))]
    class InsurrectionistSpawnSpecialPatch
    {
        static void Postfix(Sisyphus __instance)
        {
            if (!UltraVoicePlugin.InsurrectionistVoiceEnabled.value) return;

            if (!InsurrectionistCharacter.IsAngryOrRude(__instance)) return;

            if (InsurrectionistCharacter.IsAngry(__instance))
                VoiceManager.CreateVoiceSource(__instance, "Insurrectionist",
                    InsurrectionistCharacter.AngrySpawnClip,
                    "YOU... ARE CORNERED!",
                    true,
                    subtitleColor: InsurrectionistCharacter.GetColorOverride(__instance),
                    randomPitch: true
                );
            else if (InsurrectionistCharacter.IsRude(__instance))
                VoiceManager.CreateVoiceSource(__instance, "Insurrectionist",
                    InsurrectionistCharacter.RudeSpawnClip,
                    "YOU... ARE SURROUNDED!",
                    true,
                    subtitleColor: InsurrectionistCharacter.GetColorOverride(__instance),
                    randomPitch: true
                );
        }
    }


    [HarmonyPatch(typeof(Sisyphus), "Jump", new Type[] { typeof(bool) })]
    class InsurrectionistJumpPatch
    {
        static void Postfix(Sisyphus __instance)
        {
            if (!UltraVoicePlugin.InsurrectionistVoiceEnabled.value) return;

            VoiceManager.PlayRandomVoice(__instance, "Insurrectionist",
                InsurrectionistCharacter.AttackClips,
                null,
                colorOverride: InsurrectionistCharacter.GetColorOverride(__instance),
                randomPitch: true
            );
        }
    }

    [HarmonyPatch(typeof(Sisyphus), "Jump", new Type[] { typeof(Vector3), typeof(bool) })]
    class InsurrectionistJumpPatch2
    {
        static void Postfix(Sisyphus __instance)
        {
            if (!UltraVoicePlugin.InsurrectionistVoiceEnabled.value) return;

            VoiceManager.PlayRandomVoice(__instance, "Insurrectionist",
                InsurrectionistCharacter.AttackClips,
                null,
                randomPitch: true
            );
        }
    }

    [HarmonyPatch(typeof(Sisyphus), nameof(Sisyphus.AirStabAttack))]
    class InsurrectionistAirStabPatch
    {
        static void Postfix(Sisyphus __instance)
        {
            if (!UltraVoicePlugin.InsurrectionistVoiceEnabled.value) return;

            if (UnityEngine.Random.Range(0f, 1f) < 0.75f)
            {
                VoiceManager.PlayRandomVoice(__instance, "Insurrectionist",
                    InsurrectionistCharacter.AttackClips,
                    null,
                    randomPitch: true
                );
            }
            else
            {
                VoiceManager.PlayRandomVoice(__instance, "Insurrectionist",
                    InsurrectionistCharacter.AttackSpecialClips,
                    InsurrectionistCharacter.AttackSpecialSubs,
                    colorOverride: InsurrectionistCharacter.GetColorOverride(__instance),
                    randomPitch: true
                );
            }
        }
    }

    [HarmonyPatch(typeof(Sisyphus), nameof(Sisyphus.StabAttack))]
    class InsurrectionistGroundStabPatch
    {
        static void Postfix(Sisyphus __instance)
        {
            if (!UltraVoicePlugin.InsurrectionistVoiceEnabled.value) return;

            if (UnityEngine.Random.Range(0f, 1f) < 0.75f)
            {
                VoiceManager.PlayRandomVoice(__instance, "Insurrectionist",
                    InsurrectionistCharacter.AttackClips,
                    null,
                    randomPitch: true
                );
            }
            else
            {
                VoiceManager.PlayRandomVoice(__instance, "Insurrectionist",
                    InsurrectionistCharacter.AttackSpecialClips,
                    InsurrectionistCharacter.AttackSpecialSubs,
                    colorOverride: InsurrectionistCharacter.GetColorOverride(__instance),
                    randomPitch: true
                );
            }
        }
    }


    [HarmonyPatch(typeof(Sisyphus), nameof(Sisyphus.HorizontalSwingAttack))]
    class InsurrectionistHorizontalSwingPatch
    {
        static void Postfix(Sisyphus __instance)
        {
            if (!UltraVoicePlugin.InsurrectionistVoiceEnabled.value) return;

            if (UnityEngine.Random.Range(0f, 1f) < 0.75f)
            {
                VoiceManager.PlayRandomVoice(__instance, "Insurrectionist",
                    InsurrectionistCharacter.AttackClips,
                    null,
                    randomPitch: true
                );
            }
            else
            {
                VoiceManager.PlayRandomVoice(__instance, "Insurrectionist",
                    InsurrectionistCharacter.AttackSpecialClips,
                    InsurrectionistCharacter.AttackSpecialSubs,
                    colorOverride: InsurrectionistCharacter.GetColorOverride(__instance),
                    randomPitch: true
                );
            }
        }
    }

    [HarmonyPatch(typeof(Sisyphus), nameof(Sisyphus.OverheadSlamAttack))]
    class InsurrectionistOverheadSlamPatch
    {
        static void Postfix(Sisyphus __instance)
        {
            if (!UltraVoicePlugin.InsurrectionistVoiceEnabled.value) return;

            if (UnityEngine.Random.Range(0f, 1f) < 0.75f)
            {
                VoiceManager.PlayRandomVoice(__instance, "Insurrectionist",
                    InsurrectionistCharacter.AttackClips,
                    null,
                    randomPitch: true
                );
            }
            else
            {
                VoiceManager.PlayRandomVoice(__instance, "Insurrectionist",
                    InsurrectionistCharacter.AttackSpecialClips,
                    InsurrectionistCharacter.AttackSpecialSubs,
                    colorOverride: InsurrectionistCharacter.GetColorOverride(__instance),
                    randomPitch: true
                );
            }
        }
    }

    [HarmonyPatch(typeof(Animator), nameof(Animator.SetTrigger), typeof(int))]
    class InsurrectionistStompPatch
    {
        static void Prefix(Animator __instance, int id)
        {
            if (!UltraVoicePlugin.InsurrectionistVoiceEnabled.value) return;

            bool insur = __instance.gameObject.TryGetComponent<Sisyphus>(out var sisyphus);

            if (!VoiceManager.CheckCooldown(__instance, 2f))
                return;

            if (id != Sisyphus.s_Stomp) return;

            if (UnityEngine.Random.Range(0f, 1f) < 0.75f)
            {
                VoiceManager.PlayRandomVoice(__instance, "Insurrectionist",
                    InsurrectionistCharacter.AttackClips,
                    null,
                    randomPitch: true
                );
            }
            else
            {
                VoiceManager.PlayRandomVoice(__instance, "Insurrectionist",
                    InsurrectionistCharacter.AttackSpecialClips,
                    InsurrectionistCharacter.AttackSpecialSubs,
                    colorOverride: InsurrectionistCharacter.GetColorOverride(sisyphus),
                    randomPitch: true
                );
            }
        }
    }

    [HarmonyPatch(typeof(Sisyphus), nameof(Sisyphus.Knockdown))]
    class InsurrectionistKnockdowmPatch
    {
        static void Postfix(Sisyphus __instance)
        {
            if (!UltraVoicePlugin.InsurrectionistVoiceEnabled.value) return;

            if (!VoiceManager.CheckCooldown(__instance, 4f))
                return;

            VoiceManager.PlayRandomVoice(__instance, "Insurrectionist",
                InsurrectionistCharacter.StunClips,
                null,
                true,
                randomPitch: true
            );


            if (InsurrectionistCharacter.IsAngryOrRude(__instance))
                UltraVoicePlugin.Instance.StartCoroutine(PlayDowned(__instance));
            else
                UltraVoicePlugin.Instance.StartCoroutine(PlayFrustration(__instance));

            static IEnumerator<WaitForSeconds> PlayFrustration(Sisyphus sisyphus)
            {
                yield return new WaitForSeconds(1f);

                VoiceManager.PlayRandomVoice(
                        sisyphus,
                        "Insurrectionist",
                        InsurrectionistCharacter.FrustratedClips,
                        InsurrectionistCharacter.FrustratedSubs,
                        true,
                        colorOverride: InsurrectionistCharacter.GetColorOverride(sisyphus)
                    );
            }

            static IEnumerator<WaitForSeconds> PlayDowned(Sisyphus sisyphus)
            {
                yield return new WaitForSeconds(1f);

                if (InsurrectionistCharacter.IsAngry(sisyphus))
                    VoiceManager.CreateVoiceSource(
                            sisyphus,
                            "Insurrectionist",
                            InsurrectionistCharacter.AngryKnockdownClip,
                            "BROTHER... HELP!",
                            true,
                            subtitleColor: InsurrectionistCharacter.GetColorOverride(sisyphus)
                        );

                else if (InsurrectionistCharacter.IsRude(sisyphus))
                    VoiceManager.CreateVoiceSource(
                            sisyphus,
                            "Insurrectionist",
                            InsurrectionistCharacter.RudeKnockdownClip,
                            "BROTHER... PLEASE!",
                            true,
                            subtitleColor: InsurrectionistCharacter.GetColorOverride(sisyphus)
                        );
            }
        }
    }

    [HarmonyPatch(typeof(Sisyphus), nameof(Sisyphus.Death))]
    class InsurrectionistDeathPatch
    {
        static void Postfix(Sisyphus __instance)
        {
            if (!UltraVoicePlugin.InsurrectionistVoiceEnabled.value) return;

            VoiceManager.PlayRandomVoice(__instance, "Mannequin",
                InsurrectionistCharacter.DeathClips,
                null,
                true,
                randomPitch: true
            );
        }
    }
}