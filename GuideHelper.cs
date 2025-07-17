using UnityEngine;

namespace HexcellsHelper
{
    public class GuideHelper : MonoBehaviour
    {
        void Update()
        {
            if (!EventManager.IsLevelLoaded || MapManager.IsCompleted)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                {
                    // Toggle flower guide when Shift + G is pressed
                    ToggleAllFlowerGuide();
                }
                else
                {
                    // Toggle column guide when G is pressed
                    ToggleAllColumnGuide();
                }
            }
        }

        void ToggleAllFlowerGuide()
        {
            foreach (var hexGO in MapManager.Hexes)
            {
                if (hexGO.name == "Blue Hex (Flower)" &&
                    !MapManager.IsHidden(CoordUtil.WorldToGrid(hexGO.transform.position)))
                {
                    var flower = hexGO.GetComponent<BlueHexFlower>();
                    if (flower != null && !flower.playerHasMarkedComplete)
                    {
                        FlowerGuideUtil.ToggleHexGuide(flower);
                    }
                }
            }
        }

        void ToggleAllColumnGuide()
        {
            foreach (var columnGO in MapManager.Columns)
            {
                var columnNumber = columnGO.GetComponent<ColumnNumber>();
                if (columnNumber != null && !columnNumber.playerHasMarkedComplete)
                {
                    var renderer = ColumnGuideUtil.GetRenderer(columnNumber);
                    if (renderer != null)
                    {
                        renderer.enabled = !renderer.enabled;
                    }
                }
            }
        }
    }
}
