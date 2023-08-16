using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreTrigger : MonoBehaviour
{
    public int teamIndex; //if this is 4 player, we'd have to revise this
    public delegate void ScoreTriggerEvent(int teamGoal);
    public static ScoreTriggerEvent OnScoreTriggerEvent;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Triggered score zone ... " + other.gameObject.name);
        if (other.CompareTag("Puck") && other.GetComponent<PuckController>())
        {
            Debug.Log("Invoking Score Zone... ");
            OnScoreTriggerEvent?.Invoke(teamIndex);
        }
    }
}
