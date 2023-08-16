using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score_Phase : AirHockeyPhase
{
    public static int lastGoalTriggered = -1;
    public override void StartPhase()
    {
        base.StartPhase();
        //lastGoalTriggered set by InProgress_Phase when it detects a score
        _airHockeyManager.ReportGoalScoredOnTeam(lastGoalTriggered);
        
        //turn off puck or contain it somehow
        //flip score cards, any other effects needed to match new score
        //etc.


        lastGoalTriggered = -1; //revert to no goals queued.
        _airHockeyManager.DoPhaseTransition(AirHockeyManager.GamePhases.Evaluate);
    }

    public override void EndPhase()
    {
        base.EndPhase();
    }
    private void OnDestroy()
    {
    }

}
