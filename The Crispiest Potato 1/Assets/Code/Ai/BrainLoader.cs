using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.IO;
using System;

public static class BrainLoader
{
    static Assembly UserChosenAssembly = null;
    public static string ClassName = "VillagerBrain";

    static string AbsolutePath(string relativePath)
    {
        string folder = Application.dataPath + "/a";
        string lastFolder = Path.GetDirectoryName(folder);
        folder = Path.GetDirectoryName(lastFolder);
        return folder + "/" + relativePath;
    }

    public static void LoadAssembly(string relativePath)
    {
        try
        {
            Debug.Log("User has chosen " + AbsolutePath(relativePath));
            UserChosenAssembly = Assembly.LoadFrom(AbsolutePath(relativePath));
        }
        catch (Exception)
        {
            UserChosenAssembly = null;
        }

        brainType = null;
        brainExample = null;
    }

    public static bool FileExists(string relativePath)
    {
        return File.Exists(AbsolutePath(relativePath));
    }

    public static bool AssemblyLoaded()
    {
        return UserChosenAssembly != null;
    }

    static Type brainType;

    public static bool BrainClassExists()
    {
        if (UserChosenAssembly != null && brainType == null)
            brainType = UserChosenAssembly.GetType(ClassName);
        return brainType != null;
    }

    static AiProtocol.IBrain brainExample;
    public static bool BrainClassCorrect()
    { 
        
        if (UserChosenAssembly != null && brainExample == null)
        {
            brainExample = UserChosenAssembly.CreateInstance(ClassName) as AiProtocol.IBrain;
        }
        return brainExample != null;
    }

    
    public static AiProtocol.IBrain NewBrain()
    { 
        if (!BrainClassCorrect())
        {
            LoadAssembly("Assets/DefaultVillagerBrain.dll");
        }
        return (AiProtocol.IBrain)UserChosenAssembly.CreateInstance(ClassName);
    }
}
