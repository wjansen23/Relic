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

        ///////////////////////////// PUBLIC METHODS ////////////////////////////////////////////      

        /// <summary>
        /// Updates the displayed image with the send sprite.
        /// Disables image if sprite is null.
        /// </summary>
        /// <param name="item"></param>
        public void SetItem(InventoryItem item)
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
        }
    }
}
