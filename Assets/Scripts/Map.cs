using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public class Map
{
    private readonly Transform _mapTransform;

    private readonly Vector3 _origin;

    public Vector3 Origin { get { return _origin; } }

    private readonly int _width;

    public int Width { get { return _width; } }

    private readonly int _height;

    public int Height { get { return _height; } }

    public event Action<GameObject> ObstacleSpawned;

    private readonly ObstacleType[] _grid;

    private readonly Dictionary<ObstacleType, GameObject> _typeObjectDictionary;

    private readonly Collider _backgroundCollider;

    private readonly Vector3[] _surroundHomePoints;

    private readonly Transform _obstacles;

    public Map(GameObject mapGameObject)
    {
        _mapTransform = mapGameObject.transform;
        _obstacles = _mapTransform.Find("Obstacles");
        var background = _mapTransform.Find("Background");
        _backgroundCollider = background.GetComponent<Collider>();
        var mesh = background.GetComponent<MeshFilter>().sharedMesh;
        _origin = Vector3.Scale(mesh.bounds.min, background.localScale);
        _width = Mathf.RoundToInt(mesh.bounds.size.x * background.localScale.x);
        _height = Mathf.RoundToInt(mesh.bounds.size.z * background.localScale.z);
        _grid = new ObstacleType[_width * _height];
        Reset();

        _surroundHomePoints = InitSurroundHomePoints();

        var obstacleManager = mapGameObject.GetComponent<ObstacleTypeObjects>();
        _typeObjectDictionary = obstacleManager.TypeObjects.ToDictionary(obstacleTypeObject => obstacleTypeObject.Type, obstacleTypeObject => obstacleTypeObject.Object);
    }

    private Vector3[] InitSurroundHomePoints()
    {
        var home = _mapTransform.Find("Home");
        var homePosition = home.position;
        var detectionDirections = new[]
        {
            new Vector3(1.5f, 0f, -1.5f), new Vector3(1.5f, 0f, -0.5f), new Vector3(1.5f, 0f, 0.5f),
            new Vector3(1.5f, 0f, 1.5f), new Vector3(0.5f, 0f, 1.5f), new Vector3(-0.5f, 0f, 1.5f),
            new Vector3(-1.5f, 0f, 1.5f), new Vector3(-1.5f, 0f, 0.5f), new Vector3(-1.5f, 0f, -0.5f),
            new Vector3(-1.5f, 0f, -1.5f), new Vector3(-0.5f, 0f, -1.5f), new Vector3(0.5f, 0f, -1.5f),
        };
        List<Vector3> points = new List<Vector3>();
        for (int i = 0; i < detectionDirections.Length; i++)
        {
            var point = homePosition + detectionDirections[i];
            if (point.x > _origin.x && point.x < _origin.x + _width && point.z > _origin.z &&
                point.z < _origin.z + _height)
            {
                points.Add(point);
            }
        }
        return points.ToArray();
    }

    public void AddObstacle(ObstacleType obstacleType, Vector3 position)
    {
        SetObstacleType(position, obstacleType);
        
        InstantiateObstacle(obstacleType, position);
    }

    public void ClearMark(Vector3 position)
    {
        SetObstacleType(position, ObstacleType.None);
    }

    public void Build()
    {
        for (int i = 0; i < _grid.Length; i++)
        {
            var type = _grid[i];
            if (type == ObstacleType.None) continue;

            var offsetZ = i / _width;
            var offsetX = i - offsetZ * _width;
            var position = new Vector3(_origin.x + offsetX + 0.5f, _origin.y + 0.5f, _origin.z + offsetZ + 0.5f);

            AddObstacle(type, position);
        }
    }

    public void Reset()
    {
        for (int i = 0; i < _obstacles.childCount; i++)
        {
            Object.Destroy(_obstacles.GetChild(i).gameObject);
        }

        for (int i = 0; i < _grid.Length; i++)
        {
            _grid[i] = ObstacleType.None;
        }
    }

    public bool IsBackgroundCollider(Collider collider)
    {
        return _backgroundCollider == collider;
    }

    public void Export(Stream stream)
    {
        using (var writer = new BinaryWriter(stream))
        {
            for (int i = 0; i < _grid.Length; i++)
            {
                writer.Write((int)_grid[i]);
            }
        }
    }

    public void Import(Stream stream)
    {
        using (var reader = new BinaryReader(stream))
        {
            for (int i = 0; i < _grid.Length; i++)
            {
                var value = reader.ReadInt32();
                _grid[i] = (ObstacleType)value;
            }
        }
    }

    public void BuildSurroundHome(ObstacleType obstacleType, Action<GameObject> obstacleBuilded)
    {
        for (int i = 0; i < _surroundHomePoints.Length; i++)
        {
            var obstacle = InstantiateObstacle(obstacleType, _surroundHomePoints[i]);
            if (obstacleBuilded != null) obstacleBuilded(obstacle);
        }
    }

    public void ClearSurroundHome()
    {
        for (int i = 0; i < _surroundHomePoints.Length; i++)
        {
            var ray = new Ray(_surroundHomePoints[i] + Vector3.up, Vector3.down);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo))
            {
                if (hitInfo.transform.CompareTag("Obstacle"))
                {
                    Object.Destroy(hitInfo.transform.gameObject);
                }
            }
        }
    }

    private void SetObstacleType(Vector3 position, ObstacleType type)
    {
        var offset = position - _origin;
        var offsetX = Mathf.FloorToInt(offset.x);
        var offsetZ = Mathf.FloorToInt(offset.z);
        _grid[offsetZ * _width + offsetX] = type;
    }

    public GameObject InstantiateObstacle(ObstacleType obstacleType, Vector3 position)
    {
        var obstacle = Object.Instantiate<GameObject>(_typeObjectDictionary[obstacleType]);
        obstacle.transform.position = position;
        obstacle.transform.parent = _obstacles;

        if (ObstacleSpawned != null) ObstacleSpawned(obstacle);

        return obstacle;
    }
}

public enum ObstacleType
{
    None = 0,
    Brick = 1,
    Steel = 2,
    Grass = 3,
    Water = 4,
}
