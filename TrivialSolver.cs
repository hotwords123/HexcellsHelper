using System.Linq;
using UnityEngine;

namespace HexcellsHelper
{
    public class TrivialSolver : MonoBehaviour
    {
        enum SolveMode
        {
            // Solves based on a single clue at a time
            Single,
            // Solves based on all currently visible clues
            All,
            // Solves recursively until no further progress can be made
            Recursive,
        }

        void Update()
        {
            if (EventManager.IsLevelLoaded && !MapManager.IsCompleted && Input.GetKeyDown(KeyCode.Space))
            {
                var mode = SolveMode.Single;
                if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                {
                    mode = SolveMode.Recursive;
                }
                else if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                {
                    mode = SolveMode.All;
                }

                using (UndoManager.Instance.CreateGroup())
                {
                    if (!SolveTrivial(mode))
                    {
                        GameObjectUtil.GetMusicDirector().PlayWrongNote(0.0f);
                    }
                }
            }
        }

        bool SolveTrivial(SolveMode mode)
        {
            static Clue[] GetVisibleClues()
            {
                return MapManager.Clues.Where(c => c.IsSourceVisible()).ToArray();
            }

            static bool TrySolveClue(Clue clue)
            {
                clue.CountHidden();
                if (clue.TrySolve())
                {
                    if (clue.sourceGO != null)
                    {
                        iTween.ShakePosition(clue.sourceGO, new Vector3(0.1f, 0.1f, 0f), 0.3f);
                    }
                    return true;
                }
                return false;
            }

            bool success = false;
            switch (mode)
            {
                case SolveMode.Single:
                    success = GetVisibleClues().Any(TrySolveClue);
                    break;

                case SolveMode.All:
                    var visibleClues = GetVisibleClues();
                    while (visibleClues.Count(TrySolveClue) > 0)
                    {
                        success = true;
                    }
                    break;

                case SolveMode.Recursive:
                    while (GetVisibleClues().Count(TrySolveClue) > 0)
                    {
                        success = true;
                    }
                    break;
            }

            foreach (var clue in GetVisibleClues())
            {
                clue.CountHidden();
                if (clue.IsComplete)
                {
                    clue.MarkSourceAsComplete();
                }
            }

            return success;
        }
    }
}
