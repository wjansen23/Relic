using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RPG.Core.UI.Tooltips
{
    /// <summary>
    /// Abstract base class that handles the spawning of a tooltip prefab at the
    /// correct position on screen relative to a cursor.
    /// 
    /// Override the abstract functions to create a tooltip spawner for your own
    /// data.
    /// </summary>
    public abstract class TooltipSpawner : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Tooltip("The prefab of the tooltip to spawn")]
        [SerializeField] GameObject m_TooltipPrefab = null;     //Reference to the tooltip prefab

        GameObject m_Tooltip;

        ///////////////////////////// INTERFACE METHODS ////////////////////////////////////////////

        /// <summary>
        /// Do this when the cursor enters the rect area of this selectable UI object.
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerEnter(PointerEventData eventData)
        {
            var parentcanvas = GetComponentInParent<Canvas>();

            //Tooltip exists.  Clear it.
            if (m_Tooltip && !CanCreateTooltip())
            {
                ClearTooltip();
            }

            //Tooltip does not exist.  Create it.
            if (!m_Tooltip && CanCreateTooltip())
            {
                m_Tooltip = Instantiate(m_TooltipPrefab, parentcanvas.transform);
            }

            //Update tooltip
            if (m_Tooltip)
            {
                UpdateTooltip(m_Tooltip);
                PositionTooltip();
            }
        }

        /// <summary>
        /// Do this when the cursor exits the rect area of this selectable UI object.
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerExit(PointerEventData eventData)
        {
            ClearTooltip();
        }

        ///////////////////////////// PRIVATE METHODS ////////////////////////////////////////////

        /// <summary>
        /// Handles destroying the tooltip
        /// </summary>
        void OnDestroy()
        {
            ClearTooltip();
        }

        /// <summary>
        /// Handles what to do when the tooltip is disabled.
        /// </summary>
        void OnDisable()
        {
            ClearTooltip();
        }

        /// <summary>
        /// Destroys the tooltip
        /// </summary>
        private void ClearTooltip()
        {
            if (m_Tooltip)
            {
                Destroy(m_Tooltip.gameObject);
            }
        }

        private void PositionTooltip()
        {
            // Required to ensure corners are updated by positioning elements.
            Canvas.ForceUpdateCanvases();

            //Get the corners of the tooltip and the slot that is being "queried"
            var tooltipcorners = new Vector3[4];
            var slotCorners = new Vector3[4];

            m_Tooltip.GetComponent<RectTransform>().GetWorldCorners(tooltipcorners);
            GetComponent<RectTransform>().GetWorldCorners(slotCorners);

            //Check where the spawner is in relation to center of slot.
            bool below = transform.position.y > Screen.height / 2;
            bool right = transform.position.x < Screen.width / 2;

            int slotcorner = GetCornerIndex(below, right);
            int tooltipcorner = GetCornerIndex(!below, !right);

            m_Tooltip.transform.position = slotCorners[slotcorner] - tooltipcorners[tooltipcorner] + m_Tooltip.transform.position;
        }

        private int GetCornerIndex(bool below, bool right)
        {
            if (below && !right) return 0;
            else if (!below && !right) return 1;
            else if (!below && right) return 2;
            else return 3;
        }


        ///////////////////////////// PUBLIC METHODS ////////////////////////////////////////////

        /// <summary>
        /// Return true when the tooltip spawner should be allowed to create a tooltip.
        /// </summary>
        /// <returns></returns>
        public abstract bool CanCreateTooltip();

        /// <summary>
        /// Called when it is time to update the information on the tooltip
        /// prefab. 
        /// </summary>
        /// <param name="m_Tooltip">The spawned tooltip prefab for updating.</param>
        public abstract void UpdateTooltip(GameObject tooltip);
    }
}
