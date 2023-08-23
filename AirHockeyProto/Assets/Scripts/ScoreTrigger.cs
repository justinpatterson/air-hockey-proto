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
#if UNITY_EDITOR
        Debug.Log("Triggered score zone ... " + other.gameObject.name);
#endif
        if (other.CompareTag("Puck") && other.GetComponent<PuckController>())
        {
#if UNITY_EDITOR
            Debug.Log("Invoking Score Zone... ");
#endif
            OnScoreTriggerEvent?.Invoke(teamIndex);
        }
    }
}
