using UnityEngine;
using GameDevTV.Utils;
using RPG.Combat;
using RPG.Movement;
using RPG.Core;
using RPG.Attributes;
using System;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] float m_ChaseDistance = 3f;                        //How close can a player get before being chased
        [SerializeField] float m_ChaseWaitTime = 3f;                        //How long the AI will wait, after losing sight of the player, before returning to start/guard position 
        [SerializeField] PatrolPath m_PatrolPath = null;                    //Stores the AI's patrol path
        [SerializeField] float m_WaypointStopDistance = 1f;                 //Used to determine how far from the current waypoints position the AI needs to be before being considered there
        [SerializeField] float m_AgroRadius = 5f;                           //Radius from AI to cause additional AI to aggro on the player

        public enum AIState {PATROL,ATTACK,SUSPICION,DEAD,AGGRO};                  //The different states the AI can be in.

        CharacterMovement m_Mover;                                          //Reference for character movement for the AI
        CharacterCombat m_Combat;                                           //Reference for character combat for the AI
        GameObject m_Player;                                                //Reference to player.
        Health m_Health;                                                    //Reference to AI Health component
        ActionScheduler m_ActionScheduler;                                  //Reference to Action Scheduler

        LazyValue<Vector3> m_StartPosition;                                 //Holds the starting position for the AI. Lazy Value is a wrapper class to ensure initialization befor use.
        Vector3 m_StopPosition;                                             //Holds the position the AI should stop moving at
        AIState m_CurrentState;                                             //Holds the current state of the AI 
        float m_TimeSinceLastSawPlayer = Mathf.Infinity;                    //Holds how long the AI has waiting since last seeing the player
        float m_TimeSinceReachedWaypoint = Mathf.Infinity;                  //Holds how long the AI has been waiting at a way point
        float m_TimeSinceAggrevated = Mathf.Infinity;                       //Holds how long the AI has been aggrevated
        float m_WaypointDwellTime = 2f;                                     //How long an AI in the PATROL State stays at a waypoint.
        float m_AggrevationTime = 5f;                                        //How long an AI will stay aggrevated
        int m_CurrentWaypointIndex = 0;                                     //Holds the current index of the Patrol AI


        private void Awake()
        {
            m_Mover = GetComponent<CharacterMovement>();
            m_Combat = GetComponent<CharacterCombat>();
            m_Health = GetComponent<Health>();
            m_Player = GameObject.FindWithTag("Player");
            m_ActionScheduler = GetComponent<ActionScheduler>();

            m_StartPosition = new LazyValue<Vector3>(GetStartingPosition);
        }

        private Vector3 GetStartingPosition()
        {
            return transform.position;
        }

        // Start is called before the first frame update
        void Start()
        {
            m_StartPosition.ForceInit();
            m_CurrentState = AIState.PATROL;
        }

        // Update is called once per frame
        void Update()
        {
            //Check if AI is dead
            if (m_Health.IsDead())
            {
                //Make sure the body does not move or rotate
                GetComponent<Rigidbody>().velocity = Vector3.zero;
                GetComponent<Rigidbody>().rotation = Quaternion.identity;
                return;
            }

            UpdateTimers();

            //Check other behavoirs
            if (IsAggrevated()) return;
            if (ChaseBehavoir()) return;
            if (SuspicionBehavoir()) return;
            PatrolBehavoir();
        }

        //Update all AI timers
        private void UpdateTimers()
        {
            m_TimeSinceLastSawPlayer += Time.deltaTime;
            m_TimeSinceReachedWaypoint += Time.deltaTime;
            m_TimeSinceAggrevated += Time.deltaTime;
        }

        //Handles the AI's chasing the player
        private bool ChaseBehavoir()
        {
            //if (IsAggrevated() && m_Combat.CanAttack(m_Player))
            //IF Player is is chase distance and can attack the player then CHASE the player.
            if (DistanceToPlayer() <= m_ChaseDistance && m_Combat.CanAttack(m_Player))
            {
                //Debug.Log(this.name + ": I should CHASE the player");
                m_TimeSinceLastSawPlayer = 0;
                AttackBehavoir();
                return true;
            }
            return false;
        }

        //Handles the AI being suspicious
        private bool SuspicionBehavoir()
        {            
            if (m_CurrentState == AIState.ATTACK)                       //IF coming from attack state, move to players last known position and wait.
            {
                //Debug.Log(this.name + ": I LOST the player");
                m_Mover.StartMoveAction(m_Player.transform.position);
                m_TimeSinceLastSawPlayer = 0;
                m_CurrentState = AIState.SUSPICION;
                return true;
            }
            else if (m_CurrentState == AIState.SUSPICION)               //IF under suspicion then wait for a period of time before giving up.
            {
                if (m_TimeSinceLastSawPlayer < m_ChaseWaitTime)
                {
                    //Debug.Log(this.name + ": LOOKING for the player");
                    return true;
                }
                //Debug.Log(this.name + ": I should go back and GUARD");
            }
            return false;
        }
        
        //Handles the AI attacking the player
        private void AttackBehavoir()
        {
            m_Combat.Attack(m_Player);
            m_CurrentState = AIState.ATTACK;
        }

        //Handles the AI Patrolling
        private void PatrolBehavoir()
        {
            Vector3 nextPosition = m_StartPosition.value;

            if (m_PatrolPath != null)
            {
                if (AtPatrolWaypoint())
                {
                    m_TimeSinceReachedWaypoint = 0;
                    CyclePatrolWaypoint();
                }
                nextPosition = GetCurrentPatrolWaypoint();
            }

            if (m_TimeSinceReachedWaypoint > m_WaypointDwellTime)
            {
                m_Mover.StartMoveAction(nextPosition);
            }
            m_CurrentState = AIState.PATROL;
        }

        //Sets patrol waypoint
        private void CyclePatrolWaypoint()
        {
            m_CurrentWaypointIndex = m_PatrolPath.GetNextWaypointIndex(m_CurrentWaypointIndex);
        }

        //Return position of current waypoints
        private Vector3 GetCurrentPatrolWaypoint()
        {
            return m_PatrolPath.GetWaypointPosition(m_CurrentWaypointIndex);
        }

        //Checks to see if a patrolling AI is within the acceptable stop distance for a waypoint.
        private bool AtPatrolWaypoint()
        {
            return Vector3.Distance(transform.position, GetCurrentPatrolWaypoint()) < m_WaypointStopDistance;
        }

        //Returns whether a player is within the AI's chase distance
        private float DistanceToPlayer()
        {
            if (m_Player == null) return Mathf.Infinity;
            return Vector3.Distance(transform.position, m_Player.transform.position);
        }

        //CALLED BY UNITY
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, m_ChaseDistance);           
        }

        //Cause the AI to become aggrevated and attack the player

        private bool IsAggrevated()
        {
            if (m_TimeSinceAggrevated > m_AggrevationTime) return false;
            AttackBehavoir();
            return true;
        }

        //Find all nearby enemies and aggrevate them on the player
        private void AggrevateNearbyEnemies()
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, m_AgroRadius, Vector3.up, 0);

            foreach(RaycastHit hit in hits)
            {
                AIController aiComp = hit.collider.GetComponent<AIController>();

                //check is aiComp is not null
                if (aiComp == null) continue;

                //Make sure we don't aggrevate ourselves
                if (aiComp == this) continue;

                //Only Aggro those AI's that are not already aggrod or in combat
                AIState aiCompState = aiComp.GetAIState();
                if (aiCompState != AIState.AGGRO && aiCompState != AIState.ATTACK)
                {
                    aiComp.Aggrevate();
                }
            }
        }


        ///////////////////////////// PUBLIC METHODS ////////////////////////////////////////////        

        //Is called from the take damage event
        public void Aggrevate()
        {
            m_TimeSinceAggrevated = 0;
            m_CurrentState = AIState.AGGRO;
            AggrevateNearbyEnemies();
        }

        public AIState GetAIState()
        {
            return m_CurrentState;
        }
    }
}
