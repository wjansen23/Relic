using System.Collections;
using UnityEngine;
using RPG.Control;
using RPG.Movement;
using RPG.Attributes;

namespace RPG.Combat
{
    public class HealthPickup : MonoBehaviour, IRayCastable
    {
        [SerializeField] float m_HealthToRestore = 15f;
        [SerializeField] float m_Respawntime = 5.0f;

        ///////////////////////////// INTERFACE ////////////////////////////////////////////  

        //IRaycastable interface
        public bool HandleRaycast(PlayerController playerCont)
        {
            if (Input.GetMouseButtonDown(0))
            {
                playerCont.GetComponent<CharacterMovement>().StartMoveAction(transform.position);
            }
            return true;
        }

        public CursorType GetCursorType()
        {
            return CursorType.Pickup;
        }

        ///////////////////////////// PRIVATE METHODS ////////////////////////////////////////////
        private void OnTriggerEnter(Collider other)
        {
            //Make sure that the player entered the collider. If so, then pickup
            if (other.gameObject.tag == "Player")
            {
                Pickup(other.GetComponentInParent<Health>());
            }
        }

        //Picks up and equips the weapon
        private void Pickup(Health comp)
        {
            comp.HealDamage(m_HealthToRestore);
            StartCoroutine(HideForSeconds(m_Respawntime));
        }

        //Hides the pickup for the given seconds
        private IEnumerator HideForSeconds(float seconds)
        {
            ShowPickup(false);
            yield return new WaitForSeconds(seconds);
            ShowPickup(true);
        }

        //Show/hide the pickup
        private void ShowPickup(bool bShow)
        {
            GetComponent<Collider>().enabled = bShow;
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(bShow);
            }
        }
    }
}
