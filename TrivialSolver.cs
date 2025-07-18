using System.Linq;
using UnityEngine;

namespace HexcellsHelper
{
    public class TrivialSolver : MonoBehaviour
    {
        void Update()
        {
            if (EventManager.IsLevelLoaded && !MapManager.IsCompleted && Input.GetKeyDown(KeyCode.Space))
            {
                using (UndoManager.Instance.CreateGroup())
                {
                    if (!SolveTrivial())
                    {
                        GameObjectUtil.GetMusicDirector().PlayWrongNote(0.0f);
                    }
                }
            }
        }

        bool SolveTrivial()
        {
            var visibleClues = MapManager.Clues.Where(c => c.IsSourceVisible());
            var success = false;

            foreach (var clue in visibleClues)
            {
                clue.CountHidden();
                if (clue.TrySolve())
                {
                    success = true;
                    if (clue.sourceGO != null)
                    {
                        iTween.ShakePosition(clue.sourceGO, new Vector3(0.1f, 0.1f, 0f), 0.3f);
                    }
                    break;
                }
            }

            foreach (var clue in visibleClues)
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
