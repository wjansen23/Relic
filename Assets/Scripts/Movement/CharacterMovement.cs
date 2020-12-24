using UnityEngine;
using UnityEngine.AI;
using RPG.Core;
using RPG.Saving;
using RPG.Attributes;

namespace RPG.Movement
{
    //This script is responsible for moving a character within the game.
    public class CharacterMovement : MonoBehaviour, IAction, ISaveable
    {
        [SerializeField] float m_MaxPathLength = 60f;       //Maximum distance a path can be for computing movement

        NavMeshAgent m_NMA;                                 //Reference to the Nav Mesh Agent of the Character.
        ActionScheduler m_ActionScheduler;                  //Reference to the character action scheduler.
        Health m_Health;                                    //Reference to health component.


        ///////////////////////////// INTERFACE METHODS ////////////////////////////////////////////

        //IACTION INTERFACE
        //Tells NMA to stop moving character
        public void Cancel()
        {
            m_NMA.isStopped = true;
        }

        //ISAVEABLE INTERFACE
        //Capture the state of the component
        public object CaptureState()
        {
            //Debug.Log("Capturing State for " + this.name);

            //Convert Position vector to serializable version.
            return new SerializableVector3(transform.position);
        }

        //Restores the state of the component
        public void RestoreState(object state)
        {
            //Debug.Log("Retoring State for " + this.name);
            //Turn off nav mesh before moving entity to avoid glitches
            GetComponent<NavMeshAgent>().enabled = false;

            //Need to convert serializableV3 back to Vector 3
            SerializableVector3 position = (SerializableVector3)state;
            transform.position = position.ToVector();

            //Turn back on nav mesh
            GetComponent<NavMeshAgent>().enabled = true;

            //Cancel current entity actions
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        ///////////////////////////// PRIVATE METHODS ////////////////////////////////////////////


        private void Awake()
        {
            m_NMA = GetComponent<NavMeshAgent>();
            m_ActionScheduler = GetComponent<ActionScheduler>();
            m_Health = GetComponent<Health>();
        }

        // Update is called once per frame
        void Update()
        {
            //Check if character is dead and disable nav mesh agent if so.
            if (m_Health != null)
            {
                m_NMA.enabled = !m_Health.IsDead();
            }

            //Update the animation for the character
            UpdateAnimator();
        }

        // Converts global velocity into a velocity that the animator can utilize
        //then updated blend value of animator
        private void UpdateAnimator()
        {
            Vector3 velocity = m_NMA.velocity;                                          //What the NavMeshAgent considers the global velocity of the character
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);      //Transform into a local velocity for the character. Used to commincation movement to animator
            float speed = localVelocity.z;                                              //How fast character should be moving in Z direction.

            //Update the animators forwardSpeed Parameter
            GetComponent<Animator>().SetFloat("forwardSpeed", speed);
        }

        //Compute the path length to a position
        private float GetPathLength(NavMeshPath path)
        {
            float totalDistance = 0f;
            if (path.corners.Length < 2) return totalDistance;

            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                totalDistance += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            }
            return totalDistance;
        }

        ///////////////////////////// PUBLIC METHODS ////////////////////////////////////////////

        //Tells NMA to move character to destination
        public void MoveTo(Vector3 destination)
        {
            //Ensure NMA allows character to move
            m_NMA.isStopped = false;
            m_NMA.destination = destination;
        }

        //Tells scheduler to start movement
        public void StartMoveAction(Vector3 destination)
        {
            //Schedule action
            m_ActionScheduler.StartAction(this);

            //Move to destination
            MoveTo(destination);
        }

        //Determines whether a character can move to the send location.
        public bool CanMoveTo(Vector3 location)
        {
            //Check to see if the pathing to target.
            //Is there path to the target
            NavMeshPath path = new NavMeshPath();
            bool hasPath = NavMesh.CalculatePath(transform.position, location, NavMesh.AllAreas, path);
            if (!hasPath) return false;

            //Is the path complete
            if (path.status != NavMeshPathStatus.PathComplete) return false;

            //Is path length to long
            if (GetPathLength(path) > m_MaxPathLength) return false;

            return true;
        }
    }
}
