using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;
using UltraVoice.Utilities;

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

        public static readonly string[] SpawnSubs =
        {
            "YOU… OR ME!",
            "YOU SHALL FALL!",
            "MY DEATH… OR YOURS!",
            "PROVE YOURSELF!"
        };

        public static readonly string[] FrustratedSubs =
        {
            "YOU WILL PAY!",
            "I AM NOT DONE!",
            "STILL STANDING!",
        };

        public static readonly string[] AttackSpecialSubs =
        {
            "YOU CANNOT ESCAPE!",
            "THIS WILL HURT!",
            "BE GONE!",
        };

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

            logger.LogInfo("Insurrectionist voice lines loaded successfully!");
        }
    }

    [HarmonyPatch(typeof(Sisyphus), nameof(Sisyphus.Start))]
    class InsurrectionistSpawnPatch
    {
        static void Postfix(Sisyphus __instance)
        {
            if (!UltraVoicePlugin.InsurrectionistVoiceEnabled.value) return;

            if (__instance.jumpOnSpawn) return;

            VoiceManager.PlayRandomVoice(__instance, "Insurrectionist",
                InsurrectionistCharacter.SpawnClips,
                InsurrectionistCharacter.SpawnSubs,
                true,
                randomPitch: true
            );
        }
    }

    [HarmonyPatch(typeof(Sisyphus), nameof(Sisyphus.SlamShockwave))]
    class InsurrectionistJumpSlamSpawnPatch
    {
        static void Postfix(Sisyphus __instance)
        {
            if (!UltraVoicePlugin.InsurrectionistVoiceEnabled.value) return;

            if (__instance.jumpOnSpawn)
            {
                VoiceManager.PlayRandomVoice(__instance, "Insurrectionist",
                    InsurrectionistCharacter.SpawnClips,
                    InsurrectionistCharacter.SpawnSubs,
                    true,
                    randomPitch: true
                );
            }
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

            VoiceManager.PlayRandomVoice(__instance, "Insurrectionist",
                InsurrectionistCharacter.StunClips,
                null,
                true,
                randomPitch: true
            );

            UltraVoicePlugin.Instance.StartCoroutine(PlayFrustration(__instance));

            static IEnumerator<WaitForSeconds> PlayFrustration(Sisyphus sisyphus)
            {
                yield return new WaitForSeconds(1f);

                VoiceManager.PlayRandomVoice(
                        sisyphus,
                        "Insurrectionist",
                        InsurrectionistCharacter.FrustratedClips,
                        InsurrectionistCharacter.FrustratedSubs,
                        true
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
