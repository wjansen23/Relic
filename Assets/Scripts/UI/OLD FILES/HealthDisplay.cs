using System;
using UnityEngine;
using UnityEngine.UI;
using RPG.Attributes;

namespace RPG.UI
{
    public class HealthDisplay : MonoBehaviour
    {
        Health m_PlayerHealth = null;           //Reference to the players health component

        // Start is called before the first frame update
        private void Awake()
        {
            m_PlayerHealth = GameObject.FindWithTag("Player").GetComponent<Health>();
        }

        //Update everyframe
        private void Update()
        {
            //Update player health text display
            //GetComponent<Text>().text = String.Format("{0:0}%",m_PlayerHealth.getHealthPercent());
            GetComponent<Text>().text = String.Format("{0:0}/{1,0}", m_PlayerHealth.getHealthPoints(),m_PlayerHealth.getMaxHealthPoints());
        }
    }
}
