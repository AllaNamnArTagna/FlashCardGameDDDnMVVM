using System.Collections;
using Flashcards.Domain;
using Flashcards.ViewModel;
using UnityEngine;
using UnityEngine.UI;

namespace Flashcards.View
{
    public sealed class QuizView : MonoBehaviour
    {
        private QuizViewModel _vm;
        private DogView _dog;

        private Text _scoreText;
        private Text _streakText;
        private Text _progressText;
        private Text _topicText;
        private Text _promptText;
        private Text _explanationText;
        private Text _resultText;
        private Text _nextLabel;
        private Image _explanationBg;
        private RectTransform _cardRoot;
        private RectTransform _nextButtonRoot;
        private RectTransform _resultsRoot;
        private Button[] _choiceButtons = new Button[4];
        private Text[] _choiceLabels = new Text[4];
        private FlashcardButtonView[] _choiceViews = new FlashcardButtonView[4];
        private Button _nextButton;
        private Button _againButton;
        private Coroutine _animation;

        public static QuizView Create(Canvas canvas, QuizViewModel vm, DogView dog)
        {
            var go = new GameObject("QuizView", typeof(RectTransform));
            go.transform.SetParent(canvas.transform, false);
            var view = go.AddComponent<QuizView>();
            view._vm = vm;
            view._dog = dog;
            view.BuildUi();
            view.Bind();
            return view;
        }

        private void BuildUi()
        {
            RectTransform root = (RectTransform)transform;
            StretchToParent(root);

            BuildHud(root);
            BuildCard(root);
            BuildExplanation(root);
            BuildResults(root);
        }

        private void BuildHud(RectTransform root)
        {
            _scoreText = UIElements.CreateText(root, "ScoreText", "Score: 0", 22, TextAnchor.MiddleLeft, UIElements.AccentColor);
            PlaceTopLeft(_scoreText.rectTransform, 24f, -34f, new Vector2(220f, 30f));

            _progressText = UIElements.CreateText(root, "ProgressText", "0 / 0", 22, TextAnchor.MiddleRight, UIElements.TextColor);
            PlaceTopRight(_progressText.rectTransform, -24f, -34f, new Vector2(220f, 30f));

            _streakText = UIElements.CreateText(root, "StreakText", "Streak: 0", 22, TextAnchor.MiddleCenter, UIElements.TextColor);
            PlaceTopCenter(_streakText.rectTransform, -34f, new Vector2(220f, 30f));
        }

        private void BuildCard(RectTransform root)
        {
            _cardRoot = UIElements.CreateImage(root, "Card", UIElements.RoundedSprite, UIElements.CardColor).rectTransform;
            _cardRoot.anchorMin = _cardRoot.anchorMax = new Vector2(0.5f, 0.5f);
            _cardRoot.pivot = new Vector2(0.5f, 0.5f);
            _cardRoot.sizeDelta = new Vector2(780f, 470f);
            _cardRoot.anchoredPosition = new Vector2(0f, 30f);
            _cardRoot.localScale = new Vector3(1f, 0f, 1f);

            _topicText = UIElements.CreateText(_cardRoot, "TopicText", "", 18, TextAnchor.MiddleCenter, UIElements.AccentColor);
            PlaceInside(_topicText.rectTransform, new Vector2(0f, 0.95f), new Vector2(1f, 0.95f), new Vector2(0f, -8f), new Vector2(0f, 26f));

            _promptText = UIElements.CreateText(_cardRoot, "PromptText", "", 30, TextAnchor.MiddleCenter, UIElements.TextColor);
            PlaceInside(_promptText.rectTransform, new Vector2(0f, 0.94f), new Vector2(1f, 0.52f), Vector2.zero, new Vector2(60f, 0f));

            var buttonsRoot = new GameObject("Choices", typeof(RectTransform)).transform as RectTransform;
            buttonsRoot.SetParent(_cardRoot, false);
            buttonsRoot.anchorMin = new Vector2(0.05f, 0.05f);
            buttonsRoot.anchorMax = new Vector2(0.95f, 0.48f);
            buttonsRoot.offsetMin = Vector2.zero;
            buttonsRoot.offsetMax = Vector2.zero;

            var layout = buttonsRoot.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 12f;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlHeight = true;
            layout.childControlWidth = true;
            layout.childForceExpandHeight = true;
            layout.childForceExpandWidth = true;

            for (int i = 0; i < 4; i++)
            {
                Button button = UIElements.CreateButton(buttonsRoot, $"Choice{i + 1}", "", 22);
                int index = i;
                button.onClick.AddListener(() => Submit(index));
                _choiceButtons[i] = button;
                _choiceLabels[i] = button.GetComponentInChildren<Text>();
                _choiceViews[i] = button.GetComponent<FlashcardButtonView>();
            }

            _nextButton = UIElements.CreateButton(_cardRoot, "NextButton", "Next", 22);
            _nextButtonRoot = _nextButton.GetComponent<RectTransform>();
            _nextButtonRoot.anchorMin = _nextButtonRoot.anchorMax = new Vector2(0.95f, 0.05f);
            _nextButtonRoot.pivot = new Vector2(1f, 0f);
            _nextButtonRoot.sizeDelta = new Vector2(170f, 50f);
            _nextButtonRoot.anchoredPosition = new Vector2(18f, 18f);
            _nextButtonRoot.localScale = Vector3.zero;
            _nextButton.image.color = UIElements.AccentColor;
            _nextButton.GetComponent<FlashcardButtonView>().CacheColors(UIElements.AccentColor, new Color32(255, 214, 120, 255));
            _nextLabel = _nextButton.GetComponentInChildren<Text>();
            _nextLabel.color = new Color32(40, 36, 30, 255);
            _nextButton.onClick.AddListener(OnNextClicked);
        }

