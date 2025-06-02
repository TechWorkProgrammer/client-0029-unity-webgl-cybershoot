using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class TestStatic : MonoBehaviour
{
    public ApiConfig apiConfig;

    public TextMeshProUGUI score;

    public void AddScore() {
        ScoreManager.Instance.AddScore();
        score.text = ScoreManager.Instance.currentScore.ToString();
    }

    public void SubmitScore() {
        ScoreManager.Instance.SubmitScore(apiConfig);
    }
}
