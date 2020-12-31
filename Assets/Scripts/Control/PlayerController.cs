using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using RPG.Attributes;
using System;
using UnityEngine.EventSystems;
using UnityEngine.AI;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] float m_MaxNavMeshProjDist = 1f;   //The distance to search for a nav mesh point for a ray cast

        CharacterMovement m_Mover;                      //Reference to character movement for the Player
        CharacterCombat m_Combat;                       //Reference to character combat for the player
        Health m_Health;                                //Reference to health component;

        //This structure holds the mapping of cursor images to cursor types
        [System.Serializable]
        struct CursorMapping
        {
            public CursorType type;
            public Texture2D image;
            public Vector2 hotspot;
        }

        [SerializeField] CursorMapping[] m_cursorMap = null;

        // Start is called before the first frame update
        void Awake()
        {
            m_Mover = GetComponent<CharacterMovement>();
            m_Combat = GetComponent<CharacterCombat>();
            m_Health = GetComponent<Health>();
        }

        // Update is called once per frame
        void Update()
        {
            //Check if over ui element
            if (InteractWithUI()) return;

            //Check is player is dead
            if (m_Health.IsDead())
            {
                SetCursor(CursorType.None);

                //Make sure the body does not move or rotate
                GetComponent<Rigidbody>().velocity = Vector3.zero;
                GetComponent<Rigidbody>().rotation = Quaternion.identity;
                return;
            }

            if (InteractWithComponent()) return;

            //Check Potential Behavoirs
            if (InteractWithCombat()) return;
            if (InteractWithMovement()) return;

            SetCursor(CursorType.None);
        }

        //This routine handles the player interacting with the UI
        private bool InteractWithUI()
        {
            //Is the mouse over a UI gameobject
            if (EventSystem.current.IsPointerOverGameObject())
            {
                SetCursor(CursorType.UI);
                return true;
            }
            return false;
        }

        //Handles interacting with components
        private bool InteractWithComponent()
        {
            //Get a list of all hits from the raycast
            RaycastHit[] rayHits = RaycastAllSorted();

            foreach (RaycastHit hit in rayHits)
            {
                IRayCastable[] raycastables = hit.transform.GetComponents<IRayCastable>();
                foreach (IRayCastable item in raycastables)
                {
                    if (item.HandleRaycast(this))
                    {
                        SetCursor(item.GetCursorType());
                        return true;
                    }
                }
            }
            return false;
        }

        //This routine handles combat for the player.
        //Returns true if mouse is hovering over a combat target
        private bool InteractWithCombat()
        {
            //Get a list of all hits from the raycast
            RaycastHit[] rayHits = RaycastAllSorted();

            foreach (RaycastHit hit in rayHits)
            {
                IRayCastable[] raycastables = hit.transform.GetComponents<IRayCastable>();
                foreach (IRayCastable item in raycastables)
                {
                    if (item.HandleRaycast(this))
                    {
                        SetCursor(item.GetCursorType());
                        return true;
                    }
                }
            }
            return false;
        }

        //This routine handles movement for the player.
        private bool InteractWithMovement()
        {
            // RaycastHit rayHit;
            ////Holds whether or not the raycast hit something.
            //bool hasHit = Physics.Raycast(GetMouseRay(), out rayHit);

            Vector3 moveLocation;
            bool hasHit = RaycastNavMesh(out moveLocation);

            //Check if mouse is pointing to a movable position
            if (hasHit)
            {
                //Check to see if we can move to target
                if (!m_Mover.CanMoveTo(moveLocation)) return false;

                if (Input.GetMouseButton(0))
                {
                    //Move to destination
                    m_Mover.StartMoveAction(moveLocation);
                }
                SetCursor(CursorType.Movement);
                return true;
            }
            return false;
        }

        //Check to see if the ray cast hits the Nav Mesh and return where it does
        private bool RaycastNavMesh(out Vector3 location)
        {
            //Set default value for out variable
            location = new Vector3();

            RaycastHit rayHit;
            NavMeshHit meshhit;

            //Holds whether or not the raycast hit something.
            bool hasHit = Physics.Raycast(GetMouseRay(), out rayHit);

            if (!hasHit) return false;  //Return if nothing hit

            //Find nearest nav mesh point
            bool foundNMP = NavMesh.SamplePosition(rayHit.point, out meshhit, m_MaxNavMeshProjDist, NavMesh.AllAreas);

            //If no point is found within the distance then return false.
            if (!foundNMP) return false;

            //Update location and return true
            location = meshhit.position;

            return true;
        }

        //Handles changing the type of curosr that is displayed.
        private void SetCursor(CursorType type)
        {
            CursorMapping cursormap = GetCursorMapping(type);
            Cursor.SetCursor(cursormap.image, cursormap.hotspot,CursorMode.Auto);
        }

        //Return curosrmap for sent cursortype
        private CursorMapping GetCursorMapping(CursorType type)
        {
            foreach(CursorMapping item in m_cursorMap)
            {
                if (item.type == type)
                {
                    return item;
                }
            }

            //If type cannot be found then return first item in array
            return m_cursorMap[0];
        }

        //Create a raycast based in the mouse position.
        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }

        //Sort the received hits by their distance to the player
        private RaycastHit[] RaycastAllSorted()
        {
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
            float[] distances = new float[hits.Length];

            //Set distances to each hit
            for (int i=0;i<hits.Length; i++)
            {
                distances[i] = hits[i].distance;
            }

            //sort array by distance.
            Array.Sort(distances, hits);
            return hits;
        }
    }
}
