using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using System.Collections;
using RPG.Control;
using RPG.Movement;

namespace RPG.SceneManagement
{
    public class Portal : MonoBehaviour, IRayCastable
    {
        
        enum DestinationID { destA,destB,destC,destD,destE,destF }          //Used for telling a portal what it is connected to

        [SerializeField] int m_SceneToLoad = -1;                            //What scene does this portal go to
        [SerializeField] Transform m_SpawnPoint = null;                     //Where will a player spawn for this portal
        [SerializeField] DestinationID m_Destination;                       //Holds the ID of the destination portal the portal is connected to.
        [SerializeField] float m_FadeOutTime = 2f;
        [SerializeField] float m_FadeInTime = 2f;
        [SerializeField] float m_FadeWaitTime = 1f;



        ///////////////////////////// INTERFACE METHODS ////////////////////////////////////////////

        //IRAYCASTABLE INTERFACE
        public CursorType GetCursorType()
        {
            return CursorType.Door;
        }

        public bool HandleRaycast(PlayerController playerCont)
        {
            if (Input.GetMouseButtonDown(0))
            {
                playerCont.GetComponent<CharacterMovement>().StartMoveAction(transform.position);
            }
            return true;
        }

        ///////////////////////////// PRIVATE METHODS ////////////////////////////////////////////

        private void OnTriggerEnter(Collider other)
        {
            //Debug.Log(other.name + ": Portal Triggered");
            if (other.tag == "Player")
            {
                StartCoroutine(SceneTransition());
            }
        }

        //Transition to the next scene
        private IEnumerator SceneTransition()
        {
            if (m_SceneToLoad < 0)
            {
                Debug.Log(gameObject.name + " Has no destination scene set");
                yield break;
            }

            //Ensure Portal continues to exist until all actions are completed
            DontDestroyOnLoad(gameObject);

            SceneFader sceneFade = FindObjectOfType<SceneFader>();
            SavingWrapper savingWrapper = FindObjectOfType<SavingWrapper>();

            //Get the player controller and disable
            PlayerController oldPlayerControlComp = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            oldPlayerControlComp.enabled = false;

            //Fade out the scene
            yield return sceneFade.SceneFadeOut(m_FadeOutTime);

            //Save game state old scene
            savingWrapper.Save();

            yield return SceneManager.LoadSceneAsync(m_SceneToLoad);

            //Get the player controller and disable
            PlayerController newPlayerControlComp = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            newPlayerControlComp.enabled = false;

            //Load game state after new scene is loaded.
            savingWrapper.Load();

            Portal destinationPortal = GetDestinationPortal();
            UpdatePlayerPosition(destinationPortal);

            //Save game state new scene
            savingWrapper.Save();

            yield return new WaitForSeconds(m_FadeWaitTime);
            yield return sceneFade.SceneFadeIn(m_FadeInTime);

            //Enable player control
            newPlayerControlComp.enabled = true;

            //Debug.Log("SCENE LOADED");
            //Destroy old Portal after all transistions are completed.
            Destroy(gameObject);

        }

        //Move the player to the portal spawn point
        private void UpdatePlayerPosition(Portal dPortal)
        {
            if (dPortal == null) return;
            GameObject player  = GameObject.FindWithTag("Player");

            //Disable NavMeshAgent for the player to avoid issues
            player.GetComponent<NavMeshAgent>().enabled = false;

            //Reposition the player
            player.GetComponent<NavMeshAgent>().Warp(dPortal.m_SpawnPoint.position);
            player.transform.rotation = dPortal.m_SpawnPoint.rotation;

            //Re-enable NavMeshAgent for the player
            player.GetComponent<NavMeshAgent>().enabled = true;
        } 

        //Loop through available portals in the scene and return one
        private Portal GetDestinationPortal()
        {
            foreach (Portal portal in FindObjectsOfType<Portal>())
            {
                if (portal == this) continue;
                if (portal.m_Destination != m_Destination) continue;
                return portal;
            }

            return null;
        }
    }
}
