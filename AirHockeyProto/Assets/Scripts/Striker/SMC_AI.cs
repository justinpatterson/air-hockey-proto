using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMC_AI : StrikerMovementController
{
    [SerializeField, Range(0f,1f)]
    float _aiServeCountdown = 1f;

    public override void Reset()
    {
        _aiServeCountdown = 1f;
        base.Reset();
    }

    protected override void MovementInputBehavior()
    {
        //no input for now -- AI doesn't hit keys like a Player.  Could virtualize it in the future
    }

    [SerializeField]
    float _distanceExpander = 0f;

    protected override void MovementFixedUpdateBehavior()
    {
        base.MovementFixedUpdateBehavior();

        Vector3 lastPos = rb.transform.position;
        Vector3 nextPos = lastPos;
        float speedModifier = 1f;

        if (puckTarget!=null)
        {
            Vector3 dir = puckTarget.transform.position - lastPos;
            Rigidbody puckRB = puckTarget.transform.GetComponent<Rigidbody>();

            if (airHockeyManager.currentPhase == AirHockeyManager.GamePhases.Serve)
            {
                if (airHockeyManager.IsValidServe(playerIndex)) {
                    _aiServeCountdown -= Time.deltaTime;
                    if (_aiServeCountdown <= 0f)
                    {
                        Vector3 strikerCenter = rb.transform.position;
                        Vector3 puckCenter = puckTarget.transform.position;
                        Vector3 hitDir = puckCenter - strikerCenter;
                        nextPos = puckCenter;
                    }
                    else 
                    {
                        //maybe move up and down in a sinewave -- TRICKY!
                        nextPos.z = table.tableInfo.zBounds.y;
                        float rand = Mathf.PerlinNoise(Time.time, Time.time);
                        float xPos = Mathf.Lerp(table.tableInfo.xBounds.x, table.tableInfo.xBounds.y, rand);
                        speedModifier = 1f;
                        nextPos.x = xPos;
                    }
                }
                else
                {
                    //maybe move up and down in a sinewave -- TRICKY!
                    nextPos.z = table.tableInfo.zBounds.y;
                    float rand = Mathf.PerlinNoise(Time.time, Time.time);
                    float xPos = Mathf.Lerp(table.tableInfo.xBounds.x, table.tableInfo.xBounds.y, rand);
                    speedModifier = 1f;
                    nextPos.x = xPos;
                }
            }
            else if (airHockeyManager.currentPhase == AirHockeyManager.GamePhases.InProgress)
            {

                /* SLOWLY EXPAND RADIUS WHILE PUCK IS ON AI SIDE */
                if (puckTarget.transform.position.z > 0f)
                    _distanceExpander += Time.deltaTime * 0.1f; //slowly expand attack radius to catch edge cases
                else
                    _distanceExpander = 0f;

                float puckDot = Vector3.Dot(puckRB.velocity.normalized, this.transform.forward); //"this" should be rotated to have forward be appropriate for player.
                                                                                                    //Debug.Log("Magnitude " + dir.magnitude);
                bool inCorner = table.isPuckInPlayerCorners(playerIndex);
                if (dir.magnitude < (AirHockeyGlobals.AIStrikerSettings.baseAttackDistance + _distanceExpander) && (puckDot < 0f || (puckDot > 0f && Mathf.Abs(puckRB.velocity.z) < 0.05f)) && !inCorner)
                {
                    //score trajectory
                    Vector3 puckCenter = puckTarget.transform.position;
                    Vector3 opposingGoalPosition = new Vector3((table.tableInfo.xBounds.x + table.tableInfo.xBounds.y) / 2f, puckCenter.y, table.tableInfo.zBounds.x);

                    Vector3 myGoalPosition = new Vector3((table.tableInfo.xBounds.x + table.tableInfo.xBounds.y) / 2f, puckCenter.y, table.tableInfo.zBounds.y);

                    Vector3 scoreTrajectory = puckCenter - opposingGoalPosition;
                    Vector3 hitTarget = puckCenter + scoreTrajectory.normalized * 0.03f;
                    Debug.DrawLine(opposingGoalPosition, hitTarget, Color.red);


                    //brainstorming how to compare whether or not the AI would be accidentally knocking the puck into its own goal
                    //if (Vector3.Dot(myGoalPosition - hitTarget, myGoalPosition - rb.transform.position) < 0.1f) ;

                    //move toward it
                    nextPos =  hitTarget;// puckTarget.transform.position;
                }
                else
                {
                    if (inCorner)
                    {
                        nextPos.x = table.tableInfo.xBounds.y / 2f;
                        nextPos.z = table.tableInfo.zBounds.y; /// 2f;
                        nextPos.y = rb.transform.position.y;
                    }
                    else
                    {
                        //be defensive
                        
                        //maybe have some variation in z position
                        nextPos.z = table.tableInfo.zBounds.y;
                        
                        float zPosA = table.tableInfo.zBounds.y/2f; //halfway across the AI board
                        float zPosB = table.tableInfo.zBounds.y; //the end of the AI board
                        float nextZPos = Mathf.Lerp(zPosA, zPosB, Mathf.PerlinNoise(Time.time, Time.time));
                        nextPos.z = nextZPos;

                        float speedNoise = Mathf.PerlinNoise(Time.time, Time.time); //give the ai some natural feeling variation
                        speedModifier *= speedNoise;
                        nextPos.x = puckTarget.transform.position.x;
                        Vector3 puckCenter = puckTarget.transform.position;
                        Vector3 myGoalPosition = new Vector3((table.tableInfo.xBounds.x + table.tableInfo.xBounds.y) / 2f, puckCenter.y, table.tableInfo.zBounds.y);

                        if (Vector3.Distance(rb.transform.position, myGoalPosition) > Vector3.Distance(puckCenter, myGoalPosition)) 
                        {
                            nextPos.z = puckTarget.transform.position.z;
                        }

                    }
                }
                nextPos.z = Mathf.Clamp(nextPos.z, (table.tableInfo.zBounds.y+table.tableInfo.zBounds.x)/2f, table.tableInfo.zBounds.y);
            }
            Vector3 targetPos = Vector3.Lerp(lastPos, nextPos, AirHockeyGlobals.AIStrikerSettings.interpolationSpeed*speedModifier);
            targetPos = table.GetClosestPointInBounds(targetPos);
            //Debug.Log("targetPos " + targetPos);
            rb.MovePosition(targetPos);
        }
        
    }
}
