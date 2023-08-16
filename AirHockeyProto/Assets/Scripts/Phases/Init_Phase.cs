using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Init_Phase : AirHockeyPhase
{
    public override void StartPhase()
    {
        base.StartPhase();

        PuckController pc = FindObjectOfType<PuckController>();
        pc?.Reset();

        _airHockeyManager.teamModel.Reset();
        _airHockeyManager.ReportGoalScoredOnTeam(-1); //this will force all score UI to reset
        _airHockeyManager.DoPhaseTransition(AirHockeyManager.GamePhases.Serve);


    }
}
