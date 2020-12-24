using System.Collections;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class SceneFader : MonoBehaviour
    {
        [SerializeField] float m_FadeInTime=1f;
        [SerializeField] float m_FadeOutTime=3f;

        CanvasGroup m_CanvasGrp;        //Referene to the canvas group on the object
        Coroutine m_ActiveRoutine = null;      //Referene to the active co-rouine

        private void Awake()
        {
            m_CanvasGrp = GetComponent<CanvasGroup>();
        }

        //Fade the scene out by increasing the alpha of the attach canvas group
        private IEnumerator FadeOutRoutine(float time)
        {
            while (m_CanvasGrp.alpha < 1)
            {
                if (time == -1)
                {
                    m_CanvasGrp.alpha += Time.deltaTime / m_FadeOutTime;
                }
                else
                {
                    m_CanvasGrp.alpha += Time.deltaTime / time;
                }

                //Wait one frame
                yield return null;
            }
        }

        //Fade the scene in by decreasing the alpha of the attach canvas group
        private IEnumerator FadeInRoutine(float time)
        {
            while (m_CanvasGrp.alpha > 0)
            {
                if (time == -1)
                {
                    m_CanvasGrp.alpha -= Time.deltaTime / m_FadeInTime;
                }
                else
                {
                    m_CanvasGrp.alpha -= Time.deltaTime / time;
                }

                //Wait one frame
                yield return null;
            }
        }


        ///////////////////////////// PUBLIC METHODS ////////////////////////////////////////////        


        //Fade out the scene
        public IEnumerator SceneFadeOut(float time)
        {
            if (m_ActiveRoutine != null)
            {
                StopCoroutine(m_ActiveRoutine);
            }
            m_ActiveRoutine = StartCoroutine(FadeOutRoutine(time));
            yield return m_ActiveRoutine;
        }

        //Fade in the scene
        public IEnumerator SceneFadeIn(float time)
        {
            if (m_ActiveRoutine != null)
            {
                StopCoroutine(m_ActiveRoutine);
            }
            m_ActiveRoutine = StartCoroutine(FadeInRoutine(time));
            yield return m_ActiveRoutine;
        }


        //Immediately Fade the scene out
        public void FadeOutImmediately()
        {
            m_CanvasGrp.alpha = 1;
        }
    }
}
