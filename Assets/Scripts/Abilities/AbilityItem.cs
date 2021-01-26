
using UnityEngine;
using System;
using RPG.Inventories;
using RPG.Stats;
using RPG.Combat;
using RPG.Attributes;

[CreateAssetMenu(fileName = "Abilities", menuName = "Abilities/New Player Ability", order = 0)]
public class AbilityItem : ActionItem
{
    [Tooltip("Does this ability affect the player or enemies")]
    [SerializeField] bool m_OnPlayer = false;
    [Tooltip("What does this ability cause")]
    [SerializeField] StatType m_DamageType = StatType.PhysicalDamage;
    [Tooltip("What does this ability consume on use")]
    [SerializeField] StatType m_CostType = StatType.Magic;
    [Tooltip("How must does the ability cost to use")]
    [SerializeField] float m_CostValue = 25f;
    [Tooltip("How long does the value last")]
    [SerializeField] float m_Duration = 0f;
    [Tooltip("Range of ability if not using a weapon config")]
    [SerializeField] float m_Range = 1f;
    [Tooltip("How long does the player wait between uses")]
    [SerializeField] float m_CoolDown = 3f;
    [Tooltip("What, if any, weapon config does the ability utilize")]
    [SerializeField] WeaponConfig m_WeaponConfig = null;


    float m_TimeSinceUsed = Mathf.Infinity;                    //Holds how long since the ability was used


    ///////////////////////////// PRIVATE METHODS //////////////////////////////////////////// 

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("timer");
        UpdateTimers();
    }

    /// <summary>
    /// Checks if the character is within the range of the ability
    /// </summary>
    /// <param name="targetTransform"></param>
    /// <returns></returns>
    private bool InAbilityRange(Transform targetTransform)
    {
        var m_Player = GameObject.FindWithTag("Player");
        float disttotarget = Vector3.Distance(m_Player.transform.position, targetTransform.position);

        //Check if a weapon config is used for this ability.
        if (m_WeaponConfig != null)
        {
            return disttotarget <= m_WeaponConfig.GetWeaponRange();
        }
        return disttotarget <= m_Range;
    }

    /// <summary>
    /// Uses the ability on what the player is current targeting or attacking.
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    private bool UseOnTarget(GameObject user, Transform target)
    {
        //Get references to combat target and make sure its valid
        CombatTarget combattarget = target.GetComponent<CombatTarget>();
        if (combattarget == null) return false;

        //Get Health component of target
        var targethealth = target.GetComponent<Health>();

        //Check if target is in range of the ability
        if (!InAbilityRange(target)) return false;

        //Check if the player has enough stat to use the ability
        if (!CanUseAbility()) return false;

        //Determine what type of effect on target
        if (m_DamageType == StatType.PhysicalDamage)
        {
            AttackTarget(user,targethealth);
            return true;
        }
        return false;
    }

    private void AttackTarget(GameObject user, Health target)
    {
        //TODO Ability effects and animation
        //Check if weapon has projectile
        if (m_WeaponConfig.HasProjectile())
        {
            var combatcomp = user.GetComponent<CharacterCombat>();
            m_WeaponConfig.LaunchProjectile(user, combatcomp.GetRightHand(), combatcomp.GetLeftHand(), target);
        }
        else
        {
            target.TakeDamage(user, m_WeaponConfig.GetWeaponDamage());
        }

    }


    /// <summary>
    /// Determines if the player can use the ability
    /// </summary>
    /// <returns></returns>
    private bool CanUseAbility()
    {
        var m_Player = GameObject.FindWithTag("Player"); 

        //Is the ability off cool down
        if (m_TimeSinceUsed < m_CoolDown) return false;

        //Does the ability have enough value to be cast
        //Based off the stat type it consumes.
        if (m_CostType == StatType.Magic)
        {
            var magiccomp = m_Player.GetComponent<Magic>();
            if (m_CostValue > magiccomp.GetCurrentMagic())
            {
                return false;
            }
            else
            {
                magiccomp.UseMagic(m_CostValue);
            }
        }
        return true;
    }


    /// <summary>
    /// Uses the ability on the player.
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    private bool UseOnSelf(GameObject user)
    {
        return false;
    }

    /// <summary>
    /// Updatees timers
    /// </summary>
    private void UpdateTimers()
    {
        m_TimeSinceUsed += Time.deltaTime;
    }



    ///////////////////////////// PUBLIC METHODS //////////////////////////////////////////// 

    /// <summary>
    /// Attempts to use the ability
    /// </summary>
    /// <param name="user"></param>
    public bool Use(GameObject user, Transform target)
    {
        //Debug.Log("Used by " + user.name);
        if (m_OnPlayer)
        {
            return UseOnSelf(user);
        }
        else
        {
            return UseOnTarget(user, target);
        }
    }

    
}
