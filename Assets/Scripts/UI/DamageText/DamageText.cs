
using System;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI
{
    public class DamageText : MonoBehaviour
    {
        [SerializeField] Text m_DamageText = null;              //reference to display text

        ///////////////////////////// PUBLIC METHODS ////////////////////////////////////////////        

        public void SetValue(float amount)
        {
            m_DamageText.text = String.Format("{0:0}", amount);
        }
    }
}
