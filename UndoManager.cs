using UnityEngine;
using System.Collections.Generic;

namespace HexcellsHelper
{

    public class UndoManager : MonoBehaviour
    {
        readonly Stack<Coordinate> undoStack = new();

        void Update()
        {
            if (EventManager.IsLevelLoaded && Input.GetKeyDown(KeyCode.Z))
            {
                UndoLastAction();
            }
        }

        void OnEnable()
        {
            EventManager.LevelUnloaded += OnLevelUnloaded;
            EventManager.DestroyClicked += OnHexRevealed;
            EventManager.HighlightClicked += OnHexRevealed;
        }

        void OnDisable()
        {
            EventManager.LevelUnloaded -= OnLevelUnloaded;
            EventManager.DestroyClicked -= OnHexRevealed;
            EventManager.HighlightClicked -= OnHexRevealed;
        }

        void OnLevelUnloaded()
        {
            undoStack.Clear();
        }

        void OnHexRevealed(HexBehaviour hexBehaviour)
        {
            undoStack.Push(CoordUtil.WorldToGrid(hexBehaviour.transform.position));
        }

        public void UndoLastAction()
        {
            if (undoStack.Count > 0)
            {
                MapManager.SetHidden(undoStack.Pop());
            }
        }
    }
}
