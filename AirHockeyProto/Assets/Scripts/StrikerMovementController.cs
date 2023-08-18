using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        float _aiAttackDistance = 2f;
        float _aiServeCountdown = 1f;
        Collider _rbCollider;

        public PlayerAvatarIKController avatar;
        public TableController table;
        public PuckController puckTarget;
        public AirHockeyManager airHockeyManager;
        public Transform mallet_mesh; //a child of the rigidbody, we can raise and lower it for "goalie" mode without changing physics
        public float goalieModeLiftAmt = 0.02f;
        public Transform GetStriker() { return rb.transform; }

        // Start is called before the first frame update
        void Start()
        {
            if (rb == null) rb = GetComponent<Rigidbody>();
            if(rb != null && _rbCollider == null) { _rbCollider = rb.GetComponent<Collider>(); }
            if (rayCam == null) rayCam = Camera.main;
            if (puckTarget == null) puckTarget = FindObjectOfType<PuckController>();
            if (airHockeyManager == null) airHockeyManager = FindObjectOfType<AirHockeyManager>();

        }

        public void Reset()
        {
            _inControl = false;
            _aiServeCountdown = 1f;
            SetGoalieMode(false);
            if (!airHockeyManager)
                return;

            int teamIndex = airHockeyManager.GetTeamIndexForPlayer(playerIndex);
            int playerCount = (teamIndex == 0) ? airHockeyManager.teamModel.teamL.playerIDs.Count : airHockeyManager.teamModel.teamR.playerIDs.Count; 
            //based on player count, eventually we should position players differently
            switch (teamIndex) 
            {
                case 0:
                    if(playerCount == 1)
                    {
                        float x = (table.tableInfo.xBounds.x + table.tableInfo.xBounds.y) / 2f; 
                        float z = table.tableInfo.zBounds.x; 
                        rb.MovePosition(new Vector3(x, rb.position.y, z));
                    }
                    else if (playerCount == 2) { } //not yet supported
                    //else... there really shouldn't be 3+ people on one side
                    break;
                case 1:
                    if (playerCount == 1)
                    {
                        float x = (table.tableInfo.xBounds.x + table.tableInfo.xBounds.y) / 2f;
                        float z = table.tableInfo.zBounds.y;
                        rb.MovePosition(new Vector3(x, rb.position.y, z));
                    }
                    else if (playerCount == 2) { } //not yet supported
                    //else... there really shouldn't be 3+ people on one side
                    break;
                default:
                    //players 2 and 3 don't exist yet
                    break;
            }
        }
        bool _inControl = false;
        float _strikerInteractionThreshold = 15f;
        bool _goalieMode = false;
        bool _goalieGrab = false;
        // Update is called once per frame
        private void Update()
        {
            if (!isAI)
            {
                //inControl sets it so you have to click on the paddle first before gaining control.  
                if (!_inControl && Input.GetKeyDown(KeyCode.Mouse0))
                {
                    Vector3 mousePos = Input.mousePosition;
                    Vector3 strikerPos = Camera.main.WorldToScreenPoint(rb.transform.position);
                    strikerPos.z = mousePos.z; //I think I remember Cameras doing something silly with z
                    float dist = Vector3.Distance(mousePos, strikerPos);
                    if (dist < _strikerInteractionThreshold) { _inControl = true; }
                }
                else if (_inControl && Input.GetKeyUp(KeyCode.Mouse0)) { _inControl = false; }

                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    SetGoalieMode(true);
                }
                else if (Input.GetKeyUp(KeyCode.Mouse1))
                {
                    SetGoalieMode(false);
                }
            }
        }

        protected void SetGoalieMode(bool isGoalieMode) 
        {
            _goalieMode = isGoalieMode;
            if(_rbCollider != null)
                _rbCollider.isTrigger = _goalieMode; //now when the puck hits it, we can "grab" it
            mallet_mesh.localPosition = Vector3.up * (_goalieMode ? goalieModeLiftAmt : 0f);

            if (_goalieGrab)
                SetGoalieGrab(false);
        }
        protected void SetGoalieGrab(bool isGrabbing) 
        {
            _goalieGrab = isGrabbing;

            puckTarget.Grab(isGrabbing, this); //should just turn on/off "isKinematic."  all other behaviors will be following-based.
        }
        
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

                    if(airHockeyManager.currentPhase == AirHockeyManager.GamePhases.Serve && airHockeyManager.IsValidServe(playerIndex)) 
                    {
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
                    else if(airHockeyManager.currentPhase == AirHockeyManager.GamePhases.InProgress) 
                    {
                        float puckDot = Vector3.Dot(puckRB.velocity.normalized, this.transform.forward); //"this" should be rotated to have forward be appropriate for player.
                        //Debug.Log("Magnitude " + dir.magnitude);
                        bool inCorner = table.isPuckInPlayerCorners(playerIndex);
                        if (dir.magnitude < _aiAttackDistance && (puckDot < 0f || (puckDot > 0f && Mathf.Abs(puckRB.velocity.z) < 0.05f )) && !inCorner) 
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
            }
            else 
            {
                
                if ( /*_inControl && */ Input.GetKey(KeyCode.Mouse0)) //inControl is kind of finnicky when you have to defend against AI serving (at least if you remove "in control" when resetting the position after a goal)
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

            if (_goalieMode && _rbCollider && _goalieGrab == false) 
            {
                //goalie mode doesn't really control movement of striker, only whether the puck should teather to it
                if(puckTarget != null) 
                {
                    if( _rbCollider.bounds.Contains(puckTarget.transform.position) ) 
                    {
                        SetGoalieGrab(true);
                    }
                }
            }
            else if (_goalieMode && _goalieGrab) 
            {
                Vector3 grabNodePos = _rbCollider.transform.position + this.transform.forward * 1.1f;
                puckTarget.GrabMove(grabNodePos);
            }
        }
    }
