using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.UI
{
    public class DamageTextSpawner : MonoBehaviour
    {
        [SerializeField] DamageText m_DamageTextPrefab=null;        //Reference to damage text prefab


        ///////////////////////////// PUBLIC METHODS ////////////////////////////////////////////     
        
        //Spawns the prefab with the sent value
        public void SpawnText(float amount)
        {
            DamageText instance = Instantiate<DamageText>(m_DamageTextPrefab, transform);
            instance.SetValue(amount);
        }
    }
}
