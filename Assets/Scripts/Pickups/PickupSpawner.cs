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
            //Debug.Log("Save Pick " + this.name + "(" + m_WasCollected + ")");
            return m_WasCollected;
        }

        /// <summary>
        /// Saves the state of the object
        /// </summary>
        /// <param name="state"></param>
        public void RestoreState(object state)
        {
            //Get whether the object collected or not and store in class variable
            m_WasCollected = (bool)state;
            //Debug.Log("Restore Pick " + this.name + "(" + m_WasCollected + ")");

            //Now compare the save state to the current state and either spawn or destroy the pickup
            if (m_WasCollected && AlreadySpawned())
            {
                DestroyPickup();
            }

            if (!m_WasCollected && !AlreadySpawned())
            {
                SpawnPickup();
            }
        }


        ///////////////////////////// PRIVATE METHODS //////////////////////////////////////////// 


        // Start is called before the first frame update
        private void Awake()
        {

        }

        private void Start()
        {
            if (!m_WasCollected)
            {
                SpawnPickup();
            }
        }

        /// <summary>
        /// Spawns the pickup item
        /// </summary>
        private void SpawnPickup()
        {
            //Debug.Log("Spawn Pick " + this.name + "(" + m_WasCollected + ")");
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

        private bool AlreadySpawned()
        {
            return GetPickup() == null;
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
        /// Returns whether the pickup has been taken
        /// </summary>
        /// <returns></returns>
        public bool IsCollected()
        {
            return m_WasCollected;
        }


        /// <summary>
        /// Sets whether the pickup has been collected  
        /// </summary>
        /// <returns></returns>
        public void WasCollected(bool flag)
        {
            m_WasCollected=flag;
        }

    }
}