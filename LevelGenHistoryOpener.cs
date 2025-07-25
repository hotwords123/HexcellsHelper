using TMPro;
using UnityEngine;

namespace HexcellsHelper
{
    public class LevelGenHistoryOpener : MonoBehaviour
    {
        float textAlphaTarget = 1.0f;
        float textAlpha = 1.0f;

        TextMeshPro textMesh;
        MusicDirector musicDirector;

        void Awake()
        {
            textMesh = GetComponent<TextMeshPro>();
            musicDirector = GameObjectUtil.GetMusicDirector();
        }

        void Update()
        {
            if (textAlpha != textAlphaTarget)
            {
                textAlpha = Mathf.MoveTowards(textAlpha, textAlphaTarget, Time.deltaTime * 2.0f);
                textMesh.alpha = textAlpha;
            }
        }

        void OnMouseEnter()
        {
            textAlphaTarget = 0.4f;
            musicDirector.PlayMouseOverSound();
        }

        void OnMouseExit()
        {
            textAlphaTarget = 1.0f;
        }

        void OnMouseOver()
        {
            if (Input.GetMouseButtonDown(0))
            {
                LevelGenHistoryScreen.Instance.EnterScreen();
                musicDirector.PlayNoteA(-0.5f);
            }
        }
    }
}
