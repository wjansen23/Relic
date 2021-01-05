using UnityEngine;

namespace RPG.Attributes
{

    /// <summary>
    /// This class manages the updating and display of a character health bar
    /// </summary>
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] Health m_HealthComp = null;            //Reference to health component
        [SerializeField] RectTransform m_ImageForeground;       //Referene to foreground image
        [SerializeField] Canvas m_BarCanvas = null;

        // Update is called once per frame
        void Update()
        {
            UpdateHealthBar();
        }

        /// <summary>
        /// Updates the health bar display based on percentage of health remaining
        /// </summary>
        private void UpdateHealthBar()
        {
            if (Mathf.Approximately(m_HealthComp.getHealthPercent() / 100, 1) || m_HealthComp.IsDead())
            {
                ShowHideBar(false);
                return;
            }
            m_ImageForeground.localScale = new Vector3(m_HealthComp.getHealthPercent() / 100, 1, 1);
            ShowHideBar(true);
        }

        /// <summary>
        /// Show or hide the health bar
        /// </summary>
        /// <param name="flag"></param>
        private void ShowHideBar(bool flag)
        {
            m_BarCanvas.enabled= flag;
        }
    }
}
