using UnityEngine;
using UnityEngine.Tilemaps;

namespace Minesweeper
{
    /// <summary>
    /// Tile for revealed squares
    /// </summary>
    public class RevealedSquare : TileBase
    {
        /// <summary>
        /// Ordered list of sprites.  Indexes 0-9 should be sprites of the respective number
        /// </summary>
        public Sprite[] NumberedSprites;

        public Sprite BombSprite;

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            // Query the map information for how many bombs we are touching
            MapInformation mapInfo = tilemap.GetComponent<Transform>().gameObject.GetComponentInParent<MapInformation>();

            int count = mapInfo.GetBombsTouchingSquare(position.x, position.y);

            // Less than zero means we have a bomb
            if (count < 0)
            {
                tileData.sprite = BombSprite;
            }
            else
            {
                tileData.sprite = NumberedSprites[count];
            }
        }
    }
}
