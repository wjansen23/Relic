using UnityEngine;
using RPG.Attributes;

namespace RPG.UI
{
    public class PlayerMagicDisplay : MonoBehaviour
    {
        [SerializeField] RectTransform m_ImageForeground;   //Referene to foreground image

        Magic m_MagicComp = null;                           //Reference to Magic component

        // Start is called before the first frame update
        void Start()
        {
            m_MagicComp = GameObject.FindWithTag("Player").GetComponent<Magic>();
        }

        // Update is called once per frame
        void Update()
        {
            m_ImageForeground.localScale = new Vector3(m_MagicComp.getMagicPercent() / 100, 1, 1);
        }
    }
}
