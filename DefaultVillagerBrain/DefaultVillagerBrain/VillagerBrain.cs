using System;
using System.Collections.Generic;
using AiProtocol.Command;
using AiProtocol.Descriptions;
using AiProtocol;



public class VillagerBrain : AiProtocol.IBrain
{
    // action enforced by the player
    BaseCommand nextAction = null;

    public void SetNextAction(BaseCommand action)
    {
        nextAction = action;
    }

    public Queue<Talk> TalkMemory = new Queue<Talk>();
    public void Hear(Talk talk)
    {
        TalkMemory.Enqueue(talk);
    }

    public void See(IEnumerable<BaseDescription> descriptions)
    {
        // analize the surroundings
    }

    public void Feel(BodilyFunctions functions)
    {
        // analize the body's state
    }

    public IEnumerable<BaseCommand> GetDecisions()
    {
        while (true)
        {
            // analize the talks we heard since we returned our last instruction
            TalkMemory.Clear();
            if (nextAction != null)
            {
                BaseCommand tmp = nextAction;
                nextAction = null;
                yield return tmp;
                continue;
            }
            yield return new AiProtocol.Command.Movement(MovementStyle.Run, 10, 10);
        }
    }

    public void Initialize(BrainGenetics genetics)
    {
        if (genetics is MyBrainGenetics)
        {
            // initialize internal state according to the genetics
        }
    }

    public BrainGenetics GetGeneticMaterial()
    {
        return new MyBrainGenetics()
        {
            //what do we want to be inherited?
        };
    }
}

public class MyBrainGenetics : AiProtocol.BrainGenetics
{
    //representation of this brain's internal state that we wish to be inherited
    public override BrainGenetics Cross(BrainGenetics other)
    {
        if (other is MyBrainGenetics)
            return new MyBrainGenetics()
            {
                //...how are they crossed?
            };
        else return null;
    }
}

