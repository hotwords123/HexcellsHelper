using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace HexcellsHelper
{
    public class ClickUndoAction(Coordinate coord) : IUndoableAction
    {
        public void Undo()
        {
            MapManager.SetHidden(coord);
        }
    }

    public class MarkFlowerAsCompleteUndoAction(BlueHexFlower flower) : IUndoableAction
    {
        private static readonly FieldInfo guideIsOffField = AccessTools.Field(typeof(BlueHexFlower), "guideIsOff");
        private static readonly MethodInfo toggleHexGuideMethod = AccessTools.Method(typeof(BlueHexFlower), "ToggleHexGuide");

        private readonly BlueHexFlower flower = flower;
        private readonly bool previousGuideState = (bool)guideIsOffField.GetValue(flower);

        public void Undo()
        {
            if (flower.playerHasMarkedComplete)
            {
                flower.ToggleMarkComplete();
                if (previousGuideState && !(bool)guideIsOffField.GetValue(flower))
                {
                    toggleHexGuideMethod.Invoke(flower, null);
                }
            }
        }
    }

    public class MarkColumnAsCompleteUndoAction(ColumnNumber column) : IUndoableAction
    {
        private static readonly FieldInfo thisRendererField = AccessTools.Field(typeof(ColumnNumber), "thisRenderer");

        private readonly ColumnNumber column = column;
        private readonly bool previousGuideState = ((Renderer)thisRendererField.GetValue(column)).enabled;

        public void Undo()
        {
            if (column.playerHasMarkedComplete)
            {
                column.ToggleMarkComplete();
                var renderer = (Renderer)thisRendererField.GetValue(column);
                if (previousGuideState && !renderer.enabled)
                {
                    renderer.enabled = true;
                }
            }
        }
    }

    public class UndoActionGroup : IUndoableAction
    {
        private readonly List<IUndoableAction> actions = [];

        public bool IsEmpty => actions.Count == 0;

        public void Undo()
        {
            foreach (var action in actions)
            {
                action.Undo();
            }
        }

        public void AddAction(IUndoableAction action)
        {
            actions.Add(action);
        }
    }

    public class HexHypothesisUndoAction(HexHypothesis hexHypothesis, HypothesisState previousState) : IUndoableAction
    {
        private readonly HexHypothesis hexHypothesis = hexHypothesis;
        private readonly HypothesisState previousState = previousState;

        public void Undo()
        {
            if (!HypothesisManager.Instance.IsHypothesisModeActive)
            {
                HypothesisManager.Instance.ToggleHypothesisMode();
            }

            hexHypothesis.State = previousState;

            GameObjectUtil.GetMusicDirector().PlayMouseOverSound();
            iTween.ShakePosition(hexHypothesis.gameObject, new Vector3(0.1f, 0.1f, 0f), 0.3f);
        }
    }
}
