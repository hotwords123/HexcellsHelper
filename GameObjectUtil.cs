using System.Collections.Generic;
using UnityEngine;

namespace HexcellsHelper
{
    public static class GameObjectUtil
    {
        public static MusicDirector GetMusicDirector()
        {
            return GameObject.Find("Music Director(Clone)")?.GetComponent<MusicDirector>();
        }

        public static GameManagerScript GetGameManager()
        {
            return GameObject.Find("Game Manager(Clone)")?.GetComponent<GameManagerScript>();
        }

        public static OptionsManager GetOptionsManager()
        {
            return GameObject.Find("Game Manager(Clone)").GetComponent<OptionsManager>();
        }

        public static GameObject GetHexGrid()
        {
            return GameObject.Find("Hex Grid");
        }

        public static GameObject GetHexGridOverlay()
        {
            return GameObject.Find("Hex Grid Overlay");
        }

        public static IEnumerable<GameObject> GetChildren(GameObject parent)
        {
            if (parent == null)
            {
                yield break;
            }

            foreach (Transform child in parent.transform)
            {
                yield return child.gameObject;
            }
        }
    }
}
