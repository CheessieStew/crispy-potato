using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.CodeDom.Compiler;
using System.Reflection;
using VillageBotInterface;
using Microsoft.CSharp;
using System;
using System.Text;
using System.IO;

public class BotComms : MonoBehaviour {

    IVillageBot bot;
    GameStateInfo info;
    static Assembly _assembly;
    static Assembly assembly
    {
        get
        {
            if (_assembly == null)
            {
                StreamReader reader = new StreamReader(@"Assets/BotMain.csnvm");
                _assembly = CompileAssembly(reader.ReadToEnd());
            }
            return _assembly;
        }
    }


    // Use this for initialization
    void Start()
    {
        info = new GameStateInfo();
        info.SomeNumber = 5;
        bot = Activator.CreateInstance(assembly.GetType("VillagerBot")) as IVillageBot;
        if (bot == null)
            throw new Exception("Wat");
        bot.Info = info;
    }

    // Update is called once per frame
    void Update()
    {
        print("Bot knows: " + bot.Info.SomeNumber);
        print("Bot says: " + bot.GetDesiredAction().SomeNumber);
    }

    public static Assembly CompileAssembly(string source)
    {
        var provider = new CSharpCodeProvider();
        var param = new CompilerParameters();

        // Add ALL of the assembly references
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            param.ReferencedAssemblies.Add(assembly.Location);
        }

        param.GenerateExecutable = false;
        param.GenerateInMemory = true;

        // Compile the source
        var result = provider.CompileAssemblyFromSource(param, source);

        if (result.Errors.Count > 0)
        {
            var msg = new StringBuilder();
            foreach (CompilerError error in result.Errors)
            {
                msg.AppendFormat("Error ({0}): {1}\n",
                    error.ErrorNumber, error.ErrorText);
            }
            throw new Exception(msg.ToString());
        }

        // Return the assembly
        return result.CompiledAssembly;
    }



}
