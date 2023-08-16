using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirHockeyPhase : MonoBehaviour
{
    [SerializeField]
    protected AirHockeyManager _airHockeyManager;

    protected bool _active;
    private void Awake()
    {
        if(_airHockeyManager == null)
            _airHockeyManager = FindObjectOfType<AirHockeyManager>();
    }
    public virtual void StartPhase() 
    {
        _active = true;
    }
    public virtual void UpdatePhase() 
    {
    
    }
    public virtual void EndPhase() 
    {
        _active = false;
    }
}
