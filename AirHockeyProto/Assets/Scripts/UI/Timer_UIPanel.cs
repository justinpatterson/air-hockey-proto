using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Timer_UIPanel : UIPanel
{
    AirHockeyManager _airHockeyManager;
    public TextMeshProUGUI countdownText;
    public override void OpenPanel()
    {
        base.OpenPanel();
        _airHockeyManager = FindObjectOfType<AirHockeyManager>(); //yeah it's time to make it a singleton

    }
    private void Update()
    {
        if (countdownText == null)
            return;
        if (_airHockeyManager)
        {
            float timeRemaining = Mathf.Clamp(_airHockeyManager.teamModel.timeRemaining, 0f, 1000f);
            float minutes = Mathf.Floor(timeRemaining / 60f);
            float seconds = Mathf.Floor(timeRemaining % 60f);
            countdownText.text = minutes.ToString("00") + ":" + seconds.ToString("00");
        }
        else
            countdownText.text = "00:00";
    }
}
