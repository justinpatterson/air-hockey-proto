using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Serve_Phase : AirHockeyPhase
{
    public override void StartPhase()
    {
        base.StartPhase();
        PuckController.OnCollisionDelegate += OnPuckHit;
    }

    public override void EndPhase()
    {
        _speedListener = false;
        if (_active) 
            PuckController.OnCollisionDelegate -= OnPuckHit;
        base.EndPhase();
    }
    private void OnDestroy()
    {
        if (_active)
            PuckController.OnCollisionDelegate -= OnPuckHit;
    }
    bool _speedListener = false;
    private void OnPuckHit(Collision collision)
    {
        if (_airHockeyManager == null)
            return;
        //Debug.Log("HIT");
        if (collision.gameObject.GetComponentInParent<StrikerMovementController>()) 
        {
            StrikerMovementController smc = collision.gameObject.GetComponentInParent<StrikerMovementController>();

            if (_airHockeyManager.IsValidServe(smc.playerIndex))
            {
                _speedListener = true;
            }
            else
            {
                Debug.Log("PENALTY");
                _airHockeyManager?.DoPhaseTransition(AirHockeyManager.GamePhases.ResetPuck); //reset isn't actually resetting consistently for some reason.  Why?
            }
        }
    }
    public override void UpdatePhase()
    {
        base.UpdatePhase();
        if (_speedListener) 
        {
            if(PuckController.lastPuckSpeed >= 0.3f)
            {
                _airHockeyManager?.DoPhaseTransition(AirHockeyManager.GamePhases.InProgress);
            }
        }

    }
}
