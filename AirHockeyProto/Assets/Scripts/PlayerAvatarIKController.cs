using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MCS.Player {
    public class PlayerAvatarIKController : MonoBehaviour
    {
        [SerializeField]
        StrikerMovementController pc;
        [SerializeField]
        Transform avatar_torso;
        [SerializeField]
        float speed = 2f;

        [SerializeField]
        protected Animator animator;
        public bool ikActive = false;
        [SerializeField]
        public Transform rightHandObj = null;
        [SerializeField]
        public Transform lookObj = null;


        //a callback for calculating IK
        void OnAnimatorIK()
        {
            if (animator)
            {
                //Debug.Log("Valid Animator...");
                //if the IK is active, set the position and rotation directly to the goal.
                if (ikActive)
                {

                    //Debug.Log("Active IK...");
                    // Set the look target position, if one has been assigned
                    if (lookObj != null)
                    {
                        /*
                        animator.SetLookAtWeight(1);
                        animator.SetLookAtPosition(lookObj.position);
                        */
                    }

                    // Set the right hand target position and rotation, if one has been assigned
                    if (rightHandObj != null)
                    {
                        //Debug.Log("Attempting to IK...");

                        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                        animator.SetIKPosition(AvatarIKGoal.LeftHand, rightHandObj.position);
                        animator.SetIKRotation(AvatarIKGoal.LeftHand, rightHandObj.rotation);

                        /* //option 1, some blend of rotation calcs between up/down and left/right movement
                        float strikerZ = rightHandObj.transform.position.z;
                        strikerZ = Mathf.Abs(strikerZ); //the farther from 0, the closer to Rotation Y of 0f; 
                        float strikerX = rightHandObj.transform.position.x;
                        float maxCenterDistanceZ = 0.36f; //get from table info 
                        float maxCenterDistanceX = 0.18f; //get from table info
                        float targetYRot = Mathf.Lerp(0f,-90f, (maxCenterDistanceZ - strikerZ)/(0.36f));
                        strikerX = Mathf.Clamp(strikerX, -maxCenterDistanceX, 0f); //Debug.Log(strikerX);
                        float xPercent = Mathf.Abs(strikerX/maxCenterDistanceX); Debug.Log(xPercent);
                        targetYRot = Mathf.Min(targetYRot, Mathf.Lerp(0f, -90f, xPercent));
                        transform.localRotation = Quaternion.Euler(0f, targetYRot, 0f);
                        */

                        //transform.right = (rightHandObj.transform.position - transform.position).normalized;

                    }

                }

                //if the IK is not active, set the position and rotation of the hand and head back to the original position
                else
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
                    animator.SetLookAtWeight(0);
                }
            }
        }


        private void Update()
        {
         
            /*
            if (pc?.puckTarget) 
            {
                Vector3 targetTorsoPos = new Vector3
                (
                    pc.puckTarget.transform.position.x,
                    avatar_torso.position.y,
                    avatar_torso.position.z
                );
                Vector3 nextTorsoPos = Vector3.Lerp(avatar_torso.position, targetTorsoPos, speed);
                avatar_torso.position = nextTorsoPos;
            }
            */
        }
    }
}