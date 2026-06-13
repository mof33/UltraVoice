using HarmonyLib;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UltraVoice.Patches
{
    [HarmonyPatch(typeof(SubtitleController), nameof(SubtitleController.DisplaySubtitle),
        new[] { typeof(string), typeof(AudioSource), typeof(bool) })]
    public static class SubtitleLimitPatch
    {
        public static void Prefix(SubtitleController __instance)
        {
            if (__instance == null)
                return;

            int max = 5;
            if (UltraVoicePlugin.SubtitleLimit != null)
                max = UltraVoicePlugin.SubtitleLimit.value;

            Transform container = __instance.container;

            if (container != null)
            {
                int activeCount = CountActiveSubtitleChildren(container);
                bool destroyedAny = false;

                while (activeCount >= max)
                {
                    Subtitle oldest = FindOldestSubtitleChild(container);
                    if (oldest == null)
                        break;

                    Transform parent = oldest.transform.parent;
                    Object.Destroy(oldest.gameObject);
                    activeCount--;
                    destroyedAny = true;

                    if (parent != null)
                        ForceRebuildContainerLayout(parent);
                }

                if (destroyedAny)
                    ForceRebuildContainerLayout(container);
            }
            else
            {
                var all = Object.FindObjectsOfType<Subtitle>();
                int activeCount = 0;
                foreach (var s in all)
                {
                    if (s != null && s.gameObject.activeInHierarchy)
                        activeCount++;
                }

                bool destroyedAny = false;
                while (activeCount >= max)
                {
                    Subtitle oldest = null;
                    int lowestSibling = int.MaxValue;
                    foreach (var s in all)
                    {
                        if (s == null || !s.gameObject.activeInHierarchy) continue;
                        int idx = s.transform.GetSiblingIndex();
                        if (idx < lowestSibling)
                        {
                            lowestSibling = idx;
                            oldest = s;
                        }
                    }

                    if (oldest == null) break;

                    Transform parent = oldest.transform.parent;
                    Object.Destroy(oldest.gameObject);
                    activeCount--;
                    destroyedAny = true;

                    if (parent != null)
                        ForceRebuildContainerLayout(parent);
                }

                if (destroyedAny)
                {
                    if (all.Length > 0 && all[0] != null && all[0].transform.parent != null)
                        ForceRebuildContainerLayout(all[0].transform.parent);
                }
            }
        }

        public static void Postfix(string caption, AudioSource audioSource, bool ignoreSetting, SubtitleController __instance)
        {
            if (audioSource == null || __instance == null)
                return;

            Transform container = __instance.container;
            if (container == null)
                return;

            Subtitle created = null;
            for (int i = container.childCount - 1; i >= 0; i--)
            {
                var child = container.GetChild(i);
                if (child == null) continue;
                var sub = child.GetComponent<Subtitle>();
                if (sub != null)
                {
                    created = sub;
                    break;
                }
            }

            if (created == null) return;

            if (created.GetComponent<SubtitleAudioWatcher>() == null)
            {
                var watcher = created.gameObject.AddComponent<SubtitleAudioWatcher>();
                watcher.source = audioSource;
                watcher.controller = __instance;
            }
        }

        private static int CountActiveSubtitleChildren(Transform container)
        {
            int count = 0;
            for (int i = 0; i < container.childCount; i++)
            {
                var child = container.GetChild(i);
                if (child == null) continue;
                var sub = child.GetComponent<Subtitle>();
                if (sub != null && child.gameObject.activeInHierarchy)
                    count++;
            }
            return count;
        }

        private static Subtitle FindOldestSubtitleChild(Transform container)
        {
            Subtitle oldest = null;
            int lowestSibling = int.MaxValue;
            for (int i = 0; i < container.childCount; i++)
            {
                var child = container.GetChild(i);
                if (child == null) continue;
                var sub = child.GetComponent<Subtitle>();
                if (sub == null || !child.gameObject.activeInHierarchy) continue;

                int idx = child.GetSiblingIndex();
                if (idx < lowestSibling)
                {
                    lowestSibling = idx;
                    oldest = sub;
                }
            }
            return oldest;
        }

        public static void ForceRebuildContainerLayout(Transform container)
        {
            if (container == null) return;
            var rect = container as RectTransform;
            if (rect == null) rect = container.GetComponent<RectTransform>();
            if (rect != null)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
            }
        }
    }

    public class SubtitleAudioWatcher : MonoBehaviour
    {
        public AudioSource source;
        public SubtitleController controller;

        private Subtitle subtitle;
        private CanvasGroup group;
        private float fadeOutSpeed = -1f;
        private bool triggered;
        private Coroutine waitCoroutine;

        private const float WaitBeforeFade = 3f;
        private const float FallbackFadeDuration = 0.6f;

        private void Awake()
        {
            subtitle = GetComponent<Subtitle>();

            if (subtitle != null)
            {
                group = subtitle.group;
                fadeOutSpeed = subtitle.fadeOutSpeed;
            }

            if (group == null && subtitle != null)
                group = subtitle.GetComponent<CanvasGroup>();
        }

        private void OnDestroy()
        {
            if (waitCoroutine != null)
                StopCoroutine(waitCoroutine);
        }

        private void Update()
        {
            if (triggered || subtitle == null)
                return;

            if (source == null || !source.isPlaying)
            {
                triggered = true;

                int active = CountActiveSubtitles();

                int max = 5;
                if (UltraVoicePlugin.SubtitleLimit != null)
                    max = UltraVoicePlugin.SubtitleLimit.value;

                if (active > max)
                {
                    Transform parent = subtitle.transform.parent;
                    Object.Destroy(subtitle.gameObject);
                    SubtitleLimitPatch.ForceRebuildContainerLayout(parent);
                    Destroy(this);
                    return;
                }

                waitCoroutine = StartCoroutine(WaitThenFade());
            }
        }

        private IEnumerator WaitThenFade()
        {
            float elapsed = 0f;

            while (elapsed < WaitBeforeFade)
            {
                int active = CountActiveSubtitles();
                int max = 5;
                if (UltraVoicePlugin.SubtitleLimit != null)
                    max = UltraVoicePlugin.SubtitleLimit.value;

                if (active > max)
                {
                    Transform parent = subtitle.transform.parent;
                    Object.Destroy(subtitle.gameObject);
                    SubtitleLimitPatch.ForceRebuildContainerLayout(parent);
                    Destroy(this);
                    yield break;
                }

                if (source != null && source.isPlaying)
                {
                    triggered = false;
                    yield break;
                }

                elapsed += Time.deltaTime;
                yield return null;
            }

            if (subtitle == null)
            {
                Destroy(this);
                yield break;
            }

            yield return StartCoroutine(FadeOutAndDestroy());
        }

        private int CountActiveSubtitles()
        {
            if (controller != null)
            {
                Transform container = controller.container;
                if (container != null)
                {
                    int c = 0;
                    for (int i = 0; i < container.childCount; i++)
                    {
                        var child = container.GetChild(i);
                        if (child == null) continue;
                        var s = child.GetComponent<Subtitle>();
                        if (s != null && child.gameObject.activeInHierarchy)
                            c++;
                    }
                    return c;
                }
            }

            var all = Object.FindObjectsOfType<Subtitle>();
            int activeCount = 0;
            foreach (var s in all)
            {
                if (s != null && s.gameObject.activeInHierarchy)
                    activeCount++;
            }
            return activeCount;
        }

        private IEnumerator FadeOutAndDestroy()
        {
            if (group == null && subtitle != null)
            {
                group = subtitle.GetComponent<CanvasGroup>();
            }

            if (group == null)
            {
                Transform parent = subtitle.transform.parent;
                Object.Destroy(subtitle.gameObject);
                SubtitleLimitPatch.ForceRebuildContainerLayout(parent);
                Destroy(this);
                yield break;
            }

            float startAlpha = Mathf.Clamp01(group.alpha);
            float duration = FallbackFadeDuration;
            if (fadeOutSpeed > 0.00001f)
            {
                duration = Mathf.Max(0.01f, startAlpha / fadeOutSpeed);
            }

            if (duration <= 0.0001f)
            {
                Transform parent = subtitle.transform.parent;
                Object.Destroy(subtitle.gameObject);
                SubtitleLimitPatch.ForceRebuildContainerLayout(parent);
                Destroy(this);
                yield break;
            }

            float t = 0f;
            while (t < duration)
            {
                if (group == null) break;
                t += Time.deltaTime;
                float a = Mathf.Lerp(startAlpha, 0f, t / duration);
                group.alpha = a;
                yield return null;
            }

            if (group != null)
                group.alpha = 0f;

            controller?.NotifyHoldEnd(subtitle);

            Transform parentAfter = subtitle.transform.parent;
            Object.Destroy(subtitle.gameObject);

            SubtitleLimitPatch.ForceRebuildContainerLayout(parentAfter);

            Destroy(this);
        }
    }
}