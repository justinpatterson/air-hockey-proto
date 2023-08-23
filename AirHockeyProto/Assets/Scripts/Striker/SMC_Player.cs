using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.InputSystem;

public class SMC_Player : StrikerMovementController
{
    bool _inControl = false;
    float _inControlStrikerMaxPixelDistance = 15f;


    //public InputActionAsset actions; 
    //private InputAction moveAction;
    private void Awake()
    {
        //moveAction = actions.FindActionMap("gameplay").FindAction("move");
    }
    protected override void MovementFixedUpdateBehavior()
    {
        base.MovementFixedUpdateBehavior();
        if ( /*_inControl && */ Input.GetKey(KeyCode.Mouse0)) //inControl is kind of finnicky when you have to defend against AI serving (at least if you remove "in control" when resetting the position after a goal)
        {
            //MOUSE MOVEMENT
            Vector3 mousePos = Input.mousePosition;
            mousePos.x = Mathf.Clamp(mousePos.x, 0f, Screen.width); 
            Ray r = rayCam.ScreenPointToRay(mousePos);
            RaycastHit hit;
            if (Physics.Raycast(r, out hit))
            {
                Vector3 lastPos = rb.transform.position;
                Vector3 nextPos = hit.point;
                Vector3 targetPos = Vector3.Lerp(lastPos, nextPos, AirHockeyGlobals.StrikerSettings.interpolationSpeed);
                if (airHockeyManager.currentPhase == AirHockeyManager.GamePhases.Serve && !airHockeyManager.IsValidServe(playerIndex))
                {
                    targetPos.z = Mathf.Clamp(targetPos.z, table.tableInfo.zBounds.x, table.tableInfo.zBounds.x/2f); //eventually this should be based on the team their own if there's players on both sides
                }
                else 
                {
                    targetPos.z = Mathf.Clamp(targetPos.z, table.tableInfo.zBounds.x, (table.tableInfo.zBounds.x + table.tableInfo.zBounds.y) / 2f);
                }
                targetPos = table.GetClosestPointInBounds(targetPos);
                rb.MovePosition(targetPos);
            }
        }

        //INPUT SYSTEM MOVEMENT
        /*
        else { 
            Vector2 moveVector = moveAction.ReadValue<Vector2>();
            Vector3 lastPos = rb.transform.position;
            Vector3 right = Camera.main.transform.right;
            right.y = 0f;
            Vector3 forward = Camera.main.transform.forward;
            forward.y = 0f;

            Vector3 nextPos = lastPos + (right * (moveVector.x) + forward * moveVector.y) * Time.deltaTime * AirHockeyGlobals.PlayerStrikerSettings.inputModifierSpeed;

            Vector3 targetPos = Vector3.Lerp(lastPos, nextPos, AirHockeyGlobals.PlayerStrikerSettings.interpolationSpeed);
            //targetPos = nextPos; (no lerping)
            if (airHockeyManager.currentPhase == AirHockeyManager.GamePhases.Serve && !airHockeyManager.IsValidServe(playerIndex))
            {
                targetPos.z = Mathf.Clamp(targetPos.z, table.tableInfo.zBounds.x, table.tableInfo.zBounds.x/2f); //eventually this should be based on the team their own if there's players on both sides
            }
            else
            {
                targetPos.z = Mathf.Clamp(targetPos.z, table.tableInfo.zBounds.x, (table.tableInfo.zBounds.x + table.tableInfo.zBounds.y) / 2f);
            }
            targetPos = table.GetClosestPointInBounds(targetPos);
            rb.MovePosition(targetPos);
        }
        */
    }
    protected override void MovementInputBehavior()
    {
        base.MovementInputBehavior();
        //inControl sets it so you have to click on the paddle first before gaining control.  
        //new input cases

        //Vector2 moveVector = moveAction.ReadValue<Vector2>();
        if (!_inControl)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Vector3 mousePos = Input.mousePosition;
                Vector3 strikerPos = Camera.main.WorldToScreenPoint(rb.transform.position);
                strikerPos.z = mousePos.z; //I think I remember Cameras doing something silly with z
                float dist = Vector3.Distance(mousePos, strikerPos);
                if (dist < _inControlStrikerMaxPixelDistance) { _inControl = true; }
                else Debug.Log("NOT CLOSE ENOUGH");
            }
            else
            {
                /*
                if (moveVector.magnitude > 0.01f)
                {
                    _inControl = true;
                }
                */
            }
            //else if (InputSystem.) { }
        }
        else if (_inControl) {
            if (Input.GetKeyUp(KeyCode.Mouse0)) { _inControl = false; } 
            /*
            else if(moveVector.magnitude <= 0.01f) { _inControl = false; }
            */
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            SetGoalieMode(true);
        }
        else if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            SetGoalieMode(false);
        }
    }
    public override void Reset()
    {
        _inControl = false;
        base.Reset();
    }
    /*
    void OnEnable()
    {
        actions.FindActionMap("gameplay").Enable();
    }
    void OnDisable()
    {
        actions.FindActionMap("gameplay").Disable();
    }
    */
}
