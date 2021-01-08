using UnityEngine;
using RPG.Inventories;

namespace RPG.Control
{
    [RequireComponent(typeof(Pickup))]
    public class ClickablePickup : MonoBehaviour, IRayCastable
    {

        Pickup m_Pickup;            //Reference to the pickup object

        ///////////////////////////// INTERFACES //////////////////////////////////////////// 
        
        /// <summary>
        /// Display the appropriate cursor type based upon inventory state
        /// </summary>
        /// <returns></returns>
        public CursorType GetCursorType()
        {
            if (m_Pickup.CanBePickedUp())
            {
                return CursorType.Pickup;
            }
            else
            {
                return CursorType.FullInventory;
            }
        }

        /// <summary>
        /// Define how a raycast is handled
        /// </summary>
        /// <param name="playerCont"></param>
        /// <returns></returns>
        public bool HandleRaycast(PlayerController playerCont)
        {
            if (Input.GetMouseButtonDown(0))
            {
                m_Pickup.PickupItem();
            }
            return true;
        }

        ///////////////////////////// PRIVATE METHODS //////////////////////////////////////////// 


        // Start is called before the first frame update
        void Awake()
        {
            m_Pickup = GetComponent<Pickup>();
        }

        // Update is called once per frame
        void Update()
        {

        }
   
    }
}
