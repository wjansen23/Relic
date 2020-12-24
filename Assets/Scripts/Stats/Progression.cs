using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression", order = 0)]
    public class Progression : ScriptableObject
    {
        [SerializeField] ProgressionCharacterClass[] m_CharacterClasses;                //Array that holds stat objects for each character class

        Dictionary<CharacterClass, Dictionary<StatType, float[]>> m_StatTable = null;  //Dictionary that holds all the stat values for a progression object


        //Creates the stat lookup table if it doesn't exist
        private void BuildStatTable()
        {
            if (m_StatTable != null) return;

            //Create stat dictionary
            m_StatTable = new Dictionary<CharacterClass, Dictionary<StatType, float[]>>();

            //Loop through class in the progression object and populate their stats in the dictionary
            foreach (ProgressionCharacterClass progClass in m_CharacterClasses)
            {
                //Create stat value dictionary and add to class dictionary
                Dictionary<StatType, float[]> statValues = new Dictionary<StatType, float[]>();

                //Loop through stats and add the level array to the dictionary
                foreach (ProgressionStat progStat in progClass.m_stats)
                {
                    statValues[progStat.m_StatType] = progStat.m_Levels;
                }

                //Add stat values to dictionary
                m_StatTable[progClass.m_CharacterClass] = statValues;
            }
        }

        ///////////////////////////// PUBLIC METHODS ////////////////////////////////////////////  

        //Returns the health for the given class at the given given level
        public float GetStatByLevel(StatType reqStat, CharacterClass reqClass, int level)
        {
            //Build table if does not exist
            BuildStatTable();

            //Make sure stat exists for the class, if not throw error
            float[] valArray;
            if (!m_StatTable[reqClass].TryGetValue(reqStat, out valArray))
            {
                Debug.Log("ERROR: " + reqStat + " stat has not been defined for " + reqClass + " but is being asked for");
                return 0;
            }
            else
            {
                //Check to ensure level is not outside the bounds of the stat value array
                if (valArray.Length < level) return 0;
                return valArray[level - 1];
            }
            //OLD CODE:Check to ensure level is not outside the bounds of the stat value array
            //float[] valArray = m_StatTable[reqClass][reqStat];
            //if (valArray.Length < level) return 0;
            //return valArray[level - 1];
        }

        public int GetMaxStatLevelForClass(StatType stat, CharacterClass charClass)
        {
            //Build table if does not exist
            BuildStatTable();

            //Get stat level array and return size
            float[] tmpLevels = m_StatTable[charClass][stat];
            return tmpLevels.Length;
        }


        ///////////////////////////// SUB CLASSES ////////////////////////////////////////////  

        //Sub-class holds the stats for a various character classs
        [System.Serializable]
        class ProgressionCharacterClass
        {
            public CharacterClass m_CharacterClass;
            public ProgressionStat[] m_stats;
        }

        //Sub-class that holds the level values for a particular stata
        [System.Serializable]
        class ProgressionStat
        {
            public StatType m_StatType;
            public float[] m_Levels;
        }
    }
}


