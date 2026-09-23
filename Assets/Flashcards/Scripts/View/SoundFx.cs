using UnityEngine;

namespace Flashcards.View
{
    public sealed class SoundFx : MonoBehaviour
    {
        private static SoundFx _instance;
        private AudioSource _source;
        private AudioClip _click;
        private AudioClip _correct;
        private AudioClip _wrong;
        private AudioClip _win;

        private void Awake()
        {
            _instance = this;
            _source = gameObject.AddComponent<AudioSource>();
            _source.playOnAwake = false;
            _click = BuildClick();
            _correct = BuildCorrect();
            _wrong = BuildWrong();
            _win = BuildWin();
        }

        private void Play(AudioClip clip)
        {
            _source.PlayOneShot(clip);
        }

        public static void Click() => _instance?.Play(_instance._click);
        public static void Correct() => _instance?.Play(_instance._correct);
        public static void Wrong() => _instance?.Play(_instance._wrong);
        public static void Win() => _instance?.Play(_instance._win);

        private static AudioClip BuildClick()
        {
            return Build("click", 0.05f, t =>
            {
                float env = 1f - t;
                return Mathf.Sin(2f * Mathf.PI * 950f * t) * env * env * 0.3f;
            });
        }

        private static AudioClip BuildCorrect()
        {
            return Build("correct", 0.22f, t =>
            {
                float env = 1f - t;
                float freq = t < 0.45f ? 660f : 880f;
                return Mathf.Sin(2f * Mathf.PI * freq * t) * env * 0.45f;
            });
        }

        private static AudioClip BuildWrong()
        {
            return Build("wrong", 0.3f, t =>
            {
                float env = 1f - t;
                float buzz = Mathf.Sign(Mathf.Sin(2f * Mathf.PI * 165f * t));
                return buzz * env * env * 0.18f;
            });
        }

        private static AudioClip BuildWin()
        {
            return Build("win", 0.9f, t =>
            {
                float[] notes = { 523f, 659f, 784f, 1047f };
                float step = t / 0.22f;
                int i = (int)step;
                if (i > notes.Length - 1) i = notes.Length - 1;
                if (i < 0) i = 0;
                float noteT = Mathf.Clamp01((t - i * 0.22f) / 0.25f);
                float env = 1f - noteT;
                return Mathf.Sin(2f * Mathf.PI * notes[i] * t) * env * 0.4f;
            });
        }

        private static AudioClip Build(string name, float seconds, System.Func<float, float> sample)
        {
            const int rate = 44100;
            int count = (int)(seconds * rate);
            var data = new float[count];
            for (int i = 0; i < count; i++)
            {
                float t = (float)i / rate;
                data[i] = sample(t);
            }

            AudioClip clip = AudioClip.Create(name, count, 1, rate, false);
            clip.SetData(data, 0);
            return clip;
        }
    }
}
