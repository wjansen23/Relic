using UnityEngine;
using GameDevTV.Utils;
using RPG.Movement;
using RPG.Core;
using RPG.Saving;
using RPG.Attributes;
using RPG.Stats;
using RPG.Inventories;


namespace RPG.Combat
{

    /// <summary>
    /// This class is responsible for enable a character to engage in combat within the game
    /// </summary>
    public class CharacterCombat : MonoBehaviour, IAction, ISaveable
    {

        [SerializeField] float m_timeBetweenAttacks = 2f;               //How long the between attacks
        [SerializeField] Transform m_WeaponRightHandTransform = null;   //Hand holding weapon
        [SerializeField] Transform m_WeaponLeftHandTransform = null;    //Hand holding weapon
        [SerializeField] WeaponConfig m_defaultWeapon = null;           //Equiped Weapon at start

        float m_timeSinceLastAttack= Mathf.Infinity;            //Tracks how long since the character last attacked.

        ActionScheduler m_ActionScheduler;                      //Reference to the character action scheduler.
        Animator m_Animator;                                    //Reference to the animator component on the character
        CharacterMovement m_Mover;                              //Reference to the character movement component
        Health m_Target;                                        //Reference to The targets the character is currently attacking
        WeaponConfig m_currentWeaponConfig;                     //Config for the Weapon currently equipped. Lazy Value is a wrapper class to ensure initialization befor use.
        LazyValue<Weapon> m_currentWeapon;                      //Reference to weapon objectect
        Equipment m_Equipment;                                  //Reference to the Equipment component


        ///////////////////////////// INTERFACE METHODS ////////////////////////////////////////////

        //IACTION INTERFACE
        /// <summary>
        /// Clear the current attack target
        /// </summary>
        public void Cancel()
        {
            m_Target = null;
            SetStopAttackTriggers();
        }

        //ISAVEABLE INTERFACE
        /// <summary>
        /// Capture state of combat component
        /// </summary>
        /// <returns></returns>
        public object CaptureState()
        {
            return m_currentWeaponConfig.name;
        }

        /// <summary>
        /// Restore state of combat component
        /// </summary>
        /// <param name="state"></param>
        public void RestoreState(object state)
        {
            //Look for weapon types in the resources folder with the name equal to m_defaultWeaponName.
            string weaponName = (string)state;
            WeaponConfig weapon = UnityEngine.Resources.Load<WeaponConfig>(weaponName);
            EquipWeapon(weapon);
        }

        ////IModifierProvider
        ///// <summary>
        ///// Returns additive stat modifiers
        ///// </summary>
        ///// <param name="reqStat"></param>
        ///// <returns></returns>
        //public IEnumerable<float> GetStatAdditiveModifiers(StatType reqStat)
        //{
        //    //This is an example.  Combat will not return any stat modifiers for damage
        //    if (reqStat == StatType.PhysicalDamage)
        //    {
        //        yield return 0;
        //    }
        //}

        ///// <summary>
        ///// Returns multiplicative stat modifiers
        ///// </summary>
        ///// <param name="reqStat"></param>
        ///// <returns></returns>
        //public IEnumerable<float> GetStatPercentageModifiers(StatType reqStat)
        //{
        //    //This is an example.  Combat will not return any stat modifiers for damage
        //    if (reqStat == StatType.PhysicalDamage)
        //    {
        //        yield return 0;
        //    }
        //}

        ///////////////////////////// PRIVATE METHODS ////////////////////////////////////////////


        private void Awake()
        {
            m_Mover = GetComponent<CharacterMovement>();
            m_ActionScheduler = GetComponent<ActionScheduler>();
            m_Animator = GetComponent<Animator>();

            m_currentWeaponConfig = m_defaultWeapon;
            m_currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
            m_Equipment = GetComponent<Equipment>();

            //Check if character has an equipment component.  If so, register for updates.
            if (m_Equipment)
            {
                m_Equipment.equipmentUpdated += UpdateWeapon;
            }
        }

        /// <summary>
        /// Sets up the default weapon for the character
        /// </summary>
        /// <returns></returns>
        private Weapon SetupDefaultWeapon()
        {
            return AttachWeaponToCharacter(m_defaultWeapon);
        }

        /// <summary>
        /// Start is called before the first frame update
        /// </summary>
        void Start()
        {
            m_currentWeapon.ForceInit();
        }

        /// <summary>
        /// Update is called once per frame
        /// </summary>
        void Update()
        {
            m_timeSinceLastAttack += Time.deltaTime;

            //If no target or target is dead then return and don't do anything.
            if (m_Target == null) return;
            if (m_Target.IsDead()) return;

            if (!InWeaponRange(m_Target.transform))
            {
                m_Mover.MoveTo(m_Target.transform.position);
            }
            else
            {
                m_Mover.Cancel();
                AttackBehavoir();
            }
        }

        /// <summary>
        /// Deals with the attacking behavoir of the character
        /// </summary>
        private void AttackBehavoir()
        {
            //Make sure looking at target
            transform.LookAt(m_Target.transform);

            //Check if enough time has elapsed between attack animations
            if (m_timeSinceLastAttack >= m_timeBetweenAttacks)
            {
                //This will trigger the hit event on the animation
                SetAttackTriggers();
                m_timeSinceLastAttack = 0;
            }

        }

