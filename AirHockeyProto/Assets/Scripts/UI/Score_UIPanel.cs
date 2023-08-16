using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score_UIPanel : UIPanel
{
    [SerializeField]
    public TeamScoreComponentUI teamScoreL, teamScoreR;
    // Start is called before the first frame update
    void Start()
    {
        AirHockeyManager.OnAirHockeyScoreUpdated += UpdateScoreListener;
    }

    private void UpdateScoreListener(AirHockeyManager.AirHockeyModel teamModel)
    {
        teamScoreL.UpdateScore(teamModel.teamL.score);
        teamScoreR.UpdateScore(teamModel.teamR.score);
    }

    private void OnDestroy()
    {
        AirHockeyManager.OnAirHockeyScoreUpdated -= UpdateScoreListener;
    }
}
