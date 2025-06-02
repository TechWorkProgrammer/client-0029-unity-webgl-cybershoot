using System;
using UnityEngine;
using UnityEngine.UI;

public class UICounter : MonoBehaviour
{
    [SerializeField] private Text TextInGameCounter;
    [SerializeField] private Text TextFrameRate;

    private float deltaTime;

    private void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

        DisplayTimer();
        DisplayFrameRate();
    }

    public void DisplayTimer()
    {
        float counter = GameManager.Instance.inGameTimerCounterValue;

        int minutes = Mathf.FloorToInt(counter / 60);
        int seconds = Mathf.FloorToInt(counter % 60);

        TextInGameCounter.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void DisplayFrameRate()
    {
        float fps = 1.0f / deltaTime;
        TextFrameRate.text = string.Format("FPS: {0:0.}", fps);
    }
}
