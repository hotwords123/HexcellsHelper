using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HexcellsHelper
{
    public class TrivialSolver : MonoBehaviour
    {
        List<Clue> clues;

        void OnEnable()
        {
            EventManager.LevelLoaded += OnLevelLoaded;
            EventManager.LevelUnloaded += OnLevelUnloaded;
        }

        void OnDisable()
        {
            EventManager.LevelLoaded -= OnLevelLoaded;
            EventManager.LevelUnloaded -= OnLevelUnloaded;
        }

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

        void OnLevelLoaded()
        {
            clues = [];
            clues.AddRange(CoordUtil.AllCoords()
                .Select(coord => Clue.FromHex(MapManager.GridAt(coord)))
                .Where(clue => clue != null));
            clues.AddRange(MapManager.Columns
                .Select(Clue.FromColumn)
                .Where(clue => clue != null));
            clues.Add(Clue.FromWholeLevel());
        }

        void OnLevelUnloaded()
        {
            clues = null;
        }

        bool SolveTrivial()
        {
            var visibleClues = clues.Where(c => c.IsSourceVisible());
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
