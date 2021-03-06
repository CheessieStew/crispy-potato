﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AiProtocol
{

    public interface IBrain
    {

        /// <summary>
        /// Load the world's rules
        /// </summary>
        void SetRules(GameRules rules);

        /// <summary>
        /// Set next action (based on user console)
        /// </summary>
        void SetNextAction(Command.BaseCommand command);

        /// <summary>
        /// Load a Talk 
        /// </summary>
        void Hear(Words words);

        /// <summary>
        /// Load a list of entities that are visible to this entity
        /// </summary>
        void See(IEnumerable<Descriptions.BaseDescription> descriptions);

        /// <summary>
        /// Load the detailed description of this entity
        /// </summary>
        void Feel(Descriptions.BodilyFunctions functions);

        /// <summary>
        /// An infinite list of commands.
        /// </summary>
        IEnumerable<Command.BaseCommand> GetDecisions();

        /// <summary>
        /// Initalize a brain with genetics derived from parents
        /// </summary>
        void Initialize(IBrainGenetics genetics);

        /// <summary>
        /// Get the brain's genetic material so some aspects of it can be inherited by this Villager's children
        /// </summary
        IBrainGenetics GetGeneticMaterial();

    }

    public interface IBrainGenetics
    {
        /// <summary>
        /// Cross this genetic material with another so a new brain can be created through IBrain.
        /// </summary>
        IBrainGenetics Cross(IBrainGenetics other);
    }
}
