// Lưu thông tin của screen
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public void SaveGameState(List<Block> blocks, int score)
    {
        int size = blocks.Count;
        List<Vector2Int> listCoordinateses = new List<Vector2Int>(size);
        List<int> listNumber = new List<int>(size);
        List<string> listOfState = new List<string>(size);

        foreach (Block block in blocks)
        {
            listCoordinateses.Add(block.cell.coordinates);
            listNumber.Add(block.number);
            listOfState.Add(block.state.name);
        }

        string strCoor = string.Join("|", listCoordinateses);
        string strNum = string.Join("|", listNumber);

        PlayerPrefs.SetString("coor", strCoor);
        PlayerPrefs.SetString("number", strNum);
        PlayerPrefs.SetInt("score", score);
        PlayerPrefs.SetInt("blocksSize", size);
        PlayerPrefs.Save();
    }

    public void ResetGameState()
    {
        int highScore = PlayerPrefs.GetInt("best", 0);

        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("isWon", 0);
        PlayerPrefs.SetInt("best", highScore);
        PlayerPrefs.Save();
    }
}
