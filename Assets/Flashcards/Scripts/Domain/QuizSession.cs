using System;
using System.Collections.Generic;

namespace Flashcards.Domain
{
    public sealed class QuizSession
    {
        private readonly IReadOnlyList<Question> _questions;
        private readonly IScorePolicy _scorePolicy;
        private int _index;

        public event Action<AnswerOutcome> Answered;

        public QuizSession(IReadOnlyList<Question> questions, IScorePolicy scorePolicy)
        {
            if (questions == null || questions.Count == 0)
                throw new ArgumentException("A quiz session needs at least one question.", nameof(questions));
            _scorePolicy = scorePolicy ?? throw new ArgumentNullException(nameof(scorePolicy));
            _questions = questions;
        }

        public Question CurrentQuestion => IsFinished ? null : _questions[_index];
        public int QuestionCount => _questions.Count;
        public int QuestionsAnswered => _index;
        public bool IsFinished => _index >= _questions.Count;

        public int Score { get; private set; }
        public int Streak { get; private set; }
        public int BestStreak { get; private set; }
        public int CorrectCount { get; private set; }

        public AnswerOutcome Answer(int choiceIndex)
        {
            if (IsFinished)
                throw new InvalidOperationException("The quiz is already finished.");
            if (choiceIndex < 0 || choiceIndex > 3)
                throw new ArgumentOutOfRangeException(nameof(choiceIndex), "Choice index must be 0-3.");

            Question question = _questions[_index];
            bool wasCorrect = question.IsCorrect(choiceIndex);

            if (wasCorrect)
            {
                CorrectCount++;
                Streak++;
                BestStreak = Math.Max(BestStreak, Streak);
            }
            else
            {
                Streak = 0;
            }

            int points = _scorePolicy.AwardFor(wasCorrect, Streak);
            Score += points;

            var outcome = new AnswerOutcome(
                wasCorrect,
                choiceIndex,
                question.CorrectIndex,
                points,
                Streak,
                question.Explanation);

            _index++;
            Answered?.Invoke(outcome);
            return outcome;
        }
    }
}
