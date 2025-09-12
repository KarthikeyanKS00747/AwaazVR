using UnityEngine;
using TMPro;
using System.Collections;

public class PresentationTimer : MonoBehaviour
{
    [Header("UI Reference")]
    public TextMeshProUGUI timerText;

    private float elapsedTime = 0f;
    private bool isRunning = false;

    private void Update()
    {
        if (isRunning)
        {
            elapsedTime += Time.deltaTime;
            timerText.text = FormatTime(elapsedTime);
        }
    }

    // Call this when the Start button is clicked
    public void StartTimer()
    {
        elapsedTime = 0f;
        isRunning = true;
    }

    // Call this when the Stop button is clicked
    public void StopTimer()
    {
        isRunning = false;
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int milliseconds = Mathf.FloorToInt((time * 1000f) % 1000f);
        return string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds);
    }
}
