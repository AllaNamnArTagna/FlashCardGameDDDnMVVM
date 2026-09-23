using UnityEngine;

namespace Flashcards.View
{
    public sealed class DogView : MonoBehaviour
    {
        private SpriteRenderer _renderer;
        private Sprite _neutral;
        private Sprite _happy;
        private Sprite _sad;
        private Sprite _celebrate;
        private Vector3 _baseScale;
        private Coroutine _animation;

        private void Awake()
        {
            _renderer = gameObject.AddComponent<SpriteRenderer>();
            _renderer.sortingOrder = 10;
            _neutral = Resources.Load<Sprite>("DogSprites/dog_neutral");
            _happy = Resources.Load<Sprite>("DogSprites/dog_happy");
            _sad = Resources.Load<Sprite>("DogSprites/dog_sad");
            _celebrate = Resources.Load<Sprite>("DogSprites/dog_celebrate");
            _baseScale = transform.localScale;
            _renderer.sprite = _neutral;
        }

        public void SetNeutral()
        {
            if (_animation != null) StopCoroutine(_animation);
            _animation = null;
            transform.localScale = _baseScale;
            transform.rotation = Quaternion.identity;
            _renderer.sprite = _neutral;
        }

        public void React(bool wasCorrect)
        {
            if (_animation != null) StopCoroutine(_animation);
            _animation = StartCoroutine(wasCorrect ? HappyRoutine() : SadRoutine());
        }

        public void Celebrate()
        {
            if (_animation != null) StopCoroutine(_animation);
            _animation = StartCoroutine(CelebrateRoutine());
        }

        private System.Collections.IEnumerator HappyRoutine()
        {
            _renderer.sprite = _happy;
            float duration = 0.5f;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float bounce = Mathf.Abs(Mathf.Sin(elapsed / duration * Mathf.PI * 3f)) * 0.25f;
                transform.localScale = _baseScale * (1f + bounce);
                yield return null;
            }
            transform.localScale = _baseScale;
            _renderer.sprite = _neutral;
        }

        private System.Collections.IEnumerator SadRoutine()
        {
            _renderer.sprite = _sad;
            float duration = 0.6f;
            float elapsed = 0f;
            float startRotation = 0f;
            float endRotation = -12f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(startRotation, endRotation, t));
                yield return null;
            }
            yield return new WaitForSeconds(0.6f);
            elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(endRotation, startRotation, t));
                yield return null;
            }
            transform.rotation = Quaternion.identity;
            _renderer.sprite = _neutral;
        }

        private System.Collections.IEnumerator CelebrateRoutine()
        {
            _renderer.sprite = _celebrate;
            float duration = 1.6f;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float bounce = Mathf.Abs(Mathf.Sin(elapsed * 10f)) * 0.3f;
                transform.localScale = _baseScale * (1f + bounce);
                transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Sin(elapsed * 12f) * 8f);
                yield return null;
            }
            transform.localScale = _baseScale;
            transform.rotation = Quaternion.identity;
            _renderer.sprite = _neutral;
        }
    }
}
