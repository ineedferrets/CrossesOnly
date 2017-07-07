using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameMaster : MonoBehaviour {

    public byte[] state = new byte[27];
    public GameObject[] grids;
    public GameObject playerText;
    public GameObject aISystem;

    private GameMaster instance;
    private int playerTurn = -1;
    private int[] gridsWon = { 0, 0, 0 };

	void Awake () {
        if (instance == null) {
            instance = this;
            state = new byte[27];
            for (int i = 0; i < 27; i++)
                state[i] = 0;
        }
        tossCoin();
	}

    // turn input is taken as string "a b" where a is the grid of choice and b is the cell of choice
    public void takeTurn(string gridCellChoice)
    {
        int grid = int.Parse(gridCellChoice.Split(' ')[0]) - 1;
        int cell = int.Parse(gridCellChoice.Split(' ')[1]) - 1;
        state[grid * 9 + cell] = 1;
        if (checkCross(grid))
        {
            nullifyGrid(grid);
            gridsWon[grid] = 1;
            int end = 1;
            for (int i = 0; i < 3; i++)
                end *= gridsWon[i];
            if (end == 1)
            { endGame(); return; }
        }
        playerTurn *= -1;
        if (playerTurn == -1)
        {
            setButtons(true);
            playerText.GetComponent<Text>().text = "Player Turn";
        }
        else { 
            setButtons(false);
            playerText.GetComponent<Text>().text = "Computer Turn";
            aISystem.GetComponent<AISystem>().takeTurn(state);
        }
    }

    private bool checkCross(int grid) {
        byte[] tempGrid = new byte[9];

        int gridDisplacement = grid * 9;

        for (int i = 0; i < 9; i++)
        {
            tempGrid[i] = state[gridDisplacement + i];
            switch (i) {
                case 2:
                    if ((tempGrid[0] == 1) && tempGrid[1] == 1 && tempGrid[2] == 1)
                        return true;
                    break;
                case 5:
                    if (tempGrid[3] == 1 && tempGrid[4] == 1 && tempGrid[5] == 1)
                        return true;
                    break;
                case 6:
                    if (tempGrid[0] == 1 && tempGrid[3] == 1 && tempGrid[6] == 1)
                        return true;
                    if (tempGrid[2] == 1 && tempGrid[4] == 1 && tempGrid[6] == 1)
                        return true;
                    break;
                case 7:
                    if (tempGrid[1] == 1 && tempGrid[4] == 1 && tempGrid[7] == 1)
                        return true;
                    break;
                case 8:
                    if (tempGrid[2] == 1 && tempGrid[5] == 1 && tempGrid[8] == 1)
                        return true;
                    if (tempGrid[0] == 1 && tempGrid[4] == 1 && tempGrid[8] == 1)
                        return true;
                    if (tempGrid[6] == 1 && tempGrid[7] == 1 && tempGrid[8] == 1)
                        return true;
                    break;
                default:
                    break;
            }
        }
        return false;
    }

    private void nullifyGrid(int grid) {
        for (int i = 0; i < grids[grid].transform.childCount; i++) {
            Transform child = grids[grid].transform.GetChild(i);
            if (child.childCount != 0) {
                Color tempColor = child.GetChild(0).GetComponent<Text>().color;
                tempColor.a = 0.5f;
                child.GetChild(0).GetComponent<Text>().color = tempColor;
                child.GetComponent<Button>().enabled = false;
            }
            else
            { Color tempColor = child.GetComponent<Image>().color; tempColor.a = 0.5f; child.GetComponent<Image>().color = tempColor; }
        }
        for (int i = 0; i < 9; i++)
            state[grid * 9 + i] = 1;
    }

    private void endGame() {
        if (playerTurn == -1)
            playerText.GetComponent<Text>().text = "Computer Wins!";
        else
            playerText.GetComponent<Text>().text = "Player wins!";
    }

    public byte[] getState() {
        return state;
    }

    private void setButtons(bool b) {
        for (int i = 0; i < 3; i++)
        {
            if (gridsWon[i] == 0)
            {
                for (int j = 0; j < grids[i].transform.childCount; j++)
                {
                    if (grids[i].transform.GetChild(j).childCount != 0)
                        grids[i].transform.GetChild(j).GetComponent<Button>().enabled = b;
                }
            }
        }
    }

    public void tossCoin() {
        int i = Random.Range(0, 100);
        if (i < 50) {
            playerTurn = -1;
            playerText.GetComponent<Text>().text = "Player Turn";
        }
        else {
            playerTurn = 1;
            setButtons(false);
            playerText.GetComponent<Text>().text = "Computer Turn";
            aISystem.GetComponent<AISystem>().takeTurn(state);
        }
    }

    public void resetGame()
    {
        SceneManager.LoadScene(0);
    }

    public void destroyGame()
    {
        Application.Quit();
    }

}
