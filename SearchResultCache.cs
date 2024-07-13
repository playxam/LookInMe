using System.Collections.Generic;
using System.Data;

public class SearchResultCache
{
    private Dictionary<string, DataTable> cache = new Dictionary<string, DataTable>();

    // Methode zum Abrufen eines Eintrags aus dem Cache
    public DataTable Get(string searchTerm)
    {
        if (cache.ContainsKey(searchTerm))
        {
            return cache[searchTerm];
        }
        return null;
    }

    // Methode zum Hinzufügen eines Eintrags zum Cache
    public void Add(string searchTerm, DataTable results)
    {
        if (!cache.ContainsKey(searchTerm))
        {
            cache.Add(searchTerm, results);
        }
    }

    // Methode zur Überprüfung, ob ein Eintrag im Cache vorhanden ist
    public bool Contains(string searchTerm)
    {
        return cache.ContainsKey(searchTerm);
    }

    // Methode zum Abrufen aller Einträge im Cache
    public Dictionary<string, DataTable> GetAll()
    {
        return cache;
    }

    // In der Klasse SearchResultCache hinzufügen
    public void Clear()
    {
        cache.Clear();
    }


}
