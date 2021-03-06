﻿using UnityEngine;
using RPG.Core.UI.Dragging;
using RPG.Inventories;

namespace RPG.UI.Inventories
{
    /// <summary>
    /// Handles the spawning of pickups when items are dropped into the world.
    /// 
    /// Must be places on the root canvas where items can be dragged.  will be
    /// called if dropped over empty space.
    /// </summary>
    public class InventoryDropTarget : MonoBehaviour, IDragDestination<InventoryItem>
    {
        ///////////////////////////// INTERFACES //////////////////////////////////////////// 
        
        public void AddItems(InventoryItem item, int number)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<ItemDropper>().DropItem(item, number);
        }

        public int MaxAcceptable(InventoryItem item)
        {
            //If the item trying to be dropped in the scene is an ability do not drop it.
            var dropitem = item as AbilityItem;
            if (dropitem != null) return 0;

            return int.MaxValue;
        }
    }
}
