using System;
using UnityEngine;

namespace VillageBotInterface
{
    public class GameStateInfo
    {
        public int SomeNumber;
    }

    public class GameAction
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

        GameAction GetDesiredAction();
    }


    
}
