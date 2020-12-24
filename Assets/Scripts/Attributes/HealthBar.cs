using UnityEngine;

namespace RPG.Attributes
{
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

        //Show or hide the bar
        private void ShowHideBar(bool flag)
        {
            m_BarCanvas.enabled= flag;
        }
    }
}
