﻿using UnityEngine;
using GameDevTV.Utils;
using RPG.Saving;
using RPG.Stats;
using RPG.Core;
using UnityEngine.Events;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] float m_LevelUpHealthBoost=50f;            //How much health does a player gain when leveling up
        [SerializeField] TakeDamageEvent m_TakeDamageEvent;         //Event for spawning overhead damage text 
        [SerializeField] UnityEvent m_onDieEvent;                   //Event for dieing
        
        LazyValue<float> m_CurrentHealth;               //Amount of Health a character or object has. Lazy Value is a wrapper class to ensure initialization befor use.
        BaseStats m_BaseStats = null;                   //Reference to base stats component

        bool m_IsDead = false;                          //Boolean to indicate whether the character or object is dead


        [System.Serializable]
        public class TakeDamageEvent : UnityEvent<float>
        {
            //EMPTY
        }

        ///////////////////////////// INTERFACE METHODS ////////////////////////////////////////////

        //ISAVABLE INTERFACE
        //Capture current health state
        public object CaptureState()
        {
            return m_CurrentHealth.value;
        }

        //Restore health state and check to see if dead.  If so, die.
        public void RestoreState(object state)
        {
            m_CurrentHealth.value = (float)state;
            if (m_CurrentHealth.value <= 0) DeathBehavoir();
        }

        ///////////////////////////// PRIVATE METHODS ////////////////////////////////////////////

        private void Awake()
        {
            //Get Base Stats component
            m_BaseStats = GetComponent<BaseStats>();

            //Get initial health via lazy value.  Must register the method you want called for getting the value
            m_CurrentHealth = new LazyValue<float>(GetInitialHealth);
        }

        //Used for setting initial health in awake.  Must be of type float because health is a 
        //Lazy value of type health
        private float GetInitialHealth()
        {
            return m_BaseStats.GetStat(StatType.Health);
        }

        private void Start()
        {
            //Make sure that health has been initialized
            m_CurrentHealth.ForceInit();
        }

        private void OnEnable()
        {
            m_BaseStats.onLevelUp += LevelUp;
        }

        private void OnDisable()
        {
            m_BaseStats.onLevelUp -= LevelUp;
        }

        //Boosts a players healths everytime the level up.
        private void LevelUp()
        {
            //Compute health based on percentage and difference from max health for level
            float healthBoost = (m_BaseStats.GetStat(StatType.Health) - m_CurrentHealth.value) * (m_LevelUpHealthBoost/100);
            HealDamage(healthBoost);
        }

        //Deals with the death behavoir of the character
        private void DeathBehavoir()
        {
            if (!m_IsDead)
            {
                m_IsDead = true;
                GetComponent<Animator>().SetTrigger("isDead");
                GetComponent<ActionScheduler>().CancelCurrentAction();
            }
        }

        //Award experience to the attacker on death
        private void AwardXP(GameObject attacker)
        {
            Experience xpComp = attacker.GetComponent<Experience>();

            if (xpComp==null) return;
            xpComp.gainXP(m_BaseStats.GetStat(StatType.XpReward));
        }


        ///////////////////////////// PUBLIC METHODS ////////////////////////////////////////////        

        //Reduces the health of the character or object
        public void TakeDamage(GameObject attacker, float damage)
        {
            //Decrement health but only to zero.
            m_CurrentHealth.value = Mathf.Max(0, m_CurrentHealth.value - damage);

            //If character dies, then award XP to attacker
            if (m_CurrentHealth.value <= 0)
            {
                m_onDieEvent.Invoke();
                DeathBehavoir();
                AwardXP(attacker);
            }
            else
            {
                //Invoke Unity Events
                m_TakeDamageEvent.Invoke(damage);
            }

        }

        //Increases the health of a the character or object
        public void HealDamage(float healing)
        {
            float maxLvlHP = m_BaseStats.GetStat(StatType.Health);

            //Ensure we don't accidently go above the level maximum health
            m_CurrentHealth.value = Mathf.Min(maxLvlHP, m_CurrentHealth.value + healing);
            //Debug.Log("Health-HealDamage: " + maxLvlHP + ":" + healing);
        }

        //Return the percentage of health remaining
        public float getHealthPercent()
        {
            return 100 * m_CurrentHealth.value/m_BaseStats.GetStat(StatType.Health);
        }

        //Returns current health value
        public float getHealthPoints()
        {
            return m_CurrentHealth.value;
        }

        //Returns maximum health points for current level
        public float getMaxHealthPoints()
        {
            return m_BaseStats.GetStat(StatType.Health);
        }

        //Returns isDead boolena
        public bool IsDead()
        {
            return m_IsDead;
        }
    }
}