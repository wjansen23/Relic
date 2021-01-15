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

        InventorySlot[] m_slots;                        //Array which represents the slots in the inventory

        public struct InventorySlot                     //Structure that holds the item and the number for the inventory slot
        {
            public InventoryItem item;
            public int number;
        }

        [System.Serializable]
        private struct InventorySlotRecord
        {
            public string itemID;
            public int number;
        }

        /// <summary>
        /// Broadcasts when the inventory has been changed.
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
            var state = new InventorySlotRecord[m_InventorySize];

            //Loop through players inventory and store the itemID for each item in the inventory
            for (int i = 0; i < m_InventorySize; i++)
            {
                if (m_slots[i].item != null)
                {
                    state[i].itemID = m_slots[i].item.GetItemID();
                    state[i].number = m_slots[i].number;
                }
            }
            return state;
        }

        /// <summary>
        /// Loads player inventory from savefile
        /// </summary>
        /// <param name="state"></param>
        public void RestoreState(object state)
        {
            var stateRecords = (InventorySlotRecord[])state;

            for (int i = 0; i < m_InventorySize; i++)
            {
                m_slots[i].item = InventoryItem.GetFromID(stateRecords[i].itemID);
                m_slots[i].number = stateRecords[i].number;
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
            m_slots = new InventorySlot[m_InventorySize];
        }


        /// <summary>
        /// Find a slot that can accomodate the given item.
        /// </summary>
        /// <returns>-1 if no slot is found.</returns>
        private int FindSlot(InventoryItem item)
        {
            //See if item stack exists
            int i = FindStack(item);

            //If a stack doesn't exist then find first empty
            if (i < 0)
            { 
                i=FindEmptySlot(); 
            }

            return i;
        }

        /// <summary>
        /// See if a stake of an item exists.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>return -1 if no stake exists</returns>
        private int FindStack(InventoryItem item)
        {
            //Check to see if the item is stackable
            if (!item.IsStackable()) return -1;

            for (int i = 0; i < m_InventorySize; i++)
            {
                if (object.ReferenceEquals(m_slots[i].item, item))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Find an empty slot.
        /// </summary>
        /// <returns>-1 if all slots are full.</returns>
        private int FindEmptySlot()
        {
            for (int i = 0; i < m_InventorySize; i++)
            {
                if (m_slots[i].item == null)
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
        public bool AddToFirstEmptySlot(InventoryItem item, int number)
        {
            //Get empty slot
            int i = FindSlot(item);

            //Return if no empty slot is available
            if (i < 0) return false;

            //Upate item at specific slot and update inventory
            m_slots[i].item = item;
            m_slots[i].number += number;

            //TODO: Check for max size and handle behavoir

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
            return m_slots[slot].item;
        }

        /// <summary>
        /// Returns the number of items in the inventory slot
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public int GetNumberInSlot(int index)
        {
            return m_slots[index].number;
        }

        /// <summary>
        /// Remove the item from the given slot.
        /// </summary>
        /// <param name="slot"></param>
        public void RemoveFromSlot(int slot, int number)
        {
            //Remove a specific number of the item from the slot
            m_slots[slot].number -= number;

            //If number if <=0 then clear the slot of the item
            if (m_slots[slot].number <= 0)
            {
                m_slots[slot].item = null;
                m_slots[slot].number = 0;
            }

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
        public bool AddItemToSlot(int slot, InventoryItem item, int number)
        {
            //TODO: Check for max size and handle behavoir

            //IF the slot we are attempt to add to has the item and its stackable then add to that slot.
            if (object.ReferenceEquals(m_slots[slot].item, item)&& item.IsStackable())
            {
                m_slots[slot].number += number;
                inventoryUpdated();
                return true;
            }

            //if the slot we are trying to add the item too is not null then add to first available
            //This is for items that do not stack.
            if (m_slots[slot].item != null)
            {
                 return AddToFirstEmptySlot(item,number);
            }

            //Slot is empty so add the item.
            m_slots[slot].item = item;
            m_slots[slot].number += number;

            if (inventoryUpdated != null)
            {
                inventoryUpdated();
            }

            return true;
        }

    }
}
