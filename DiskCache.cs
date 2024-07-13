using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

public class DiskCache
{
    private string cacheDirectory;

    // Konstruktor, der das Verzeichnis für den Cache festlegt
    public DiskCache(string cacheDirectory)
    {
        this.cacheDirectory = cacheDirectory;

        if (!Directory.Exists(cacheDirectory))
        {
            Directory.CreateDirectory(cacheDirectory);
        }
    }

    // Methode zum Speichern des Caches auf der Festplatte
    public void SaveCache(Dictionary<string, DataTable> cache)
    {
        string cacheFilePath = Path.Combine(cacheDirectory, $"cache_{DateTime.Now:yyyyMMdd_HHmmss}.bin");

        using (FileStream stream = new FileStream(cacheFilePath, FileMode.Create))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, cache);
        }
    }

    // Methode zum Laden des Caches von der Festplatte
    public Dictionary<string, DataTable> LoadCache()
    {
        string[] cacheFiles = Directory.GetFiles(cacheDirectory, "cache_*.bin");

        if (cacheFiles.Length > 0)
        {
            string latestCacheFile = cacheFiles.OrderByDescending(f => f).First();

            using (FileStream stream = new FileStream(latestCacheFile, FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return (Dictionary<string, DataTable>)formatter.Deserialize(stream);
            }
        }
        return new Dictionary<string, DataTable>();
    }

    // In der Klasse DiskCache hinzufügen
    public void Clear()
    {
        foreach (var file in Directory.GetFiles(cacheDirectory, "cache_*.bin"))
        {
            File.Delete(file);
        }
    }



}
