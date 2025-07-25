using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HexcellsHelper
{
    public class DisplayModeManager : MonoBehaviour
    {
        public static DisplayModeManager Instance { get; private set; }

        public bool CountRemainingOnly
        {
            get => ModOptionsManager.Options.countRemainingOnly;
            private set
            {
                ModOptionsManager.Options.countRemainingOnly = value;
                ModOptionsManager.SaveOptions();
            }
        }

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
            if (!EventManager.IsLevelLoaded)
            {
                return;
            }

            // Toggle display mode when T is pressed
            if (Input.GetKeyDown(KeyCode.T))
            {
                ToggleDisplayMode();
            }

            // Toggle display mode when Tab is pressed or released
            if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyUp(KeyCode.Tab))
            {
                ToggleDisplayMode();
            }
        }

        void OnEnable()
        {
            EventManager.LevelLoaded += OnLevelLoaded;
            EventManager.HighlightClicked += OnHighlightClicked;
        }

        void OnDisable()
        {
            EventManager.LevelLoaded -= OnLevelLoaded;
            EventManager.HighlightClicked -= OnHighlightClicked;
        }

        void ToggleDisplayMode()
        {
            CountRemainingOnly = !CountRemainingOnly;
            UpdateHexNumbers();

            if (CountRemainingOnly)
            {
                GameObjectUtil.GetMusicDirector().PlayNoteA(0.0f);
            }
            else
            {
                GameObjectUtil.GetMusicDirector().PlayNoteB(0.0f);
            }
        }

        void OnLevelLoaded()
        {
            UpdateHexNumbers();
        }

        void OnHighlightClicked(HexBehaviour hexBehaviour)
        {
            // Update hex numbers when a hex is highlighted
            UpdateHexNumbers();
        }

        public void UpdateHexNumbers()
        {
            var countRemainingOnly = CountRemainingOnly;

            foreach (var clue in MapManager.Clues)
            {
                if (clue.sourceGO == null)
                {
                    continue;
                }

                var number = clue.coords.Count(coord =>
                    MapManager.CellTypeAt(coord) == CellType.Blue &&
                    (!countRemainingOnly || MapManager.IsHidden(coord))
                );
                var text = clue.modifier switch
                {
                    Clue.Modifier.Consecutive => "{" + number + "}",
                    Clue.Modifier.NonConsecutive => "-" + number + "-",
                    _ => number.ToString(),
                };

                if (clue.kind == Clue.Kind.Surround || clue.kind == Clue.Kind.Flower)
                {
                    clue.sourceGO.transform.Find("Hex Number").GetComponent<TextMesh>().text = text;
                }
                else if (clue.kind == Clue.Kind.Column)
                {
                    clue.sourceGO.GetComponent<TextMesh>().text = text;
                }
            }
        }
    }
}
