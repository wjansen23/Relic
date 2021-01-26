using GameDevTV.Utils;
using UnityEngine;
using RPG.Saving;
using RPG.Stats;


namespace RPG.Attributes
{
    /// <summary>
    /// This class handles the management of a players magic
    /// </summary>
    public class Magic : MonoBehaviour, ISaveable
    {
        [SerializeField] LazyValue<float> m_CurrentMagic;
        [SerializeField] float m_LevelUpMagicBoost = 50f;            //How much health does a player gain when leveling up
        [SerializeField] float m_MagicRegenTime = 1f;                //How often should magic regenerate

        BaseStats m_BaseStats = null;                                //Reference to base stats component

        float m_TimeSinceLastRegen;                                  //Timer for tracking passive magic regeneration

        ///////////////////////////// INTERFACES ////////////////////////////////////////////  

        /// <summary>
        /// Restores the state of the object from a saved file
        /// </summary>
        /// <param name="state"></param>
        public void RestoreState(object state)
        {
            m_CurrentMagic.value = (float)state;
        }

        /// <summary>
        /// Capture state of the object
        /// </summary>
        /// <returns></returns>
        public object CaptureState()
        {
            return m_CurrentMagic.value;
        }

        ///////////////////////////// PRIVATE METHODS ////////////////////////////////////////////    

        private void Awake()
        {
            //Get Base Stats component
            m_BaseStats = GetComponent<BaseStats>();

            //Get initial magic via lazy value.  Must register the method you want called for getting the value
            m_CurrentMagic = new LazyValue<float>(GetInitialMagic);
        }

        private void Start()
        {
            //Make sure that magic has been initialized
            m_CurrentMagic.ForceInit();

            //Set all timers.
            m_TimeSinceLastRegen = Mathf.Infinity;
        }

        private void Update()
        {
            //do not regenerate if dead
            if (GetComponent<Health>().IsDead()) return;

            //Check if current magic level is less than level maximum
            float maxmagic = m_BaseStats.GetStat(StatType.Magic);
            if (m_CurrentMagic.value < maxmagic)
            {
                //Check if enough time has passed since last boost to magic value
                if (m_TimeSinceLastRegen> m_MagicRegenTime)
                {
                    float maxMagic = m_BaseStats.GetStat(StatType.Magic);

                    //Determine the smaller of the two.  Difference between current and max or the regen percentage of the max
                    float amount = Mathf.Min(maxMagic-m_CurrentMagic.value,m_BaseStats.GetStat(StatType.MagicRegen) * maxMagic);

                    //Restore magic and reset timer
                    RestoreMagic(amount);
                    m_TimeSinceLastRegen = 0;                    
                }
            }
            UpdateTimers();
        }

        /// <summary>
        /// Perform actions when component is enabled
        /// </summary>
        private void OnEnable()
        {
            m_BaseStats.onLevelUp += LevelUp;
        }

        /// <summary>
        /// Perform actions when component is disabled
        /// </summary>
        private void OnDisable()
        {
            m_BaseStats.onLevelUp -= LevelUp;
        }


        /// <summary>
        /// Used for setting initial magic in awake.  Must be of type float because magic is a 
        /// Lazy value of type health
        /// </summary>
        /// <returns></returns>
        private float GetInitialMagic()
        {
            return m_BaseStats.GetStat(StatType.Magic);
        }

        /// <summary>
        /// Boosts a players Magic everytime they level up.
        /// </summary>
        private void LevelUp()
        {
            //Compute health based on percentage and difference from max health for level
            float magicBoost = (m_BaseStats.GetStat(StatType.Magic) - m_CurrentMagic.value) * (m_LevelUpMagicBoost / 100);
            RestoreMagic(magicBoost);
        }

        //Update all timers
        private void UpdateTimers()
        {
            m_TimeSinceLastRegen += Time.deltaTime;
        }

        ///////////////////////////// PUBLIC METHODS ////////////////////////////////////////////        

        /// <summary>
        /// Attempts to reduce mana by the sent about
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public bool UseMagic(float amount)
        {
            //Make sure we have enough
            if (amount > m_CurrentMagic.value) return false;
            m_CurrentMagic.value -= amount;

            //Set regeneration timer
            m_TimeSinceLastRegen = 0;

            return true;
        }

        /// <summary>
        /// Return current magic value
        /// </summary>
        /// <returns></returns>
        public float GetCurrentMagic()
        {
            return m_CurrentMagic.value;
        }

        /// <summary>
        /// Return the percentage of magic remaining
        /// </summary>
        /// <returns></returns>
        public float getMagicPercent()
        {
            return 100 * m_CurrentMagic.value / m_BaseStats.GetStat(StatType.Magic);
        }

        /// <summary>
        /// Increases the magic of a character or object
        /// </summary>
        /// <param name="amount"></param>
        public void RestoreMagic(float amount)
        {
            float maxLvlMagic = m_BaseStats.GetStat(StatType.Magic);

            //Ensure we don't accidently go above the level maximum magic
            m_CurrentMagic.value = Mathf.Min(maxLvlMagic, m_CurrentMagic.value + amount);
        }

    }
}
