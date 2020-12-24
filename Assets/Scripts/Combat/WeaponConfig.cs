using UnityEngine;
using RPG.Attributes;

namespace RPG.Combat
{
    //Adds a new menu choice off of the create menu item
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]

    public class WeaponConfig : ScriptableObject
    {
        [SerializeField] AnimatorOverrideController m_weaponAOC = null;     //Animation override controller for weapon
        [SerializeField] Weapon m_equippedPrefab = null;                //What is the character equipping.  Usually the weapon being used
        [SerializeField] float m_weaponRange = 2f;                          //Attack Range of Character
        [SerializeField] float m_weaponDamage = 20f;                        //Amount of damage that each attack does
        [SerializeField] bool m_weaponHandRight = true;                     //Is the weapon right or left handed
        [SerializeField] Projectile m_projectile = null;                    //For a ranged weapon what is its projectile

        const string m_WeaponName = "Weapon";                               //String reference for locating weapon objects

        //Returns the hand the weapon is in
        private Transform GetWeaponHand(Transform rthand, Transform lfhand)
        {
            Transform handTransform = rthand;
            if (!m_weaponHandRight) handTransform = lfhand;
            return handTransform;
        }

        //Destroys any weapons currently in hands
        private void DestroyOldWeapon(Transform rthand, Transform lfhand)
        {
            //Check for null references
            if (rthand == null && lfhand == null) return;

            Transform oldWeapon = null;
            if (rthand != null)
            {
                oldWeapon = rthand.Find(m_WeaponName);
                if (oldWeapon==null && lfhand != null)
                {
                    oldWeapon = lfhand.Find(m_WeaponName);
                }
            } else
            {
                oldWeapon = lfhand.Find(m_WeaponName);
            }

            //If no weapon found return otherwise destroy
            if (oldWeapon == null) return;
            oldWeapon.name = "DESTROYING";              //Rename to avoid naming confusion
            Destroy(oldWeapon.gameObject);
        }


        ///////////////////////////// PUBLIC METHODS ////////////////////////////////////////////        

        //Spawn the weapon in the players hand
        public Weapon Spawn(Transform rthand,Transform lfhand, Animator animator)
        {
            //Remove the old weapon
            DestroyOldWeapon(rthand, lfhand);

            Weapon weapon = null;

            //Spawn the weapon entity
            if (m_equippedPrefab != null)
            {
                Transform handTransform = GetWeaponHand(rthand, lfhand);
                weapon = Instantiate(m_equippedPrefab, handTransform);
                weapon.gameObject.name = m_WeaponName;
            }

            //Swap out animation contoller. Check for case when weaponAOC is null
            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
            if (m_weaponAOC != null)
            {
                animator.runtimeAnimatorController = m_weaponAOC;
            }
            else if (overrideController != null)
            {
                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            }

            return weapon;
        }

        //Returns range of weapon
        public float GetWeaponRange()
        {
            return m_weaponRange;
        }

        //Returns damage of weapon
        public float GetWeaponDamage()
        {
            return m_weaponDamage;
        }

        //Return if the weapon has a projectile
        public bool HasProjectile()
        {
            return m_projectile != null;
        }

        //Launches the projectile from the weapon
        public void LaunchProjectile(GameObject attacker, Transform rthand, Transform lfhand,Health target)
        {
            //Create the projectile
            Projectile projectileInstance = Instantiate(m_projectile, GetWeaponHand(rthand, lfhand).position, Quaternion.identity);
            projectileInstance.setTarget(attacker, target,m_weaponDamage);
        }

    }
}
