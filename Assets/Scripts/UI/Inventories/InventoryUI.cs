using UnityEngine;
using RPG.Inventories;

namespace RPG.UI.Inventories
{
    /// <summary>
    /// To be placed on the root of the inventory UI. Handles spawning all the
    /// inventory slot prefabs.
    /// </summary>
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] InventorySlotUI m_InventoryItemPrefab = null;

        Inventory m_PlayerInventory;        //Reference to player inventory.

        // Start is called before the first frame update
        void Start()
        {
            Redraw();
        }

        // Update is called once per frame
        void Awake()
        {
            m_PlayerInventory = Inventory.GetPlayerInventory();
            m_PlayerInventory.inventoryUpdated += Redraw;
        }

        /// <summary>
        /// Redraws the players inventory
        /// </summary>
        private void Redraw()
        {
            foreach(Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < m_PlayerInventory.GetSize(); i++)
            {
                var itemUI = Instantiate(m_InventoryItemPrefab, transform);
                itemUI.Setup(m_PlayerInventory, i);
            }
        }
    }
}
