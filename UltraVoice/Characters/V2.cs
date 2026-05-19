using HarmonyLib;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UltraVoice.Utilities;

namespace UltraVoice.Characters
{
    public class V2Character
    {
        // Voice line storage
        public static AudioClip IntroFirstClip;
        public static AudioClip IntroFirstRestartClip;
        public static AudioClip IntroSecondClip;
        public static AudioClip IntroSecondRestartClip;
        public static AudioClip FastDefeatClip;
        public static AudioClip DefeatClip;
        public static AudioClip DeathClip;
        public static AudioClip FlailingClip;
        public static AudioClip EnragePatienceClip;
        public static AudioClip EnragePunchedClip;
        public static AudioClip EscapingClip;

        public static AudioClip[] ChatterClips;
        public static AudioClip[] ChatterPissedClips;
        public static AudioClip[] PainClips;

        // Subtitle storage
        public static readonly string[] ChatterSubs =
        {
            "Do keep up with me",
            "You can do better than that",
            "Is this your best effort",
            "Keep your eye open now",
            "Mind your footing",
        };

        public static readonly string[] ChatterPissedSubs =
        {
            "What's the matter? Does your arm hurt!?",
            "You really think you can kill me now!?",
            "I won't lose to you again!",
            "I'll make quick work of you this time!",
            "You don't stand a chance against me!"
        };

        public static UnityEngine.Color V2Color = new UnityEngine.Color(1f, 1f, 1f);

        public static float V2IntroTime = -999f;
        public static bool V2CutsceneVoicePlayed = false;
        public static bool V2SecondVoiceRestartPlayed = false;
        public static bool V2DeathPlayed = false;

        public static void LoadVoiceLines(BepInEx.Logging.ManualLogSource logger)
        {
            IntroFirstClip = UltraVoicePlugin.LoadClip("V2.v2_IntroFirst.wav");
            IntroFirstRestartClip = UltraVoicePlugin.LoadClip("V2.v2_RestartIntroFirst.wav");
            IntroSecondClip = UltraVoicePlugin.LoadClip("V2.v2_IntroSecond.wav");
            IntroSecondRestartClip = UltraVoicePlugin.LoadClip("V2.v2_RestartIntroSecond.wav");

            FastDefeatClip = UltraVoicePlugin.LoadClip("V2.v2_FastDefeat.wav");
            DefeatClip = UltraVoicePlugin.LoadClip("V2.v2_Defeat.wav");
            DeathClip = UltraVoicePlugin.LoadClip("V2.v2_Death.wav");
            FlailingClip = UltraVoicePlugin.LoadClip("V2.v2_Flailing.wav");
            EnragePatienceClip = UltraVoicePlugin.LoadClip("V2.v2_EnragePatience.wav");
            EnragePunchedClip = UltraVoicePlugin.LoadClip("V2.v2_EnragePunched.wav");
            EscapingClip = UltraVoicePlugin.LoadClip("V2.v2_Escaping.wav");

            ChatterClips = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip("V2.v2_Chatter1.wav"),
                UltraVoicePlugin.LoadClip("V2.v2_Chatter2.wav"),
                UltraVoicePlugin.LoadClip("V2.v2_Chatter3.wav"),
                UltraVoicePlugin.LoadClip("V2.v2_Chatter4.wav"),
                UltraVoicePlugin.LoadClip("V2.v2_Chatter5.wav")
            };

            ChatterPissedClips = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip("V2.v2_ChatterPissed1.wav"),
                UltraVoicePlugin.LoadClip("V2.v2_ChatterPissed2.wav"),
                UltraVoicePlugin.LoadClip("V2.v2_ChatterPissed3.wav"),
                UltraVoicePlugin.LoadClip("V2.v2_ChatterPissed4.wav"),
                UltraVoicePlugin.LoadClip("V2.v2_ChatterPissed5.wav")
            };

