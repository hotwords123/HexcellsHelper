using UnityEngine;
using System.Collections.Generic;
using System;

namespace HexcellsHelper
{
    public interface IUndoableAction
    {
        void Undo();
    }

    public class UndoManager : MonoBehaviour
    {
        public static UndoManager Instance { get; private set; }

        readonly Stack<IUndoableAction> undoStack = new();
        UndoActionGroup currentGroup;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(this);
            }
        }

        void Update()
        {
            if (EventManager.IsLevelLoaded && !MapManager.IsCompleted && Input.GetKeyDown(KeyCode.Z))
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
            AddAction(new ClickUndoAction(hexBehaviour.gameObject));
        }

        public void BeginGroup()
        {
            if (currentGroup != null)
            {
                Debug.LogWarning("[UndoManager] Already in a group, cannot start a new one.");
                return;
            }
            currentGroup = new UndoActionGroup();
        }

        public void EndGroup()
        {
            if (currentGroup == null)
            {
                Debug.LogWarning("[UndoManager] No active group to end.");
                return;
            }
            if (!currentGroup.IsEmpty)
            {
                undoStack.Push(currentGroup);
            }
            currentGroup = null;
        }

        private class UndoGroupContext : IDisposable
        {
            readonly UndoManager manager;

            public UndoGroupContext(UndoManager manager)
            {
                this.manager = manager;
                manager.BeginGroup();
            }

            public void Dispose()
            {
                manager.EndGroup();
            }
        }

        public IDisposable CreateGroup()
        {
            return new UndoGroupContext(this);
        }

        public void AddAction(IUndoableAction action)
        {
            if (currentGroup != null)
            {
                currentGroup.AddAction(action);
            }
            else
            {
                undoStack.Push(action);
            }
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
