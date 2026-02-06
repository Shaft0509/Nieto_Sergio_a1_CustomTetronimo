using UnityEngine;
using UnityEngine.Events;

// handles score + game over state
public class TetrisManager : MonoBehaviour
{
    public int score;
    public bool gameOver;

    // events let UI listen without tight coupling
    public UnityEvent<int> OnScoreChanged;
    public UnityEvent<bool> OnGameOver;

    public void AddScore(int amount)
    {
        score += amount;
        OnScoreChanged?.Invoke(score); // tell UI score changed
    }

    public void SetGameOver(bool value)
    {
        gameOver = value;
        OnGameOver?.Invoke(gameOver);
    }

    public void ResetState()
    {
        score = 0;
        OnScoreChanged?.Invoke(score);
        SetGameOver(false);
    }
}
