using UnityEngine;
using UnityEngine.UI;
using System;
using RPG.Attributes;
using RPG.Combat;

namespace RPG.UI
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        CharacterCombat m_Combat = null;           //Reference to the players character combat component


        // Start is called before the first frame update
        private void Awake()
        {
            m_Combat = GameObject.FindWithTag("Player").GetComponent<CharacterCombat>();
        }
        //Update everyframe
        private void Update()
        {
            Health m_target = m_Combat.GetTarget();

            if (m_target == null || m_target.IsDead())
            {
                GetComponent<Text>().text = String.Format("N/A");
            }
            else
            {
                //Update enemy health text display
                //GetComponent<Text>().text = String.Format("{0:0}%", m_target.getHealthPercent());
                GetComponent<Text>().text = String.Format("{0:0}/{1,0}", m_target.getHealthPoints(), m_target.getMaxHealthPoints());
            }
        }
    }
}
