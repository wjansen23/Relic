using System;
using UnityEngine;
using UnityEngine.UI;
using RPG.Stats;

namespace RPG.UI
{
    public class PlayerXPDisplay : MonoBehaviour
    {
        Experience m_XPComp;        //Reference to XP component
        BaseStats m_BaseStats;      //Reference to Base Stats componet

        [SerializeField] RectTransform m_ImageForeground;       //Referene to foreground image

        private void Awake()
        {
            m_XPComp = GameObject.FindWithTag("Player").GetComponent<Experience>();
            m_BaseStats = GameObject.FindWithTag("Player").GetComponent<BaseStats>();
        }

        private void Update()
        {
            //Get the players level, XP and the range of XP for the level
            int level = m_BaseStats.GetLevel();
            float xp = m_XPComp.GetCurrentXP();
            float levelXP = m_BaseStats.GetStat(StatType.LevelXp);
            float baseXP = 0;

            //If current level is 1 then baseXP is 0
            if (level > 1)
            {
                baseXP = m_BaseStats.GetStatAtLevel(StatType.LevelXp, level - 1);
            }

            //Amount of XP that needs to be earned to move from current level to next
            float xpRange = levelXP - baseXP;


            //Update Display bar
            m_ImageForeground.localScale = new Vector3((xp-baseXP) / xpRange, 1, 1);
        }

    }
}
