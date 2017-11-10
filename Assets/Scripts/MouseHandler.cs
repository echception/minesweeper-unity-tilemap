using UnityEngine;

namespace Minesweeper
{
    /// <summary>
    /// Handles mouse interaction with the minesweeper map
    /// </summary>
    [RequireComponent(typeof(MapInformation))]
    [RequireComponent(typeof(Grid))]
    public class MouseHandler : MonoBehaviour
    {
        private MapInformation m_map;
        private Grid m_grid;

        /// <summary>
        /// Initializes the mouse handler
        /// </summary>
        public void Start()
        {
            m_map = GetComponent<MapInformation>();
            m_grid = GetComponent<Grid>();
        }

        /// <summary>
        /// Called to update the mouse handler
        /// </summary>
        public void Update()
        {
            // Left click
            if (m_grid && Input.GetMouseButtonDown(0))
            {
                // Get the grid square, then reveal the square
                Vector3 world = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int gridPos = m_grid.WorldToCell(world);

                m_map.RevealSquare(gridPos.x, gridPos.y);
            }
            // Right click
            else if (m_grid && Input.GetMouseButtonDown(1))
            {
                // Get the grid square, then toggle the flag on unrevealed squares
                Vector3 world = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int gridPos = m_grid.WorldToCell(world);

                m_map.FlagSquare(gridPos.x, gridPos.y);
            }
        }
    }
}
