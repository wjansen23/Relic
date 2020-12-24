using UnityEngine;
using RPG.Attributes;

namespace RPG.UI
{
    public class PlayerHealthDisplay : MonoBehaviour
    {
        [SerializeField] RectTransform m_ImageForeground;   //Referene to foreground image

        Health m_HealthComp = null;                         //Reference to health component

        // Start is called before the first frame update
        void Start()
        {
            m_HealthComp = GameObject.FindWithTag("Player").GetComponent<Health>();
        }

        // Update is called once per frame
        void Update()
        {
            m_ImageForeground.localScale = new Vector3(m_HealthComp.getHealthPercent() / 100, 1, 1);
        }
    }
}

