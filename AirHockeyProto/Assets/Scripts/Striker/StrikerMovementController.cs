using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrikerMovementController : MonoBehaviour
{
    [SerializeField]
    protected Rigidbody rb;
    [SerializeField]
    protected Camera rayCam;
    [SerializeField]
    protected float speed = 2f;

    public int playerIndex;
        
    Collider _rbCollider;

    //public PlayerAvatarIKController avatar;
    public TableController table;
    public PuckController puckTarget;
    public AirHockeyManager airHockeyManager;

    //goalie mode considerations
    public Transform mallet_mesh; //a child of the rigidbody, we can raise and lower it for "goalie" mode without changing physics
    bool _goalieMode = false;
    bool _goalieGrab = false;
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

    public virtual void Reset()
    {
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
    // Update is called once per frame
    private void Update()
    {
        MovementInputBehavior();   
    }
    void FixedUpdate()
    {
        MovementFixedUpdateBehavior();
        GoalieModeUpdateBehavior();
    }


    protected virtual void MovementInputBehavior() { }
    protected virtual void MovementFixedUpdateBehavior() { }


    protected void SetGoalieMode(bool isGoalieMode) 
    {
        _goalieMode = isGoalieMode;
        if(_rbCollider != null)
            _rbCollider.isTrigger = _goalieMode; //now when the puck hits it, we can "grab" it
        mallet_mesh.localPosition = Vector3.up * (_goalieMode ? AirHockeyGlobals.PlayerStrikerSettings.goalieModeLiftAmt : 0f);

        if (_goalieGrab)
            SetGoalieGrab(false);
    }
    protected void SetGoalieGrab(bool isGrabbing) 
    {
        _goalieGrab = isGrabbing;
        puckTarget.Grab(isGrabbing, this); //should just turn on/off "isKinematic."  all other behaviors will be following-based.
    }
        

    protected virtual void GoalieModeUpdateBehavior() 
    {
        if (_goalieMode && _rbCollider && _goalieGrab == false)
        {
            //goalie mode doesn't really control movement of striker, only whether the puck should teather to it
            if (puckTarget != null)
            {
                if (_rbCollider.bounds.Contains(puckTarget.transform.position))
                {
                    SetGoalieGrab(true);
                }
            }
        }
        else if (_goalieMode && _goalieGrab)
        {
            Vector3 grabNodePos = _rbCollider.transform.position + this.transform.forward * _grabObjectForwardOffset;
            puckTarget.GrabMove(grabNodePos);
        }
    }
    float _grabObjectForwardOffset = 0.05f;
}
