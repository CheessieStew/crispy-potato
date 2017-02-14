using System;
using System.Collections.Generic;
using AiProtocol.Descriptions;


public class Hive
{
    private Dictionary<Type, Dictionary<int, BaseDescription>> knownObjects;

    public void Notify(BaseDescription desc)
    {
        knownObjects[desc.Type][desc.EntityID] = desc;
    }
}


public static class HiveManager
{
    static Dictionary<Faction, Hive> Hives;
}