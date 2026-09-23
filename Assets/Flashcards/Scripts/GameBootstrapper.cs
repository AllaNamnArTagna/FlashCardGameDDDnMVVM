using System;
using Flashcards.Domain;
using Flashcards.Infrastructure;
using Flashcards.View;
using Flashcards.ViewModel;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Flashcards
{
    public sealed class GameBootstrapper : MonoBehaviour
    {
        [Header("Dog placement (world units)")]
        [SerializeField] private Vector2 _dogScreenPosition = new Vector2(4.2f, -1.6f);
        [SerializeField] private float _dogScale = 1.8f;

        private Canvas _canvas;
        private QuizView _quizView;
        private StartScreenView _startScreen;

        private void Awake()
        {
            CreateCamera();
            CreateEventSystemIfMissing();
            _canvas = CreateCanvas();
            DogView dog = CreateDog();
            QuizViewModel vm = new QuizViewModel(new ResourceQuestionRepository(), new DefaultScorePolicy());

            _startScreen = StartScreenView.Create(_canvas, () =>
            {
                _startScreen.Hide();
                vm.StartNewQuiz();
            });
            _quizView = QuizView.Create(_canvas, vm, dog);
        }

        private void CreateCamera()
        {
            Camera cam = Camera.main;
            if (cam == null)
            {
                var go = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
                cam = go.GetComponent<Camera>();
                cam.backgroundColor = new Color32(24, 26, 38, 255);
                go.transform.position = new Vector3(0f, 0f, -10f);
            }

            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.orthographic = true;
            cam.orthographicSize = 5f;
        }

        private void CreateEventSystemIfMissing()
        {
            if (EventSystem.current != null)
                return;

            var go = new GameObject("EventSystem", typeof(EventSystem));
            var inputSystemModule = Type.GetType("UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem");
            if (inputSystemModule != null && !IsLegacyInputEnabled())
                go.AddComponent(inputSystemModule);
            else
                go.AddComponent<StandaloneInputModule>();
        }

        private static bool IsLegacyInputEnabled()
        {
            try
            {
                return UnityEngine.Input.touchCount >= 0;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        private Canvas CreateCanvas()
        {
            var go = new GameObject("FlashcardsCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = go.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = go.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280f, 720f);
            scaler.matchWidthOrHeight = 0.5f;

            return canvas;
        }

        private DogView CreateDog()
        {
            var go = new GameObject("Dog");
            go.transform.position = _dogScreenPosition;
            go.transform.localScale = Vector3.one * _dogScale;
            return go.AddComponent<DogView>();
        }
    }
}
