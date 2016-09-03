using System;
using UnityEngine;

public class ObstacleTypeObjects : MonoBehaviour
{
    public ObstacleTypeObject[] TypeObjects;
}

[Serializable]
public class ObstacleTypeObject
{
    public ObstacleType Type;

    public GameObject Object;
}
