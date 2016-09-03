using System.IO;
using System.Linq;
using UnityEngine;

public class MapFileManager
{
    private static MapFileManager _instance;

    public static MapFileManager Instance
    {
        get { return _instance ?? (_instance = new MapFileManager()); }
    }

#if UNITY_EDITOR || UNITY_STANDALONE
    private readonly string _mapDirectory = Path.Combine(Application.dataPath, "../Maps");
#else
    private readonly string _mapDirectory = Path.Combine(Application.persistentDataPath, "Maps");
#endif
    private const string MapFileExtension = "map";

    private MapFileManager()
    {
        if (!Directory.Exists(_mapDirectory))
        {
            Directory.CreateDirectory(_mapDirectory);
        }
    }

    public string[] GetAllMapFilenames()
    {
        var filenameList = Directory.GetFiles(_mapDirectory, string.Format("*.{0}", MapFileExtension)).Select<string, string>(Path.GetFileNameWithoutExtension).ToList();
        filenameList.Sort();
        return filenameList.ToArray();
    }

    public string GetPath(string filename)
    {
        return string.Format("{0}/{1}.{2}", _mapDirectory, filename, MapFileExtension);
    }
}
