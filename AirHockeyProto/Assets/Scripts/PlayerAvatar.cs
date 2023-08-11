using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MCS.Player {
    public class PlayerAvatar : MonoBehaviour
    {
        [SerializeField]
        PlayerController pc;
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
                        animator.SetLookAtWeight(1);
                        animator.SetLookAtPosition(lookObj.position);
                        
                    }

                    // Set the right hand target position and rotation, if one has been assigned
                    if (rightHandObj != null)
                    {
                        //Debug.Log("Attempting to IK...");
                        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                        animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandObj.position);
                        animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandObj.rotation);
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
            if (pc?.GetPuck()) 
            {
                Vector3 targetTorsoPos = new Vector3
                (
                    pc.GetPuck().position.x,
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