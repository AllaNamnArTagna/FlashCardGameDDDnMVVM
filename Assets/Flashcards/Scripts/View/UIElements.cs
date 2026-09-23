using UnityEngine;
using UnityEngine.UI;

namespace Flashcards.View
{
    public static class UIElements
    {
        public static readonly Color PanelColor = new Color32(36, 40, 59, 255);
        public static readonly Color CardColor = new Color32(52, 58, 84, 255);
        public static readonly Color ButtonColor = new Color32(72, 80, 116, 255);
        public static readonly Color ButtonHoverColor = new Color32(92, 102, 146, 255);
        public static readonly Color CorrectColor = new Color32(78, 176, 108, 255);
        public static readonly Color WrongColor = new Color32(214, 86, 86, 255);
        public static readonly Color TextColor = new Color32(236, 240, 248, 255);
        public static readonly Color AccentColor = new Color32(255, 200, 87, 255);

        public static Sprite RoundedSprite => Resources.Load<Sprite>("UI/rounded_bg");

        public static Text CreateText(Transform parent, string name, string content, int fontSize, TextAnchor anchor, Color color)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Text));
            go.transform.SetParent(parent, false);
            var text = go.GetComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.text = content;
            text.fontSize = fontSize;
            text.alignment = anchor;
            text.color = color;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.raycastTarget = false;
            return text;
        }

        public static Image CreateImage(Transform parent, string name, Sprite sprite, Color color)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            var image = go.GetComponent<Image>();
            image.sprite = sprite;
            image.color = color;
            image.raycastTarget = false;
            if (sprite != null && sprite.border != Vector4.zero)
            {
                image.type = Image.Type.Sliced;
                image.pixelsPerUnitMultiplier = 2f;
            }
            return image;
        }

        public static Button CreateButton(Transform parent, string name, string label, int fontSize)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);
            var image = go.GetComponent<Image>();
            image.sprite = RoundedSprite;
            image.type = Image.Type.Sliced;
            image.pixelsPerUnitMultiplier = 2f;
            image.color = ButtonColor;

            var button = go.GetComponent<Button>();
            var colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(1.1f, 1.1f, 1.1f, 1f);
            colors.pressedColor = new Color(0.85f, 0.85f, 0.85f, 1f);
            colors.disabledColor = new Color(1f, 1f, 1f, 0.35f);
            button.colors = colors;

            Text text = CreateText(go.transform, "Label", label, fontSize, TextAnchor.MiddleCenter, TextColor);
            RectTransform labelRect = text.rectTransform;
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;
            text.color = TextColor;

            var target = go.AddComponent<FlashcardButtonView>();
            target.CacheColors(image.color, ButtonHoverColor);
            button.targetGraphic = image;
            return button;
        }
    }
}
