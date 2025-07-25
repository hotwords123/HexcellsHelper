using TMPro;
using UnityEngine;

namespace HexcellsHelper
{
    public class LevelGenHistoryBackButton : MonoBehaviour
    {
        float textColorTarget = 0.706f;
        float textColor = 0.706f;

        TextMeshPro textMesh;
        MusicDirector musicDirector;

        void Awake()
        {
            textMesh = GetComponent<TextMeshPro>();
            musicDirector = GameObjectUtil.GetMusicDirector();
        }

        void Update()
        {
            if (textColor != textColorTarget)
            {
                textColor = Mathf.MoveTowards(textColor, textColorTarget, Time.deltaTime * 1.5f);
                textMesh.color = new Color(textColor, textColor, textColor, 1f);
            }
        }

        void OnMouseEnter()
        {
            textColorTarget = 0.35f;
            musicDirector.PlayMouseOverSound();
        }

        void OnMouseExit()
        {
            textColorTarget = 0.706f;
        }

        void OnMouseOver()
        {
            if (Input.GetMouseButtonDown(0))
            {
                LevelGenHistoryScreen.Instance.ExitScreen();
                musicDirector.PlayNoteB(0.5f);
            }
        }
    }
}
