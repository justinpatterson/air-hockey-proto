using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InProgress_Phase : AirHockeyPhase
{
    public static int PenaltyPlayerIndex = -1;

    public override void StartPhase()
    {
        base.StartPhase();

        if (PenaltyPlayerIndex != -1) 
        {
            TriggerPenalty();
        }
        else
        {
            ScoreTrigger.OnScoreTriggerEvent += OnScore;
        }
    }

    void TriggerPenalty() 
    {
        Debug.Log("SHOULD RESET...");
        _airHockeyManager?.DoPhaseTransition(AirHockeyManager.GamePhases.ResetPuck);
    }

    private void OnScore(int teamGoal)
    {
        Score_Phase.lastGoalTriggered = teamGoal;
        _airHockeyManager?.DoPhaseTransition(AirHockeyManager.GamePhases.Score);
    }

    public override void UpdatePhase()
    {
        base.UpdatePhase();
        if (_airHockeyManager == null)
            return;
        if (_airHockeyManager.currentPhase == AirHockeyManager.GamePhases.InProgress)
        {
            _airHockeyManager.teamModel.timeRemaining -= Time.deltaTime;
            if(_airHockeyManager.teamModel.timeRemaining <= 0f)
                _airHockeyManager.DoPhaseTransition(AirHockeyManager.GamePhases.GameOver);
        }
    }

    public override void EndPhase()
    {
        if (_active)
            ScoreTrigger.OnScoreTriggerEvent -= OnScore;
        PenaltyPlayerIndex = -1;
        base.EndPhase();
    }
    private void OnDestroy()
    {
        if (_active)
            ScoreTrigger.OnScoreTriggerEvent -= OnScore;
    }
}
