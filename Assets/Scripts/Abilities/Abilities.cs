using UnityEngine;
using System;
using RPG.Saving;
using RPG.Inventories;


public class Abilities : MonoBehaviour, ISaveable
{
    [SerializeField] AbilitySlot[] m_Abilities;                          //Stores all the abilities a player can have

    [System.Serializable]
    public struct AbilitySlot
    {
        public AbilityItem ability;
        public int level;
        public bool active;
    }

    [System.Serializable]
    private struct AbilitySlotRecord
    {
        public string abilityID;
        public int level;
        public bool active;
    }

    /// <summary>
    /// Broadcasts when ablities have been changed.
    /// </summary>
    public event Action abilitiesUpdated;

    ///////////////////////////// INTERFACES //////////////////////////////////////////// 

    public object CaptureState()
    {
        //Create a string array to hold the IDs from the players inventory
        var state = new AbilitySlotRecord[m_Abilities.Length];

        //Loop through players inventory and store the itemID for each item in the inventory
        for (int i = 0; i < m_Abilities.Length; i++)
        {
            if (m_Abilities[i].ability != null)
            {
                state[i].abilityID = m_Abilities[i].ability.GetItemID();
                state[i].level = m_Abilities[i].level;
                state[i].active = m_Abilities[i].active;
            }
        }
        return state;
    }

    public void RestoreState(object state)
    {
        var stateRecords = (AbilitySlotRecord[])state;

        for (int i = 0; i < stateRecords.Length; i++)
        {
            m_Abilities[i].ability = InventoryItem.GetFromID(stateRecords[i].abilityID) as AbilityItem;
            m_Abilities[i].level = stateRecords[i].level;
            m_Abilities[i].active = stateRecords[i].active;
        }

        if (abilitiesUpdated != null)
        {
            abilitiesUpdated();
        }
    }


    ///////////////////////////// PRIVATE METHODS //////////////////////////////////////////// 


    // Start is called before the first frame update
    void Start()
    {
        if (abilitiesUpdated != null)
        {
            abilitiesUpdated();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    ///////////////////////////// PUBLIC METHODS //////////////////////////////////////////// 

    /// <summary>
    /// Convenience for getting the player's abilities.
    /// </summary>
    /// <returns></returns>
    public static Abilities GetPlayerAbilities()
    {
        var player = GameObject.FindWithTag("Player");
        return player.GetComponent<Abilities>();
    }

    /// <summary>
    /// Returns the ability assigned to the particular slot
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public AbilityItem GetAbility(int index)
    {
        //Debug.Log(index + "::" + m_Abilities.Length);
        //Make sure index is in range
        if (index < 0 || index > m_Abilities.Length) return null;
        return m_Abilities[index].ability;
    }

    /// <summary>
    /// Uses the ability
    /// </summary>
    /// <param name="index"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    //public bool UseItem(int index, GameObject user)
    //{
    //    //Make sure index is in range
    //    if (index < 0 || index > m_Abilities.Length - 1) return false;

    //    m_Abilities[index].ability.Use(user);
    //    return true;
    //}

    /// <summary>
    /// Uses the ability
    /// </summary>
    /// <param name="index"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    //public bool UseAbility(int index, GameObject user, Transform target)
    //{

    //    //Make sure index is in range
    //    if (index < 0 || index > m_Abilities.Length - 1) return false;

    //    m_Abilities[index].ability.Use(user,target);
    //    return true;
    //}


}
