using UnityEngine;

namespace HexcellsHelper
{
    public class LevelGenHistoryItem : MonoBehaviour
    {
        public LevelGenHistoryEntry entry;

        public bool isAlternate;

        public Material mouseOff;
        public Material mouseOffAlternate;
        public Material mouseOver;
        public Material border;
        public Material borderHighlight;

        private Renderer thisRenderer;
        private Renderer borderRenderer;
        private MusicDirector musicDirector;

        void Awake()
        {
            thisRenderer = GetComponent<Renderer>();
            borderRenderer = transform.parent.Find("Border").GetComponent<Renderer>();
            musicDirector = GameObjectUtil.GetMusicDirector();
        }

        void OnMouseEnter()
        {
            thisRenderer.material = mouseOver;
            borderRenderer.material = borderHighlight;
            musicDirector.PlayMouseOverSound();
        }

        void OnMouseExit()
        {
            thisRenderer.material = isAlternate ? mouseOffAlternate : mouseOff;
            borderRenderer.material = border;
        }

        void OnMouseOver()
        {
            // Left-click to load the level
            if (Input.GetMouseButtonDown(0))
            {
                LevelGenHistoryScreen.Instance.LoadHistoryLevel(entry);

                musicDirector.PlayNoteA(0.0f);
                iTween.PunchPosition(transform.parent.gameObject, new Vector3(0.1f, 0.0f, 0.0f), 0.5f);
            }

            // Right-click to copy the seed to clipboard
            if (Input.GetMouseButtonDown(1))
            {
                GUIUtility.systemCopyBuffer = entry.seed;

                musicDirector.PlayNoteB(0.0f);
                iTween.PunchPosition(transform.parent.gameObject, new Vector3(0.1f, 0.0f, 0.0f), 0.5f);
            }
        }

        public void SetAlternate(bool alternate)
        {
            isAlternate = alternate;
            thisRenderer.material = isAlternate ? mouseOffAlternate : mouseOff;
        }
    }
}
