using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Stats;

namespace RPG.Inventories
{

    [CreateAssetMenu(menuName = ("Inventory/Equipable Item (Stat)"))]
    public class StatsEquipmentItem : EquipableItem, IModifierProvider
    {

        [SerializeField] EquipmentModifier[] m_AdditiveModifiers;       //Holds all additive modifiers
        [SerializeField] EquipmentModifier[] m_PercentageModifiers;     //Holds all percentage modifiers

        [System.Serializable]
        private struct EquipmentModifier
        {
            public StatType stat;
            public float value;
        }

        ///////////////////////////// INTERFACES ////////////////////////////////////////////   
        
        public IEnumerable<float> GetStatAdditiveModifiers(StatType reqStat)
        {
            foreach (var modifier in m_AdditiveModifiers)
            {
                if (modifier.stat == reqStat)
                {
                    yield return modifier.value;
                }
            }
        }

        public IEnumerable<float> GetStatPercentageModifiers(StatType reqStat)
        {
            foreach (var modifier in m_PercentageModifiers)
            {
                if (modifier.stat == reqStat)
                {
                    yield return modifier.value;
                }
            }
        }

        ///////////////////////////// PRIVATE METHODS ////////////////////////////////////////////   

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
