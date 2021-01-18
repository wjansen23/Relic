using UnityEngine;

namespace RPG.Inventories
{
    /// <summary>
    /// An inventory item that can be placed in the action bar and "Used".
    /// </summary>
    /// <remarks>
    /// This class should be used as a base. Subclasses must implement the `Use`
    /// method.
    /// </remarks>
    [CreateAssetMenu(menuName = "Inventory/New Action Item")]
    public class ActionItem : InventoryItem
    {
        [Tooltip("Does an instance of this item get consumed every time it's used.")]
        [SerializeField] bool m_Consumable = false;


        ///////////////////////////// PUBLIC METHODS //////////////////////////////////////////// 

        public void Use(GameObject user)
        {
            Debug.Log("Using Action " + this);
        }

        public bool IsConsumable()
        {
            return m_Consumable;
        }

    }
}
