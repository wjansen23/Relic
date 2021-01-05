using UnityEngine;

namespace RPG.UI
{
    /// <summary>
    /// Hides and unhides gameobjects based on a key input
    /// </summary>
    public class ShowHideUI : MonoBehaviour
    {
        [SerializeField] KeyCode m_ToggleKey = KeyCode.Escape;
        [SerializeField] GameObject m_UIContainer = null;

        /// <summary>
        /// Start is called before the first frame update
        /// </summary>
        void Start()
        {
            m_UIContainer.SetActive(false);
        }

        /// <summary>
        /// Update is called once per frame
        /// </summary>
        void Update()
        {
            if (Input.GetKeyDown(m_ToggleKey))
            {
                //change state of container to the opposite of what it currently is.
                m_UIContainer.SetActive(!m_UIContainer.activeSelf);
            }
        }
    }
}
