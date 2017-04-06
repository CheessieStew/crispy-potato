using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class EntityManager : Singleton<EntityManager>
{
    protected EntityManager(){} // guarantee this will be always a singleton only - can't use the constructor!

    public EntityHash entityHash;
    public static EntityHash EntityHash
    {
        get
        {
            if (Instance != null)
                return Instance.entityHash;
            return null;
        }
    }
    public static IEnumerable<EntityBody> Entities
    {
        get
        {
            if (Instance != null)
                return Instance.entityHash.Select(a => a.Value);
            return null;
        }
    }

    public int MinimumNameLength;
    public int MaximumNameLength;
    System.Random random = new System.Random();
    string[] maleNames;
    string[] femaleNames;
    string[] surnames;


    int LastID = 1;
    public static int DispenseID()
    {
        return Instance.LastID++;
    }

    public static string DispenseName(bool isFemale)
    {
        string[] arr = isFemale 
            ? Instance.femaleNames 
            : Instance.maleNames;
        if (arr == null || arr.Length == 0)
            return "Czesuaw";

        StringBuilder res = new StringBuilder();
        int length = Instance.random.Next(
            Instance.MinimumNameLength,
            Instance.MaximumNameLength);
        for (int i = 0; i < length; i++)
            res.Append(arr[Instance.random.Next(arr.Length - 1)]);

        return char.ToUpper(res[0]) + res.ToString().Substring(1);

    }

    public static string DispenseSurname()
    { 
        string b = DispenseName(Instance.random.Next(1) == 1 ? true : false);
        return b + Instance
            .surnames[Instance.random.Next(Instance.surnames.Length - 1)];
    }

    public static EntityBody FindById(int entityID)
    {
        return Instance.entityHash[entityID];
    }

    void Initialize()
    {
        if (entityHash == null) entityHash = new EntityHash();
        TextAsset male = Resources.Load<TextAsset>("namesmale");
        TextAsset female = Resources.Load<TextAsset>("namesfemale");
        TextAsset sur = Resources.Load<TextAsset>("surnames");
        maleNames = male.text.Split('\n');
        femaleNames = female.text.Split('\n');
        surnames = sur.text.Split('\n');
    }

    void Start()
    {
        Initialize();
        
    }

    public static void SceneLoaded(bool IsMenu)
    {
        if (!IsMenu)
            EntityHash.Clear();
    }

}

[Serializable]
public class EntityDictionary : SerializableDictionary<int, EntityBody> {}

[Serializable]
public class EntityHash : EntityDictionary
{
    public override EntityBody this[int key]
    {
        get { return this[key, null]; }
        set { Insert(key, value, false); }
    }
}