            PainClips = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip("V2.v2_Pain1.wav"),
                UltraVoicePlugin.LoadClip("V2.v2_Pain2.wav"),
                UltraVoicePlugin.LoadClip("V2.v2_Pain3.wav"),
                UltraVoicePlugin.LoadClip("V2.v2_Pain4.wav"),
                UltraVoicePlugin.LoadClip("V2.v2_Pain5.wav")
            };

            logger.LogInfo("V2 voice lines loaded successfully!");
        }

    }

    // V2 PATCHES

    [HarmonyPatch(typeof(V2), "Start")]
    class V2IntroFirstPatch
    {
        static void Postfix(V2 __instance)
        {
            if (StatsManager.Instance.restarts >= 1)
                return;

            if (!UltraVoicePlugin.V2VoiceEnabled.value)
                return;

            if (__instance.secondEncounter)
                return;

            if (SceneManager.GetActiveScene().name != "36abcaae9708abc4d9e89e6ec73a2846") // Limbo Climax Scene
                return;

            if (!UltraVoicePlugin.V2VoiceEnabled.value)
                return;

            if (!__instance.inIntro)
                return;

            VoiceManager.enemySpawnTimes[__instance] = Time.time;
            UltraVoicePlugin.Instance.StartCoroutine(PlayIntroFirst(__instance));
        }

        static IEnumerator PlayIntroFirst(V2 v2)
        {
            yield return new WaitForSeconds(0.9f);

            var src = VoiceManager.CreateVoiceSource(
                v2,
                "V2Intro",
                V2Character.IntroFirstClip
            );

            if (src == null)
                yield break;

            V2Character.V2IntroTime = Time.time;
            VoiceManager.spawnVoiceEndTimes[v2] = Time.time + V2Character.IntroFirstClip.length;

            VoiceManager.ShowSubtitle("So, you're my predecessor", src);

            yield return new WaitForSeconds(2.25f);

            if (v2 == null || !v2.inIntro)
                yield break;

            VoiceManager.ShowSubtitle("How quaint...", src);

            yield return new WaitForSeconds(1.25f);

            if (v2 == null || !v2.inIntro)
                yield break;

            VoiceManager.ShowSubtitle("I suppose I'll have to show you what an upgrade looks like.", src);
        }
    }

    [HarmonyPatch(typeof(V2), "Start")]
    class V2IntroRetrySectionPatch
    {
        static void Postfix(V2 __instance)
        {
            if (StatsManager.Instance.restarts == 0)
                return;

            if (!UltraVoicePlugin.V2VoiceEnabled.value)
                return;

            if (!__instance.secondEncounter)
                UltraVoicePlugin.Instance.StartCoroutine(PlayRestartFirst(__instance));

            if (__instance.secondEncounter)
                if (!V2Character.V2SecondVoiceRestartPlayed)
                    UltraVoicePlugin.Instance.StartCoroutine(PlayRestartSecond(__instance));

            VoiceManager.enemySpawnTimes[__instance] = Time.time;
        }

        static IEnumerator PlayRestartFirst(V2 v2)
        {
            if (SceneManager.GetActiveScene().name != "36abcaae9708abc4d9e89e6ec73a2846") // Limbo Climax Scene
                yield break;

            yield return new WaitForSeconds(0.75f);

            var src = VoiceManager.CreateVoiceSource(
                v2,
                "V2IntroFirstRestart",
                V2Character.IntroFirstRestartClip,
                "Back so soon?",
                true
            );

            if (src == null)
                yield break;

            VoiceManager.spawnVoiceEndTimes[v2] = Time.time + V2Character.IntroFirstRestartClip.length;
        }

        static IEnumerator PlayRestartSecond(V2 v2)
        {
            if (V2Character.V2SecondVoiceRestartPlayed)
                yield break;

            if (SceneManager.GetActiveScene().name != "ac1675e648695a343bd064c6d0c56e57") // Greed Climax Scene
                yield break;

            var src = VoiceManager.CreateVoiceSource(
                v2,
                "V2IntroSecondRestart",
                V2Character.IntroSecondRestartClip,
                "Just stay down!",
                true
            );

            if (src == null)
                yield break;

            VoiceManager.spawnVoiceEndTimes[v2] = Time.time + V2Character.IntroSecondRestartClip.length;
            V2Character.V2SecondVoiceRestartPlayed = true;
        }
    }

    [HarmonyPatch(typeof(V2), "Update")]
    class V2CombatChatterPatch
    {
        static void Postfix(V2 __instance)
        {
            if (__instance == null)
                return;

            if (!UltraVoicePlugin.V2VoiceEnabled.value)
                return;

            if (ULTRAKILL.Cheats.BlindEnemies.Blind)
                return;

            if (!__instance.active || __instance.target == null)
                return;

            if (VoiceManager.IsSpawnVoicePlaying(__instance))
                return;

            if (VoiceManager.TooSoonAfterSpawn(__instance, 6f))
                return;

            if (!VoiceManager.CheckCooldown(__instance, 6f))
                return;

            if (__instance.inIntro)
                return;

            if (__instance.enraged)
                return;

            if (Random.Range(0f, 1f) < 0.75f)
                if (!__instance.secondEncounter)
                    VoiceManager.PlayRandomVoice(__instance, "V2",
                        V2Character.ChatterClips,
                        V2Character.ChatterSubs
                    );
                else
                    VoiceManager.PlayRandomVoice(__instance, "V2",
                        V2Character.ChatterPissedClips,
                        V2Character.ChatterPissedSubs
                    );
        }
    }

    [HarmonyPatch(typeof(V2), "KnockedOut")]
    class V2DefeatPatch
    {
        static void Prefix(V2 __instance)
        {
            if (!UltraVoicePlugin.V2VoiceEnabled.value)
                return;

            float timeSinceIntro = Time.time - V2Character.V2IntroTime;

            if (!__instance.secondEncounter)
                if (__instance.inIntro || timeSinceIntro < 15f)
                {
                    UltraVoicePlugin.Instance.StartCoroutine(PlayFastDefeat(__instance));
                }
                else
                {
                    UltraVoicePlugin.Instance.StartCoroutine(PlayDefeat(__instance));
                }
            else
                if (!__instance.dead)
                    UltraVoicePlugin.Instance.StartCoroutine(PlayEscape(__instance));
                else
                    UltraVoicePlugin.Instance.StartCoroutine(PlayRealization(__instance));
        }

        static IEnumerator PlayFastDefeat(V2 v2)
        {
            var src = VoiceManager.CreateVoiceSource(
                v2,
                "V2FastDefeat",
                V2Character.FastDefeatClip,
                null,
                true
            );

            if (src == null)
                yield break;

            VoiceManager.ShowSubtitle("Enough", src);

            yield return new WaitForSeconds(1.5f);

            VoiceManager.ShowSubtitle("You've proven your point...", src);
        }

        static IEnumerator PlayDefeat(V2 v2)
        {
            yield return new WaitForSeconds(1.25f);

            var src = VoiceManager.CreateVoiceSource(
                v2,
                "V2Defeat",
                V2Character.DefeatClip,
                null,
                true
            );

            if (src == null)
                yield break;

            VoiceManager.ShowSubtitle("This isn't over! Mark my words...", src);
        }

        static IEnumerator PlayEscape(V2 v2)
        {
            yield return new WaitForSeconds(1f);

            VoiceManager.CreateVoiceSource(
                v2,
                "V2Escape",
                V2Character.EscapingClip,
                "I won't give you the PLEASURE of killing me!",
                true
            );
        }

        static IEnumerator PlayRealization(V2 v2)
        {
            yield return new WaitForSeconds(0.5f);

            VoiceManager.CreateVoiceSource(
                v2.transform,
                "V2Realization",
                V2Character.FlailingClip,
                "No... NO!",
                true
            );
        }
    }

    [HarmonyPatch(typeof(V2), "OnDamage")]
    class V2PainPatch
    {
        static void Postfix(V2 __instance, ref DamageData data)
        {
            if (data.damage <= 2f)
                return;

            if (__instance.dead)
                return;

            if (!UltraVoicePlugin.V2VoiceEnabled.value)
                return;

            if (!VoiceManager.CheckCooldown(__instance, 0.1f))
                return;

            int i = UnityEngine.Random.Range(0, V2Character.PainClips.Length);

            VoiceManager.CreateVoiceSource(
                __instance,
                "V2Pain",
                V2Character.PainClips[i]
            );
        }
    }

    [HarmonyPatch(typeof(V2), "InstaEnrage")]
    class V2InstaEnragePatch
    {
        static void Prefix(V2 __instance)
        {
            if (!UltraVoicePlugin.V2VoiceEnabled.value)
                return;

            if (__instance.enraged)
                return;

            VoiceManager.CreateVoiceSource(
                __instance,
                "V2Enrage",
                V2Character.EnragePunchedClip,
                "EXCUSE ME?!",
                true
            );
        }
    }

    [HarmonyPatch(typeof(V2), "Enrage", new System.Type[] { })]
    class V2PatienceEnragePatch
    {
        static void Postfix(V2 __instance)
        {
            if (!UltraVoicePlugin.V2VoiceEnabled.value)
                return;

            VoiceManager.CreateVoiceSource(
                __instance,
                "V2Enrage",
                V2Character.EnragePatienceClip,
                "COME HERE!"
            );
        }
    }

    [HarmonyPatch(typeof(Animator), "Play",
                new System.Type[] { typeof(string), typeof(int), typeof(float) })]
    class V2CutsceneVoicePatch
    {
        static void Postfix(Animator __instance, string stateName, int layer, float normalizedTime)
        {
            if (!UltraVoicePlugin.V2VoiceEnabled.value)
                return;

            if (__instance == null)
                return;

            var go = __instance.gameObject;

            if (go.name != "v2_GreenArm")
                return;

            if (V2Character.V2CutsceneVoicePlayed)
                return;

            UltraVoicePlugin.Instance.StartCoroutine(PlayCutsceneVoice(__instance));
        }

        static IEnumerator PlayCutsceneVoice(Animator v2)
        {
            yield return new WaitForSeconds(0.25f);

            V2Character.V2CutsceneVoicePlayed = true;

            var src = VoiceManager.CreateVoiceSource(
                v2,
                "V2Cutscene",
                V2Character.IntroSecondClip
            );

            VoiceManager.ShowSubtitle("There you are.", src);

            if (v2 == null || !src.isPlaying) yield break;
            yield return new WaitForSeconds(1.25f);

            VoiceManager.ShowSubtitle("I was wondering how long you'd keep my arm...", src);

            if (v2 == null || !src.isPlaying) yield break;
            yield return new WaitForSeconds(2.75f);

            VoiceManager.ShowSubtitle("Don't worry, I'll pry it off you myself.", src);
        }
    }

    [HarmonyPatch(typeof(GameObject), "SetActive")]
    class V2DeathPatch
    {
        static void Postfix(GameObject __instance, bool value)
        {
            if (__instance.name != "v2_GreenArm")
                return;

            if (!UltraVoicePlugin.V2VoiceEnabled.value)
                return;

            if (V2Character.V2DeathPlayed) return;

            Transform t = __instance.transform;
            bool found = false;

            while (t != null)
            {
                if (t.name.Contains("8 Stuff(Clone)(Clone)"))
                {
                    found = true;
                    break;
                }
                t = t.parent;
            }

            if (!found)
                return;

            VoiceManager.CreateVoiceSource(
                __instance.transform,
                "V2Death",
                V2Character.DeathClip,
                "NOOOOOOO",
                true
            );
            V2Character.V2DeathPlayed = true;
        }
    }
}