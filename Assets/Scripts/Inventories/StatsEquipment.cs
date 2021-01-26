using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Stats;


namespace RPG.Inventories
{
    /// <summary>
    /// This class implements a sub-class of equipment that provides stats to the character
    /// </summary>
    public class StatsEquipment : Equipment, IModifierProvider
    {


        ///////////////////////////// INTERFACES ////////////////////////////////////////////   

        public IEnumerable<float> GetStatAdditiveModifiers(StatType reqStat)
        {
            //Loop through all populated slots
            foreach (var slot in GetAllPopulatedSlots())
            {
                //Ensure item in slot provides stats
                var item = GetItemInSlot(slot) as IModifierProvider;
                if (item == null) continue;

                //Loop through all the stats provided by the item
                foreach (float modifier in item.GetStatAdditiveModifiers(reqStat))
                {
                    yield return modifier;
                }
            }
        }

        public IEnumerable<float> GetStatPercentageModifiers(StatType reqStat)
        {
            //Loop through all populated slots
            foreach (var slot in GetAllPopulatedSlots())
            {
                //Ensure item in slot provides stats
                var item = GetItemInSlot(slot) as IModifierProvider;
                if (item == null) continue;

                //Loop through all the stats provided by the item
                foreach (float modifier in item.GetStatPercentageModifiers(reqStat))
                {
                    yield return modifier;
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
