using UnityEngine;
using RPG.Core.UI.Dragging;
using RPG.Inventories;

namespace RPG.UI.Inventories
{
    /// <summary>
    /// To be put on the object representing an inventory slot. Allows the slot to
    /// update the icon and number.
    /// </summary>
    public class InventorySlotUI : MonoBehaviour, IItemHolder, IDragContainer<InventoryItem>
    {
        [SerializeField] InventoryItemIcon m_Icon = null;

        int m_Index = 1;                //Index number of the slot
        Inventory m_Inventory;          //Inventory this slot is a part of

        ///////////////////////////// INTERFACES //////////////////////////////////////////// 
        
        /// <summary>
        /// Updates inventory sprite.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="number"></param>
        public void AddItems(InventoryItem item, int number)
        {
            m_Inventory.AddItemToSlot(m_Index, item);
        }


        /// <summary>
        /// Returns current sprite for inventory slot
        /// </summary>
        /// <returns></returns>
        public InventoryItem GetItem()
        {
            return m_Inventory.GetItemInSlot(m_Index);
        }

        /// <summary>
        /// Returns current number of items in slot
        /// </summary>
        /// <returns></returns>
        public int GetNumber()
        {
            return 1;
        }

        /// <summary>
        /// Returns maximum number of acceptable items for the slot
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int MaxAcceptable(InventoryItem item)
        {
            if (m_Inventory.HasSpaceFor(item))
            {
                return int.MaxValue;
            }

            return 0;
        }

        /// <summary>
        /// Removes the item from the slot
        /// </summary>
        /// <param name="index"></param>
        public void RemoveItems(int number)
        {
            m_Inventory.RemoveFromSlot(m_Index);
        }

        ///////////////////////////// PUBLIC METHODS ////////////////////////////////////////////
        
        public void Setup(Inventory inventory, int slotnum)
        {
            m_Inventory = inventory;
            m_Index = slotnum;
            m_Icon.SetItem(m_Inventory.GetItemInSlot(slotnum));

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
    }
}
