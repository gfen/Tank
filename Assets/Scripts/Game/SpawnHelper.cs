using UnityEngine;

namespace Game
{
    public static class SpawnHelper
    {
        public static GameObject Spawn(GameObject gameObject, Transform spawnPoint, Transform parent)
        {
            DestroyObstaclesSurroundSpawnPoint(new Vector2(spawnPoint.position.x, spawnPoint.position.z));

            var obj = (GameObject)Object.Instantiate(gameObject, spawnPoint.position, spawnPoint.rotation);
            obj.transform.parent = parent;

            return obj;
        }

        private static void DestroyObstaclesSurroundSpawnPoint(Vector2 point)
        {
            var points = new[]
            {
                new Vector3(point.x - 0.5f, 10f, point.y - 0.5f),
                new Vector3(point.x - 0.5f, 10f, point.y + 0.5f),
                new Vector3(point.x + 0.5f, 10f, point.y - 0.5f),
                new Vector3(point.x + 0.5f, 10f, point.y + 0.5f),
            };
            for (int i = 0; i < points.Length; i++)
            {
                var ray = new Ray(points[i], Vector3.down);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.tag == "Obstacle")
                    {
                        Object.Destroy(hit.transform.gameObject);
                    }
                }
            }
        }

        public static void ClearAll(Transform parent)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                Object.Destroy(parent.GetChild(i).gameObject);
            }
        }
    }
}
