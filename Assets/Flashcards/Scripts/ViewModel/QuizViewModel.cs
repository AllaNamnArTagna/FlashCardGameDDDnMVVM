using System;
using System.Collections.Generic;
using Flashcards.Domain;

namespace Flashcards.ViewModel
{
    public sealed class QuizViewModel
    {
        private readonly IQuestionRepository _repository;
        private readonly IScorePolicy _scorePolicy;
        private QuizSession _session;
        private bool _canSubmit;

        public event Action StateChanged;
        public event Action QuestionPresented;
        public event Action<AnswerOutcome> Answered;
        public event Action QuizFinished;

        public QuizViewModel(IQuestionRepository repository, IScorePolicy scorePolicy)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _scorePolicy = scorePolicy ?? throw new ArgumentNullException(nameof(scorePolicy));
        }

        public QuizSession Session => _session;
        public Question CurrentQuestion => _session?.CurrentQuestion;
        public IReadOnlyList<string> CurrentChoices => CurrentQuestion?.Choices;
        public bool IsStarted => _session != null;
        public bool IsFinished => _session != null && _session.IsFinished;

        public int Score => _session?.Score ?? 0;
        public int Streak => _session?.Streak ?? 0;
        public int BestStreak => _session?.BestStreak ?? 0;
        public int CorrectCount => _session?.CorrectCount ?? 0;
        public int QuestionCount => _session?.QuestionCount ?? 0;
        public int QuestionsAnswered => _session?.QuestionsAnswered ?? 0;

        public void StartNewQuiz()
        {
            _session = new QuizSession(_repository.LoadAll(), _scorePolicy);
            _session.Answered += OnSessionAnswered;
            _canSubmit = true;
            StateChanged?.Invoke();
            QuestionPresented?.Invoke();
        }

        public void SubmitAnswer(int choiceIndex)
        {
            if (_session == null)
                throw new InvalidOperationException("The quiz has not been started.");
            if (!_canSubmit || _session.IsFinished)
                return;
            _canSubmit = false;
            _session.Answer(choiceIndex);
        }

        public void Next()
        {
            if (_session == null)
                throw new InvalidOperationException("The quiz has not been started.");
            if (_session.IsFinished)
            {
                QuizFinished?.Invoke();
                return;
            }
            _canSubmit = true;
            QuestionPresented?.Invoke();
        }

        private void OnSessionAnswered(AnswerOutcome outcome)
        {
            StateChanged?.Invoke();
            Answered?.Invoke(outcome);
        }
    }
}
