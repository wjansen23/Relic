using UnityEngine;

namespace RPG.Inventories
{
    /// <summary>
    /// To be placed at the root of a Pickup prefab. Contains the data about the
    /// pickup such as the type of item and the number.
    /// </summary>
    public class Pickup : MonoBehaviour
    {
        int m_Number;                           //Number of item in pickup
        InventoryItem m_Item = null;            //Reference to the item this pickup is for
        Inventory m_PlayerInventory;            //Reference to the players inventory

        // Start is called before the first frame update
        void Awake()
        {
            //Get reference to player inventory
            var player = GameObject.FindGameObjectWithTag("Player");
            m_PlayerInventory = player.GetComponent<Inventory>();
        }

        ///////////////////////////// PUBLIC METHODS //////////////////////////////////////////// 

        /// <summary>
        /// Set the vital data after creating the prefab.
        /// </summary>
        /// <param name="item">The type of item this prefab represents.</param>
        public void Setup(InventoryItem item, int number)
        {
            m_Item = item;
            m_Number = number;
        }

        /// <summary>
        /// Returns the associated item prefab
        /// </summary>
        /// <returns></returns>
        public InventoryItem GetItem()
        {
            return m_Item;
        }

        /// <summary>
        /// Attempts to pickup the item and add to player inventory
        /// </summary>
        public void PickupItem()
        {
            //Try to add to inventory. If successful then destroy gameobject.
            bool wasAdded = m_PlayerInventory.AddToFirstEmptySlot(m_Item,m_Number);
            if (wasAdded)
            {
                //Get reference to pickup spawner is it exists and tell it the pick has been collected
                PickupSpawner spawner = GetComponentInParent<PickupSpawner>();
                if (spawner != null) spawner.WasCollected(true);

                Destroy(gameObject);
            }
        }

        /// <summary>
        /// returns whether there is space for the item or not.
        /// </summary>
        /// <returns></returns>
        public bool CanBePickedUp()
        {
            return m_PlayerInventory.HasSpaceFor(m_Item);
        }

        /// <summary>
        /// Returns the number of items for the pickup
        /// </summary>
        public int  GetNumber()
        {
            return m_Number;
        }
    }
}
