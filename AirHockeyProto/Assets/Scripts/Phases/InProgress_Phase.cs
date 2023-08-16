using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InProgress_Phase : AirHockeyPhase
{
    public override void StartPhase()
    {
        base.StartPhase();
        ScoreTrigger.OnScoreTriggerEvent += OnScore;
    }

    private void OnScore(int teamGoal)
    {
        Score_Phase.lastGoalTriggered = teamGoal;
        _airHockeyManager?.DoPhaseTransition(AirHockeyManager.GamePhases.Score);
    }

    public override void EndPhase()
    {
        if (_active)
            ScoreTrigger.OnScoreTriggerEvent -= OnScore;
        base.EndPhase();
    }
    private void OnDestroy()
    {
        if (_active)
            ScoreTrigger.OnScoreTriggerEvent -= OnScore;
    }
}
