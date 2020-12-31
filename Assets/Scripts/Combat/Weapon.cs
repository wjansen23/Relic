using UnityEngine;
using UnityEngine.Events;

namespace RPG.Combat
{
    /// <summary>
    /// Represents weapons in the game.
    /// Placed on weapon objects in the game to handle the OnHit event.
    /// </summary>
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