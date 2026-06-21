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
            try
            {
                if (__instance == null) return;

                int max = (UltraVoicePlugin.SubtitleLimit != null) ? UltraVoicePlugin.SubtitleLimit.value : 5;
                Transform container = __instance.container;
                if (container == null) return;

                int activeCount = 0;
                for (int i = 0; i < container.childCount; i++)
                {
                    var child = container.GetChild(i);
                    var sub = child.GetComponent<Subtitle>();
                    if (sub != null && child.gameObject.activeInHierarchy) activeCount++;
                }

                while (activeCount >= max)
                {
                    Subtitle oldest = null;
                    int lowestSibling = int.MaxValue;
                    for (int i = 0; i < container.childCount; i++)
                    {
                        var child = container.GetChild(i);
                        var sub = child.GetComponent<Subtitle>();
                        if (sub == null || !child.gameObject.activeInHierarchy) continue;
                        int idx = child.GetSiblingIndex();
                        if (idx < lowestSibling)
                        {
                            lowestSibling = idx;
                            oldest = sub;
                        }
                    }

                    if (oldest == null) break;

                    Transform parent = oldest.transform.parent;
                    Object.Destroy(oldest.gameObject);
                    activeCount--;
                    if (parent != null) ForceRebuildContainerLayout(parent);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"SubtitleLimitPatch.Prefix error: {ex}");
            }
        }

        public static void Postfix(string caption, AudioSource audioSource, bool ignoreSetting, SubtitleController __instance)
        {
            try
            {
                if (__instance == null) return;

                Transform container = __instance.container;
                if (container == null) return;

                Subtitle created = null;
                for (int i = container.childCount - 1; i >= 0; i--)
                {
                    var sub = container.GetChild(i).GetComponent<Subtitle>();
                    if (sub != null)
                    {
                        created = sub;
                        break;
                    }
                }

                if (created == null) return;

                if (created.GetComponent<SubtitleWatcher>() == null)
                {
                    var watcher = created.gameObject.AddComponent<SubtitleWatcher>();
                    watcher.source = audioSource;      // may be null
                    watcher.controller = __instance;
                    watcher.spawnTime = Time.time;    // set before starting
                    watcher.StartWatching();          // start coroutine now that spawnTime is set
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"SubtitleLimitPatch.Postfix error: {ex}");
            }
        }

        public static void ForceRebuildContainerLayout(Transform container)
        {
            if (container == null) return;
            var rect = container as RectTransform ?? container.GetComponent<RectTransform>();
            if (rect != null) LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        }
    }

    public class SubtitleWatcher : MonoBehaviour
    {
        public AudioSource source;
        public SubtitleController controller;
        public float spawnTime;

        private Subtitle subtitle;
        private CanvasGroup group;
        private float fadeOutSpeed;
        private Coroutine monitorCoroutine;

        private const float WaitBeforeFadeAfterAudio = 5f;
        private const float LifetimeBeforeAutoFade = 5f;
        private const float FallbackFadeDuration = 0.6f;

        private void Awake()
        {
            subtitle = GetComponent<Subtitle>();
            if (subtitle != null)
            {
                group = subtitle.group ?? subtitle.GetComponent<CanvasGroup>();
                fadeOutSpeed = subtitle.fadeOutSpeed;
            }
        }

        public void StartWatching()
        {
            if (monitorCoroutine == null)
                monitorCoroutine = StartCoroutine(MonitorLifecycle());
        }

        private void OnDisable()
        {
            if (monitorCoroutine != null) StopCoroutine(monitorCoroutine);
        }

        private IEnumerator MonitorLifecycle()
        {
            while (true)
            {
                int active = CountActiveSubtitles();
                int max = (UltraVoicePlugin.SubtitleLimit != null) ? UltraVoicePlugin.SubtitleLimit.value : 5;
                if (active > max)
                {
                    Transform parent = subtitle.transform.parent;
                    Object.Destroy(subtitle.gameObject);
                    SubtitleLimitPatch.ForceRebuildContainerLayout(parent);
                    Destroy(this);
                    yield break;
                }

                if (Time.time - spawnTime >= LifetimeBeforeAutoFade)
                {
                    yield return StartCoroutine(FadeOutAndDestroy());
                    yield break;
                }

                if (source != null && source.isPlaying)
                {
                    while (source != null && source.isPlaying)
                    {
                        active = CountActiveSubtitles();
                        if (active > max)
                        {
                            Transform parent = subtitle.transform.parent;
                            Object.Destroy(subtitle.gameObject);
                            SubtitleLimitPatch.ForceRebuildContainerLayout(parent);
                            Destroy(this);
                            yield break;
                        }

                        if (Time.time - spawnTime >= LifetimeBeforeAutoFade)
                        {
                            yield return StartCoroutine(FadeOutAndDestroy());
                            yield break;
                        }

                        yield return null;
                    }

                    // audio stopped: wait 2s, with checks
                    float waited = 0f;
                    while (waited < WaitBeforeFadeAfterAudio)
                    {
                        active = CountActiveSubtitles();
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
                            break;
                        }

                        if (Time.time - spawnTime >= LifetimeBeforeAutoFade)
                        {
                            yield return StartCoroutine(FadeOutAndDestroy());
                            yield break;
                        }

                        waited += Time.deltaTime;
                        yield return null;
                    }
                }
                else
                {
                    float timeUntilLifetime = LifetimeBeforeAutoFade - (Time.time - spawnTime);
                    if (timeUntilLifetime > 0f)
                    {
                        float sleep = Mathf.Min(0.2f, timeUntilLifetime);
                        yield return new WaitForSeconds(sleep);
                    }
                }

                yield return null;
            }
        }

        private int CountActiveSubtitles()
        {
            SubtitleController ctrl = controller ?? MonoSingleton<SubtitleController>.Instance;
            Transform container = (ctrl != null) ? ctrl.container : null;

            if (container != null)
            {
                int c = 0;
                for (int i = 0; i < container.childCount; i++)
                {
                    var s = container.GetChild(i).GetComponent<Subtitle>();
                    if (s != null && s.gameObject.activeInHierarchy) c++;
                }
                return c;
            }

            var all = Object.FindObjectsOfType<Subtitle>();
            int activeCount = 0;
            foreach (var s in all) if (s != null && s.gameObject.activeInHierarchy) activeCount++;
            return activeCount;
        }

        private IEnumerator FadeOutAndDestroy()
        {
            if (group == null)
            {
                Transform p = subtitle.transform.parent;
                Object.Destroy(subtitle.gameObject);
                SubtitleLimitPatch.ForceRebuildContainerLayout(p);
                Destroy(this);
                yield break;
            }

            float startAlpha = Mathf.Clamp01(group.alpha);
            float duration = FallbackFadeDuration;
            if (fadeOutSpeed > 0.00001f) duration = Mathf.Max(0.01f, startAlpha / fadeOutSpeed);

            if (duration <= 0.0001f)
            {
                Transform p = subtitle.transform.parent;
                Object.Destroy(subtitle.gameObject);
                SubtitleLimitPatch.ForceRebuildContainerLayout(p);
                Destroy(this);
                yield break;
            }

            float t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                group.alpha = Mathf.Lerp(startAlpha, 0f, t / duration);
                yield return null;
            }

            group.alpha = 0f;
            MonoSingleton<SubtitleController>.Instance?.NotifyHoldEnd(subtitle);
            Transform parentAfter = subtitle.transform.parent;
            Object.Destroy(subtitle.gameObject);
            SubtitleLimitPatch.ForceRebuildContainerLayout(parentAfter);
            Destroy(this);
        }
    }
}