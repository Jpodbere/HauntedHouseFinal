using UnityEngine;

public class EnemyScoreZone : MonoBehaviour
{
    public ScoreManager scoreManager;   
    public Transform player;           
    public int scoreAmount = 10;       

    bool m_HasScored = false;

    void OnTriggerEnter(Collider other)
    {
        if (m_HasScored) return;

        if (other.transform == player)
        {
            if (scoreManager != null)
            {
                for (int i = 0; i < scoreAmount / scoreManager.scorePerEnemy; i++)
                {
                    scoreManager.AddEnemyPassed();
                }

                Debug.Log("Player entered score zone: " + gameObject.name);
            }
            else
            {
                Debug.LogWarning("EnemyScoreZone on " + gameObject.name + " has no ScoreManager assigned.");
            }

            m_HasScored = true;
        }
    }
}

