using System;
using UnityEngine;
using UnityEngine.UI;
using RPG.Stats;

namespace RPG.UI
{
    public class XpDisplay : MonoBehaviour
    {
         Experience m_xp = null;           //Reference to the players experience component

        // Start is called before the first frame update
        private void Awake()
        {
            m_xp = GameObject.FindWithTag("Player").GetComponent<Experience>();
        }

        //Update everyframe
        private void Update()
        {
            //Update player health text display
            GetComponent<Text>().text = String.Format("{0:0}", m_xp.GetCurrentXP());
        }
    }
}
