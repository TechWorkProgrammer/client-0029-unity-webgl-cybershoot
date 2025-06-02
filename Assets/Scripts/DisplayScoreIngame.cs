using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayScoreIngame : MonoBehaviour
{
    private TextMeshProUGUI scoreText;

    private void OnEnable() {
       
        StartCoroutine(SubscribeToGameManagerEvent());
    }

    private void Awake() {
        scoreText = GetComponent<TextMeshProUGUI>();
    }

    private void OnDisable() {
        GameManager.Instance.OnEnemyKilledEvent -= UpdateDisplayScore;
    }

    private void UpdateDisplayScore() {
        scoreText.text = "$cybers: " + ScoreManager.Instance.currentScore.ToString();
    }

    private IEnumerator SubscribeToGameManagerEvent() {
        while (GameManager.Instance == null) {
            yield return null;
        }
        GameManager.Instance.OnEnemyKilledEvent += UpdateDisplayScore;
    }
}
