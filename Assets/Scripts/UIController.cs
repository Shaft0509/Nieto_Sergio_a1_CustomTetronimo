using TMPro;
using UnityEngine;

// very simple UI controller
public class UIController : MonoBehaviour
{
    public TetrisManager manager;
    public Board board;

    public TextMeshProUGUI scoreText;
    public GameObject endgamePanel;

    private void Start()
    {
        if (endgamePanel != null)
            endgamePanel.SetActive(false);

        UpdateScore(0);
    }

    // called when score changes
    public void UpdateScore(int newScore)
    {
        if (scoreText != null)
            scoreText.text = "Score: " + newScore;
    }

    // called when game ends
    public void UpdateGameOver(bool isGameOver)
    {
        if (endgamePanel != null)
            endgamePanel.SetActive(isGameOver);
    }

    // button hook
    public void PlayAgain()
    {
        manager.ResetState();
        board.ResetBoard();
    }
}
