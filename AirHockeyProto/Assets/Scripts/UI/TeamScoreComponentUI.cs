using UnityEngine.UI;

[System.Serializable]
public class TeamScoreComponentUI 
{
    public Text teamNameText;
    public Text scoreText;
    string _team, _score;
    public void UpdateScore(int score) 
    {
        _score = score.ToString("00");
        scoreText.text = _score;
    }
    public void UpdateTeam(string teamName) 
    {
        _team = teamName;
        teamNameText.text = _team;
    }
}
