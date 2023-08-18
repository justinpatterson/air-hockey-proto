using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirHockeyManager : MonoBehaviour
{
    [System.Serializable]
    public struct AirHockeyModel
    {
        [System.Serializable]
        public struct TeamData
        {
            public int score;
            public string name;
            public List<int> playerIDs; //for now hard-coded in editor.  Perhaps in the future players could join and pick a side.
        }

        [SerializeField]
        public TeamData teamL; //not sure the best way we'd like to store teams. Teams consist of 1-2 players; there are always only 2 teams.
        [SerializeField]
        public TeamData teamR;

        public void Reset() 
        {
            teamL.score = 0;
            teamR.score = 0;
            timeRemaining = 240f;
        }

        public float timeRemaining;
    }
    [SerializeField]
    public AirHockeyModel teamModel;
    public delegate void AirHockeyScoreUpdate(AirHockeyModel teamModel);
    public static AirHockeyScoreUpdate OnAirHockeyScoreUpdated;

    //Init -> Serve -> In Progress -> Score OR Penalty -> Evaluate -> Reset (to Serve) OR GameOver
    public enum GamePhases
    {
        Init,
        Serve,
        InProgress,
        Score,
        Penalty,
        Evaluate,
        ResetPuck,
        GameOver
    }

    [System.Serializable]
    public struct PhaseInfo 
    {
        public GamePhases phase;
        public AirHockeyPhase phaseLogic;
    }
    public delegate void PhaseTransition(GamePhases phase);
    public static PhaseTransition OnPhaseTransition;

    public PhaseInfo[] phaseLogics;
    public GamePhases currentPhase = GamePhases.Init;
    AirHockeyPhase _currentPhaseLogic;

    private void Awake()
    {
    }
    private void Start()
    {
        DoPhaseTransition(GamePhases.Init);
    }

    public void DoPhaseTransition(GamePhases nextPhase) 
    {
        if (currentPhase != nextPhase)
        {
            _currentPhaseLogic?.EndPhase();
        }
        else 
        {
            //no point in ending a phase that is also being started.
        }
        
        currentPhase = nextPhase;
        foreach (PhaseInfo pl in phaseLogics)
        {
            if (pl.phase == currentPhase)
            {
                _currentPhaseLogic = pl.phaseLogic;
                break;
            }
        }
        _currentPhaseLogic?.StartPhase();
        OnPhaseTransition?.Invoke(currentPhase);
    }
    private void Update()
    {
        _currentPhaseLogic?.UpdatePhase();
        

    }
    int _servingTeamIndex = -1;
    public void ReportGoalScoredOnTeam(int teamGoalIndex) 
    {
        switch (teamGoalIndex) 
        {
            case 0: //left goal triggered, so right team gains
                teamModel.teamR.score++;
                _servingTeamIndex = 0;
                break;
            case 1: //right goal triggered, so right team gains
                teamModel.teamL.score++;
                _servingTeamIndex = 1;
                break;
            default:
                _servingTeamIndex = 0;
                //maybe -1, scored on no one
                break;
        }
        OnAirHockeyScoreUpdated?.Invoke(teamModel);
    }
    public bool IsValidServe(int playerIndex) 
    {
        int teamIndex = GetTeamIndexForPlayer(playerIndex);
        //Debug.Log("Player " + playerIndex + " on team " + teamIndex + " hit puck, valid person is " + _servingTeamIndex);
        return teamIndex == _servingTeamIndex || _servingTeamIndex == -1;

    }
    public int GetTeamIndexForPlayer(int playerIndex) 
    {
        if (teamModel.teamL.playerIDs.Contains(playerIndex)) { return 0; }
        else if (teamModel.teamR.playerIDs.Contains(playerIndex)) { return 1; }
        else return -1;
    }
    public bool IsGameOver() 
    {
        return teamModel.teamL.score >= 7 || teamModel.teamR.score >= 7;
    }
}
