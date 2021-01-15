using System;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;

namespace RPG.Inventories
{
    /// <summary>
    /// Provides a store for the items equipped to a player. Items are stored by
    /// their equip locations.
    /// 
    /// This component should be placed on the GameObject tagged "Player".
    /// </summary>
    public class Equipment : MonoBehaviour, ISaveable
    {
        //Stores all equipped items by location
        Dictionary<EquipLocation, EquipableItem> m_EquippedItems = new Dictionary<EquipLocation, EquipableItem>();

        /// <summary>
        /// Broadcasts when the equipment on the character has changed
        /// </summary>
        public event Action equipmentUpdated;

        ///////////////////////////// INTERFACES //////////////////////////////////////////// 

        /// <summary>
        /// Captures the players currently equipped items
        /// </summary>
        /// <returns></returns>
        public object CaptureState()
        {
            var state = new Dictionary<EquipLocation, string> ();

            foreach (var item in m_EquippedItems)
            {
                state[item.Key] = item.Value.GetItemID();
            }

            return state;
        }

        /// <summary>
        /// Restores the equipment state of the player on loading a save.
        /// </summary>
        /// <param name="state"></param>
        public void RestoreState(object state)
        {
            //Create the equipment dictionary
            m_EquippedItems = new Dictionary<EquipLocation, EquipableItem>();

            //Load state data
            Dictionary<EquipLocation, string> stateDict = (Dictionary<EquipLocation, string>)state;

            //Iterate through state data and equipped items to the player.
            foreach (var pair in stateDict)
            {
                var equippeditem = (EquipableItem)InventoryItem.GetFromID(pair.Value);

                if (equippeditem != null)
                {
                    m_EquippedItems[pair.Key] = equippeditem;
                }
            }
        }

        ///////////////////////////// PUBLIC METHODS //////////////////////////////////////////// 

        /// <summary>
        /// returns the item in the request equipment slot
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public EquipableItem GetItemInSlot(EquipLocation location)
        {
            //Check if the sent location is valid
            if (!m_EquippedItems.ContainsKey(location)) return null;

            return m_EquippedItems[location];
        }


        /// <summary>
        /// Attempts to add item to the request equipment slot
        /// </summary>
        /// <param name="item"></param>
        /// <param name="location"></param>
        public void AddItem(EquipableItem item, EquipLocation location)
        {
            //Check if the item can go into the requested slot
            Debug.Assert(item.GetAllowedEquipLocation() == location);

            //Equip the item
            m_EquippedItems[location] = item;
            
            if (equipmentUpdated != null)
            {
                equipmentUpdated();
            }
        }

        /// <summary>
        /// Remove the item for the given slot.
        /// </summary>
        public void RemoveItem(EquipLocation location)
        {
            //Check if the sent location is valid
            if (!m_EquippedItems.ContainsKey(location)) return;

            m_EquippedItems.Remove(location);

            if (equipmentUpdated != null)
            {
                equipmentUpdated();
            }
        }

        /// <summary>
        /// Enumerate through all the slots that currently contain items.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<EquipLocation> GetAllPopulatedSlots()
        {
            return m_EquippedItems.Keys;
        }

    }
}
