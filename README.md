# DDD & MVVM Flashcards 🐶

A tiny Unity quiz game for learning **Domain-Driven Design** and **MVVM** — and the twist is that **the game's own code is a working example of both patterns**. Answer questions, earn points, build streaks, and watch the dog react.

Built for **Unity 6 (any 6.x LTS)** with the **Universal 2D** template.

---

## 1. Get it running (5 minutes)

You should already have:
- Unity Hub + Unity 6 LTS installed
- A Unity project created from the **Universal 2D** template

Then:

1. Clone the repo (or download as ZIP) and open the folder as a project in Unity Hub.
2. If the project was freshly cloned (no `Packages/` yet): in Unity, **Window → Package Manager → Unity Registry** → install **Unity UI (ugui)**. Unity recompiles.
3. Press **Play**. 🎉

There is no scene setup at all: `GameLauncher` auto-creates the `GameBootstrapper` in whatever scene is loaded, and the bootstrapper sets up the camera, EventSystem, canvas and dog from code. (You can still add a `GameBootstrapper` component manually to a GameObject if you want the dog placement fields tweakable in the Inspector — the auto-launcher then stays out of the way.)

### If the Console shows an error
Select the error text, copy it, and paste it into our chat. Pressing **Ctrl+C** in Unity's Console window copies the selected error.

---

## 2. How to play

- A question card flips in with a topic tag, the prompt, and four choices.
- Click an answer:
  - **Correct:** the button flashes green, points fly in, the dog does a happy bounce, and a short explanation appears.
  - **Wrong:** the button flashes red, the correct one glows green, the dog droops, and the explanation tells you why.
- Points: **+10** base, **+5 per streak step**, up to **+25** streak bonus.
- After all questions: results screen with your score, correct answers, and best streak. The dog celebrates. `Play again` reshuffles everything.

---

## 3. The architecture — what to study

```
Assets/Flashcards/Scripts/
├── Domain/                    ← the "business" of being a quiz. Zero Unity code.
│   ├── Question.cs             ← Entity (has an Id, validated at creation)
│   ├── AnswerOutcome.cs        ← Value Object (immutable, no identity)
│   ├── QuizSession.cs          ← Aggregate Root (score/streak change only via Answer)
│   ├── IScorePolicy.cs         ← abstraction for scoring rules
│   └── DefaultScorePolicy.cs   ← the rules: 10 + streak bonus
├── Infrastructure/             ← where data actually comes from
│   ├── QuestionDto.cs          ← serializable JSON shape
│   └── ResourceQuestionRepository.cs  ← implements the domain interface
├── ViewModel/                  ← presentation logic. Zero Unity code.
│   └── QuizViewModel.cs        ← state + commands + events the View binds to
├── View/                       ← the ONLY layer that knows Unity exists
│   ├── QuizView.cs             ← builds the UI, subscribes to VM events
│   ├── StartScreenView.cs
│   ├── DogView.cs              ← sprite + reaction animations
│   ├── FlashcardButtonView.cs
│   └── UIElements.cs           ← helpers for building UI in code
├── GameBootstrapper.cs         ← Composition Root: wires everything together
└── GameLauncher.cs             ← auto-creates the bootstrapper in any scene on Play
```

**The dependency rule:** View → ViewModel → Domain, and Infrastructure plugs into the Domain *through* its interfaces (dependency inversion). The domain and viewmodel folders import no Unity namespaces — check for yourself; that's the whole point.

**Where's the binding?** Unity has no XAML-style automatic binding, so the View subscribes to ViewModel events (`StateChanged`, `QuestionPresented`, `Answered`, `QuizFinished`) and updates Texts/Images in the handlers. Same MVVM discipline, Unity dialect.

### DDD concepts mapped to this codebase

| DDD concept | Where to look |
|---|---|
| Entity | `Question` — identity via `Id` |
| Value Object | `AnswerOutcome` — immutable struct, defined by values |
| Aggregate Root | `QuizSession` — the only way to mutate score/streak is `Answer()` |
| Repository | `IQuestionRepository` (domain) ↔ `ResourceQuestionRepository` (infra) |
| Domain Service | `IScorePolicy` / `DefaultScorePolicy` |
| Ubiquitous Language | Streak, Answer, Score — the words a teacher would use |
| Composition Root | `GameBootstrapper.Awake()` |

### MVVM concepts mapped to this codebase

| MVVM concept | Where to look |
|---|---|
| Model | `QuizSession`, `Question` (the domain) |
| ViewModel | `QuizViewModel` — pure C#, raises events |
| View | `QuizView` + friends — only layer with `UnityEngine` imports |
| Binding | event subscription in `QuizView.Bind()` |
| Command | `SubmitAnswer(int)` called by button clicks |

---

## 4. Adding your own questions

Edit `Assets/Flashcards/Resources/Flashcards/questions.json`:

```json
{
  "id": "my-q-1",
  "topic": "MVVM",
  "prompt": "Your question here?",
  "choices": ["A", "B", "C", "D"],
  "correctIndex": 2,
  "explanation": "Why the right answer is right."
}
```

Rules (enforced by `Question.Create`, and checkable via **Tools → Flashcards → Validate Questions** in the editor):
- exactly 4 non-empty choices
- `correctIndex` between 0 and 3
- unique `id`s, non-empty `topic`, `prompt`, `explanation`

Note: `UnityEngine.JsonUtility` is strict — no trailing commas, and UTF-8 characters outside strings will fail.

---

## 5. VS 2026 + Unity note

Visual Studio 2026 works with Unity the same way VS 2022 does: in VS's installer, enable the **Game development with Unity** workload (it installs the Unity Hub if needed). Then in Unity: **Edit → Preferences → External Tools → External Script Editor** → pick **Visual Studio 2026**. Double-clicking a script in Unity will then open it in VS 2026, with full IntelliSense for Unity APIs.

## 6. Repository layout for Unity work

Since your local Unity project folder is itself a git repo (or will be), the cleanest setup is:

```
your-unity-project/          ← git repo; .gitignore excludes Library/ etc.
├── Assets/
│   ├── Flashcards/           ← everything from this repo
│   ├── Scenes/               ← yours, not committed here
│   └── ...
└── Packages/ ProjectSettings/ ← yours, not committed here
```

This repo ships only the `Assets/Flashcards` folder (plus docs), so dropping it into any Unity 6 2D project just works. If you'd rather make the *entire* Unity project the repo, copy these folders in and commit everything except what's in `.gitignore`.
