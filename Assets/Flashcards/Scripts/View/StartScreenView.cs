using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Flashcards.View
{
    public sealed class StartScreenView : MonoBehaviour
    {
        private RectTransform _root;
        private Action _onStart;

        public static StartScreenView Create(Canvas canvas, Action onStart)
        {
            var go = new GameObject("StartScreenView", typeof(RectTransform));
            go.transform.SetParent(canvas.transform, false);
            var view = go.AddComponent<StartScreenView>();
            view._onStart = onStart;
            view.BuildUi();
            return view;
        }

        private void BuildUi()
        {
            _root = (RectTransform)transform;

            var panel = UIElements.CreateImage(_root, "Panel", UIElements.RoundedSprite, UIElements.CardColor).rectTransform;
            panel.anchorMin = panel.anchorMax = new Vector2(0.5f, 0.5f);
            panel.sizeDelta = new Vector2(640f, 400f);
            panel.localScale = Vector3.zero;

            var title = UIElements.CreateText(panel, "Title", "DDD & MVVM Flashcards", 44, TextAnchor.MiddleCenter, UIElements.TextColor);
            Stretch(title.rectTransform);
            title.rectTransform.anchoredPosition = new Vector2(0f, 70f);
            title.rectTransform.sizeDelta = new Vector2(560f, 60f);

            var subtitle = UIElements.CreateText(panel, "Subtitle", "Learn architecture patterns with the help of a very good dog.", 20, TextAnchor.MiddleCenter, new Color(0.93f, 0.94f, 0.97f, 0.8f));
            Stretch(subtitle.rectTransform);
            subtitle.rectTransform.anchoredPosition = new Vector2(0f, 10f);
            subtitle.rectTransform.sizeDelta = new Vector2(520f, 60f);

            Button start = UIElements.CreateButton(panel, "StartButton", "Start", 26);
            RectTransform startRect = start.GetComponent<RectTransform>();
            startRect.anchorMin = startRect.anchorMax = new Vector2(0.5f, 0f);
            startRect.pivot = new Vector2(0.5f, 0f);
            startRect.sizeDelta = new Vector2(220f, 56f);
            startRect.anchoredPosition = new Vector2(0f, 40f);
            start.image.color = UIElements.AccentColor;
            start.GetComponent<FlashcardButtonView>().CacheColors(UIElements.AccentColor, new Color32(255, 214, 120, 255));
            start.GetComponentInChildren<Text>().color = new Color32(40, 36, 30, 255);
            start.onClick.AddListener(OnStartClicked);

            StartCoroutine(AnimateIn(panel));
        }

        private void OnStartClicked()
        {
            _onStart?.Invoke();
        }

        public void Hide()
        {
            StartCoroutine(AnimateOut(() => gameObject.SetActive(false)));
        }

        private IEnumerator AnimateIn(RectTransform panel)
        {
            yield return ScaleTo(panel, Vector3.one, 0.3f);
        }

        private IEnumerator AnimateOut(Action onDone)
        {
            yield return ScaleTo((RectTransform)transform, Vector3.zero, 0.22f);
            onDone?.Invoke();
        }

        private static void Stretch(RectTransform rect)
        {
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
        }

        private static IEnumerator ScaleTo(RectTransform target, Vector3 targetScale, float duration)
        {
            Vector3 start = target.localScale;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / duration));
                target.localScale = Vector3.Lerp(start, targetScale, t);
                yield return null;
            }
            target.localScale = targetScale;
        }
    }
}
