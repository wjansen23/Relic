using UnityEngine;

namespace RPG.Core
{
    //This script makes the camera follow the player. Assumes an orthigraphic projection.
    public class FollowCamera : MonoBehaviour
    {
        //Camera Controls Limits
        [SerializeField] float m_MinZoomLevel = 2f;     //How close can the camera zoom in
        [SerializeField] float m_MaxZoomLevel = 10f;    //How far can the camera zoom out
        [SerializeField] float m_ZoomScale = 1f;        //Rate at which zoom is changed
        [SerializeField] float m_RotateScale = 50f;     //Rate at which camera is rotated.
        [SerializeField] Transform target;              //What is the camer pointing to.

        //Non-Serlized Fields
        Camera m_Camera;                                //The camera that is controlled by the script
        float m_BaseFieldOfView;                        //Holds the default zoom level for the camera
        Transform m_Transform;                          //Holds the follow camera transform. Saves on Get Component calls.



        // Start is called before the first frame update
        void Start()
        {
            //Setup script variables.
            m_Camera = GetComponentInChildren<Camera>();
            m_Transform = GetComponent<Transform>();
            m_BaseFieldOfView = m_Camera.orthographicSize;
        }

        // Update is called once per frame and AFTER update.  Camera updates should happen here.
        void LateUpdate()
        {
            //Keep the camera point to the players position.
            transform.position = target.position;

            if (Input.mouseScrollDelta.y != 0) ChangeZoomLevel(Input.mouseScrollDelta.y);

            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.E))
            {
                RotateCameraYAxis(m_RotateScale);
            }

            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.Q))
            {
                RotateCameraYAxis(-m_RotateScale);
            }

        }

        //Allows for limited zooming in or out based upon the max and min zoom level
        void ChangeZoomLevel(float delta)
        {
            float currentzoom = m_Camera.orthographicSize;
            float zoomchange = delta * m_ZoomScale;

            if (zoomchange < 0)
            {
                m_Camera.orthographicSize = Mathf.Min(m_MaxZoomLevel, currentzoom - zoomchange);
            }

            if (zoomchange > 0)
            {
                m_Camera.orthographicSize = Mathf.Max(m_MinZoomLevel, currentzoom - zoomchange);
            }

        }

        //Rotates the camera around its Y axis based upon the delta and scaled by time.deltatime.
        void RotateCameraYAxis(float delta)
        {
            m_Transform.Rotate(0, delta * Time.deltaTime, 0, Space.World);
        }
    }
}
