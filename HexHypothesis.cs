using UnityEngine;

namespace HexcellsHelper
{
    public enum HypothesisState
    {
        None,
        Black,
        Blue,
    }

    public class HexHypothesis : MonoBehaviour
    {
        HypothesisState state = HypothesisState.None;
        public HypothesisState State
        {
            get => state;
            set
            {
                if (state != value)
                {
                    state = value;
                    UpdateBorder();
                }
            }
        }

        public static Material baseBorder;
        public static Material grayBorder;
        public static Material blackBorder;
        public static Material blueBorder;

        MeshRenderer borderRenderer;

        bool mouseButtonsInverted;

        void Awake()
        {
            borderRenderer = transform.Find("Graphic/Border")?.GetComponent<MeshRenderer>();
            mouseButtonsInverted = GameObjectUtil.GetOptionsManager()?.currentOptions.mouseButtonsSwapped ?? false;
        }

        void OnEnable()
        {
            var hexBehaviour = GetComponent<HexBehaviour>();
            if (hexBehaviour != null)
            {
                hexBehaviour.enabled = false;
            }

            UpdateBorder();
        }

        void OnDisable()
        {
            var hexBehaviour = GetComponent<HexBehaviour>();
            if (hexBehaviour != null)
            {
                hexBehaviour.enabled = true;
            }

            UpdateBorder();
        }

        void OnMouseOver()
        {
            if (!enabled)
            {
                return;
            }

            var previousState = State;

            if (Input.GetMouseButtonDown(0))
            {
                if (State == HypothesisState.None)
                {
                    State = mouseButtonsInverted ? HypothesisState.Black : HypothesisState.Blue;
                }
                else
                {
                    // TODO: Toggle marker text
                }
            }
            if (Input.GetMouseButtonDown(1))
            {
                if (State == HypothesisState.None)
                {
                    State = mouseButtonsInverted ? HypothesisState.Blue : HypothesisState.Black;
                }
                else
                {
                    State = HypothesisState.None;
                }
            }

            if (previousState != State)
            {
                UndoManager.Instance.AddAction(new HexHypothesisUndoAction(this, previousState));
                GameObjectUtil.GetMusicDirector().PlayMouseOverSound();
            }
        }

        public void UpdateBorder()
        {
            if (!enabled)
            {
                borderRenderer.material = baseBorder;
                return;
            }

            borderRenderer.material = State switch
            {
                HypothesisState.Black => blackBorder,
                HypothesisState.Blue => blueBorder,
                _ => grayBorder
            };
        }
    }
}
