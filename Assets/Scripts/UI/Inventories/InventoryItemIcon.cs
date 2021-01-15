using UnityEngine;
using UnityEngine.UI;
using RPG.Inventories;

namespace RPG.UI.Inventories
{
    /// <summary>
    /// To be put on the icon representing an inventory item. Allows the slot to
    /// update the icon and number.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class InventoryItemIcon : MonoBehaviour
    {
        [SerializeField] GameObject m_TextContainer = null; //Reference to the stackable UI container
        [SerializeField] Text m_ItemNumber = null;          //Reference to the text display

        ///////////////////////////// PUBLIC METHODS ////////////////////////////////////////////      

        /// <summary>
        /// Updates the displayed image with the send sprite.
        /// Disables image if sprite is null.
        /// </summary>
        /// <param name="item"></param>
        public void SetItem(InventoryItem item, int number)
        {
            var iconImage = GetComponent<Image>();
            if (item == null)
            {
                //No item image then disable
                iconImage.enabled = false;
            }
            else
            {
                //Enable and update sprite
                iconImage.enabled = true;
                iconImage.sprite = item.GetIcon();
            }

            //See if slot as a multi item display and if so show or hide display based on number
            if (m_ItemNumber)
            {
                if (number <= 1)
                {
                    m_TextContainer.SetActive(false);
                }
                else
                {
                    m_TextContainer.SetActive(true);
                    m_ItemNumber.text = number.ToString();
                }
            }
        }
    }
}
