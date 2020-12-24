using UnityEngine;
using RPG.Attributes;
using RPG.Control;

namespace RPG.Combat
{
    [RequireComponent(typeof(Health))]

    //Indicates that object can be attacked by the player.
    public class CombatTarget : MonoBehaviour, IRayCastable
    {
        ///////////////////////////// INTERFACE ////////////////////////////////////////////  

        //IRaycastable interface
        public bool HandleRaycast(PlayerController playerCont)
        {
            //Can the player attack this target
            if (!playerCont.GetComponent<CharacterCombat>().CanAttack(gameObject)) return false;

            //Does the player select to attack this target
            if (Input.GetMouseButtonDown(0))
            {
                playerCont.GetComponent<CharacterCombat>().Attack(gameObject);
            }
            return true;
        }

        public CursorType GetCursorType()
        {
            return CursorType.Combat;
        }

    }
}
