using System.Collections.Generic;
using UnityEngine;

namespace HexcellsHelper
{
    public class ClickUndoAction(GameObject overlayHex) : IUndoableAction
    {
        private readonly Coordinate coord = Coordinate.FromGameObject(overlayHex);
        private readonly HypothesisState previousState =
            overlayHex.GetComponent<HexHypothesis>()?.State ?? HypothesisState.None;

        public void Undo()
        {
            MapManager.SetHidden(coord);

            var hexHypothesis = MapManager.GridOverlayAt(coord)?.GetComponent<HexHypothesis>();
            if (hexHypothesis != null)
            {
                hexHypothesis.State = previousState;
            }
        }
    }

    public class MarkFlowerAsCompleteUndoAction(BlueHexFlower flower) : IUndoableAction
    {
        private readonly bool previousGuideState = FlowerGuideUtil.GetGuideIsOff(flower);

        public void Undo()
        {
            if (flower.playerHasMarkedComplete)
            {
                flower.ToggleMarkComplete();
                if (previousGuideState && !FlowerGuideUtil.GetGuideIsOff(flower))
                {
                    FlowerGuideUtil.ToggleHexGuide(flower);
                }
            }
        }
    }

    public class MarkColumnAsCompleteUndoAction(ColumnNumber column) : IUndoableAction
    {
        private readonly bool previousGuideState = ColumnGuideUtil.GetRenderer(column).enabled;

        public void Undo()
        {
            if (column.playerHasMarkedComplete)
            {
                column.ToggleMarkComplete();
                var renderer = ColumnGuideUtil.GetRenderer(column);
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
        private readonly Coordinate coord = Coordinate.FromGameObject(hexHypothesis.gameObject);

        public void Undo()
        {
            var hexHypothesis = MapManager.GridOverlayAt(coord)?.GetComponent<HexHypothesis>();
            if (hexHypothesis == null)
            {
                Debug.LogWarning($"HexHypothesisUndoAction: HexHypothesis at {coord} not found.");
                return;
            }

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
