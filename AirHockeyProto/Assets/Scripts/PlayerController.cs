using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MCS.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        Rigidbody rb;
        [SerializeField]
        Camera rayCam;
        [SerializeField]
        float speed = 2f;

        public int playerIndex;
        public bool isAI;

        public PlayerAvatar avatar;
        public TableController table;

        public Transform GetStriker() { return rb.transform; }

        // Start is called before the first frame update
        void Start()
        {
            if (rb == null) rb = GetComponent<Rigidbody>();
            if (rayCam == null) rayCam = Camera.main;

        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (isAI)
            {
                Vector3 lastPos = rb.transform.position;
                Vector3 nextPos = lastPos;
                if (avatar!=null && avatar.lookObj!=null)
                {
                    Vector3 dir = avatar.lookObj.position - lastPos;
                    Rigidbody puckRB = avatar.lookObj.GetComponent<Rigidbody>();
                    float puckDot = Vector3.Dot(puckRB.velocity.normalized, avatar.transform.forward);
                    if (dir.magnitude < 2f && puckDot < 0f && !table.isPuckInPlayerCorners(playerIndex)) 
                    {
                        //move toward it
                        nextPos =  avatar.lookObj.position;
                    }
                    else 
                    {
                        //be defensive
                        nextPos.z = table.tableInfo.zBounds.y;
                        nextPos.x = avatar.lookObj.position.x;
                    }
                    nextPos.z = Mathf.Clamp(nextPos.z, 0.5f, 4.3f);
                }
                Vector3 targetPos = Vector3.Lerp(lastPos, nextPos, speed);
                targetPos = table.GetClosestPointInBounds(targetPos);
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