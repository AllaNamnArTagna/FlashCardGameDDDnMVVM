namespace Flashcards.Domain
{
    public readonly struct AnswerOutcome
    {
        public bool WasCorrect { get; }
        public int ChosenIndex { get; }
        public int CorrectIndex { get; }
        public int PointsAwarded { get; }
        public int StreakAfter { get; }
        public string Explanation { get; }

        public AnswerOutcome(bool wasCorrect, int chosenIndex, int correctIndex, int pointsAwarded, int streakAfter, string explanation)
        {
            WasCorrect = wasCorrect;
            ChosenIndex = chosenIndex;
            CorrectIndex = correctIndex;
            PointsAwarded = pointsAwarded;
            StreakAfter = streakAfter;
            Explanation = explanation;
        }
    }
}
