using UnityEngine;
using System.Collections.Generic;

namespace HexcellsHelper
{

    public class UndoManager : MonoBehaviour
    {
        readonly Stack<Coordinate> undoStack = new();

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                UndoLastAction();
            }
        }

        void OnEnable()
        {
            EventManager.LevelLoaded += undoStack.Clear;
            EventManager.DestroyClicked += AddAction;
            EventManager.HighlightClicked += AddAction;
        }

        void OnDisable()
        {
            EventManager.LevelLoaded -= undoStack.Clear;
            EventManager.DestroyClicked -= AddAction;
            EventManager.HighlightClicked -= AddAction;
        }

        void AddAction(HexBehaviour hexBehaviour)
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
