using UnityEngine;
using RPG.Core.UI.Tooltips;

namespace RPG.UI.Inventories
{
    /// <summary>
    /// To be placed on a UI slot to spawn and show the correct item tooltip.
    /// </summary>

    [RequireComponent(typeof(IItemHolder))]
    public class ItemTooltipSpawner : TooltipSpawner
    {

        ///////////////////////////// PUBLIC METHODS ////////////////////////////////////////////
        
        /// <summary>
        /// See if there is an item in the slot.  If so, then indicate a tooltip can be created.
        /// </summary>
        /// <returns></returns>
        public override bool CanCreateTooltip()
        {
            var item = GetComponent<IItemHolder>().GetItem();
            if (!item) return false;

            return true;
        }

        /// <summary>
        /// Updates and display item tooltip
        /// </summary>
        /// <param name="tooltip"></param>
        public override void UpdateTooltip(GameObject tooltip)
        {
            var itemtooltip = tooltip.GetComponent<ItemTooltip>();
            if (!itemtooltip) return;

            var item = GetComponent<IItemHolder>().GetItem();
            itemtooltip.Setup(item);
        }
    }
}
