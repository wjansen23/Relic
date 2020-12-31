using UnityEngine;
using RPG.Core.UI.Dragging;

namespace RPG.UI.Inventory
{
    /// <summary>
    /// To be put on the object representing an inventory slot. Allows the slot to
    /// update the icon and number.
    /// </summary>
    public class InventorySlotUI : MonoBehaviour, IDragContainer<Sprite>
    {
        [SerializeField] InventoryItemIcon m_Icon = null;

        ///////////////////////////// INTERFACES //////////////////////////////////////////// 
        
        /// <summary>
        /// Updates inventory sprite.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="number"></param>
        public void AddItems(Sprite item, int number)
        {
            m_Icon.SetItem(item);
        }


        /// <summary>
        /// Returns current sprite for inventory slot
        /// </summary>
        /// <returns></returns>
        public Sprite GetItem()
        {
            return m_Icon.GetItem();
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
        public int MaxAcceptable(Sprite item)
        {
            if (GetItem() == null)
            {
                return int.MaxValue;
            }

            return 0;
        }

        /// <summary>
        /// Removes the item from the slot
        /// </summary>
        /// <param name="numnber"></param>
        public void RemoveItems(int numnber)
        {
            m_Icon.SetItem(null);
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
