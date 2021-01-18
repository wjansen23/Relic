using UnityEngine;
using RPG.Inventories;
using RPG.Movement;

namespace RPG.Control
{
    [RequireComponent(typeof(Pickup))]
    public class RunoverPickup : MonoBehaviour, IRayCastable
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
                playerCont.GetComponent<CharacterMovement>().StartMoveAction(transform.position);
            }

            return true;
        }

        ///////////////////////////// PRIVATE METHODS ////////////////////////////////////////////


        // Start is called before the first frame update
        void Awake()
        {
            m_Pickup = GetComponent<Pickup>();
        }

        /// <summary>
        /// Attempts to add item to player when player triggers collider
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter(Collider other)
        {
            //Get reference to pickup spawner
            PickupSpawner spawner = GetComponentInParent<PickupSpawner>();

            //Make sure that the player entered the collider. If so, then pickup
            if (other.gameObject.tag == "Player")
            {
                //This check is required given that if the player quits the game at the spot of a
                //runover pickup and then restarts the game they pickup the item again.  This does
                //not happen if the player reloads while playing only on game start.
                if (!spawner.WasCollected())
                {
                    m_Pickup.PickupItem();
                }
            }
        }
    }
}
