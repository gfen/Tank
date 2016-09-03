using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MapEditor
{
    public class MapManager : MonoBehaviour
    {
        public GameObject MapGameObject;

        public InputField FilenameInputField;

        public LineRenderer Line;

        private ObstacleType _currentObstacleType;

        private Action<RaycastHit> _currentHitAction;

        private Map _map;

        void Start()
        {
            _currentObstacleType = ObstacleType.Brick;
            _currentHitAction = InstantiateObstacle;

            _map = new Map(MapGameObject);

            StartCoroutine(DrawLines());
        }

        private IEnumerator DrawLines()
        {
            yield return null;

            var origin = _map.Origin;

            for (int i = 0; i <= _map.Width; i++)
            {
                DrawLine(new Vector3(origin.x + i, 2f, origin.z), new Vector3(origin.x + i, 2f, origin.z + _map.Height));
            }

            for (int i = 0; i <= _map.Height; i++)
            {
                DrawLine(new Vector3(origin.x, 2f, origin.z + i), new Vector3(origin.x + _map.Width, 2f, origin.z + i));
            }
        }

        private void DrawLine(Vector3 start, Vector3 end)
        {
            var line = Instantiate<LineRenderer>(Line);
            line.SetPosition(0, start);
            line.SetPosition(1, end);
        }

        void Update()
        {
            if (Input.GetMouseButton(0))
            {
                var mousePosition = Input.mousePosition;
                
                var rayFromScreen = Camera.main.ScreenPointToRay(new Vector3(mousePosition.x, mousePosition.y));
                Hit(rayFromScreen, _currentHitAction);
            }
        }

        private void Save(string filename)
        {
            var path = MapFileManager.Instance.GetPath(filename);

            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                _map.Export(fileStream);
            }
        }

        private void Hit(Ray ray, Action<RaycastHit> hitAction)
        {
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                hitAction(hit);
            }
        }

        private void InstantiateObstacle(RaycastHit hit)
        {
            if (_map.IsBackgroundCollider(hit.collider))
            {
                var position = hit.point;
                
                _map.AddObstacle(_currentObstacleType, new Vector3(Mathf.Floor(position.x) + 0.5f, position.y + 0.5f, Mathf.Floor(position.z) + 0.5f));
            }
        }

        private void DestroyObstacle(RaycastHit hit)
        {
            if (hit.transform.CompareTag("Obstacle"))
            {
                Destroy(hit.transform.gameObject);

                _map.ClearMark(hit.point);
            }
        }

        public void OnToggleBrickValueChanged(bool isChecked)
        {
            if (isChecked)
            {
                _currentObstacleType = ObstacleType.Brick;
                _currentHitAction = InstantiateObstacle;
            }
        }

        public void OnToggleSteelValueChanged(bool isChecked)
        {
            if (isChecked)
            {
                _currentObstacleType = ObstacleType.Steel;
                _currentHitAction = InstantiateObstacle;
            }
        }

        public void OnToggleGrassValueChanged(bool isChecked)
        {
            if (isChecked)
            {
                _currentObstacleType = ObstacleType.Grass;
                _currentHitAction = InstantiateObstacle;
            }
        }

        public void OnToggleWaterValueChanged(bool isChecked)
        {
            if (isChecked)
            {
                _currentObstacleType = ObstacleType.Water;
                _currentHitAction = InstantiateObstacle;
            }
        }

        public void OnToggleNoneValueChanged(bool isChecked)
        {
            if (isChecked)
            {
                _currentHitAction = DestroyObstacle;
            }
        }

        public void OnSaveButtonClick()
        {
            var filename = FilenameInputField.text;
            if (string.IsNullOrEmpty(filename)) return;

            Save(filename);
        }

        public void OnResetButtonClick()
        {
            _map.Reset();
        }

        public void OnExitButtonClick()
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
