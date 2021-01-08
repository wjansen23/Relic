using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;

namespace RPG.Inventories
{
    /// <summary>
    /// To be placed on anything that wishes to drop pickups into the world
    /// Tracks the drops for saving and loading
    /// </summary>
    public class ItemDropper : MonoBehaviour, ISaveable
    {
        private List<Pickup> m_DroppedItems = new List<Pickup>();           //Holds reference to all dropped items.

        [System.Serializable]
        private struct DropRecord
        {
            public string itemID;
            public SerializableVector3 position;
        }

        ///////////////////////////// INTERFACES //////////////////////////////////////////// 
        
        public object CaptureState()
        {
            RemoveDestroyedDrops();

            var droppedItemList = new DropRecord[m_DroppedItems.Count];
            for (int i = 0; i < droppedItemList.Length; i++)
            {
                droppedItemList[i].itemID = m_DroppedItems[i].GetItem().GetItemID();
                droppedItemList[i].position = new SerializableVector3(m_DroppedItems[i].transform.position);
            }

            return droppedItemList;
        }

        public void RestoreState(object state)
        {
            throw new System.NotImplementedException();
        }

        ///////////////////////////// PRIVATE METHODS //////////////////////////////////////////// 

        /// <summary>
        /// Spawns the dropped item as a pickup
        /// </summary>
        /// <param name="item"></param>
        /// <param name="position"></param>
        private void SpawnPickup(InventoryItem item, Vector3 position)
        {
            //Spawn the item and add to dropped list
            var pickup = item.SpawnPickup(position);
            m_DroppedItems.Add(pickup);

        }

        /// <summary>
        /// Removes all destroyed drops from tracking list.
        /// </summary>
        private void RemoveDestroyedDrops()
        {
            //Create a new list
            var newlist = new List<Pickup>();

            //Loop through existing list and add items that still exist to the new list
            foreach (var item in m_DroppedItems)
            {
                if (item != null)
                {
                    newlist.Add(item);
                }
            }

            //update tracking list with new list values
            m_DroppedItems = newlist;
        }

        ///////////////////////////// PROTECTED METHODS //////////////////////////////////////////// 

        protected virtual Vector3 GetDropLocation()
        {
            return transform.position;
        }

        ///////////////////////////// PUBLIC METHODS //////////////////////////////////////////// 

        /// <summary>
        /// Drops the item into the world.
        /// </summary>
        /// <param name="item"></param>
        public void DropItem(InventoryItem item)
        {
            SpawnPickup(item, GetDropLocation());
        }
    }
}
