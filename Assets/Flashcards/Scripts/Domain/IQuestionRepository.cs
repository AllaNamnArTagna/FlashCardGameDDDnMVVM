using System.Collections.Generic;

namespace Flashcards.Domain
{
    public interface IQuestionRepository
    {
        IReadOnlyList<Question> LoadAll();
    }
}