        private void BuildExplanation(RectTransform root)
        {
            _explanationBg = UIElements.CreateImage(root, "Explanation", UIElements.RoundedSprite, UIElements.PanelColor);
            RectTransform panel = _explanationBg.rectTransform;
            panel.anchorMin = new Vector2(0.03f, 0.03f);
            panel.anchorMax = new Vector2(0.97f, 0.16f);
            panel.offsetMin = Vector2.zero;
            panel.offsetMax = Vector2.zero;
            panel.gameObject.SetActive(false);

            _explanationText = UIElements.CreateText(panel, "ExplanationText", "", 20, TextAnchor.MiddleLeft, UIElements.TextColor);
            StretchWithPadding(_explanationText.rectTransform, 24f, 14f);
        }

        private void BuildResults(RectTransform root)
        {
            _resultsRoot = UIElements.CreateImage(root, "Results", UIElements.RoundedSprite, UIElements.CardColor).rectTransform;
            _resultsRoot.anchorMin = _resultsRoot.anchorMax = new Vector2(0.5f, 0.5f);
            _resultsRoot.sizeDelta = new Vector2(680f, 360f);
            _resultsRoot.anchoredPosition = new Vector2(0f, 30f);
            _resultsRoot.localScale = Vector3.zero;

            _resultText = UIElements.CreateText(_resultsRoot, "ResultText", "", 30, TextAnchor.MiddleCenter, UIElements.TextColor);
            PlaceInside(_resultText.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 0.2f), new Vector2(0f, -30f), new Vector2(60f, 0f));

            _againButton = UIElements.CreateButton(_resultsRoot, "PlayAgainButton", "Play again", 24);
            RectTransform againRoot = _againButton.GetComponent<RectTransform>();
            againRoot.anchorMin = againRoot.anchorMax = new Vector2(0.5f, 0f);
            againRoot.pivot = new Vector2(0.5f, 0f);
            againRoot.sizeDelta = new Vector2(220f, 56f);
            againRoot.anchoredPosition = new Vector2(0f, 36f);
            _againButton.image.color = UIElements.CorrectColor;
            _againButton.GetComponent<FlashcardButtonView>().CacheColors(UIElements.CorrectColor, new Color32(110, 210, 140, 255));
            _againButton.GetComponentInChildren<Text>().color = new Color32(28, 44, 34, 255);
            _againButton.onClick.AddListener(OnPlayAgain);
        }

        private void Bind()
        {
            _vm.StateChanged += RefreshHud;
            _vm.QuestionPresented += OnQuestionPresented;
            _vm.Answered += OnAnswered;
            _vm.QuizFinished += OnQuizFinished;
        }

        private void OnDestroy()
        {
            if (_vm == null) return;
            _vm.StateChanged -= RefreshHud;
            _vm.QuestionPresented -= OnQuestionPresented;
            _vm.Answered -= OnAnswered;
            _vm.QuizFinished -= OnQuizFinished;
        }

        private void RefreshHud()
        {
            _scoreText.text = $"Score: {_vm.Score}";
            _streakText.text = $"Streak: {_vm.Streak}";
            _progressText.text = $"{_vm.QuestionsAnswered} / {_vm.QuestionCount}";
        }

        private void Submit(int choiceIndex)
        {
            _vm.SubmitAnswer(choiceIndex);
        }

        private void OnNextClicked()
        {
            HideNextButton();
            _vm.Next();
        }

        private void OnPlayAgain()
        {
            StartCoroutine(ScaleTo(_resultsRoot, Vector3.zero, 0.2f));
            _explanationBg.gameObject.SetActive(false);
            _cardRoot.gameObject.SetActive(true);
            _cardRoot.localScale = Vector3.zero;
            _dog.SetNeutral();
            _vm.StartNewQuiz();
        }

