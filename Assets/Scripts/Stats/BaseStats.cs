using System;
using GameDevTV.Utils;
using UnityEngine;

namespace RPG.Stats
{
    /// <summary>
    /// This class handles all the stats in the game.
    /// </summary>
    public class BaseStats : MonoBehaviour
    {
        [Range (1,99)]
        [SerializeField] int m_startLevel = 1;              //Starting level for the character
        [SerializeField] CharacterClass m_CharacterClass;   //Holds a reference to the class this particular character is
        [SerializeField] Progression m_Progression = null;  //Holds a reference to the progression script this character uses
        [SerializeField] GameObject m_LevelUpFX = null;     //Referene to the level effect for a character
        [SerializeField] bool m_ShouldUseModifiers = false; //Flag which determines whether the character uses modifiers or not

        LazyValue<int> m_currentLevel;                      //Current level of a character. Lazy Value is a wrapper class to ensure initialization befor use.
        Experience m_xpComp = null;                         //Reference to the xP component on a character

        //DELEGATES-EVENTS
        public event Action onLevelUp;


        private void Awake()
        {
            m_xpComp = GetComponent<Experience>();

            //Set the level of the character
            m_currentLevel = new LazyValue<int>(CalculateLevel);
        }
        private void Start()
        {
            //Set the level of the character if it hasn't been set already
            m_currentLevel.ForceInit();
        }

        private void OnEnable()
        {
            //If XP component exists add the update level method level to its list of delegates
            if (m_xpComp != null)
            {
                m_xpComp.onXPGained += UpdateLevel;
            }
        }

        private void OnDisable()
        {
            //If XP component exists add the update level method level to its list of delegates
            if (m_xpComp != null)
            {
                m_xpComp.onXPGained -= UpdateLevel;
            }
        }

        /// <summary>
        /// Checks to see if the characters level has changed.
        /// </summary>
        private void UpdateLevel()
        {
            int tmpLevel = CalculateLevel();
            if (tmpLevel > m_currentLevel.value)
            {
                m_currentLevel.value = tmpLevel;
                PlayLevelUpEffect();
                onLevelUp();
            }
        }

        /// <summary>
        /// Computes the characters level
        /// </summary>
        /// <returns></returns>
        private int CalculateLevel()
        {
            //Check for XP component
            if (m_xpComp == null) return m_startLevel;

            //Get player XP and the maximum number of levels that have been defined for the stat
            //A character is said to be at a certain lvl of xp if there current XP is less then the
            //define level xp.  i.e. for level 3 you 30 or more experience. 
            float curXP = m_xpComp.GetCurrentXP();
            int maxStatLevel = m_Progression.GetMaxStatLevelForClass(StatType.LevelXp, m_CharacterClass);

            //Calculate Current level
            for (int level = 1; level <= maxStatLevel; level++)
            {
                float levelXP = m_Progression.GetStatByLevel(StatType.LevelXp, m_CharacterClass, level);
                if (curXP < levelXP) return level;

            }
            return maxStatLevel + 1;
        }

        /// <summary>
        /// Plays the level up effect for a character
        /// </summary>
        private void PlayLevelUpEffect()
        {
            Instantiate(m_LevelUpFX, transform);
        }

        /// <summary>
        /// Get all additive modifiers for a stat
        /// </summary>
        /// <param name="reqStat"></param>
        /// <returns></returns>
        private float GetStatAdditiveModifier(StatType reqStat)
        {
            //chech for modifier use
            if (!m_ShouldUseModifiers) return 0;

            float totalMod = 0;

            //Get Non-multiplicative Modifiers
            foreach(IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                //Get all additive modifiers for character stats
                foreach(float modifier in provider.GetStatAdditiveModifiers(reqStat))
                {
                    totalMod += modifier;
                }
            }
            return totalMod;
        }

        /// <summary>
        /// /Get all multiplicative modifiers for a stat
        /// </summary>
        /// <param name="reqStat"></param>
        /// <returns></returns>
        private float GetStatPercentageModifier(StatType reqStat)
        {
            //chech for modifier use
            if (!m_ShouldUseModifiers) return 0;

            float totalMod = 0;

            //Get Percentage Modifiers
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                //Get all Percetage modifiers for the character stats
                foreach (float modifier in provider.GetStatPercentageModifiers(reqStat))
                {
                    totalMod += modifier;
                }
            }
            return totalMod;
        }

        ///////////////////////////// PUBLIC METHODS ////////////////////////////////////////////  

        /// <summary>
        /// Returns the value of a stat at the current level and scaled by any modifieres
        /// </summary>
        /// <param name="reqStat"></param>
        /// <returns></returns>
        public float GetStat(StatType reqStat)
        {
            float baseValue = m_Progression.GetStatByLevel(reqStat, m_CharacterClass, GetLevel());
            return baseValue * (1+GetStatPercentageModifier(reqStat)/100) + GetStatAdditiveModifier(reqStat);
        }

        /// <summary>
        /// Returns the value of a stat for the given level and scaled by any modifieres
        /// </summary>
        /// <param name="reqStat"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public float GetStatAtLevel(StatType reqStat,int level)
        {
            //Check for out of bounds
            if (level < 1) return 0;

            float baseValue = m_Progression.GetStatByLevel(reqStat, m_CharacterClass, level);
            return baseValue * (1 + GetStatPercentageModifier(reqStat) / 100) + GetStatAdditiveModifier(reqStat);
        }

        /// <summary>
        /// Returns what level the character is
        /// </summary>
        /// <returns></returns>
        public int GetLevel()
        {
            return m_currentLevel.value;
        }
    }
}