using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMC_Player : StrikerMovementController
{
    bool _inControl = false;
    float _inControlStrikerMaxPixelDistance = 15f;


    protected override void MovementFixedUpdateBehavior()
    {
        base.MovementFixedUpdateBehavior();
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
    protected override void MovementInputBehavior()
    {
        base.MovementInputBehavior();
        //inControl sets it so you have to click on the paddle first before gaining control.  
        if (!_inControl && Input.GetKeyDown(KeyCode.Mouse0))
        {
            Vector3 mousePos = Input.mousePosition;
            Vector3 strikerPos = Camera.main.WorldToScreenPoint(rb.transform.position);
            strikerPos.z = mousePos.z; //I think I remember Cameras doing something silly with z
            float dist = Vector3.Distance(mousePos, strikerPos);
            if (dist < _inControlStrikerMaxPixelDistance) { _inControl = true; }
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
    public override void Reset()
    {
        _inControl = false;
        base.Reset();
    }
}
