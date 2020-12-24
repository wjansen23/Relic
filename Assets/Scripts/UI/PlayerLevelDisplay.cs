using System;
using UnityEngine;
using UnityEngine.UI;
using RPG.Stats;

namespace RPG.UI
{ 
    public class PlayerLevelDisplay : MonoBehaviour
    {
        BaseStats m_BaseStats;

        private void Awake()
        {
            m_BaseStats = GameObject.FindWithTag("Player").GetComponent<BaseStats>();
        }

        private void Update()
        {
            //Update player health text display
            GetComponent<Text>().text = String.Format("{0:0}", m_BaseStats.GetLevel());
        }

        ///////////////////////////// PUBLIC METHODS ////////////////////////////////////////////  

        public void UpdateLevelLabel(int level)
        {
            //Update player health text display
            GetComponent<Text>().text = String.Format("{0:0}", m_BaseStats.GetLevel());
        }

    }
}
