using System;
using System.Collections.Generic;
using Flashcards.Domain;
using UnityEngine;

namespace Flashcards.Infrastructure
{
    public sealed class ResourceQuestionRepository : IQuestionRepository
    {
        private readonly string _resourcePath;

        public ResourceQuestionRepository(string resourcePath = "Flashcards/questions")
        {
            _resourcePath = resourcePath;
        }

        public IReadOnlyList<Question> LoadAll()
        {
            TextAsset asset = Resources.Load<TextAsset>(_resourcePath);
            if (asset == null)
                throw new InvalidOperationException($"No questions found at 'Resources/{_resourcePath}'. Expected a TextAsset named questions.json.");

            QuestionCollectionDto collection = JsonUtility.FromJson<QuestionCollectionDto>(asset.text);
            if (collection.questions == null || collection.questions.Count == 0)
                throw new InvalidOperationException("The question file contains no questions.");

            var questions = new List<Question>(collection.questions.Count);
            foreach (QuestionDto dto in collection.questions)
            {
                questions.Add(Question.Create(dto.id, dto.topic, dto.prompt, dto.choices, dto.correctIndex, dto.explanation));
            }

            Shuffle(questions);
            return questions;
        }

        private static void Shuffle(List<Question> questions)
        {
            var rng = new System.Random();
            for (int i = questions.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                (questions[i], questions[j]) = (questions[j], questions[i]);
            }
        }
    }
}
