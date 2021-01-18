using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using RPG.Attributes;
using System;
using UnityEngine.EventSystems;
using UnityEngine.AI;

namespace RPG.Control
{
    /// <summary>
    /// This class handles all player controls and actions
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] float m_MaxNavMeshProjDist = 1f;   //The distance to search for a nav mesh point for a ray cast

        CharacterMovement m_Mover;                      //Reference to character movement for the Player
        CharacterCombat m_Combat;                       //Reference to character combat for the player
        Health m_Health;                                //Reference to health component;
        bool m_DraggingUI = false;                      //Are we dragging a UI component

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

        /// <summary>
        /// Update is called once per frame. 
        /// Performs a series of checks against different actions the player can take
        /// </summary>

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

        /// <summary>
        /// This routine handles the player interacting with the UI 
        /// </summary>
        /// <returns></returns>
        private bool InteractWithUI()
        {
            if (Input.GetMouseButtonUp(0)||Input.GetMouseButtonUp(1))
            {
                m_DraggingUI = false;
            }

            //Is the mouse over a UI gameobject
            if (EventSystem.current.IsPointerOverGameObject())
            {
                if (Input.GetMouseButtonDown(0)||Input.GetMouseButtonDown(1))
                {
                    m_DraggingUI = true;
                }
                SetCursor(CursorType.UI);
                return true;
            }

            if (m_DraggingUI)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Handles interacting with components and world objcts that can be interacted with
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// This routine handles combat for the player.
        /// Returns true if mouse is hovering over a combat target
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// This routine handles movement for the player.
        /// </summary>
        /// <returns></returns>
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

                if (Input.GetMouseButton(1))
                {
                    //Move to destination
                    m_Mover.StartMoveAction(moveLocation);
                }
                SetCursor(CursorType.Movement);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Check to see if the ray cast hits the Nav Mesh and return where it does
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Handles changing the type of curosr that is displayed.
        /// </summary>
        /// <param name="type"></param>
        private void SetCursor(CursorType type)
        {
            CursorMapping cursormap = GetCursorMapping(type);
            Cursor.SetCursor(cursormap.image, cursormap.hotspot,CursorMode.Auto);
        }

        /// <summary>
        /// Return curosrmap for sent cursortype
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Create a raycast based in the mouse position.
        /// </summary>
        /// <returns></returns>
        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }

        /// <summary>
        /// Sort the received hits by their distance to the player
        /// </summary>
        /// <returns></returns>
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
