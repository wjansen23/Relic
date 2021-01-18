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
        [SerializeField] int m_Number = 1;                  //Number of the item to spawn

        bool m_WasCollected = false;

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
            //Get whether the object collected or not and store in class variable
            //bool wasCollected = (bool)state;
            m_WasCollected = (bool)state;

            //Now compare the save state to the current state and either spawn or destroy the pickup
            if (m_WasCollected && !IsCollected())
            {
                DestroyPickup();
            }

            if (!m_WasCollected && IsCollected())
            {
                SpawnPickup();
            }
        }


        ///////////////////////////// PRIVATE METHODS //////////////////////////////////////////// 


        // Start is called before the first frame update
        private void Awake()
        {
            if (!m_WasCollected)
            {
                SpawnPickup();
            }
        }

        private void Start()
        {

        }

        /// <summary>
        /// Spawns the pickup item
        /// </summary>
        private void SpawnPickup()
        {
            var spawnedPickup = m_Item.SpawnPickup(transform.position, m_Number);
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


        /// <summary>
        /// Returns whether the pickup was collected.  
        /// 
        /// This for instances where IsCollected is not sufficient to check if pickup was taken during
        /// the loading of a save game on game start.
        /// </summary>
        /// <returns></returns>
        public bool WasCollected()
        {
            return m_WasCollected;
        }
    }
}