using System;
using System.Collections.Generic;

namespace Flashcards.Domain
{
    public sealed class Question
    {
        public string Id { get; }
        public string Topic { get; }
        public string Prompt { get; }
        public IReadOnlyList<string> Choices { get; }
        public int CorrectIndex { get; }
        public string Explanation { get; }

        private Question(string id, string topic, string prompt, IReadOnlyList<string> choices, int correctIndex, string explanation)
        {
            Id = id;
            Topic = topic;
            Prompt = prompt;
            Choices = choices;
            CorrectIndex = correctIndex;
            Explanation = explanation;
        }

        public static Question Create(string id, string topic, string prompt, IReadOnlyList<string> choices, int correctIndex, string explanation)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Question id must not be empty.", nameof(id));
            if (string.IsNullOrWhiteSpace(topic))
                throw new ArgumentException($"Question '{id}': topic must not be empty.", nameof(topic));
            if (string.IsNullOrWhiteSpace(prompt))
                throw new ArgumentException($"Question '{id}': prompt must not be empty.", nameof(prompt));
            if (choices == null)
                throw new ArgumentException($"Question '{id}': choices must not be null.", nameof(choices));
            if (choices.Count != 4)
                throw new ArgumentException($"Question '{id}': must have exactly 4 choices, found {choices.Count}.", nameof(choices));
            for (int i = 0; i < choices.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(choices[i]))
                    throw new ArgumentException($"Question '{id}': choice {i + 1} must not be empty.", nameof(choices));
            }
            if (correctIndex < 0 || correctIndex > 3)
                throw new ArgumentException($"Question '{id}': correctIndex must be 0-3, found {correctIndex}.", nameof(correctIndex));
            if (string.IsNullOrWhiteSpace(explanation))
                throw new ArgumentException($"Question '{id}': explanation must not be empty.", nameof(explanation));

            return new Question(id, topic, prompt, choices, correctIndex, explanation);
        }

        public bool IsCorrect(int choiceIndex) => choiceIndex == CorrectIndex;
    }
}
