using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reset_Phase : AirHockeyPhase
{
    public override void StartPhase()
    {
        base.StartPhase();
        //move puck back to start. <-- maybe this happens in reset

        PuckController puck = GameObject.FindObjectOfType<PuckController>();
        puck.Reset();
        
        StrikerMovementController[] strikers = GameObject.FindObjectsOfType<StrikerMovementController>();
        foreach (StrikerMovementController s in strikers)
            s.Reset();

        //turn puck back on
        _airHockeyManager.DoPhaseTransition(AirHockeyManager.GamePhases.Serve);


    }

    public override void EndPhase()
    {
        base.EndPhase();
    }
    private void OnDestroy()
    {
    }

}
