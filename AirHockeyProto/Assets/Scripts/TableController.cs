using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableController : MonoBehaviour
{
    [SerializeField]
    public TableInfo tableInfo;
    [System.Serializable]
    public struct TableInfo 
    {
        public Vector2 xBounds;
        public Vector2 zBounds;
    }

    public Collider arenaCollider;
    public Transform puck;
    public float wallDistanceThreshold = 0.05f;
    public float wallPushStrength = 1f;

    private void FixedUpdate()
    {
        Vector3 outNormal = Vector3.zero;
        if (IsPuckOnAnyWall(out outNormal)) 
        {
            //Debug.Log("WALL - " + outNormal);
            PuckForceBounce(outNormal);
        }
        /*
        Vector3 closestPos = arenaCollider.ClosestPoint(puck.position);
        closestPos.y = puck.position.y;
        puck.position = closestPos;
        */
    }
    public Vector3 GetClosestPointInBounds(Vector3 target) 
    {
        return new Vector3
        (
            Mathf.Clamp(target.x, tableInfo.xBounds.x, tableInfo.xBounds.y),
            target.y,
            Mathf.Clamp(target.z, tableInfo.zBounds.x, tableInfo.zBounds.y)
        );
    }

    bool IsPuckOnWall(Vector3 wallCenter, Vector3 wallNormal) 
    {
        Vector3 dir = puck.transform.position - wallCenter;
        float dot = Vector3.Dot(dir, wallNormal);
        if ((dot) < wallDistanceThreshold)
            return true;
        return false;
    }
    public bool IsPuckOnAnyWall(out Vector3 wallNormal)
    {
        wallNormal = Vector3.zero;
        
        //left
        Vector3 left_wall = new Vector3(
            (tableInfo.xBounds.x + tableInfo.xBounds.y) /2f,
            0f,
            tableInfo.zBounds.x
        );
        
        wallNormal =new Vector3(0f, 0f, 1f);
        if (IsPuckOnWall(left_wall, wallNormal)) 
        {
            return true;
        }

        //right
        Vector3 right_wall = new Vector3(
             (tableInfo.xBounds.x + tableInfo.xBounds.y) /2f,
             0f,
             tableInfo.zBounds.y
        );
        
        wallNormal =new Vector3(0f, 0f, -1f); 
        if (IsPuckOnWall(right_wall, wallNormal))
        {
            return true;
        }
        
        //top
        Vector3 top_wall = new Vector3(
             tableInfo.xBounds.x,
             0f,
             (tableInfo.zBounds.x + tableInfo.zBounds.y) /2f
         );
        //top_wall = new Vector3(2f,0f,0f);
        wallNormal = new Vector3(1f, 0f, 0f);
        if (IsPuckOnWall(top_wall, wallNormal))
        {
            return true;
        }

        //bottom
        Vector3 bottom_wall = new Vector3(
             tableInfo.xBounds.y,
             0f,
             (tableInfo.zBounds.x + tableInfo.zBounds.y) /2f
         );
        //bottom_wall = new Vector3(2f, 0f, 0f);
        wallNormal = new Vector3(-1f,0f,0f);
        if (IsPuckOnWall(bottom_wall, wallNormal))
        {
            return true;
        }
        return false;
    }

    public bool isPuckInPlayerCorners(int playerIndex) 
    {

        Vector3 wallNormal = Vector3.zero;

        //left
        Vector3 left_wall = new Vector3(
            (tableInfo.xBounds.x + tableInfo.xBounds.y) /2f,
            0f,
            tableInfo.zBounds.x
        );
        //left_wall = new Vector3(0, 0, -5.1f);
        wallNormal =new Vector3(0f, 0f, 1f);
        bool b_l = (IsPuckOnWall(left_wall, wallNormal)) ;
        //right
        Vector3 right_wall = new Vector3(
             (tableInfo.xBounds.x + tableInfo.xBounds.y) /2f,
             0f,
             tableInfo.zBounds.y
        );
        //right_wall = new Vector3(0, 0, 5.1f);
        wallNormal =new Vector3(0f, 0f, -1f);
        bool b_r = (IsPuckOnWall(right_wall, wallNormal));
        
        //top
        Vector3 top_wall = new Vector3(
             tableInfo.xBounds.x,
             0f,
             (tableInfo.zBounds.x + tableInfo.zBounds.y) /2f
         );
        //top_wall = new Vector3(2f,0f,0f);
        wallNormal = new Vector3(1f, 0f, 0f);
        bool b_t = (IsPuckOnWall(top_wall, wallNormal));

        //bottom
        Vector3 bottom_wall = new Vector3(
             tableInfo.xBounds.y,
             0f,
             (tableInfo.zBounds.x + tableInfo.zBounds.y) /2f
         );
        //bottom_wall = new Vector3(2f, 0f, 0f);
        wallNormal = new Vector3(-1f, 0f, 0f);
        bool b_b = (IsPuckOnWall(bottom_wall, wallNormal));
        
        if(playerIndex == 0) 
            return (b_b && b_l) || (b_t && b_l);
        else
            return (b_b && b_r) || (b_t && b_r);
    }

    void PuckForceBounce(Vector3 forceDir)
    {
        puck.GetComponent<Rigidbody>().AddForce(forceDir * Time.deltaTime * wallPushStrength);//puck.position + (forceDir * Time.deltaTime));
    }

    private void OnDrawGizmos()
    {
        Vector3 start = Vector3.zero;
        Vector3 finish = Vector3.zero;
        for(int i = 0; i < 4; i++) 
        {
            switch (i) 
            {
                case 0: //back
                    start.x = tableInfo.xBounds.x;
                    start.z = tableInfo.zBounds.x;
                    finish.x = tableInfo.xBounds.x;
                    finish.z = tableInfo.zBounds.y;
                    break;
                case 1: //right
                    start.x = tableInfo.xBounds.y;
                    start.z = tableInfo.zBounds.y;
                    finish.x = tableInfo.xBounds.x;
                    finish.z = tableInfo.zBounds.y;
                    break;
                case 2: //front
                    start.x = tableInfo.xBounds.y;
                    start.z = tableInfo.zBounds.x;
                    finish.x = tableInfo.xBounds.y;
                    finish.z = tableInfo.zBounds.y;
                    break;
                case 3:
                    start.x = tableInfo.xBounds.x;
                    start.z = tableInfo.zBounds.x;
                    finish.x = tableInfo.xBounds.x;
                    finish.z = tableInfo.zBounds.y;
                    break;
            }
            start.y = 0.36f;
            finish.y = 0.36f;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(start, finish);
        }
    }
}
