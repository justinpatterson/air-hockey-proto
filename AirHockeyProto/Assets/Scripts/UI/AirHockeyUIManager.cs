using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirHockeyUIManager : MonoBehaviour
{
    public AirHockeyManager _airHockeyManager;

    public UIPanel scorePanel, gameOverPanel;

    private void Awake()
    {
        _airHockeyManager = FindObjectOfType<AirHockeyManager>();
    }
    private void Start()
    {
        AirHockeyManager.OnPhaseTransition += PhaseTransitionListener;
    }

    public void ReportResetClicked() 
    {
        _airHockeyManager?.DoPhaseTransition(AirHockeyManager.GamePhases.Init);
    }

    private void PhaseTransitionListener(AirHockeyManager.GamePhases phase)
    {
        switch (phase) 
        {
            case AirHockeyManager.GamePhases.GameOver:
                gameOverPanel.OpenPanel();
                scorePanel.ClosePanel();
                break;
            default:
                gameOverPanel.ClosePanel();
                scorePanel.OpenPanel();
                break;
        
        }
    }
}
