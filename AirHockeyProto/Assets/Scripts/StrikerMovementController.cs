using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MCS.Player
{
    public class StrikerMovementController : MonoBehaviour
    {
        [SerializeField]
        Rigidbody rb;
        [SerializeField]
        Camera rayCam;
        [SerializeField]
        float speed = 2f;

        public int playerIndex;
        public bool isAI;

        public PlayerAvatarIKController avatar;
        public TableController table;
        public PuckController puckTarget;
        [SerializeField]
        float _aiAttackDistance = 2f;

        public Transform GetStriker() { return rb.transform; }

        // Start is called before the first frame update
        void Start()
        {
            if (rb == null) rb = GetComponent<Rigidbody>();
            if (rayCam == null) rayCam = Camera.main;
            if (puckTarget == null) puckTarget = FindObjectOfType<PuckController>();

        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (isAI)
            {
                Vector3 lastPos = rb.transform.position;
                Vector3 nextPos = lastPos;
                float speedModifier = 1f;
                if (puckTarget!=null)
                {
                    Vector3 dir = puckTarget.transform.position - lastPos;
                    Rigidbody puckRB = puckTarget.transform.GetComponent<Rigidbody>();
                    float puckDot = Vector3.Dot(puckRB.velocity.normalized, transform.forward); //instead of its transform.forward, eventually this would be the AvatarIKController
                    //Debug.Log("Magnitude " + dir.magnitude);
                    bool inCorner = table.isPuckInPlayerCorners(playerIndex);
                    if (dir.magnitude < _aiAttackDistance && (puckDot < 0f || (puckDot > 0f && Mathf.Abs(puckRB.velocity.z) < 0.05f )) && !inCorner) 
                    {
                        //score trajectory
                        Vector3 puckCenter = puckTarget.transform.position;
                        Vector3 opposingGoalPosition = new Vector3((table.tableInfo.xBounds.x + table.tableInfo.xBounds.y) / 2f, puckCenter.y, table.tableInfo.zBounds.x);

                        Vector3 scoreTrajectory = puckCenter - opposingGoalPosition;
                        Vector3 hitTarget = puckCenter + scoreTrajectory.normalized * 0.03f;
                        Debug.DrawLine(opposingGoalPosition, hitTarget, Color.red);

                        //move toward it
                        nextPos =  hitTarget;// puckTarget.transform.position;
                    }
                    else 
                    {
                        if (inCorner) 
                        {
                            nextPos.x = table.tableInfo.xBounds.y / 2f;
                            nextPos.z = table.tableInfo.zBounds.y / 2f;
                            nextPos.y = rb.transform.position.y;
                        }
                        else
                        {
                            //be defensive
                            nextPos.z = table.tableInfo.zBounds.y;
                            float speedNoise = Mathf.PerlinNoise(Time.time, Time.time); //give the ai some natural feeling variation
                            speedModifier *= speedNoise;
                            nextPos.x = puckTarget.transform.position.x;
                        }
                    }
                    nextPos.z = Mathf.Clamp(nextPos.z, (table.tableInfo.zBounds.y+table.tableInfo.zBounds.x)/2f, table.tableInfo.zBounds.y);
                }
                Vector3 targetPos = Vector3.Lerp(lastPos, nextPos, speed*speedModifier);
                targetPos = table.GetClosestPointInBounds(targetPos);
                //Debug.Log("targetPos " + targetPos);
                rb.MovePosition(targetPos);
            }
            else 
            {
                if (Input.GetKey(KeyCode.Mouse0))
                {
                    Vector3 mousePos = Input.mousePosition;
                    mousePos.x = Mathf.Clamp(mousePos.x, 0f, Screen.width/2f);
                    Ray r = rayCam.ScreenPointToRay(mousePos);
                    RaycastHit hit;
                    if (Physics.Raycast(r, out hit))
                    {
                        Vector3 lastPos = rb.transform.position;
                        Vector3 nextPos = hit.point;
                        Vector3 targetPos = Vector3.Lerp(lastPos, nextPos, speed);
                        /*
                        targetPos = new Vector3
                            (
                                Mathf.Clamp(targetPos.x, -1.9f, 1.9f),
                                targetPos.y,
                                Mathf.Clamp(targetPos.z, -4.3f, 4.3f)
                            );
                        */
                        targetPos = table.GetClosestPointInBounds(targetPos);
                        rb.MovePosition(targetPos);
                    }
                }
            }
        }
    }
}