using Sirenix.OdinInspector;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour {
    public static ScoreManager Instance;

    public PlayerData playerData;

    public Token sessionToken;

    public int currentScore;

    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private TextMeshProUGUI loadingIndicator;
    private Coroutine loadingCoroutine;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public void AddScore() {
        var score = 100;
        currentScore += score;
    }

    public void SubmitScore(ApiConfig apiConfig) {

        playerData.total_score += currentScore;
        Debug.Log("Submitting score: " + playerData.total_score);

        apiConfig.SubmitScore(currentScore, () => {
            IEnumerator ShowSuspiciousPanel() {
                // Show the suspicious activity panel
                loadingPanel.SetActive(true);
                loadingIndicator.text = "Error submitting score, trying again";
                playerData.total_score -= currentScore;
                currentScore = 0;
                yield return new WaitForSeconds(2);
                loadingPanel.SetActive(false);
            }
            StartCoroutine(ShowSuspiciousPanel());
        },
        () => {
            ShowLoadingIndicator(true);
        },
        () => {
            ShowLoadingIndicator(false);
            currentScore = 0;
        });
    }

    // Function to show or hide the loading indicator
    [Button]
    public void ShowLoadingIndicator(bool show) {
        if (show) {
            loadingPanel.SetActive(true);
            if (loadingCoroutine == null) {
                loadingCoroutine = StartCoroutine(LoadingIndicatorCoroutine());
            }
        } else {
            loadingPanel.SetActive(false);
            if (loadingCoroutine != null) {
                StopCoroutine(loadingCoroutine);
                loadingCoroutine = null;
            }
        }
    }

    // Coroutine to update the loading indicator text
    private IEnumerator LoadingIndicatorCoroutine() {
        while (true) {
            loadingIndicator.text = "Loading";
            for (int i = 0; i < 3; i++) {
                yield return new WaitForSeconds(0.3f);
                loadingIndicator.text += ".";
            }
            yield return new WaitForSeconds(0.3f);
        }
    }
}
