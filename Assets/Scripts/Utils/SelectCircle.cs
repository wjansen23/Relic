using UnityEngine;
using RPG.Combat;
using RPG.Attributes;

namespace RPG.UI
{
    /// <summary>
    /// This class is responsible for displaying a circle underneath a character when moused over
    /// </summary>
    public class SelectCircle : MonoBehaviour
    {
        [SerializeField] ParticleSystem m_ParticleSystem = null;                 //Reference to particle system
        [SerializeField] Color m_Color1 = new Color(100, 20, 10);               //First color of gradient
        [SerializeField] Color m_Color2 = new Color(220, 20, 20);                 //Seconde color of gradient
        [SerializeField] Color m_Color3 = new Color(195, 63, 66);                //Third color of gradient
        [SerializeField] Color m_Color4 = new Color(197, 113, 111);              //Forth color of gradient

        ParticleSystem.ColorOverLifetimeModule m_ColorModule;                   //Holds reference to color modeul
        GameObject m_Player;                                                    //Reference to player.
        CharacterCombat m_Combat;                                               //Reference to character combat component
        Health m_Health;                                                        //Reference to Health component.

        Gradient m_Gradient;                                                    //color over lifetime gradient

        bool m_BeingAttacked = false;                                           //Used to determine whether to show the circle if being attacked or not

        /// <summary>
        /// Sets up necessary component references along with the particle system
        /// </summary>
        void Start()
        {
            m_Player = GameObject.FindWithTag("Player");
            m_Combat = GetComponent<CharacterCombat>();
            m_Health = GetComponent<Health>();

            //Only initialize if particle system actually exists
            if (m_ParticleSystem != null)
            {
                //Get the color module.
                m_ColorModule = m_ParticleSystem.colorOverLifetime;

                //Create gradient and setit.
                m_Gradient = new Gradient();
                SetGradient(m_Color1, m_Color2, m_Color3, m_Color4);
            }
        }

        /// <summary>
        /// Checks each update to see if a the cursor is over a character and whether or not the turn the circle on or off
        /// </summary>
        private void Update()
        {
            if (m_Health.IsDead())
            {
                if (m_ParticleSystem.isPlaying) TurnOffCircle();
                return;
            }

            if (m_Player.GetComponent<CharacterCombat>().GetTarget() != m_Health && m_BeingAttacked)
            {
                TurnOffCircle();
                m_BeingAttacked = false;
            }

            if (m_Player.GetComponent<CharacterCombat>().GetTarget() == m_Health && !m_ParticleSystem.isPlaying)
            {
                TurnOnCircle();
            }
        }

        /// <summary>
        /// Called whenever the mouse hovers over a gameObject with a collider and this script
        /// </summary>
        void OnMouseOver()
        {
            //Make sure not dead
            if (m_Health.IsDead()) return;
            TurnOnCircle();
            //Debug.Log("In mouse over");
        }

        /// <summary>
        /// The mouse is no longer hovering over the GameObject
        /// </summary>
        void OnMouseExit()
        {
            //Make sure not dead
            if (m_Health.IsDead()) return;

            if (m_Player.GetComponent<CharacterCombat>().GetTarget() == GetComponent<Health>())
            {
                m_BeingAttacked = true;
                return;
            }
            TurnOffCircle();
        }

        /// <summary>
        /// Displays the character circle
        /// </summary>
        void TurnOnCircle()
        {
            if (m_ParticleSystem == null) return;
            if (!m_ParticleSystem.isPlaying)
            {
                m_ParticleSystem.Play();
            }
        }

        /// <summary>
        /// Stops displaying the circle
        /// </summary>
        void TurnOffCircle()
        {
            if (m_ParticleSystem == null) return;
            m_ParticleSystem.Stop();
        }

        /// <summary>
        /// Puts a new four color gradient on the object.
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <param name="c3"></param>
        /// <param name="c4"></param>
        void SetGradient(Color c1, Color c2, Color c3, Color c4)
        {
            // Reduce the alpha
            float alpha = 1f;
            m_Gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(c1, 0.0f), new GradientColorKey(c2, .35f), new GradientColorKey(c3, .70f), new GradientColorKey(c4, 1f) },
                new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, .65f), new GradientAlphaKey(0, 1.0f) }
                );

            // Apply the changed gradient.
            m_ColorModule.color = m_Gradient;
        }
    }
}
