using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core.UI.Dragging;
using RPG.Inventories;

namespace RPG.UI.Inventories
{
    /// <summary>
    /// The UI slot for the player action bar.
    /// </summary>
    public class ActionSlotUI : MonoBehaviour, IItemHolder,IDragContainer<InventoryItem>
    {
        [SerializeField] InventoryItemIcon m_Icon = null;       //Reference to icon display
        [SerializeField] int m_ActionSlot = 0;                  //What action slot this is for
        [SerializeField] bool m_UseConsumable = false;          //Whether the action slot can take a consumable item or not

        ActionStore m_PlayerActionStore;                        //Reference to the action store on the player


        ///////////////////////////// INTERFACES //////////////////////////////////////////// 
        
        /// <summary>
        /// Adds items to the action slot
        /// </summary>
        /// <param name="item"></param>
        /// <param name="number"></param>
        public void AddItems(InventoryItem item, int number)
        {
            m_PlayerActionStore.AddAction(item, m_ActionSlot, number);
        }

        /// <summary>
        /// Returns the action item stored in the slot
        /// </summary>
        /// <returns></returns>
        public InventoryItem GetItem()
        {
            return m_PlayerActionStore.GetAction(m_ActionSlot);
        }

        /// <summary>
        /// Returns the number of items in the action slot
        /// </summary>
        /// <returns></returns>
        public int GetNumber()
        {
            //Debug.Log(m_ActionSlot);
            return m_PlayerActionStore.GetConsumableNumber(m_ActionSlot);
        }

        /// <summary>
        /// Returns the maximum number of items the slot can accept
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int MaxAcceptable(InventoryItem item)
        {
            var actionTemp = item as ActionItem;
            int maxAccept = 0;

            if (actionTemp == null) return 0;

            if (actionTemp.IsConsumable() == true)
            {
                if (m_UseConsumable == true)
                {
                    maxAccept = m_PlayerActionStore.MaxAcceptable(item, m_ActionSlot);
                }
            }
            else if (m_UseConsumable == false)
            {
                maxAccept = 1;
            }

            return maxAccept;            
        }

        /// <summary>
        /// Removes items from the slot
        /// </summary>
        /// <param name="number"></param>
        public void RemoveItems(int number)
        {
            m_PlayerActionStore.RemoveItems(m_ActionSlot, number);
        }

        ///////////////////////////// PUBLIC METHODS //////////////////////////////////////////// 
        
        void Awake()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            m_PlayerActionStore = player.GetComponent<ActionStore>();
            m_PlayerActionStore.actionStoreUpdated += UpdateIcon;
        }

        private void Start()
        {
            UpdateIcon();
        }

        /// <summary>
        /// Updates the display graphic for the slot
        /// </summary>
        private void UpdateIcon()
        {
            m_Icon.SetItem(GetItem(), GetNumber());
        }

    }
}
