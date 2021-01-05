using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class destroys the assigned gameObject.  For those items that can't be removed from the game by other means.
/// </summary>
public class Destroyer : MonoBehaviour
{
    [SerializeField] GameObject m_TargetToDestroy=null;              //Gameobject to destroy

    ///////////////////////////// PUBLIC METHODS ////////////////////////////////////////////     

    public void DestroyTarget()
    {
        Destroy(m_TargetToDestroy);
    }
}
