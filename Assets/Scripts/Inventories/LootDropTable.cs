using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Inventories
{
    [CreateAssetMenu(menuName =("Loot/Loot Drop Table"))]
    public class LootDropTable : ScriptableObject
    {
        [SerializeField] float[] m_LootDropChance;
        [SerializeField] int[] m_MinNumDrops;
        [SerializeField] int[] m_MaxNumDrops;
        [SerializeField] LootDropRecord[] m_PotentialDrops;

        public struct LootDrop
        {
            public InventoryItem item;
            public int num;            
        }

        ///////////////////////////// PRIVATE METHODS ////////////////////////////////////////////  

        /// <summary>
        /// Returns a loot drop
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private LootDrop GetRandomDrop(int level)
        {
            var drop = SelectRandomItem(level);
            var result = new LootDrop();

            result.item = drop.item;
            result.num = drop.GetRandomNumber(level);

            return result;
        }

        /// <summary>
        /// Determines whether loot is dropped or not
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private bool ShouldRandomDrop(int level)
        {
            return UnityEngine.Random.Range(0, 100) < GetByLevel(m_LootDropChance, level);
        }       

        private int GetRandomNumberofDrops(int level)
        {
            int minDrop = GetByLevel(m_MinNumDrops, level);
            int maxDrop = GetByLevel(m_MaxNumDrops, level);
            return UnityEngine.Random.Range(minDrop,maxDrop);
        }

        /// <summary>
        /// Selects a drop from the loot table randomly
        /// </summary>
        /// <returns></returns>
        private LootDropRecord SelectRandomItem(int level)
        {
            float totalchance = GetTotalChance(level);
            float randomroll = UnityEngine.Random.Range(0, totalchance);


            float runTotal = 0;

            foreach (var drop in m_PotentialDrops)
            {
                runTotal += GetByLevel(drop.chance,level);
                if (randomroll < runTotal)
                {
                    return drop;
                }
            }

            return null;
        }

        /// <summary>
        /// Sums all chances for drop items
        /// </summary>
        /// <returns></returns>
        private float GetTotalChance(int level)
        {
            float total = 0;
            foreach (var drop in m_PotentialDrops)
            {
                total += GetByLevel(drop.chance,level);
            }
            return total;
        }


        static T GetByLevel<T>(T[] values, int level)
        {
            //Check is array has any length
            if (values.Length == 0)
            {
                return default;
            }

            //Check if index is greater then array length. If so, return last element.
            if (level > values.Length)
            {
                return values[values.Length - 1];
            }

            //Check if level is <=0
            if (level <= 0)
            {
                return default;
            }

            return values[level - 1];
        }

        ///////////////////////////// PUBLIC METHODS ////////////////////////////////////////////  

        /// <summary>
        /// Determines whether or not loot is dropped. If so, passes back the loot drop.
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public IEnumerable<LootDrop> GetRandomDrops(int level)
        {
            if (!ShouldRandomDrop(level)) yield break;
            
            for (int i=0; i < GetRandomNumberofDrops(level); i++)
            {
                yield return GetRandomDrop(level);
            }
        }


        ///////////////////////////// SUB CLASSES ////////////////////////////////////////////  

        //Sub-class holds the drop stats for an item
        [System.Serializable]
        class LootDropRecord
        {
            public InventoryItem item;
            public float[] chance;
            public int[] minNum;
            public int[] maxNum;

            public int GetRandomNumber(int level)
            {
                //Check if item is stackable.
                if (!item.IsStackable()) return 1;

                //Compute number of stackable items to return.
                int min = GetByLevel(minNum,level);
                int max = GetByLevel(maxNum, level); 
                return UnityEngine.Random.Range(min, max+1);
            }
        }
    }
}
