using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;
using System;

namespace RPG.Inventories
{
    /// <summary>
    /// Provides the storage for an action bar. The bar has a finite number of
    /// slots that can be filled and actions in the slots can be "used".
    /// 
    /// This component should be placed on the GameObject tagged "Player".
    /// </summary>
    public class ActionStore : MonoBehaviour, ISaveable
    {
        Dictionary<int, DockedItemSlot> m_DockedItems = new Dictionary<int, DockedItemSlot>();      //A collection  of actions by action slot number

        //Holds what action is stored in what slot
        private class DockedItemSlot
        {
            public ActionItem actionitem;
            public int number;
        }

        public event Action actionStoreUpdated; //Event that causes the action bar to be updated

        /// <summary>
        /// This is a record used in saving/loading action items.
        /// </summary>
        [System.Serializable]
        private struct DockedItemRecord
        {
            public string itemID;
            public int number;
        }

        ///////////////////////////// INTERFACES //////////////////////////////////////////// 

        /// <summary>
        /// Captures the current state of the players action items.
        /// </summary>
        /// <returns></returns>
        public object CaptureState()
        {
            var state = new Dictionary<int, DockedItemRecord>();
            foreach(var pair in m_DockedItems)
            {
                var record = new DockedItemRecord();
                record.itemID = pair.Value.actionitem.GetItemID();
                record.number = pair.Value.number;
                state[pair.Key] = record;
            }

            return state;
        }

        /// <summary>
        /// Restore the state of the players action items.
        /// </summary>
        /// <param name="state"></param>
        public void RestoreState(object state)
        {
            //Recreate the docked item list.  This is to avoid is item multiplication when restoring during game play
            //m_DockedItems = new Dictionary<int, DockedItemSlot>();
            if (m_DockedItems != null)
            {
                m_DockedItems.Clear();
            }
            else
            {
                m_DockedItems = new Dictionary<int, DockedItemSlot>();
            }

            //Load the saved state
            var stateDict = (Dictionary<int, DockedItemRecord>)state;

            foreach (var pair in stateDict)
            {
                //AddAction(InventoryItem.GetFromID(pair.Value.itemID), pair.Key, pair.Value.number);

                //Create a new record and store.
                var slot = new DockedItemSlot();
                slot.actionitem = InventoryItem.GetFromID(pair.Value.itemID) as ActionItem;
                slot.number = pair.Value.number;
                m_DockedItems[pair.Key] = slot;
            }

            actionStoreUpdated();

        }




        ///////////////////////////// PRIVATE METHODS //////////////////////////////////////////// 

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }


        ///////////////////////////// PUBLIC METHODS //////////////////////////////////////////// 

        /// <summary>
        /// Returns the action assigned to the particular slot
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ActionItem GetAction(int index)
        {
            if (m_DockedItems.ContainsKey(index))
            {
                return m_DockedItems[index].actionitem;
            }

            return null;
        }

        /// <summary>
        /// Returns the number of consumables in an action slot
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public int GetConsumableNumber(int index)
        {

            if (m_DockedItems.ContainsKey(index))
            {
                //Debug.Log(index + " :: " + m_DockedItems[index].number);
                return m_DockedItems[index].number;
            }

            return 0;
        }

        /// <summary>
        /// Add an action item to the given index/slot
        /// </summary>
        /// <param name="item"></param>
        /// <param name="index"></param>
        /// <param name="number"></param>
        public void AddAction(InventoryItem item, int index, int number)
        {
            if (m_DockedItems.ContainsKey(index))
            {
                if (object.ReferenceEquals(item, m_DockedItems[index].actionitem))
                {
                    m_DockedItems[index].number += number;
                }
            }
            else
            {
                var slot = new DockedItemSlot();
                slot.actionitem = item as ActionItem;
                slot.number = number;
                m_DockedItems[index] = slot;
            }

            if (actionStoreUpdated != null)
            {
                actionStoreUpdated();
            }
        }

        /// <summary>
        /// Uses the action item in the slot
        /// </summary>
        /// <param name="index"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool UseItem(int index, GameObject user)
        {
            if (m_DockedItems.ContainsKey(index))
            {
                m_DockedItems[index].actionitem.Use(user);
                if (m_DockedItems[index].actionitem.IsConsumable())
                {
                    RemoveItems(index, 1);
                }

                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes an items from the action bar when consumed
        /// </summary>
        /// <param name="index"></param>
        /// <param name="number"></param>
        public void RemoveItems(int index, int number)
        {
            if (m_DockedItems.ContainsKey(index))
            {
                m_DockedItems[index].number -= number;
                if (m_DockedItems[index].number <= 0)
                {
                    m_DockedItems.Remove(index);
                }

                if (actionStoreUpdated != null)
                {
                    actionStoreUpdated();
                }
            }
        }


        /// <summary>
        /// What is the maximum number of items allowed in this slot.
        /// 
        /// This takes into account whether the slot already contains an item
        /// and whether it is the same type. Will only accept multiple if the
        /// item is consumable.
        /// </summary>
        /// <returns>Will return int.MaxValue when there is not effective bound.</returns>
        public int MaxAcceptable(InventoryItem item, int index)
        {
            var actionItem = item as ActionItem;

            //Check if item is an action
            if (!actionItem) return 0;

            //Check if item in slot matches item in question and that the slot is valid
            if (m_DockedItems.ContainsKey(index) && !object.ReferenceEquals(m_DockedItems[index].actionitem, item)) return 0;

            //Check if consumable. If so return max value
            if (actionItem.IsConsumable())
            {
                return int.MaxValue;
            }

            if (m_DockedItems.ContainsKey(index)) return 0;

            return 1;
        }

    }
}
