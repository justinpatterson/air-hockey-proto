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
        if(_active) 
            PuckController.OnCollisionDelegate -= OnPuckHit;
        base.EndPhase();
    }
    private void OnDestroy()
    {
        if (_active)
            PuckController.OnCollisionDelegate -= OnPuckHit;
    }
    private void OnPuckHit(Collision collision)
    {
        if (collision.gameObject.GetComponentInParent<AirHockey.Player.StrikerMovementController>()) 
        {
            //eventually test which player hit, and who should currently be serving
            _airHockeyManager?.DoPhaseTransition(AirHockeyManager.GamePhases.InProgress);
        }
    }
}
