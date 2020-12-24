using UnityEngine;

namespace RPG.Core
{
    public class CameraFacing : MonoBehaviour
    {
        // Update is called once per frame
        void LateUpdate()
        {
            //Set forward vector to camera forward vector
            transform.forward = Camera.main.transform.forward;
        }
    }
}
