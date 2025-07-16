using System.Linq;
using UnityEngine;

namespace HexcellsHelper
{
    public class HypothesisManager : MonoBehaviour
    {
        public static HypothesisManager Instance { get; private set; }

        public bool IsHypothesisModeActive { get; private set; } = false;

        void Awake()
        {
            if (!LoadMaterials())
            {
                Destroy(this);
                return;
            }

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

        bool LoadMaterials()
        {
            var materials = Resources.FindObjectsOfTypeAll<Material>();
            var baseBorder = materials.FirstOrDefault(m => m.name == "Hex White Border");
            if (baseBorder == null)
            {
                Debug.LogError("Failed to load base border material.");
                return false;
            }

            Material CreateMaterial(string materialName, string texturePath)
            {
                var texture = AssetManager.LoadTexture(texturePath);
                if (texture == null)
                {
                    return null;
                }

                var material = Object.Instantiate(baseBorder);
                material.name = materialName;
                material.mainTexture = texture;
                return material;
            }

            var grayBorder = CreateMaterial("Hex Gray Border", "Texture2D/Hex Gray Border.png");
            var blackBorder = CreateMaterial("Hex Black Border", "Texture2D/Hex Black Border.png");
            var blueBorder = CreateMaterial("Hex Blue Border", "Texture2D/Hex Blue Border.png");
            if (grayBorder == null || blackBorder == null || blueBorder == null)
            {
                Debug.LogError("Failed to load hypothesis border materials.");
                return false;
            }

            HexHypothesis.baseBorder = baseBorder;
            HexHypothesis.grayBorder = grayBorder;
            HexHypothesis.blackBorder = blackBorder;
            HexHypothesis.blueBorder = blueBorder;

            return true;
        }

        void Update()
        {
            if (EventManager.IsLevelLoaded && !MapManager.IsCompleted)
            {
                if (Input.GetKeyDown(KeyCode.H))
                {
                    ToggleHypothesisMode();
                }

                if (Input.GetKeyDown(KeyCode.C))
                {
                    ClearHypotheses();
                }
            }
        }

        void OnEnable()
        {
            EventManager.LevelLoaded += OnLevelLoaded;
        }

        void OnDisable()
        {
            EventManager.LevelLoaded -= OnLevelLoaded;
        }

        void OnLevelLoaded()
        {
            IsHypothesisModeActive = false;

            foreach (var orangeHex in MapManager.OverlayHexes)
            {
                SetupHexHypothesis(orangeHex);
            }
        }

        public void SetupHexHypothesis(GameObject hex)
        {
            if (hex.GetComponent<HexHypothesis>() != null)
            {
                return;
            }

            var hexHypothesis = hex.AddComponent<HexHypothesis>();
            hexHypothesis.enabled = IsHypothesisModeActive;
        }

        public void ToggleHypothesisMode()
        {
            IsHypothesisModeActive = !IsHypothesisModeActive;

            var hexGridOverlay = MapManager.HexGridOverlay;
            if (hexGridOverlay == null)
            {
                return;
            }

            var hexHypotheses = hexGridOverlay.GetComponentsInChildren<HexHypothesis>();
            foreach (var hexHypothesis in hexHypotheses)
            {
                hexHypothesis.enabled = IsHypothesisModeActive;
            }
        }

        public void ClearHypotheses()
        {
            var hexGridOverlay = MapManager.HexGridOverlay;
            if (hexGridOverlay == null)
            {
                return;
            }

            var hexHypotheses = hexGridOverlay.GetComponentsInChildren<HexHypothesis>();
            foreach (var hexHypothesis in hexHypotheses)
            {
                hexHypothesis.State = HypothesisState.None;
            }
        }
    }
}
