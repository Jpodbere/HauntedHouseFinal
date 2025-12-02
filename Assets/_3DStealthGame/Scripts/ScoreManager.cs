using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public Text scoreText;
    public int scorePerEnemy = 10;

    int m_CurrentScore;

    void Start()
    {
        UpdateScoreUI();
    }

    public void AddEnemyPassed()
    {
        m_CurrentScore += scorePerEnemy;
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + m_CurrentScore;
        }
    }
}
