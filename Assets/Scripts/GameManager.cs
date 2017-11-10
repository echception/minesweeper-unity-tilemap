using UnityEngine;
using UnityEngine.UI;

namespace Minesweeper
{
    /// <summary>
    /// Manages the state of the active game
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public GameObject Buttons;
        public GameObject Title;
        public GameObject GameOver;
        public GameObject TopPanel;

        /// <summary>
        /// Starts an 'Easy' difficulty game
        /// </summary>
        public void StartEasy()
        {
            this.StartGame(9, 9, 10);
        }

        /// <summary>
        /// Starts a 'Medium' difficulty game
        /// </summary>
        public void StartMedium()
        {
            this.StartGame(16, 16, 40);
        }

        /// <summary>
        /// Starts a 'Difficult' difficulty game
        /// </summary>
        public void StartDifficult()
        {
            this.StartGame(25, 25, 99);
        }

        /// <summary>
        /// Starts a game with the given size, and the given number of bombs
        /// </summary>
        /// <param name="sizeX">The number of squares horizontally</param>
        /// <param name="sizeY">The number of squares vertically</param>
        /// <param name="numberOfBombs">The number of bombs to place.  This should be less than (sizeX * sizeY)</param>
        public void StartGame(int sizeX, int sizeY, int numberOfBombs)
        {
            GameObject board = GameObject.Find("Board");

            board.SetActive(true);

            board.GetComponent<MapInformation>().Restart(sizeX, sizeY, numberOfBombs);

            this.Buttons.SetActive(false);
            this.Title.SetActive(false);
            this.GameOver.SetActive(false);
            this.TopPanel.SetActive(true);
        }

        /// <summary>
        /// Shows the game over screen
        /// </summary>
        /// <param name="victory">Flag whether the game ended in victory</param>
        public void OnGameOver(bool victory)
        {
            this.Buttons.SetActive(true);
            this.Title.SetActive(false);
            this.GameOver.SetActive(true);

            if(victory)
            {
                this.GameOver.GetComponent<Text>().text = "Victory!!";
            }
            else
            {
                this.GameOver.GetComponent<Text>().text = "Game Over :(";
            }
        }
    }
}
