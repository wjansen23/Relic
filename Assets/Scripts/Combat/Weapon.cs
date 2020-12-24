using UnityEngine;
using UnityEngine.Events;

namespace RPG.Combat
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] UnityEvent m_onHitEvent;           //On hit event

        ///////////////////////////// PUBLIC METHODS ////////////////////////////////////////////        

        //Do stuff on weapon hit
        public void OnHit()
        {
            m_onHitEvent.Invoke();
        }
    }
}