using UnityEngine;
using System;
using RPG.Saving;

namespace RPG.Inventories
{
    /// <summary>
    /// Provides storage for the player inventory. A configurable number of
    /// slots are available.
    ///
    /// This component should be placed on the GameObject tagged "Player".
    /// </summary>
    public class Inventory : MonoBehaviour,ISaveable
    {
        [Tooltip("Allowed Size")]
        [SerializeField] int m_InventorySize = 16;      //Maximum number of items that can be in the inventory

        InventoryItem[] m_slots;                        //Array which represents the slots in the inventory

        /// <summary>
        /// Broadcasts when the items in the slots are added/removed.
        /// </summary>
        public event Action inventoryUpdated;

        ///////////////////////////// INTERFACES //////////////////////////////////////////// 

        /// <summary>
        /// Saves the players inventory
        /// </summary>
        /// <returns></returns>
        public object CaptureState()
        {
            //Create a string array to hold the IDs from the players inventory
            var slotStrings = new string[m_InventorySize];

            //Loop through players inventory and store the itemID for each item in the inventory
            for (int i = 0; i < m_InventorySize; i++)
            {
                if (m_slots[i] != null)
                {
                    slotStrings[i] = m_slots[i].GetItemID();
                }
            }

            return slotStrings;
        }

        /// <summary>
        /// Loads player inventory from savefile
        /// </summary>
        /// <param name="state"></param>
        public void RestoreState(object state)
        {
            var slotStrings = (string[])state;

            for (int i = 0; i < m_InventorySize; i++)
            {
                m_slots[i] = InventoryItem.GetFromID(slotStrings[i]);
            }

            if (inventoryUpdated!=null)
            {
                inventoryUpdated();
            }
        }

        ///////////////////////////// PRIVATE METHODS //////////////////////////////////////////// 

        // Start is called before the first frame update
        void Awake()
        {
            m_slots = new InventoryItem[m_InventorySize];
            //Set up the slots array.  THIS IS FOR DEBUGGIN. CHANGE UUID.
            //m_slots[0] = InventoryItem.GetFromID("db4ec621-110b-490f-b18c-7bf427e93c78");
            //m_slots[1] = InventoryItem.GetFromID("a9d3213f-3ce6-4999-aec9-fcd40c882305");
            //m_slots[10] = InventoryItem.GetFromID("954f42be-081d-43be-8d77-772c3af5aaaf");
        }


        /// <summary>
        /// Find a slot that can accomodate the given item.
        /// </summary>
        /// <returns>-1 if no slot is found.</returns>
        private int FindSlot(InventoryItem item)
        {
            return FindEmptySlot();
        }

        /// <summary>
        /// Find an empty slot.
        /// </summary>
        /// <returns>-1 if all slots are full.</returns>
        private int FindEmptySlot()
        {
            for (int i = 0; i < m_InventorySize; i++)
            {
                if (m_slots[i] == null)
                {
                    return i;
                }
            }
            return -1;
        }

        ///////////////////////////// PUBLIC METHODS //////////////////////////////////////////// 

        /// <summary>
        /// Convenience for getting the player's inventory.
        /// </summary>
        /// <returns></returns>
        public static Inventory GetPlayerInventory()
        {
            var player = GameObject.FindWithTag("Player");
            return player.GetComponent<Inventory>();
        }

        /// <summary>
        /// Can this item fit anywhere in the inventory?
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool HasSpaceFor(InventoryItem item)
        {
            return FindSlot(item) >= 0;
        }

        /// <summary>
        /// How many slots are in the inventory?
        /// </summary>
        /// <returns></returns>
        public int GetSize()
        {
            return m_slots.Length;
        }

        /// <summary>
        /// Attempt to add the items to the first available slot.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>Whether or not the item could be added.</returns>
        public bool AddToFirstEmptySlot(InventoryItem item)
        {
            Debug.Log("Add to first slot");
            //Get empty slot
            int i = FindSlot(item);

            //Return if no empty slot is available
            if (i < 0) return false;

            //Upate item at specific slot and update inventory
            m_slots[i] = item;
            if (inventoryUpdated != null)
            {
                inventoryUpdated();
            }

            return true;
        }

        /// <summary>
        /// Is there an instance of the item in the inventory?
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool HasItem(InventoryItem item)
        {
            for (int i = 0; i < m_slots.Length; i++)
            {
                if (object.ReferenceEquals(m_slots[i], item))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Return the item type in the given slot.
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        public InventoryItem GetItemInSlot(int slot)
        {
            return m_slots[slot];
        }

        /// <summary>
        /// Remove the item from the given slot.
        /// </summary>
        /// <param name="slot"></param>
        public void RemoveFromSlot(int slot)
        {
            m_slots[slot] = null;
            if (inventoryUpdated != null)
            {
                inventoryUpdated();
            }
        }

        /// <summary>
        /// Will add an item to the given slot if possible. If there is already
        /// a stack of this type, it will add to the existing stack. Otherwise,
        /// it will be added to the first empty slot.
        /// </summary>
        /// <param name="slot">The slot to attempt to add to.</param>
        /// <param name="item">The item type to add.</param>
        /// <returns>True if the item was added anywhere in the inventory.</returns>
        public bool AddItemToSlot(int slot, InventoryItem item)
        {
            if (m_slots[slot] != null)
            {
                return AddToFirstEmptySlot(item);
            }

            m_slots[slot] = item;

            if (inventoryUpdated != null)
            {
                inventoryUpdated();
            }

            return true;
        }
    }
}
