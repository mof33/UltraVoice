using HarmonyLib;
using System;
using System.Reflection;
using UltraVoice.Utilities;
using UnityEngine;

namespace UltraVoice.Characters
{
    public class SoldierCharacter
    {
        public static AudioClip[] ChatterClips;
        public static AudioClip AttackClip;
        public static AudioClip DeathClip;

        public static void LoadVoiceLines(BepInEx.Logging.ManualLogSource logger)
        {
            ChatterClips = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip("Soldier.sol_Chatter1.wav"),
                UltraVoicePlugin.LoadClip("Soldier.sol_Chatter1.wav"),
                UltraVoicePlugin.LoadClip("Soldier.sol_Chatter1.wav"),
            };

            AttackClip = UltraVoicePlugin.LoadClip("Soldier.sol_Attack.wav");
            DeathClip = UltraVoicePlugin.LoadClip("Soldier.sol_Death.wav");

            logger.LogInfo("Soldier voice lines loaded successfully!");
        }
    }

    [HarmonyPatch(typeof(ZombieProjectiles), "Update")]
    class SoldierChatterPatch
    {
        static void Postfix(ZombieProjectiles __instance)
        {
            if (!UltraVoicePlugin.SoldierVoiceEnabled.value) return;

            if (ULTRAKILL.Cheats.BlindEnemies.Blind)
                return;

            if (__instance.eid.enemyType != EnemyType.Soldier) return;

            if (!VoiceManager.CheckCooldown(__instance, 4f))
                return;

            if (UnityEngine.Random.Range(0f, 1f) < 0.75f)
                return;

            VoiceManager.PlayRandomVoice(__instance, "Soldier",
                SoldierCharacter.ChatterClips,
                null,
                randomPitch: true
            );
        }
    }

    [HarmonyPatch(typeof(ZombieProjectiles), nameof(ZombieProjectiles.ShootProjectile))]
    class SoldierThrowPatch
    {
        static void Postfix(ZombieProjectiles __instance)
        {
            if (!UltraVoicePlugin.SoldierVoiceEnabled.value) return;

            if (__instance.eid.enemyType != EnemyType.Soldier) return;

            VoiceManager.CreateVoiceSource(__instance, "Soldier",
                SoldierCharacter.AttackClip,
                null,
                randomPitch: true
            );
        }
    }

    [HarmonyPatch(typeof(ZombieProjectiles), nameof(ZombieProjectiles.Melee))]
    class SoldierKickPatch
    {
        static void Postfix(ZombieProjectiles __instance)
        {
            if (!UltraVoicePlugin.SoldierVoiceEnabled.value) return;

            if (__instance.eid.enemyType != EnemyType.Soldier) return;

            VoiceManager.CreateVoiceSource(__instance, "Soldier",
                SoldierCharacter.AttackClip,
                null,
                randomPitch: true
            );
        }
    }

    [HarmonyPatch(typeof(ZombieProjectiles), nameof(ZombieProjectiles.OnGoLimp))]
    class SoldierDeathPatch
    {
        static void Postfix(ZombieProjectiles __instance)
        {
            if (!UltraVoicePlugin.SoldierVoiceEnabled.value) return;

            if (__instance.eid.enemyType != EnemyType.Soldier) return;

            VoiceManager.CreateVoiceSource(__instance, "Soldier",
                SoldierCharacter.DeathClip,
                null,
                true,
                randomPitch: true
            );
        }
    }

    [HarmonyPatch]
    class ZombieMuteOnSoldierPatch
    {
        static MethodBase TargetMethod()
        {
            var t = AccessTools.TypeByName("Zombie");
            return t != null ? AccessTools.Method(t, "Awake") : null;
        }

        static void Postfix(object __instance)
        {
            if (__instance == null) return;

            var eid = __instance.GetType().GetField("eid", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetValue(__instance);
            var et = eid.GetType().GetField("enemyType", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetValue(eid).ToString();
            if (et != "Soldier") return;

            UltraVoicePlugin.SetArrayField(__instance.GetType(), __instance, "hurtSounds", typeof(AudioClip));
            UltraVoicePlugin.SetField(__instance.GetType(), __instance, "deathSound", null);
        }
    }

    [HarmonyPatch]
    class ZombieProjectilesMeleePatch
    {
        static MethodBase TargetMethod()
        {
            var t = AccessTools.TypeByName("ZombieProjectiles");
            return t != null ? AccessTools.Method(t, "Melee") : null;
        }

        static void Postfix(object __instance)
        {
            if (__instance == null) return;

            var eid = __instance.GetType().GetField("eid", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetValue(__instance);
            var et = eid.GetType().GetField("enemyType", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetValue(eid).ToString();
            if (et != "Soldier") return;

            var tr = __instance.GetType().GetField("tr", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetValue(__instance) as Component;
            if (tr != null)
                tr.GetComponent<AudioSource>()?.Stop();
        }
    }
}