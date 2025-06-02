
using AuroraFPSRuntime.SystemModules;
using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    [SerializeField] private bool isGamePaused = false;
    public bool isGamePausedValue => isGamePaused;

    [SerializeField] private bool isGameEnd = false;
    [Tooltip("In Game timer in second")][SerializeField] private float inGameTimer = 10;
    public float inGameTimerValue => inGameTimer;

    [SerializeField] private float inGameTimerCounter = 0;
    public float inGameTimerCounterValue => inGameTimerCounter;

    [SerializeField] private float gunRespawnTime = 5;
    public float gunRespawnTimeValue => gunRespawnTime;

    [SerializeField] private int respawnLimit = 2;
    private int respanwCounter = 0;

    [SerializeField] private ApiConfig apiConfig;

    public static GameManager Instance { get; private set; }

    public event Action OnEnemyKilledEvent;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    private void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += Play;
    }

    private void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded += Play;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        inGameTimerCounter = inGameTimer;
    }

    private void Start() {
        inGameTimerCounter = inGameTimer;
    }
    private void Update() {
        if (!isGameEnd) TimerCounter();
    }
    public void TimerCounter() {
        if (inGameTimerCounter > 0) inGameTimerCounter -= Time.deltaTime;
        else {
            inGameTimerCounter = inGameTimer;
            OnGameEnd();
            ScoreManager.Instance.SubmitScore(apiConfig);
            isGameEnd = true;
        }
    }
    //===========================================================================================
    public void Pause() {
        Time.timeScale = 0;
        isGamePaused = true;
    }
    public void Play() {
        Time.timeScale = 1;
        isGamePaused = false;
    }

    public void Play(Scene scene) {
        Time.timeScale = 1;
        isGamePaused = false;
    }
    //===========================================================================================
    // Trigger all gun respawners
    public void TriggerAllGunRespawners() {
        // foreach (IRespawner respawner in gunRespawners) {
        //     respawner.Respawn();
        // }
    }
    //===========================================================================================
    [Button]
    public void OnEnemyKilled() {
        ScoreManager.Instance.AddScore();
        OnEnemyKilledEvent.Invoke();
    }
    public void OnEnemyRevived() {
        Debug.Log("Enemy Revived");
    }
    public void OnPlayerDead() {
        if (respanwCounter >= respawnLimit) {
            OnGameOver();
            ScoreManager.Instance.SubmitScore(apiConfig);
        }
        respanwCounter++;
    }

    [Button]
    public void OnGameOver() {
        FindAnyObjectByType<CanvasManager>()?.ActivateGameOverCanvas();
        Pause();
    }
    public void OnGameEnd() {
        FindAnyObjectByType<CanvasManager>()?.ActivateGameEndCanvas();
        Pause();
    }

}
