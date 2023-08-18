using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver_Phase : AirHockeyPhase
{
    public override void StartPhase()
    {
        Time.timeScale = 0f;
        base.StartPhase();
    }

    public override void EndPhase()
    {
        Time.timeScale = 1f;
        base.EndPhase();
    }

}
