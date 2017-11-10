using UnityEngine;
using UnityEngine.Tilemaps;

namespace Minesweeper
{
    // Tile for unrevealed square
    public class UnrevealedSquare : TileBase
    {
        // Sprite for unexplored tiles
        public Sprite Unexplored;

        // Sprite for flagged tiels
        public Sprite Flag;

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            // Query if we are flagged or not
            MapInformation mapInfo = tilemap.GetComponent<Transform>().gameObject.GetComponentInParent<MapInformation>();

            RevealedState state = mapInfo.GetRevealedStateForSquare(position.x, position.y);

            if(state == RevealedState.Unexplored)
            {
                tileData.sprite = Unexplored;
            }
            else if(state == RevealedState.Flag)
            {
                tileData.sprite = Flag;
            }
        }
    }
}
