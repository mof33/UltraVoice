using HarmonyLib;
using System.Collections;
using System.Collections.Generic;
using UltraVoice.Utilities;
using UnityEngine;


namespace UltraVoice.Characters
{
    public class IdolCharacter
    {
        public static AudioClip[] BlessClips;
        public static AudioClip[] VulnerableClips;

        public static readonly string[] BlessSubs =
        {
            "Peace be upon thee",
            "Thou art safe",
            "I bless thee",
            "No harm shall befall thee",
            "A kindness for thee, sinner"
        };

        public static readonly string[] VulnerableSubs =
        {
            "I am vulnerable",
            "I ask for thy mercy",
            "I deserve not thy ire",
            "Spare me thy wrath",
        };

        public static void LoadVoiceLines(BepInEx.Logging.ManualLogSource logger)
        {
            BlessClips = new[]
            {
                UltraVoicePlugin.LoadClip("Idol.idol_Spawn1.wav"),
                UltraVoicePlugin.LoadClip("Idol.idol_Spawn2.wav"),
                UltraVoicePlugin.LoadClip("Idol.idol_Spawn3.wav"),
                UltraVoicePlugin.LoadClip("Idol.idol_Spawn4.wav"),
                UltraVoicePlugin.LoadClip("Idol.idol_Spawn5.wav"),
            };

            VulnerableClips = new[]
            {
                UltraVoicePlugin.LoadClip("Idol.idol_Vulnerable1.wav"),
                UltraVoicePlugin.LoadClip("Idol.idol_Vulnerable2.wav"),
                UltraVoicePlugin.LoadClip("Idol.idol_Vulnerable3.wav"),
                UltraVoicePlugin.LoadClip("Idol.idol_Vulnerable4.wav")
            };

            logger.LogInfo("Idol voice lines loaded successfully!");
        }

        public static bool CanPlayerSeeIdol(NewMovement player, Idol idol)
        {
            Vector3 playerPos = player.transform.position;
            Vector3 idolPos = idol.transform.position;

            Vector3 direction = idolPos - playerPos;
            float distance = direction.magnitude;

            if (distance > 20f)
                return false;

            if (Physics.Raycast(playerPos, direction.normalized, out RaycastHit hit, distance))
            {
                return hit.transform == idol.transform ||
                       hit.transform.IsChildOf(idol.transform);
            }

            return false;
        }

        private static HashSet<Idol> visibleLastFrame = new HashSet<Idol>();

        public static bool JustBecameVisible(NewMovement player, Idol idol)
        {
            bool visible = CanPlayerSeeIdol(player, idol);

            if (visible && !visibleLastFrame.Contains(idol))
            {
                visibleLastFrame.Add(idol);
                return true;
            }

            if (!visible)
            {
                visibleLastFrame.Remove(idol);
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(Idol), nameof(Idol.ChangeTarget))]
    class IdolProtectPatch
    {
        static void Postfix(Idol __instance)
        {
            if (!UltraVoicePlugin.IdolVoiceEnabled.value)
                return;

            VoiceManager.PlayRandomVoice(__instance, "Idol",
                    IdolCharacter.BlessClips,
                    IdolCharacter.BlessSubs,
                    randomPitch: true
            );
        }
    }

    [HarmonyPatch(typeof(Idol), nameof(Idol.Update))]
    class IdolVulnerablePatch
    {
        static void Postfix(Idol __instance)
        {
            if (!UltraVoicePlugin.IdolVoiceEnabled.value)
                return;

            if (!IdolCharacter.JustBecameVisible(NewMovement.Instance, __instance)) return;

            UltraVoicePlugin.Instance.StartCoroutine(VulnerableVoice(__instance));

            static IEnumerator VulnerableVoice(Idol ferry)
            {
                yield return new WaitForSeconds(0.1f);

                VoiceManager.PlayRandomVoice(ferry, "Idol",
                    IdolCharacter.VulnerableClips,
                    IdolCharacter.VulnerableSubs,
                    randomPitch: true
                );
            }
        }
    }
}
