using UnityEngine;


namespace RPG.Inventories
{

    /// <summary>
    /// An inventory item that can be equipped to the player. Weapons could be a
    /// subclass of this.
    /// </summary>
    [CreateAssetMenu(menuName = "Inventory/New Equipment Item", order = 0)]
    public class EquipableItem : InventoryItem
    {   
        [Tooltip("Where can the item be equipped")]
        [SerializeField] EquipLocation m_EquipLocation = EquipLocation.Weapon;     //Where can the item be equiped

        ///////////////////////////// PUBLIC METHODS ////////////////////////////////////////////     

        public EquipLocation GetAllowedEquipLocation()
        {
            return m_EquipLocation;
        }
    }
}
