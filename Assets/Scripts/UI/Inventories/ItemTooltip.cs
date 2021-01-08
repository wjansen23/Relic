using UnityEngine;
using RPG.Inventories;
using UnityEngine.UI;

namespace RPG.UI.Inventories
{
    /// <summary>
    /// Root of the tooltip prefab to expose properties to other classes.
    /// </summary>
    public class ItemTooltip : MonoBehaviour
    {
        [SerializeField] Text m_TitleText = null;       //Tooltip title
        [SerializeField] Text m_BodyText = null;        //Tooltip description

        ///////////////////////////// PUBLIC METHODS ////////////////////////////////////////////
        
        public void Setup(InventoryItem item)
        {
            m_TitleText.text = item.GetDisplayName();
            m_BodyText.text = item.GetDescription();
        }

    }
}
