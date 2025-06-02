using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Runtime.InteropServices;
using Asynkrone.UnityTelegramGame.Networking;
using UnityEngine.Networking;
using TMPro;
using Sirenix.OdinInspector;
using System;



public class LoginTelegram : MonoBehaviour
{

    [Header("UI References")]
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private TextMeshProUGUI loadingIndicator;

    private Coroutine loadingCoroutine;

    private Token sessionToken;

    [SerializeField] private ProfileHandler profileHandler;

    [SerializeField] private ApiConfig apiConfig;

    private void Start() {
        if (!String.IsNullOrEmpty(ScoreManager.Instance.playerData.username)) {
            profileHandler.DisplayProfile();
            return; 
        }
        Debug.Log("No username found, starting login process...");
        Login();
    }

    public void Login() {
#if UNITY_WEBGL && !UNITY_EDITOR
            sessionToken = new Token();
            sessionToken.accessToken = URLParameters.GetSearchParameters()["access_token"];
            sessionToken.refreshToken = URLParameters.GetSearchParameters()["refresh_token"];
            ScoreManager.Instance.sessionToken = sessionToken;
            ShowLoadingIndicator(true);
            StartCoroutine(CheckUserData(sessionToken));
#else
        sessionToken = new Token();
        sessionToken.accessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6MTMwODEyOTY0MCwiaWF0IjoxNzM1MTk2OTkzLCJleHAiOjE3MzUyMDA1OTN9.33SgbLVvldLsjBY_-01-ekWzAhveVlAk60reNtCzPLU";
        sessionToken.refreshToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6MTMwODEyOTY0MCwiaWF0IjoxNzM1MTk2OTkzLCJleHAiOjE3Mzc3ODg5OTN9.ZdspuHpHh-kAWusEkU9J-keXrjLz8aXSwyUDxschJCQ";
        ScoreManager.Instance.sessionToken = sessionToken;
        ShowLoadingIndicator(true);
        StartCoroutine(CheckUserData(sessionToken));
#endif
    }

    [Button]
    public void GetProfile() {
        ScoreManager.Instance.sessionToken = sessionToken;
        ShowLoadingIndicator(true);
        StartCoroutine(CheckUserData(sessionToken));
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

    private IEnumerator CheckUserData(Token sessionToken, int rateLimiting = 0) {

        rateLimiting = rateLimiting + 1;

        if (loadingCoroutine != null) {
            StopCoroutine(loadingCoroutine);
            loadingCoroutine = null;
        }

        loadingIndicator.text = "Checking user data...";

        string url = $"{apiConfig.serverURL}/me";

        using (UnityWebRequest www = UnityWebRequest.Get(url)) {

            // Add Authorization header with Bearer token
            www.SetRequestHeader("Authorization", $"Bearer {sessionToken.accessToken}");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success) {
                var response = JsonUtility.FromJson<UserDataResponse>(www.downloadHandler.text);

                if (response != null) {
                    // User exists, use database data
                    ScoreManager.Instance.playerData = response.payload.user;
                    profileHandler.DisplayProfile();
                    ShowLoadingIndicator(false);
                }
            } else {
                // Error
                Debug.LogError($"Error checking user data: {www.error}");
                loadingIndicator.text = "Session Expired, starting new session...";
                apiConfig.RefreshAccessToken(CheckUserData(sessionToken, rateLimiting), rateLimiting);
            }
        }
    }
}
