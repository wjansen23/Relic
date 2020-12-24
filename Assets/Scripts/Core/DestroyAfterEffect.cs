using UnityEngine;

namespace RPG.Core
{
    //Destroys a an effect after it has play
    public class DestroyAfterEffect : MonoBehaviour
    {
        [SerializeField] GameObject m_TargetToDestroy = null;       //Object to be destroyed.  Useful for destroying the parent object of an effect.


        // Update is called once per frame
        void Update()
        {
            if (!GetComponent<ParticleSystem>().IsAlive())
            {
                if (m_TargetToDestroy != null)
                {
                    Destroy(m_TargetToDestroy);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
