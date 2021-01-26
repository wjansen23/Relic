using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core.UI.Dragging;
using RPG.Inventories;

namespace RPG.UI.Inventories
{
    public class AbilitySlotUI : MonoBehaviour, IItemHolder, IDragContainer<InventoryItem>
    {
        [SerializeField] InventoryItemIcon m_Icon = null;       //Reference to icon display
        [SerializeField] int m_AbilitySlot = 0;                 //What ability "slot" this UI is representing
        
        Abilities m_Abilities;                                  //Reference to ability listing
        bool m_OnActionBar = false;                             //Flag for storing whether the user has dragged the ability to the action bar or not

        ///////////////////////////// INTERFACES //////////////////////////////////////////// 

        /// <summary>
        /// Adds items to the action slot
        /// </summary>
        /// <param name="item"></param>
        /// <param name="number"></param>
        public void AddItems(InventoryItem item, int number)
        {
            //Debug.Log("ADD ::" + this.name+ "::" + number);
            if (number > 0)
            {
                m_OnActionBar = false;
                UpdateIcon();
            }
        }

        /// <summary>
        /// Returns the Ability item stored in the slot.
        /// </summary>
        /// <returns></returns>
        public InventoryItem GetItem()
        {
            return m_Abilities.GetAbility(m_AbilitySlot);
        }

        /// <summary>
        /// Returns the number of items in the ability slot
        /// 
        /// If on the action bar returns 0
        /// </summary>
        /// <returns></returns>
        public int GetNumber()
        {
            if (m_OnActionBar)
            {
                return 0;
            }

            return 1;
        }

        /// <summary>
        /// Returns the maximum number of items the slot can accept
        /// 
        /// Returns 0 if the item in question does not match the ability this slot represents
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int MaxAcceptable(InventoryItem item)
        {
            if (CorrectAbility(item))
            {
                return 1;
            }
            return 0;
        }

        /// <summary>
        /// Removes items from the slot.
        /// 
        /// Does nothing if the number sent is zero.
        /// </summary>
        /// <param name="number"></param>
        public void RemoveItems(int number)
        {
            //Debug.Log("REMOVE ::" + this.name + "::" + number);
            //Make sure the sent number is greater than zero.  If not, then don't change the state of the slot.
            if (number > 0)
            {
                m_OnActionBar = true;
                UpdateIcon();
            }
        }


        ///////////////////////////// PRIVATE METHODS //////////////////////////////////////////// 

        // Start is called before the first frame update
        void Awake()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            m_Abilities = player.GetComponent<Abilities>();
            m_Abilities.abilitiesUpdated += UpdateIcon;
        }

        private void Start()
        {
            UpdateIcon();
        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// Checks whether the sent item is the same as the item this slot represents
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool CorrectAbility(InventoryItem item)
        {
            //Cast sent item and get item associated with slot
            var sendAbilityItem = item as AbilityItem;
            var slotAbilityItem = GetItem();

            if (object.ReferenceEquals(sendAbilityItem, slotAbilityItem))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Updates the display graphic for the slot
        /// </summary>
        private void UpdateIcon()
        {
            if (m_OnActionBar)
            {
                m_Icon.SetItem(null, 1);
            }
            else
            {
                m_Icon.SetItem(GetItem(), 1);
            }
        }

    }
}
