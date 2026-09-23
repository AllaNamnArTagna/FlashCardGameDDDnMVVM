using Flashcards.Domain;
using Flashcards.Infrastructure;
using Flashcards.ViewModel;
using UnityEngine;

namespace Flashcards
{
    public static class GameLauncher
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Launch()
        {
            if (Object.FindFirstObjectByType<GameBootstrapper>() != null)
                return;

            var go = new GameObject("GameBootstrapper (auto)");
            go.AddComponent<GameBootstrapper>();
        }
    }
}
