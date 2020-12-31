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

        // Start is called before the first frame update
        void Start()
        {
            m_UIContainer.SetActive(false);
        }

        // Update is called once per frame
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
