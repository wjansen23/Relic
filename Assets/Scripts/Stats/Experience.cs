using UnityEngine;
using RPG.Saving;
using System;

namespace RPG.Stats
{
    public class Experience : MonoBehaviour, ISaveable
    {
        [SerializeField] float m_xpPoints = 0;

        //DELEGATES.  
        //Action is a special kind of delegate that has no params and returns void 
        public Action onXPGained;


        ///////////////////////////// PUBLIC METHODS ////////////////////////////////////////////        

        //Increase current XP value
        public void gainXP(float xp)
        {
            m_xpPoints += xp;
            onXPGained();
        }

        //Return current XP value
        public float GetCurrentXP()
        {
            return m_xpPoints;
        }


        //Store state
        public void RestoreState(object state)
        {
            m_xpPoints = (float)state;
        }

        //Capture state
        public object CaptureState()
        {
            return m_xpPoints;
        }
    }
}

