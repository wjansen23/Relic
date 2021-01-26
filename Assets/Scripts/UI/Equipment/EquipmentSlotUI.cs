using UnityEngine;
using RPG.Core.UI.Dragging;
using RPG.Inventories;
using RPG.UI.Inventories;
using System;

namespace RPG.UI.Inventories
{
    /// <summary>
    /// Handles the UI aspects of equiping items
    /// </summary>
    public class EquipmentSlotUI : MonoBehaviour, IItemHolder, IDragContainer<InventoryItem>
    {

        [SerializeField] InventoryItemIcon m_Icon = null;                       //Reference to inventory icon
        [SerializeField] EquipLocation m_EquipLocation = EquipLocation.Weapon;  //What type of equipment does it represent

        Equipment m_PlayerEquipment;                                            //Reference to the players equippment

        ///////////////////////////// INTERFACES //////////////////////////////////////////// 

        /// <summary>
        /// Adds an item to the slot
        /// </summary>
        /// <param name="item"></param>
        /// <param name="number"></param>
        public void AddItems(InventoryItem item, int number)
        {
            m_PlayerEquipment.AddItem((EquipableItem)item, m_EquipLocation);
        }

        /// <summary>
        /// returns the item in the equip slot
        /// </summary>
        /// <returns></returns>
        public InventoryItem GetItem()
        {
            return m_PlayerEquipment.GetItemInSlot(m_EquipLocation);
        }

        /// <summary>
        /// return the number of items in the slot
        /// </summary>
        /// <returns></returns>
        public int GetNumber()
        {
            if (GetItem() == null)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

        /// <summary>
        /// Returns max number of items that can be in the equipment slot
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int MaxAcceptable(InventoryItem item)
        {
            //Cast to equipable item
            var equipableitem = item as EquipableItem;

            //Is it a equipable item
            if (equipableitem == null) return 0;

            //Make sure location is correct
            if (equipableitem.GetAllowedEquipLocation() != m_EquipLocation) return 0;

            //Check If slot is occupied
            if (GetItem() != null) return 0;

            return 1;
        }

        /// <summary>
        /// Removes the item from the slot
        /// </summary>
        /// <param name="number"></param>
        public void RemoveItems(int number)
        {
            m_PlayerEquipment.RemoveItem(m_EquipLocation);
        }

        ///////////////////////////// PRIVATE METHODS //////////////////////////////////////////// 

        // Start is called before the first frame update
        void Awake()
        {
            //Get reference to the players equipment
            var player = GameObject.FindGameObjectWithTag("Player");
            m_PlayerEquipment = player.GetComponent<Equipment>();

            //Register for events
            m_PlayerEquipment.equipmentUpdated += RedrawUI;

        }

        // Update is called once per frame
        void Start()
        {
            RedrawUI();
        }

        /// <summary>
        /// Updates the graphic in the UI slot
        /// </summary>
        private void RedrawUI()
        {
            m_Icon.SetItem(m_PlayerEquipment.GetItemInSlot(m_EquipLocation), 1);
        }


    }
}
