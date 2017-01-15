using System;
using UnityEngine;

public class GameStateInfo
{
    public int SomeNumber;
}


public interface IVillageBot
{
    GameStateInfo Info
    {
        get;
        set;
    }

    Action.GameAction GetDesiredAction();
}



