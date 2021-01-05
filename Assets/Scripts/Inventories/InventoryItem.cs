using System.Collections.Generic;
using UnityEngine;

namespace RPG.Inventories
{
    [CreateAssetMenu(fileName = "Inventory", menuName = "Inventory/New Inventory Item", order = 0)]

    /// <summary>
    /// A ScriptableObject that represents any item that can be put in an
    /// inventory.
    /// </summary>
    /// <remarks>
    /// In practice, you are likely to use a subclass such as `ActionItem` or
    /// `EquipableItem`.
    /// </remarks>
    public class InventoryItem : ScriptableObject, ISerializationCallbackReceiver
    {
        [Tooltip("Auto-generated UUID for saving/loading. Clear this field if you want to generate a new one.")]
        [SerializeField] string m_ItemID = null;
        [Tooltip("Item name to be displayed in UI.")]
        [SerializeField] string m_DisplayName=null;
        [Tooltip("Item description to be displayed in UI.")]
        [SerializeField] string m_Description = null;
        [Tooltip("The UI icon to represent this item in the inventory.")]
        [SerializeField] Sprite m_Icon = null;
        [Tooltip("If true, multiple items of this type can be stacked in the same inventory slot.")]
        [SerializeField] bool m_Stackable = false;
        [Tooltip("How many inventory slots does it require")]
        [SerializeField] int m_InventorySlotsUsed = 1;

        static Dictionary<string, InventoryItem> m_ItemLookupCache;


        ///////////////////////////// INTERFACES //////////////////////////////////////////// 

        public void OnAfterDeserialize()
        {
            // Require by the ISerializationCallbackReceiver but we don't need
            // to do anything with it.
        }

        /// <summary>
        /// Generates a UUID for the item if one does not already exist
        /// </summary>
        public void OnBeforeSerialize()
        {
            //Generate and save a new UUID if this is blank
            if (string.IsNullOrWhiteSpace(m_ItemID))
            {
                m_ItemID = System.Guid.NewGuid().ToString();
            }
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

        ///////////////////////////// PUBLIC METHODS //////////////////////////////////////////// 

        /// <summary>
        /// Get the inventory item instance from its UUID.
        /// </summary>
        /// <param name="itemID">
        /// String UUID that persists between game instances.
        /// </param>
        /// <returns>
        /// Inventory item instance corresponding to the ID.
        /// </returns>
        public static InventoryItem GetFromID(string itemID)
        {
            //check if lookup cache exists. If not, create and populate
            if (m_ItemLookupCache == null)
            {
                //Create lookup cache
                m_ItemLookupCache = new Dictionary<string, InventoryItem>();

                //Grab all the items in the inventory resource folder
                var itemList = Resources.LoadAll<InventoryItem>("");

                //Loop through the items and check if "key" is listed in inventory cache
                foreach (var item in itemList)
                {
                    if (m_ItemLookupCache.ContainsKey(item.m_ItemID))
                    {
                        //Duplicative item found
                        Debug.LogError(string.Format("Looks like there's a duplicate InventorySystem ID for objects: {0} and {1}", m_ItemLookupCache[item.m_ItemID], item));
                        continue;
                    }

                    //Add item to cache
                    m_ItemLookupCache[item.m_ItemID] = item;
                }
            }

            //Return null if the item is null or the item id is not in the inventory
            if (itemID == null || !m_ItemLookupCache.ContainsKey(itemID)) return null;

            return m_ItemLookupCache[itemID];
        }

        /// <summary>
        /// Returns the sprite for the inventory item
        /// </summary>
        /// <returns></returns>
        public Sprite GetIcon()
        {
            return m_Icon;
        }

        /// <summary>
        /// Returns the itemID for the inventory item
        /// </summary>
        /// <returns></returns>
        public string GetItemID()
        {
            return m_ItemID;
        }

        /// <summary>
        /// Returns the display name for the inventory item
        /// </summary>
        /// <returns></returns>
        public string GetDisplayName()
        {
            return m_DisplayName;
        }

        /// <summary>
        /// Returns item description
        /// </summary>
        /// <returns></returns>
        public string GetDescription()
        {
            return m_Description;
        }

    }
}
