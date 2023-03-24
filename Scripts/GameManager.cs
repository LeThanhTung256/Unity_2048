using System;
using System.Collections;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameState gameState;
    public BlockBoard board;
    public CanvasGroup gameOver;
    public CanvasGroup gameWon;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI bestScoreText;
    public int score;

    private bool isGameOver;
    private int bestScore;

    [SerializeField] private AudioSource winSound;
    [SerializeField] private AudioSource gameOverSound;

    private void Start()
    {
        LoadGameState();
    }

    private void LoadGameState()
    {
        if (!board.LoadGameState())
        {
            NewGame();
            return;
        }

        SetScore(GetLastScore());
        bestScore = GetBestScore();
        bestScoreText.text = bestScore.ToString();

        // Đặt giá trị alpha về 0
        gameOver.alpha = 0f;
        gameWon.alpha = 0f;

        // Tránh canvas game over, win game ngăn cản tương tác người dùng
        gameOver.interactable = false;
        gameOver.blocksRaycasts = false;
        gameWon.interactable = false;
        gameWon.blocksRaycasts = false;
    }    
    
    public void NewGame()
    {
        PlayerPrefs.Save();
        SetScore(0);
        bestScore = GetBestScore();
        bestScoreText.text = bestScore.ToString();
        PlayerPrefs.SetInt("isWon", 0);

        // Đặt giá trị alpha về 0
        gameOver.alpha = 0f;
        gameWon.alpha = 0f;

        // Tránh canvas game over, win game ngăn cản tương tác người dùng
        gameOver.interactable = false;
        gameOver.blocksRaycasts = false;
        gameWon.interactable = false;
        gameWon.blocksRaycasts = false;

        board.ClearBoard();
        board.CreateBlock();
        board.CreateBlock();
        board.enabled = true;
        this.isGameOver = false;

        winSound.Stop();
        gameOverSound.Stop();
    }

    public void GameWon()
    {
        // Chỉ lần đầu tiên mới thông báo
        this.winSound.Play();

        board.enabled = false;
        StartCoroutine(Fade(gameWon, 1f, 0.5f));

        // Ngăn chặn người dùng tương tác
        gameWon.interactable = true;
        gameWon.blocksRaycasts = true;
        PlayerPrefs.SetInt("isWon", 1);
        PlayerPrefs.Save();

    }

    public void GameOver()
    {
        this.gameOverSound.Play();

        board.enabled = false;
        StartCoroutine(Fade(gameOver, 1f, 0.5f));

        // Ngăn chặn tương tác người dùng
        gameOver.interactable = true;
        gameOver.blocksRaycasts = true;
        this.isGameOver = true;
    }

    public void IncreaseScore(int addition)
    {
        SetScore(score + addition);
    }    

    private void SetScore(int score)
    {
        this.score = score;

        scoreText.text = score.ToString();

        if (score > bestScore)
        {
            bestScoreText.text = score.ToString();
            SaveBestScore();
        }    
    }

    // Lấy điểm cao nhất đã được lưu
    private int GetBestScore()
    {
        return PlayerPrefs.GetInt("best", 0);
    }

    private int GetLastScore()
    {
        return PlayerPrefs.GetInt("score", 0);
    }    

    // Lưu điểm
    private void SaveBestScore()
    {
        int bestScore = GetBestScore();

        if (score > bestScore)
        {
            PlayerPrefs.SetInt("best", score);
        }    
    }
    // Chơi tiếp sau khi thắng
    public void Continue()
    {
        this.winSound.Stop();

        board.enabled = true;
        StartCoroutine(Fade(gameWon, 0f, 0f));

        // Ngăn chặn tương tác người dùng
        gameWon.interactable = false;
        gameWon.blocksRaycasts = false;
    }    

    private IEnumerator Fade(CanvasGroup canvasGroup,  float to, float delay)
    {
        yield return new WaitForSeconds(delay);
        float elapsed = 0f;
        float duration = 0.5f;
        float from = canvasGroup.alpha;

        while (elapsed < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = to;
    }

    // Khi thoát game, lưu state lại
    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            SaveState();
        }
    }

    private void OnApplicationPause(bool isPause)
    {
        if (isPause)
        {
            SaveState();
        }
    }

    private void OnApplicationQuit()
    {
        SaveState();
    }

    private void SaveState()
    {
        if (this.isGameOver)
        {
            gameState.ResetGameState();
        }
        else
        {
            gameState.SaveGameState(board.blocks, this.score);
        }
    }
}
