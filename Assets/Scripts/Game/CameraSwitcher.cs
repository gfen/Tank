using System;
using UnityEngine;

namespace Game
{
    public class CameraSwitcher : MonoBehaviour
    {
        public event Action<Camera> CameraSwitched;

        private Camera _camera;

        void Start()
        {
            _camera = GetComponentInChildren<Camera>(true);

            _camera.gameObject.SetActive(false);
        }
        
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                if (!_camera.gameObject.activeSelf)
                {
                    _camera.gameObject.SetActive(true);

                    if (CameraSwitched != null) CameraSwitched(_camera);
                }
            }
        }
    }
}
