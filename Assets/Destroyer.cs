using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour
{
    [SerializeField] GameObject m_TargetToDestroy=null;              //Gameobject to destroy

    ///////////////////////////// PUBLIC METHODS ////////////////////////////////////////////     

    public void DestroyTarget()
    {
        Destroy(m_TargetToDestroy);
    }
}
