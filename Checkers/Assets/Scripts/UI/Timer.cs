using System.Globalization;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public float DigitsSpacing;

    private TextMeshProUGUI timerText;
    private float time;
    private int seconds;
    private int minutes;
    private int hours;

    private void Start()
    {
        timerText = GetComponent<TextMeshProUGUI>();
        time = 0;
    }

    private void Update()
    {
        time += Time.deltaTime;
        UpdateTimer();
    }

    private void UpdateTimer()
    {
        UpdateTimerValues();
        SetTimerText();
    }

    private void UpdateTimerValues()
    {
        seconds = (int) (time % 60);
        minutes = (int) (time / 60) % 60;
        hours = (int) (time / 60 / 60) % 24;
    }

    private void SetTimerText()
    {
        timerText.text = GetFormatedTimerValue(minutes) + ':' + GetFormatedTimerValue(seconds);
        if (hours > 0)
            timerText.text = GetFormatedTimerValue(hours) + ':' + timerText.text;
    }

    private string GetFormatedTimerValue(int timeValue)
    {
        return "<mspace=" + DigitsSpacing.ToString("F2", CultureInfo.CreateSpecificCulture("en-US")) + "em>" +
               timeValue.ToString("00") + "</mspace>";
    }
}