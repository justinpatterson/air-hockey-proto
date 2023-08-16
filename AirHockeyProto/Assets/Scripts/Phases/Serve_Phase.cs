using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Save_Phase : AirHockeyPhase
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
        if (collision.gameObject.name == "PF_Striker" && collision.gameObject.GetComponentInParent<MCS.Player.StrikerMovementController>()) 
        {
            //eventually test which player hit, and who should currently be serving
            _airHockeyManager?.DoPhaseTransition(AirHockeyManager.GamePhases.InProgress);
        }
    }
}
