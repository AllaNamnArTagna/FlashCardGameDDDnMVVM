namespace Flashcards.Domain
{
    public interface IScorePolicy
    {
        int AwardFor(bool wasCorrect, int streakAfterAnswer);
    }
}
