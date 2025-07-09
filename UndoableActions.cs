using UnityEngine;

namespace HexcellsHelper
{
    public class ClickUndoAction(HexBehaviour hex) : IUndoableAction
    {
        private Vector3 position = hex.transform.position;
        private bool containsShapeBlock = hex.containsShapeBlock;

        public void Undo()
        {
            // Restore the foreground hex at the saved position
            var editorFunctions = GameObject.Find("Editor Functions").GetComponent<EditorFunctions>();
            var prefab = editorFunctions.orangeHex;
            var parent = GameObject.Find("Hex Grid Overlay").transform;
            var orangeHex = UnityEngine.Object.Instantiate(prefab, position, prefab.transform.rotation, parent);
            orangeHex.GetComponent<HexBehaviour>().containsShapeBlock = containsShapeBlock;

            // Undo score.TileRemoved()
            var score = GameObject.Find("Score Text").GetComponent<HexScoring>();
            score.tilesRemoved--;

            if (containsShapeBlock)
            {
                // Undo score.CorrectTileFound()
                score.numberOfCorrectTilesFound--;
                score.GetComponent<TextMesh>().text = (score.numberOfBlueTiles - score.numberOfCorrectTilesFound).ToString();

                // Update the hex number text
                var displayModeManager = BepInEx.Bootstrap.Chainloader.ManagerObject.GetComponent<DisplayModeManager>();
                displayModeManager.UpdateHexNumbers();
            }

            // UX feedback
            GameObject.Find("Music Director(Clone)").GetComponent<MusicDirector>().PlayWrongNote(position.x / 7.04f);
            iTween.ShakePosition(orangeHex, new Vector3(0.1f, 0.1f, 0f), 0.3f);
        }
    }
}
