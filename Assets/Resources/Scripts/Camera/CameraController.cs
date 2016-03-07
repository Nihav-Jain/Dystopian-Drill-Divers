using UnityEngine;
using System.Collections;

namespace cameraControl
{
    public class CameraController : MonoBehaviour
    {
        public float DampTime;
        public float DesiredCameraZoom;
        public bool StartZoom;

        private float ZoomSpeed;
        private Camera mainCamera;


        // Use this for initialization
        void Start()
        {
            mainCamera = gameObject.GetComponent<Camera>();
        }

        // Update is called once per frame
        void Update()
        {
            if (StartZoom && mainCamera.orthographicSize > DesiredCameraZoom)
                Zoom();
            if(StartZoom && mainCamera.orthographicSize <= DesiredCameraZoom+0.01f)
            {
                StartZoom = false;
                mainCamera.orthographicSize = DesiredCameraZoom;
            }
        }

        void Zoom()
        {

            mainCamera.orthographicSize = Mathf.SmoothDamp(mainCamera.orthographicSize, DesiredCameraZoom, ref ZoomSpeed, DampTime);


        }
    }
}