using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Control
{
    public class PatrolPath : MonoBehaviour
    {
        //Size of waypoint spheres
        const float m_wayPointRadius = .3f;        

        //Visualizes the patrol path within Unity Scene editor
        private void OnDrawGizmos()
        {
            Vector3 nextPoint;
            Vector3 curPoint;

            for (int i = 0; i < transform.childCount; i++)
            {
                curPoint = GetWaypointPosition(i);
                nextPoint = GetWaypointPosition(GetNextWaypointIndex(i));
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(curPoint, m_wayPointRadius);
                if (curPoint != nextPoint)
                {
                    Gizmos.DrawLine(curPoint, nextPoint);
                }
            }
        }

        ///////////////////////////// PUBLIC METHODS ////////////////////////////////////////////        
        
        //Returns the position vector of a way point based on child index
        public Vector3 GetWaypointPosition(int i)
        {
            return transform.GetChild(i).position;
        }

        //Returns the index of the next waypoint based on child order
        public int GetNextWaypointIndex(int index)
        {
            if (index >= transform.childCount - 1)
            {
                return 0;
            }
            else
            {
                return index + 1;
            }
        }
    }
}
