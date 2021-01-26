using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using RPG.Saving;

namespace RPG.Inventories
{
    /// <summary>
    /// To be placed on anything that wishes to drop pickups into the world
    /// Tracks the drops for saving and loading
    /// </summary>
    public class ItemDropper : MonoBehaviour, ISaveable
    {
        private List<Pickup> m_DroppedItems = new List<Pickup>();                   //Holds reference to all dropped items.
        private List<DropRecord> m_OtherSceneDroppedItems = new List<DropRecord>(); //List of items dropped in other scenes

        [System.Serializable]
        private struct DropRecord
        {
            public string itemID;
            public SerializableVector3 position;
            public int number;
            public int sceneIndex;
        }

        ///////////////////////////// INTERFACES //////////////////////////////////////////// 
        
        public object CaptureState()
        {
            //Makes sure we don't save destroyed items
            RemoveDestroyedDrops();

            //Create a new list and get the scene index
            var droppedItemList = new List<DropRecord>();
            var buildIndex = SceneManager.GetActiveScene().buildIndex;

            //Loop through the drop items list and store items.
            foreach (Pickup pickup in m_DroppedItems)
            {
                var droppeditem = new DropRecord();

                droppeditem.itemID = pickup.GetItem().GetItemID();
                droppeditem.position = new SerializableVector3(pickup.transform.position);
                droppeditem.number = pickup.GetNumber();
                droppeditem.sceneIndex = buildIndex;

                droppedItemList.Add(droppeditem);
            }

            //Merge with items from other scenes
            droppedItemList.AddRange(m_OtherSceneDroppedItems);
            return droppedItemList;
        }

        public void RestoreState(object state)
        {
            var droppedItemList = (List<DropRecord>)state;
            var buildIndex = SceneManager.GetActiveScene().buildIndex;

            //Clear list
            m_OtherSceneDroppedItems.Clear();

            foreach (var item in droppedItemList)
            {
                //Check if items was from a different scene
                if (item.sceneIndex != buildIndex)
                {
                    m_OtherSceneDroppedItems.Add(item);
                    continue;
                }

                var pickupItem = InventoryItem.GetFromID(item.itemID);
                Vector3 position = item.position.ToVector();
                int number = item.number;

                SpawnPickup(pickupItem, position, number);
            }
        }

        ///////////////////////////// PRIVATE METHODS //////////////////////////////////////////// 

        /// <summary>
        /// Spawns the dropped item as a pickup
        /// </summary>
        /// <param name="item"></param>
        /// <param name="position"></param>
        private void SpawnPickup(InventoryItem item, Vector3 position, int number)
        {
            //Spawn the item and add to dropped list
            var pickup = item.SpawnPickup(position, number);
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
        public void DropItem(InventoryItem item, int number)
        {
            SpawnPickup(item, GetDropLocation(),number);
        }
    }
}
