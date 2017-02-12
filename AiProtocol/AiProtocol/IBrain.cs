using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AiProtocol
{

    public interface IBrain
    {
        /// <summary>
        /// Set next action (based on user console)
        /// </summary>
        void SetNextAction(Command.BaseCommand command);
        /// <summary>
        /// Load a Talk 
        /// </summary>
        /// <param name="talk"></param>
        void Hear(Command.Talk talk);

        /// <summary>
        /// Load a list of entities that are visible to this entity.
        /// </summary>
        void See(IEnumerable<Descriptions.BaseDescription> descriptions);

        /// <summary>
        /// Load the detailed description of this entity.
        /// </summary>
        void Feel(Descriptions.BodilyFunctions functions);

        /// <summary>
        /// An infinite list of commands.
        /// </summary>
        IEnumerable<Command.BaseCommand> GetDecisions();
    }
}
