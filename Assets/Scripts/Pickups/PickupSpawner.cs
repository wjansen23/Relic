using UnityEngine;
using RPG.Saving;

namespace RPG.Inventories
{
    /// <summary>
    /// Spawns pickups that should exist on first load in a level. This
    /// automatically spawns the correct prefab for a given inventory item.
    /// </summary>
    public class PickupSpawner : MonoBehaviour,ISaveable
    {
        [SerializeField] InventoryItem m_Item = null;       //Reference to the Pickup item the spawner spawns

        ///////////////////////////// INTERFACES //////////////////////////////////////////// 
        
        /// <summary>
        /// Restores the state of the object
        /// </summary>
        /// <returns></returns>
        public object CaptureState()
        {
            return IsCollected();
        }

        /// <summary>
        /// Saves the state of the object
        /// </summary>
        /// <param name="state"></param>
        public void RestoreState(object state)
        {
            //Get whether the object collected or not
            bool wasCollected = (bool)state;

            //Now compare the save state to the current state and either spawn or destroy the pickup
            if (wasCollected && !IsCollected())
            {
                DestroyPickup();
            }

            if (!wasCollected && IsCollected())
            {
                SpawnPickup();
            }
        }


        ///////////////////////////// PRIVATE METHODS //////////////////////////////////////////// 
        

        // Start is called before the first frame update
        void Awake()
        {
            SpawnPickup();
        }

        /// <summary>
        /// Spawns the pickup item
        /// </summary>
        private void SpawnPickup()
        {
            var spawnedPickup = m_Item.SpawnPickup(transform.position);
            spawnedPickup.transform.SetParent(transform);
        }

        /// <summary>
        /// Handles destroying the pickup once it is taken
        /// </summary>
        private void DestroyPickup()
        {
            if (GetPickup())
            {
                Destroy(GetPickup().gameObject);
            }
        }

        ///////////////////////////// PUBLIC METHODS //////////////////////////////////////////// 

        /// <summary>
        /// Returns the pickup spawned by this class if it exists.
        /// </summary>
        /// <returns>Returns null if the pickup has been collected.</returns>
        public Pickup GetPickup()
        {
            return GetComponentInChildren<Pickup>();
        }

        /// <summary>
        /// Has the pickup been taken
        /// </summary>
        /// <returns></returns>
        public bool IsCollected()
        {
            return GetPickup() == null;
        }
    }
}