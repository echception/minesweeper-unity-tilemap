using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace Minesweeper
{
    /// <summary>
    /// Stores information about a single Mineswepeer map
    /// </summary>
    public class MapInformation : MonoBehaviour
    {
        // Map dimensions
        public int SizeX;
        public int SizeY;
        public int NumberOfBombes;

        // Tile information
        public TileBase UnrevealedSquare;
        public TileBase RevealedSquare;

        public bool Enabled;
        public GameManager UI;

        /// <summary>
        /// Stores 0 for non-bomb, 1 for bombs
        /// </summary>
        private byte[,] m_grid;
        private RevealedState[,] m_revealedState;
        private Tilemap m_tileMap;
        private bool m_gameOver;

        // UI comonents
        private Text m_bombsRemainingText;
        private Text m_timeText;
        private Stopwatch m_stopWatch;

        private int m_bombsRemaining;

        /// <summary>
        /// Initializes the map information
        /// </summary>
        public void Start()
        {
            m_stopWatch = new Stopwatch();

            InitializeBoard();

            m_timeText = UI.transform.Find("Top Panel/Time/Text").GetComponent<Text>();
            m_bombsRemainingText = UI.transform.Find("Top Panel/Bombs/Text").GetComponent<Text>();
        }

        /// <summary>
        /// Restarts the current map, replacing it with a new, randomly generated map
        /// </summary>
        /// <param name="sizeX">The size of the new map in the horizontal direction</param>
        /// <param name="sizeY">The size of the new map in the vertical direction</param>
        /// <param name="numberOfBombs">Number of bombs in the new map</param>
        public void Restart(int sizeX, int sizeY, int numberOfBombs)
        {
            this.SizeX = sizeX;
            this.SizeY = sizeY;
            this.NumberOfBombes = numberOfBombs;
            Enabled = true;

            this.InitializeBoard();
        }

        /// <summary>
        /// Retrieves the revealed state of the given square
        /// </summary>
        /// <param name="x">The x coordinate of the square</param>
        /// <param name="y">The y coordinate of the square</param>
        /// <returns>The RevealedState of the square</returns>
        public RevealedState GetRevealedStateForSquare(int x, int y)
        {
            return m_revealedState[x, y];
        }

        /// <summary>
        /// Gets the number of bombs touching the given square
        /// </summary>
        /// <param name="x">The x coordinate of the square</param>
        /// <param name="y">The y coordinate of the square</param>
        /// <returns>The number of bombs touching the square, or -1 if the square is a bomb</returns>
        public int GetBombsTouchingSquare(int x, int y)
        {
            if(m_grid[x, y] != 0)
            {
                return -1;
            }

            int count = 0;

            foreach (Vector3Int neighbor in GetNeighbors(new Vector3Int(x, y, 0)))
            {
                count += CheckSquare(neighbor.x, neighbor.y);
            }

            return count;
        }

        /// <summary>
        /// Reveals the current square.  If the square does not touch any bombs, does a flood fil of the touching 
        /// non bomb-touching squares
        /// </summary>
        /// <param name="x">The x coordinate of the square</param>
        /// <param name="y">The y coordinate of the square</param>
        public void RevealSquare(int x, int y)
        {
            if (m_gameOver || !IsOnMap(x, y))
            {
                return;
            }

            Queue<Vector3Int> squaresToReveal = new Queue<Vector3Int>();
            squaresToReveal.Enqueue(new Vector3Int(x, y, 0));

            while (squaresToReveal.Count > 0)
            {
                Vector3Int square = squaresToReveal.Dequeue();
                RevealedState state = m_revealedState[square.x, square.y];
                if (state == RevealedState.Unexplored)
                {
                    // Reveal the square and update the tile
                    m_revealedState[square.x, square.y] = RevealedState.Guessed;
                    m_tileMap.SetTile(new Vector3Int(square.x, square.y, 0), RevealedSquare);

                    if(m_grid[square.x, square.y] == 1)
                    {
                        // Hit a bomb, end the game
                        StopGame(false);
                        break;
                    }

                    // If the square isn't touching bombs, add all the unexplored neighbors to the queue of squares we're revealing
                    if (GetBombsTouchingSquare(square.x, square.y) == 0)
                    {
                        foreach (Vector3Int neighbor in GetNeighbors(square))
                        {
                            if(IsOnMap(neighbor.x, neighbor.y) && m_revealedState[neighbor.x, neighbor.y] == RevealedState.Unexplored)
                            {
                                squaresToReveal.Enqueue(neighbor);
                            }
                        }
                    }
                }
            }

            // Check if we've won the game
            if(CheckVictory())
            {
                StopGame(true);
            }
        }

        /// <summary>
        /// Flag the given square.  This doesn't reveal the square, just adds a mark saying there may be a bomb there
        /// </summary>
        /// <param name="x">The x coordinate of the square</param>
        /// <param name="y">The y coordinate of the squar</param>
        public void FlagSquare(int x, int y)
        {
            if(!IsOnMap(x, y))
            {
                return;
            }

            RevealedState state = m_revealedState[x, y];

            // Can't flag revealed squares
            if(state == RevealedState.Flag || state == RevealedState.Unexplored)
            {
                if(state == RevealedState.Flag)
                {
                    state = RevealedState.Unexplored;
                    m_bombsRemaining++;
                }
                else
                {
                    state = RevealedState.Flag;
                    m_bombsRemaining--;
                }

                // Update the tile
                m_revealedState[x, y] = state;
                m_tileMap.RefreshTile(new Vector3Int(x, y, 0));
            }
        }

        /// <summary>
        /// Called to update the map information
        /// </summary>
        public void Update()
        {
            // Update the timer
            m_bombsRemainingText.text = m_bombsRemaining.ToString();
            m_timeText.text = string.Format("{0:D2}:{1:D2}", Mathf.FloorToInt((float)m_stopWatch.Elapsed.TotalMinutes), m_stopWatch.Elapsed.Seconds);
        }

        /// <summary>
        /// Check if the player has won the game
        /// </summary>
        /// <returns>True if the player has won, otherwise false</returns>
        private bool CheckVictory()
        {
            for(int x = 0; x < SizeX; x++)
            {
                for(int y = 0; y < SizeY; y++)
                {
                    // Any non-bombs not revealed means we haven't finished
                    if(m_grid[x, y] == 0 && m_revealedState[x, y] != RevealedState.Guessed)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// End the game
        /// </summary>
        /// <param name="victory">True if the game ends in victory, otherwise false</param>
        private void StopGame(bool victory)
        {
            m_gameOver = true;
            m_stopWatch.Stop();
            UI.OnGameOver(victory);
        }

        /// <summary>
        /// Initilizes the square with the current map size information
        /// </summary>
        private void InitializeBoard()
        {
            // Argument checking
            if(!Enabled)
            {
                return;
            }

            if (SizeX <= 0)
            {
                throw new ArgumentOutOfRangeException("SizeX");
            }

            if (SizeY <= 0)
            {
                throw new ArgumentOutOfRangeException("SizeY");
            }

            if (NumberOfBombes <= 0 || NumberOfBombes > SizeX * SizeY)
            {
                throw new ArgumentOutOfRangeException("NumberOfBombes");
            }

            m_tileMap = this.transform.Find("MinesweeperMap").GetComponent<Tilemap>();

            m_grid = new byte[SizeX, SizeY];
            m_revealedState = new RevealedState[SizeX, SizeY];
            m_gameOver = false;
            m_bombsRemaining = this.NumberOfBombes;
            m_stopWatch.Reset();
            m_stopWatch.Start();

            m_tileMap.ClearAllTiles();

            // Place bombs
            System.Random rand = new System.Random();
            for (int i = 0; i < NumberOfBombes; i++)
            {
                // Dumb, brute-force approach to placing bombs
                while (true)
                {
                    int x = rand.Next(0, SizeX);
                    int y = rand.Next(0, SizeY);

                    if (m_grid[x, y] == 0)
                    {
                        m_grid[x, y] = 1;
                        break;
                    }
                }
            }

            // Mark all the squares as unexplored
            for (int x = 0; x < SizeX; x++)
            {
                for (int y = 0; y < SizeY; y++)
                {
                    m_revealedState[x, y] = RevealedState.Unexplored;
                    m_tileMap.SetTile(new Vector3Int(x, y, 0), UnrevealedSquare);
                }
            }

            // Center the camera
            Camera.main.transform.position = new Vector3(SizeX / 2f, SizeY / 2f, -10);

            // Adjust the camera size so the board fits
            Rect uiRect = UI.GetComponent<RectTransform>().rect;
            Rect topPanelRect = UI.transform.Find("Top Panel").GetComponent<RectTransform>().rect;

            float height = uiRect.height - (topPanelRect.height * 2);
            float percent = height / uiRect.height;
            Camera.main.orthographicSize = (Mathf.Max(SizeX, SizeY) / percent + 1) / 2f;
        }

        /// <summary>
        /// Returns a list of all the neighbors the given square has
        /// </summary>
        /// <param name="getSquare">The square</param>
        /// <returns>A list of all neighbors (could include invalid neighbors that are off the board)</returns>
        private Vector3Int[] GetNeighbors(Vector3Int getSquare)
        {
            int x = getSquare.x;
            int y = getSquare.y;

            return new Vector3Int[]
            {
                new Vector3Int(x - 1,   y - 1,  0),
                new Vector3Int(x - 1,   y,      0),
                new Vector3Int(x - 1,   y + 1,  0),
                new Vector3Int(x,       y - 1,  0),
                new Vector3Int(x,       y + 1,  0),
                new Vector3Int(x + 1,   y - 1,  0),
                new Vector3Int(x + 1,   y,      0),
                new Vector3Int(x + 1,   y + 1,  0),
            };
        }

        /// <summary>
        /// Checks if the given coordinate is a bomb
        /// </summary>
        /// <param name="x">The x value of the coordinate</param>
        /// <param name="y">The y value of the coordinate</param>
        /// <returns>1 if the square is a bomb, otherwise 0.  Coordinates off the map will return as not-bomb</returns>
        private int CheckSquare(int x, int y)
        {
            if(!IsOnMap(x, y))
            {
                return 0;
            }

            return m_grid[x, y];
        }

        /// <summary>
        /// Checks if the given square is on the map
        /// </summary>
        /// <param name="x">The x coordinate</param>
        /// <param name="y">The y coordinate</param>
        /// <returns>True if the coorindate is on the map, otherwise false</returns>
        private bool IsOnMap(int x, int y)
        {
            if (x < 0 || y < 0 || x >= SizeX || y >= SizeX)
            {
                return false;
            }

            return true;
        }
    }

    /// <summary>
    /// Tracks the revealed state of a square
    /// </summary>
    public enum RevealedState : byte
    {
        /// <summary>
        /// Square is unexplored
        /// </summary>
        Unexplored = 0,

        /// <summary>
        /// Square is unexplored, but the player has flagged it
        /// </summary>
        Flag = 1,

        /// <summary>
        /// Square has been revealed
        /// </summary>
        Guessed = 2
    }
}
