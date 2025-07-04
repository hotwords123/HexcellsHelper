using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace HexcellsHelper
{
    public interface IUndoableAction
    {
        void Undo();
    }

    public class UndoManager : MonoBehaviour
    {
        private Stack<IUndoableAction> undoStack = new();

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                UndoLastAction();
            }
        }

        void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            ClearActions();
        }

        public void ClearActions()
        {
            undoStack.Clear();
        }

        public void AddAction(IUndoableAction action)
        {
            undoStack.Push(action);
        }

        public void UndoLastAction()
        {
            if (undoStack.Count > 0)
            {
                var action = undoStack.Pop();
                action.Undo();
            }
        }
    }
}
