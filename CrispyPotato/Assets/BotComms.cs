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

public class BotComms : MonoBehaviour
{
    IVillageBot bot;

    void Start()
    {
        bot = new VillagerBot()
        {
            Info = new GameStateInfo()
            {
                SomeNumber = 1
            }
        };
    }

    void Update()
    {
        print("Bot knows: " + bot.Info.SomeNumber);
        print("Bot says: " + bot.GetDesiredAction().CurrentMood);
    }
}
