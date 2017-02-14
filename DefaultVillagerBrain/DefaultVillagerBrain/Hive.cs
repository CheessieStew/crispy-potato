using System;
using System.Collections.Generic;
using AiProtocol.Descriptions;


public class Hive
{
    private Faction faction;
    private Dictionary<EntityType, Dictionary<int, BaseDescription>> knownObjects = new Dictionary<EntityType, Dictionary<int, BaseDescription>>();

    public Hive(Faction faction)
    {
        this.faction = faction;
    }

    public void Notify(BaseDescription desc)
    {
        if (!knownObjects.ContainsKey(desc.GenericName))
            knownObjects.Add(desc.GenericName, new Dictionary<int, BaseDescription>());
        if (!knownObjects[desc.GenericName].ContainsKey(desc.EntityID))
            knownObjects[desc.GenericName].Add(desc.EntityID, desc);
        else
            knownObjects[desc.GenericName][desc.EntityID] = desc;
    }

    public VillageDescription MyVillage
    {
        get
        {
            if (!knownObjects.ContainsKey(EntityType.Village))
                return null;

            foreach (var village in knownObjects[EntityType.Village])
            {
                if (village.Value.Alignment == faction)
                    return village.Value as VillageDescription;
            }

            return null;
        }
    }

    public IEnumerable<BaseDescription> GetAll(EntityType type)
    {
        if (!knownObjects.ContainsKey(type))
            yield break;

        foreach (var eon in knownObjects[type])
            yield return eon.Value;
    }

    public int Count(EntityType type)
    {
        if (!knownObjects.ContainsKey(type))
            return 0;
        return knownObjects[type].Count;
    }
    
}


public static class HiveManager
{
    public static Random Generator = new Random();
    public static Dictionary<Faction, Hive> AllHives = new Dictionary<Faction, Hive>();

    static HiveManager()
    {
        AllHives.Add(Faction.Blue, new Hive(Faction.Blue));
        AllHives.Add(Faction.Red, new Hive(Faction.Red));
    }
}