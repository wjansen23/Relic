using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI
{
    /// <summary>
    /// This class controlls the spawning of random dialog text over a character
    /// </summary>
    public class RandomDialogText : MonoBehaviour
    {
        [SerializeField] Text m_RandomText = null;          //Reference to Text
        [SerializeField] string[] m_DialogTextList = null;    //Holds random text sayings
        [SerializeField] float m_spawnPercent = 75;
        [SerializeField] float m_displayTime = 5;

        bool m_Displaying = false;

        private void Start()
        {

        }

        /// <summary>
        /// Checks if dialog is already spawned.  If not, randomly determine if it should spawn text.
        /// </summary>
        private void Update()
        {
            if (m_DialogTextList == null) return;
            if (m_Displaying) return;
            if (Random.Range(0, 100) > m_spawnPercent)
            {
                StartCoroutine(SpawnNothing());
                return;
            }


            int index = Mathf.Min(m_DialogTextList.Length-1,Random.Range(0, m_DialogTextList.Length));
            StartCoroutine(SpawnText(m_DialogTextList[index]));
        }

        /// <summary>
        /// Spawns the prefab with the sent value
        /// </summary>
        /// <param name="dialogText"></param>
        /// <returns></returns>
        private IEnumerator SpawnText(string dialogText)
        {
            m_Displaying = true;
            ShowDialog(true);
            m_RandomText.text = dialogText;

            yield return new WaitForSeconds(m_displayTime);
            m_Displaying = false;
        }

        /// <summary>
        /// Don't spawn any text for the given display time
        /// </summary>
        /// <returns></returns>
        private IEnumerator SpawnNothing()
        {
            ShowDialog(false);
            m_Displaying = true;
            yield return new WaitForSeconds(m_displayTime);
            m_Displaying = false;
        }

        /// <summary>
        /// Enables/Disables the text display.
        /// </summary>
        /// <param name="bShow"></param>
        private void ShowDialog(bool bShow)
        {
            GetComponent<Canvas>().enabled = bShow;
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(bShow);
            }
        }
    }
}