        /// <summary>
        /// Checks if the character is within the range of their currently equiped weapon
        /// </summary>
        /// <param name="targetTransform"></param>
        /// <returns></returns>
        private bool InWeaponRange(Transform targetTransform)
        {
            return Vector3.Distance(this.transform.position, targetTransform.position) <= m_currentWeaponConfig.GetWeaponRange();
        }

        /// <summary>
        /// Handles the hit event from an animation
        /// </summary>
        void Hit()
        {
            //If we have lost the target then just return.  Can happen when canceling an attack prior to hit event.
            if (m_Target == null) return;
            float totalDamage = 0;

            ////Check to see if this character has equipment slots
            //var statEquipComp = GetComponent<StatsEquipment>();

            //if (statEquipComp == null)
            //{
                //Damage the character does regardless of weapon
                float charDamage = GetComponent<BaseStats>().GetStat(StatType.PhysicalDamage);

                //Enemies don't have equipments slots so just grab the damage from their weapon since
                //Get Stat will not account for it.
                float wepDamage = m_currentWeaponConfig.GetWeaponDamage();
                totalDamage = charDamage + wepDamage;
            //}
            //else
            //{
            //    //Damage character does to include modifiers based on equipped weapons.
            //    totalDamage = GetComponent<BaseStats>().GetStat(StatType.PhysicalDamage);
            //}

            //Invoke weapon hit mechanics
            if (m_currentWeapon.value != null)
            {
                m_currentWeapon.value.OnHit();
            }

            m_Target.TakeDamage(gameObject, totalDamage);
        }

        /// <summary>
        /// Handles the shoot event from an animation
        /// </summary>
        void Shoot()
        {
            //If we have lost the target then just return.  Can happen when canceling an attack prior to hit event.
            if (m_Target == null) return;

            //Check if weapon has projectile
            if (m_currentWeaponConfig.HasProjectile())
            {
                m_currentWeaponConfig.LaunchProjectile(gameObject, m_WeaponRightHandTransform, m_WeaponLeftHandTransform, m_Target);
            }
            //If no projectile then default to hit method.
            else
            {
                Hit();
            }
        }

        /// <summary>
        /// Sets the triggers for attacking
        /// </summary>
        private void SetAttackTriggers()
        {
            m_Animator.ResetTrigger("stopAttack");
            m_Animator.SetTrigger("isAttacking");
        }

        /// <summary>
        /// Sets the triggers for stopping an attack
        /// </summary>
        private void SetStopAttackTriggers()
        {
            m_Animator.SetTrigger("stopAttack");
            m_Animator.ResetTrigger("isAttacking");
        }

        /// <summary>
        /// Attachs the weapon to the character
        /// </summary>
        /// <param name="weapon"></param>
        /// <returns></returns>
        private Weapon AttachWeaponToCharacter(WeaponConfig weapon)
        {
            //Spawn the weapon
            Animator animator = GetComponent<Animator>();
            return weapon.Spawn(m_WeaponRightHandTransform, m_WeaponLeftHandTransform, animator);
        }

        /// <summary>
        /// Updates the characters weapon based upon changes in equipment
        /// </summary>
        private void UpdateWeapon()
        {
            var equipedweapon = m_Equipment.GetItemInSlot(EquipLocation.Weapon) as WeaponConfig;

            //If the weapon is null then use default weapon.
            if (equipedweapon == null)
            {
                EquipWeapon(m_defaultWeapon);
            }
            else
            {
                EquipWeapon(equipedweapon);
            }
        }

        ///////////////////////////// PUBLIC METHODS ////////////////////////////////////////////        

        /// <summary>
        /// Attacks the target
        /// </summary>
        /// <param name="combatTarget"></param>
        public void Attack(GameObject combatTarget)
        {
            //Schedule attack action
            m_ActionScheduler.StartAction(this);

            //Set combat target
            m_Target = combatTarget.GetComponent<Health>();
            //Debug.Log("Attacking: " + combatTarget.name);
         }

        /// <summary>
        /// Returns whether or not the taret can be attacked
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public bool CanAttack(GameObject target)
        {
            //Make sure target is not null
            if (target == null) return false;

            //If we can't move to the object and the object is our of weapon range we cannot attack
            if (!GetComponent<CharacterMovement>().CanMoveTo(target.transform.position) && !InWeaponRange(target.transform)) return false;

            //Make sure target has a health component and is alive
            Health targetHealth = target.GetComponent<Health>();
            return targetHealth != null && !targetHealth.IsDead();
        }

        /// <summary>
        /// Returns the current target
        /// </summary>
        /// <returns></returns>
        public Health GetTarget()
        {
            return m_Target;
        }

        /// <summary>
        /// Interface for equipping a weapon into the hand of the character
        /// </summary>
        /// <param name="weapon"></param>
        public void EquipWeapon(WeaponConfig weapon)
        {
            //Update current weapon
            m_currentWeaponConfig = weapon;
            m_currentWeapon.value = AttachWeaponToCharacter(weapon);
        }

        public Transform GetRightHand()
        {
            return m_WeaponRightHandTransform;
        }

        public Transform GetLeftHand()
        {
            return m_WeaponRightHandTransform;
        }
    }
}
