using System;

namespace Flashcards.Domain
{
    public sealed class DefaultScorePolicy : IScorePolicy
    {
        public const int BasePoints = 10;
        public const int StreakBonusPerStep = 5;
        public const int MaxStreakBonus = 25;

        public int AwardFor(bool wasCorrect, int streakAfterAnswer)
        {
            if (!wasCorrect)
                return 0;
            int bonus = Math.Min((streakAfterAnswer - 1) * StreakBonusPerStep, MaxStreakBonus);
            return BasePoints + bonus;
        }
    }
}
