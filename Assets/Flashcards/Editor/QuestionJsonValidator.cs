using System;
using System.IO;
using System.Linq;
using Flashcards.Domain;
using Flashcards.Infrastructure;
using UnityEditor;
using UnityEngine;

namespace Flashcards.Editor
{
    public static class QuestionJsonValidator
    {
        private const string QuestionsPath = "Assets/Flashcards/Resources/Flashcards/questions.json";

        [MenuItem("Tools/Flashcards/Validate Questions")]
        public static void Validate()
        {
            try
            {
                if (!File.Exists(QuestionsPath))
                {
                    Debug.LogError($"[Flashcards] Could not find {QuestionsPath}. The game will not run.");
                    return;
                }

                QuestionCollectionDto collection = JsonUtility.FromJson<QuestionCollectionDto>(File.ReadAllText(QuestionsPath));
                var ids = new System.Collections.Generic.HashSet<string>();
                int count = 0;

                foreach (QuestionDto dto in collection.questions)
                {
                    Question.Create(dto.id, dto.topic, dto.prompt, dto.choices, dto.correctIndex, dto.explanation);
                    if (!ids.Add(dto.id))
                        throw new ArgumentException($"Duplicate question id '{dto.id}'.");
                    count++;
                }

                Debug.Log($"[Flashcards] OK: {count} questions validated.");
            }
            catch (Exception e)
            {
                Debug.LogError($"[Flashcards] Question file is invalid: {e.Message}");
            }
        }
    }
}