        private void OnQuestionPresented()
        {
            Question question = _vm.CurrentQuestion;
            _topicText.text = question.Topic.ToUpperInvariant();
            _promptText.text = question.Prompt;
            for (int i = 0; i < 4; i++)
            {
                _choiceLabels[i].text = question.Choices[i];
                _choiceViews[i].ResetColor();
                _choiceButtons[i].image.color = UIElements.ButtonColor;
            }
            _explanationBg.gameObject.SetActive(false);
            RefreshHud();
            _nextLabel.text = _vm.QuestionsAnswered == _vm.QuestionCount - 1 ? "See results" : "Next";

            if (_animation != null) StopCoroutine(_animation);
            _animation = StartCoroutine(FlipInRoutine());
        }

        private void OnAnswered(AnswerOutcome outcome)
        {
            RefreshHud();

            for (int i = 0; i < 4; i++)
            {
                if (i == outcome.ChosenIndex || i == outcome.CorrectIndex)
                    continue;
                Color dim = UIElements.ButtonColor;
                dim.a = 0.45f;
                _choiceViews[i].SetColor(dim);
            }

            _choiceViews[outcome.CorrectIndex].SetColor(UIElements.CorrectColor);
            if (outcome.WasCorrect)
                _choiceViews[outcome.ChosenIndex].SetColor(UIElements.CorrectColor);
            else
                _choiceViews[outcome.ChosenIndex].SetColor(UIElements.WrongColor);

            _dog.React(outcome.WasCorrect);

            string prefix = outcome.WasCorrect
                ? $"+{outcome.PointsAwarded} points! "
                : "Woof... not quite. ";
            _explanationText.text = prefix + outcome.Explanation;
            _explanationBg.gameObject.SetActive(true);

            if (_animation != null) StopCoroutine(_animation);
            _animation = StartCoroutine(ShowNextButtonRoutine());
        }

        private void OnQuizFinished()
        {
            int total = _vm.QuestionCount;
            _resultText.text = $"You scored {_vm.Score} points!\n\nCorrect answers: {_vm.CorrectCount} / {total}\nBest streak: {_vm.BestStreak}";
            _explanationBg.gameObject.SetActive(false);
            _dog.Celebrate();

            if (_animation != null) StopCoroutine(_animation);
            _animation = StartCoroutine(ShowResultsRoutine());
        }

        private IEnumerator FlipInRoutine()
        {
            yield return ScaleTo(_cardRoot, new Vector3(1f, 0f, 1f), 0.14f);
            yield return ScaleTo(_cardRoot, Vector3.one, 0.14f);
        }

        private IEnumerator ShowNextButtonRoutine()
        {
            _nextButtonRoot.localScale = Vector3.zero;
            yield return new WaitForSeconds(0.35f);
            yield return ScaleTo(_nextButtonRoot, Vector3.one, 0.16f);
        }

        private void HideNextButton()
        {
            _nextButtonRoot.localScale = Vector3.zero;
        }

        private IEnumerator ShowResultsRoutine()
        {
            yield return ScaleTo(_cardRoot, Vector3.zero, 0.22f);
            _cardRoot.gameObject.SetActive(false);
            _nextButtonRoot.localScale = Vector3.zero;
            _resultsRoot.gameObject.SetActive(true);
            _resultsRoot.localScale = Vector3.zero;
            yield return ScaleTo(_resultsRoot, Vector3.one, 0.25f);
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

        private static void StretchToParent(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private static void StretchWithPadding(RectTransform rect, float horizontal, float vertical)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(horizontal, vertical);
            rect.offsetMax = new Vector2(-horizontal, -vertical);
        }

        private static void PlaceTopLeft(RectTransform rect, float x, float y, Vector2 size)
        {
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 0.5f);
            rect.anchoredPosition = new Vector2(x, y);
            rect.sizeDelta = size;
        }

        private static void PlaceTopRight(RectTransform rect, float x, float y, Vector2 size)
        {
            rect.anchorMin = new Vector2(1f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(1f, 0.5f);
            rect.anchoredPosition = new Vector2(x, y);
            rect.sizeDelta = size;
        }

        private static void PlaceTopCenter(RectTransform rect, float y, Vector2 size)
        {
            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchorMax = new Vector2(0.5f, 1f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = new Vector2(0f, y);
            rect.sizeDelta = size;
        }

        private static void PlaceInside(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 offset, Vector2 padding)
        {
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.offsetMin = new Vector2(padding.x, offset.y + padding.y);
            rect.offsetMax = new Vector2(-padding.x, offset.y - padding.y);
        }
    }
}
