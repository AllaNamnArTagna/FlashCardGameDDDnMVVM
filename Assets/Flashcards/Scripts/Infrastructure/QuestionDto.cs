using System;
using System.Collections.Generic;

namespace Flashcards.Infrastructure
{
    [Serializable]
    public class QuestionDto
    {
        public string id;
        public string topic;
        public string prompt;
        public List<string> choices = new List<string>();
        public int correctIndex;
        public string explanation;
    }

    [Serializable]
    public class QuestionCollectionDto
    {
        public List<QuestionDto> questions = new List<QuestionDto>();
    }
}
