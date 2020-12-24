using UnityEngine;
using RPG.Attributes;
using RPG.Stats;
using UnityEngine.Events;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] float m_Speed = 1.0f;                  //How fast does the projectile move
        [SerializeField] float m_MaxLifetime = 10.0f;           //How long does the projetile live after creation
        [SerializeField] float m_TimeAfterImpact = 2.0f;        //How long does the projetile live after creation
        [SerializeField] bool m_isHoming = false;               //Does the projectile follow the target or not
        [SerializeField] GameObject m_HitEffect = null;         //The effect that plays when the projectile hits the target.
        [SerializeField] GameObject[] m_DestroyOnImpact = null; //List of games objects to be destroyed immediately on impact
        [SerializeField] UnityEvent m_OnHitEvent;               //Event for when the projectile hits an object

        Health m_Target = null;                     //What is the projectile fired at
        float m_weaponDamage = 0;                   //Damage projectiles does on based on weapon
        float m_CharacterDamage = 0;                //Additional damage done by the projectile due to character attributes
        GameObject m_Wielder = null;                //Hold a reference to the object that fired the projectile

        private void Start()
        {
            //Look at target
            transform.LookAt(GetAimLocation());
        }

        // Update is called once per frame
        void Update()
        {
            //Protect against null
            if (m_Target == null) return;

            //check for homing
            if (m_isHoming && !m_Target.IsDead()) transform.LookAt(GetAimLocation());

            //Move projectile
            transform.Translate(Vector3.forward * m_Speed * Time.deltaTime);
        }
        //Calculates the location where the projectile is going to on the target
        private Vector3 GetAimLocation()
        {
            CapsuleCollider targetHitBox = m_Target.GetComponent<CapsuleCollider>();

            //Protect against null
            if (targetHitBox == null)
            {
                return m_Target.transform.position;
            }
            else
            {
                return m_Target.transform.position + Vector3.up * (targetHitBox.height / 1.5f);
            }
        }

        //See if projectile impacts target, if so do damage
        private void OnTriggerEnter(Collider other)
        {
            //Check to see if hit the target
            if (other.GetComponentInParent<Health>() != m_Target) return;

            //Check if target is dead
            if (m_Target.IsDead()) return;
            m_Target.TakeDamage(m_Wielder, m_weaponDamage + m_CharacterDamage);

            //Set speed to zero upon impact
            m_Speed = 0;

            //Play SFX
            m_OnHitEvent.Invoke();

            //Play Impact FX
            if (m_HitEffect != null)
            {
                Instantiate(m_HitEffect, GetAimLocation(), transform.rotation);
            }

            foreach (GameObject gobj in m_DestroyOnImpact)
            {
                Destroy(gobj);
            }

            Destroy(gameObject, m_TimeAfterImpact);
        }


        ///////////////////////////// PUBLIC METHODS ////////////////////////////////////////////    

        //Set the target the projectile was fired at
        public void setTarget(GameObject attacker, Health target, float damage)
        {
            //Set relevant variables
            m_Target = target;
            m_weaponDamage = damage;
            m_CharacterDamage = attacker.GetComponent<BaseStats>().GetStat(StatType.PhysicalDamage);
            m_Wielder = attacker;

            Destroy(gameObject, m_MaxLifetime);
        }
    }
}
