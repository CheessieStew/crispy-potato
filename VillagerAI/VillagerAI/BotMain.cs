using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VillageBotInterface;

namespace VillagerAI
{
    class VillagerBot : IVillageBot
    {
        public GameStateInfo Info
        {
            get;
            set;
        }

        public VillagerBot()
        {
            Info = new GameStateInfo();
        }

        public GameAction GetDesiredAction()
        {
            GameAction action = new GameAction
            {
                SomeNumber = Info.SomeNumber + 1
            };
            return action;
        }

    }
}
