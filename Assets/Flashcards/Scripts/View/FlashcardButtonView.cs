using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Flashcards.View
{
    [RequireComponent(typeof(Image))]
    public sealed class FlashcardButtonView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private Image _image;
        private Color _normalColor;
        private Color _hoverColor;
        private bool _locked;

        public bool Locked => _locked;

        public void CacheColors(Color normal, Color hover)
        {
            _image = GetComponent<Image>();
            _normalColor = normal;
            _hoverColor = hover;
        }

        public void SetColor(Color color)
        {
            _locked = true;
            if (_image == null) _image = GetComponent<Image>();
            _image.color = color;
        }

        public void ResetColor()
        {
            _locked = false;
            if (_image != null) _image.color = _normalColor;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_locked || _image == null) return;
            _image.color = _hoverColor;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_locked || _image == null) return;
            _image.color = _normalColor;
        }
    }
}
