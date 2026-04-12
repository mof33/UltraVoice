using System.Collections.Generic;
using ULTRAKILL.Cheats;
using UnityEngine;

namespace UltraVoice.Utilities
{
    public class VoiceManager
    {
        private static float lastVoiceTime = -999f;
        private static Dictionary<int, float> enemyVoiceCooldown = new Dictionary<int, float>();
        public static Dictionary<UnityEngine.Component, float> spawnVoiceEndTimes = new Dictionary<UnityEngine.Component, float>();
        public static Dictionary<UnityEngine.Component, float> enemySpawnTimes = new Dictionary<UnityEngine.Component, float>();

        public static UnityEngine.Color GetEnemyTypeColor(EnemyType enemyType)
        {
            return enemyType switch
            {
                EnemyType.Swordsmachine => new UnityEngine.Color(0.91f, 0.6f, 0.05f),
                EnemyType.Streetcleaner => new UnityEngine.Color(0.82f, 0.30f, 0.09f),
                EnemyType.Cerberus => new UnityEngine.Color(0.65f, 0.65f, 0.65f),
                EnemyType.Mindflayer => new UnityEngine.Color(0.26f, 0.89f, 0.74f),
                EnemyType.Virtue => new UnityEngine.Color(0.4f, 0.75f, 0.94f),
                EnemyType.Gutterman => new UnityEngine.Color(0.91f, 0.73f, 0.51f),
                EnemyType.Ferryman => new UnityEngine.Color(0f, 0.66f, 0.77f),
                EnemyType.Guttertank => new UnityEngine.Color(0.8f, 0.07f, 0.07f),
                EnemyType.Mannequin => new UnityEngine.Color(0.91f, 0.91f, 0.91f),
                EnemyType.Providence => new UnityEngine.Color(0.9f, 0.79f, 0.86f),
                _ => UnityEngine.Color.white
            };
        }

        public static UnityEngine.Color? GetSubtitleColor(UnityEngine.Component enemy, UnityEngine.Color? colorOverride)
        {
            if (colorOverride.HasValue)
                return colorOverride.Value;

            if (enemy.TryGetComponent<EnemyIdentifier>(out var eid))
            {
                return GetEnemyTypeColor(eid.enemyType);
            }

            return null;
        }

        public static bool CheckCooldown(UnityEngine.Component enemy, float cooldown)
        {
            int id = enemy.GetInstanceID();

            if (enemyVoiceCooldown.TryGetValue(id, out float last))
            {
                if (Time.time - last < cooldown)
                    return false;
            }

            enemyVoiceCooldown[id] = Time.time;
            return true;
        }

        public static bool CheckGlobalCooldown()
        {
            if (Time.time - lastVoiceTime < UltraVoicePlugin.VoiceCooldown.value)
                return false;

            lastVoiceTime = Time.time;
            return true;
        }

        public static bool IsSpawnVoicePlaying(UnityEngine.Component enemy)
        {
            if (!spawnVoiceEndTimes.TryGetValue(enemy, out float endTime))
                return false;

            return Time.time < endTime;
        }

        public static bool IsEnemyVoicePlaying(UnityEngine.Component enemy)
        {
            foreach (Transform child in enemy.transform)
            {
                if (child.name.StartsWith("UltraVoice_"))
                    return true;
            }
            return false;
        }

        public static bool TooSoonAfterSpawn(UnityEngine.Component enemy, float delay)
        {
            if (!enemySpawnTimes.TryGetValue(enemy, out float spawnTime))
                return false;

            return Time.time - spawnTime < delay;
        }

        public static AudioSource CreateVoiceSource(
            UnityEngine.Component enemy,
            string name,
            AudioClip clip,
            string subtitle = null,
            bool shouldInterrupt = false,
            UnityEngine.Color? subtitleColor = null,
            float pitch = 1f
        )
        {
            if (enemy == null || clip == null)
                return null;

            if (enemy.gameObject.name.Contains("Big Johninator"))
                return null;

            if (!shouldInterrupt)
            {
                if (!CheckGlobalCooldown())
                    return null;

                if (IsEnemyVoicePlaying(enemy))
                    return null;
            }
            else
            {
                InterruptVoices(enemy);
            }

            lastVoiceTime = Time.time;

            GameObject obj = new GameObject($"UltraVoice_{name}");
            obj.transform.SetParent(enemy.transform);
            obj.transform.localPosition = UnityEngine.Vector3.zero;

            var src = obj.AddComponent<AudioSource>();

            src.clip = clip;
            src.spatialBlend = 1f;
            src.volume = UltraVoicePlugin.VoiceVolume.value;
            src.volume *= 2.5f;
            src.pitch = pitch;
            src.minDistance = 50f;
            src.maxDistance = 200f;
            src.dopplerLevel = 0;

            var mixer = MonoSingleton<AudioMixerController>.Instance;
            if (mixer != null)
                src.outputAudioMixerGroup = mixer.allGroup;

            src.Play();

            var finalColor = GetSubtitleColor(enemy, subtitleColor);

            if (!string.IsNullOrEmpty(subtitle))
                ShowSubtitle(subtitle, src, finalColor);

            UnityEngine.Object.Destroy(obj, clip.length + 1f);

            return src;
        }

        public static void PlayRandomVoice(
            UnityEngine.Component enemy,
            string enemyName,
            AudioClip[] clips,
            string[] subtitles,
            bool interrupt = false,
            UnityEngine.Color? colorOverride = null
        )
        {
            if (clips == null || clips.Length == 0)
                return;

            int index = UnityEngine.Random.Range(0, clips.Length);
            string subtitle = null;

            if (subtitles != null && index < subtitles.Length)
                subtitle = subtitles[index];

            CreateVoiceSource(enemy, enemyName, clips[index], subtitle, interrupt, colorOverride);
        }

        public static void ShowSubtitle(string text, AudioSource src, UnityEngine.Color? color = null)
        {
            if (!string.IsNullOrEmpty(text))
            {
                if (color.HasValue)
                {
                    string coloredText = $"<color=#{ColorUtility.ToHtmlStringRGB(color.Value)}>{text}</color>";
                    MonoSingleton<SubtitleController>.Instance.DisplaySubtitle(coloredText, src);
                }
                else
                {
                    MonoSingleton<SubtitleController>.Instance.DisplaySubtitle(text, src);
                }
            }
        }

        public static void InterruptVoices(UnityEngine.Component enemy)
        {
            foreach (Transform child in enemy.transform)
            {
                if (child.name.StartsWith("UltraVoice_"))
                    UnityEngine.Object.Destroy(child.gameObject);
            }
        }
    }
}
