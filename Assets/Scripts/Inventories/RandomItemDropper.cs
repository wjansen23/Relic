using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RPG.Stats;


namespace RPG.Inventories
{
    public class RandomItemDropper : ItemDropper
    {
        [Tooltip("How far can the pickup be scattered from the dropper")]
        [SerializeField] float m_DropDistance = 1;
        [SerializeField] LootDropTable m_LootDropTable;

        
        float m_MeshHitDistance = .1f;
        const int HIT_ATTEMPTS = 30;


        ///////////////////////////// PROTECTED METHODS //////////////////////////////////////////// 
        

        /// <summary>
        /// Returns the drop location for an item.
        /// </summary>
        /// <returns></returns>
        protected override Vector3 GetDropLocation()
        {
            //Try to find a hit point
            for (int i = 0; i < HIT_ATTEMPTS; i++)
            {
                //Compute a random point within the sphere
                Vector3 randompoint = transform.position + Random.insideUnitSphere * m_DropDistance;
                NavMeshHit hit;

                //Check to see if hit location is near the nav mesh
                if (NavMesh.SamplePosition(randompoint, out hit, m_MeshHitDistance, NavMesh.AllAreas))
                {
                    return hit.position;
                }
            }

            return transform.position;
        }

        ///////////////////////////// PUBLIC METHODS //////////////////////////////////////////// 

        public void RandomDrop()
        {
            //Get reference to base stats and compute number of drops
            var baseStatsComp = GetComponent<BaseStats>();
            var drops = m_LootDropTable.GetRandomDrops(baseStatsComp.GetLevel());
            
            //Iterrate through the return drops and spawn them.
            foreach (var drop in drops)
            {
                DropItem(drop.item, drop.num);
            }
        }
    }
}
