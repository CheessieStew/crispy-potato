using System;
using System.Collections.Generic;
using AiProtocol.Command;
using AiProtocol.Descriptions;
using AiProtocol;


/*
    This is an example implementation of the IBrain interface.
    It makes the villager simply run towards the center of the map,
    but it shows how to integrate with the Protocol
*/
public class VillagerBrain : AiProtocol.IBrain
{
    static AiProtocol.GameRules rules;

    // will be called on each new Brain after it's created, 
    // before asking for the first command
    public void SetRules(AiProtocol.GameRules gameRules)
    {
        // the time at which the rules were generated is a good way to determine if
        // the game was restarted, with possibly different rules
        if (rules.TimeWhenGenerated != gameRules.TimeWhenGenerated)
            rules = gameRules;
    }

    // action enforced by the player
    BaseCommand nextAction = null;

    // This will be called whenever the player enters a correct 
    // action in the console while this Agent's Villager is selected
    public void SetNextAction(BaseCommand action)
    {
        // we can choose to obey the player's suggestion or ignore it
        // in this case, we'll obey it
        nextAction = action;
    }


    public List<Words> TalkMemory = new List<Words>();

    // Will be called whenever an entity speaks in 
    // hearing range of this Agent's Villager
    public void Hear(Words words)
    {
        // Since this method isn't simply called before requesting a new
        // command, we'll enqueue the words to analyze them together with
        // all the other senses' information
        TalkMemory.Add(words);
    }

    // this is just an example of how words can be handled
    void AnalyzeWords(List<Words> words)
    {
        // analyze the words we've heard since giving last command
    }

    // Will be called before each time a new command is requested
    public void See(IEnumerable<BaseDescription> descriptions)
    {
        // analyze the surroundings
    }

    // Will be called before each time a new command is requested

    public void Feel(BodilyFunctions functions)
    {
        // analyze the body's state
    }

    // This should be an infinite collection of commands
    // for the body to follow. See() and Feel() will be called
    // before each time a next command is needed
    public IEnumerable<BaseCommand> GetDecisions()
    {
        while (true)
        {
            AnalyzeWords(TalkMemory);
            TalkMemory.Clear();
            if (nextAction != null)
            {
                // again, we can ignore the player's suggested action
                // in this case we'll obey it
                BaseCommand tmp = nextAction;
                nextAction = null;
                yield return tmp;
                continue;
            }
            // choose an action based on what we heard, saw and felt
            // if the action is illegal we'll be asked for another one
            // we don't need to worry about obstacle avoidance, the villagers have
            // their own pathfinding
            yield return new AiProtocol.Command.Movement(MovementStyle.Run, 0, 0);
        }
    }

    public void Initialize(IBrainGenetics genetics)
    {
        if (genetics is MyBrainGenetics)
        {
            // initialize internal state according to the genetics
            // if we wish to inherit anything - maybe we don't want any
            // brain inheritance
        }
    }

    // return a description of this brain that can be crossed with another's
    // and used to create a new one
    public IBrainGenetics GetGeneticMaterial()
    {
        // we can simply return null if we don't want any brain inheritance
        return new MyBrainGenetics();
    }
}


//Example implementation of custom Words object that make it possible for agents to talk about anything
public class ExampleCustomWords : Words
{
    // Information we wish to be passed
    // we don't need to fill in TalkerID, source coords or language, the game will do it for us
}

//Example implementation of custom BrainGenetics that can make brain inheritance possible
public class MyBrainGenetics : IBrainGenetics
{
    // how do we want to store the brain's qualities
    // so they can be inherited?

    // how do we cross them?
    // "this" will always be the mother, while "other" will always be the father
    public IBrainGenetics Cross(IBrainGenetics other)
    {
        if (other is MyBrainGenetics)
            return new MyBrainGenetics();
        else return null;
    }
}

