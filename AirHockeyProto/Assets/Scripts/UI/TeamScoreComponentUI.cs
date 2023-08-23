using UnityEngine.UI;
using TMPro;
[System.Serializable]
public class TeamScoreComponentUI 
{
    public TextMeshProUGUI teamNameText;
    public TextMeshProUGUI scoreText;
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
