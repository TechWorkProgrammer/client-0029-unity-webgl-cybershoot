using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;

public class ApiConfig : MonoBehaviour {

    [Header("Configuration")]
    public string serverURL;

    [SerializeField] private GameObject loginAgainPanel;
    [SerializeField] private TextMeshProUGUI loginAgainText;

    [System.Serializable]
    private class RefreshTokenRequest {
        public string refresh_token;
    }

    [System.Serializable]
    private class ScoreSubmission {
        public string encrypted_score;
        public float time_since_last_submission;
    }

    [System.Serializable]
    public class TokenResponse {
        public string message;
        public TokenPayload payload;
    }

    [System.Serializable]
    public class TokenPayload {
        public string accessToken;
    }

    private const float MIN_SUBMISSION_INTERVAL = 2f; // Minimum time between submissions
    private const int MAX_SCORE_PER_SECOND = 1000;   // Maximum allowed score increase per second
    private float lastSubmissionTime;
    private int lastSubmittedScore = 0;


    public void SubmitScore(int score, Action onSuspiciousDetected, Action onLoading, Action onSubmitSuccess) {
        StartCoroutine(SubmitScoreCoroutine(score, onSuspiciousDetected, onLoading, onSubmitSuccess));
    }

    private string EncryptScore(int score) {
        string scoreData = $"{score}:{Time.time}";
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(scoreData);
        return Convert.ToBase64String(bytes);
    }

    private IEnumerator SubmitScoreCoroutine(int score, Action onSuspiciousDetected, Action onLoading, Action onSubmitSuccess, int rateLimiting = 0) {

        onLoading?.Invoke();

        rateLimiting = rateLimiting + 1;

        // Rate limiting check
        if (Time.time - lastSubmissionTime < MIN_SUBMISSION_INTERVAL) {
            Debug.LogWarning("Submitting scores too quickly!");
            onSuspiciousDetected?.Invoke();
            yield break;
        }

        // Score increase validation
        float timeDiff = Time.time - lastSubmissionTime;
        int scoreDiff = score - lastSubmittedScore;
        float scoreRate = scoreDiff / timeDiff;

        if (scoreRate > MAX_SCORE_PER_SECOND) {
            Debug.LogWarning("Suspicious score increase detected!");
            onSuspiciousDetected?.Invoke();
            yield break;
        }

        // Prepare submission data
        ScoreSubmission submission = new ScoreSubmission {
            encrypted_score = EncryptScore(score),
            time_since_last_submission = timeDiff
        };


      
        string jsonData = JsonUtility.ToJson(submission);
        string url = $"{serverURL}/me/score";

        using (UnityWebRequest www = new UnityWebRequest(url, "PATCH")) {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Authorization", $"Bearer {ScoreManager.Instance.sessionToken.accessToken}");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success) {
                lastSubmissionTime = Time.time;
                lastSubmittedScore = score;
                Debug.Log("Score submitted successfully");
                onSubmitSuccess?.Invoke();
            } else {
                Debug.LogError($"Score submission failed: {www.error}");
                onSuspiciousDetected?.Invoke();
                RefreshAccessToken(SubmitScoreCoroutine(score, onSuspiciousDetected, onLoading, onSubmitSuccess, rateLimiting), rateLimiting);
            }
        }
    }


    public void RefreshAccessToken(IEnumerator coroutineCallback, int rateLimiting) {
        StartCoroutine(GetNewAccessToken(coroutineCallback, rateLimiting));
    }

    private IEnumerator GetNewAccessToken(IEnumerator coroutineCallback, int rateLimiting) {
        string url = $"{serverURL}/auth/refresh-tokens"; 

        if (rateLimiting > 3) {
            loginAgainPanel.SetActive(true);
            loginAgainText.text = "Please login again from bot";
            yield break;
        }

        // Create request body
        RefreshTokenRequest requestBody = new RefreshTokenRequest {
            refresh_token = ScoreManager.Instance.sessionToken.refreshToken
        };

        string jsonBody = JsonUtility.ToJson(requestBody);

        using (UnityWebRequest www = new UnityWebRequest(url, "POST")) {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success) {
                TokenResponse response = JsonUtility.FromJson<TokenResponse>(www.downloadHandler.text);
                ScoreManager.Instance.sessionToken.accessToken = response.payload.accessToken;
                Debug.Log("Access token refreshed successfully");
                StartCoroutine(coroutineCallback);
            } else {
                Debug.LogError($"Token refresh failed: {www.error}");
                loginAgainPanel.SetActive(true);
                loginAgainText.text = "Please login again from bot";
            }
        }
    }
}
