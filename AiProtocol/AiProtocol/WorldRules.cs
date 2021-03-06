﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AiProtocol
{
    public struct VillageRules
    {
        /// <summary>
        /// How much wood can be stored in magazine per Village's level
        /// </summary>
        public int WoodPerLevel;
        /// <summary>
        /// How much food can be stored in magazine per Village's level
        /// </summary>
        public int FoodPerLevel;
        /// <summary>
        /// How much food is consumed each time the Village gives birth to a new Villager
        /// </summary>
        public int FoodPerChild;
        /// <summary>
        /// Should new villagers be created only when there's genetic material available?
        /// </summary>
        public bool ParentsRequired;
        /// <summary>
        /// The minimum strength for a randomly created villager
        /// </summary>
        public int LowStrength;
        /// <summary>
        /// The maximum strength for a randomly created villager
        /// </summary>
        public int HighStrength;
        /// <summary>
        /// The minimum agillity for a randomly created villager
        /// </summary>
        public int LowAgility;
        /// <summary>
        /// The maximum agillity for a randomly created villager
        /// </summary>
        public int HighAgility;
        /// <summary>
        /// The minimum intelligence for a randomly created villager
        /// </summary>
        public int LowIntelligence;
        /// <summary>
        /// The maximum intelligence for a randomly created villager
        /// </summary>
        public int HighIntelligence;

    }

    public struct GameRules
    {
        /// <summary>
        /// In-game time when these rules were generated
        /// </summary>
        public float TimeWhenGenerated;

        public VillageRules RedVillageRules;
        public VillageRules BlueVillageRules;
        /// <summary>
        /// The maximum distance between two Entities that allows an interaction
        /// </summary>
        public float MaxInteractionDistance;
        /// <summary>
        /// The maximum distance at which a spoken message is heard (a yelled message will be heard at 4 times this distance)
        /// </summary>
        public float MaxHearingDistance;
        /// <summary>
        /// How much time it takes to complete a Movement command (has no effect on effective speed)
        /// </summary>
        public float WalkingWindowSize;
        /// <summary>
        /// How much time it takes for a cut down/destroyed tree to regrow
        /// </summary>
        public float TreeRegrowingTime;
    }
}
