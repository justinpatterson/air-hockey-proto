using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Evaluate_Phase : AirHockeyPhase
{
    public override void StartPhase()
    {
        base.StartPhase();
        //if neither player has won yet...
        if (_airHockeyManager.IsGameOver())
        {
            _airHockeyManager.DoPhaseTransition(AirHockeyManager.GamePhases.GameOver);
        }
        else
            {
            _airHockeyManager.DoPhaseTransition(AirHockeyManager.GamePhases.ResetPuck);
        }
        //else
    }

    public override void EndPhase()
    {
        base.EndPhase();
    }
    private void OnDestroy()
    {
    }

}
