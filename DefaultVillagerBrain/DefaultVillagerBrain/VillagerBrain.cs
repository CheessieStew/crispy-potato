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

    private Queue<Talk> TalkMemory = new Queue<Talk>();
    public void Hear(Talk talk)
    {
        TalkMemory.Enqueue(talk);
    }

    private List<BaseDescription> Surroundings;
    public void See(IEnumerable<BaseDescription> descriptions)
    {
        Surroundings = new List<BaseDescription>(descriptions);
    }

    private BodilyFunctions Description;
    public void Feel(BodilyFunctions functions)
    {
        Description = functions;
    }

    private bool EnemyClose()
    {
        foreach (var eon in Surroundings)
        {
            if (eon.Alignment != Description.Alignment && eon.Type == villager)
                return true;
        }

        return false;
    }

    private double distance(BaseDescription a, BaseDescription b)
    {
        double dx = a.xCoord - b.xCoord, dy = a.yCoord - b.yCoord;
        return Math.Sqrt(dx * dx + dy * dy);
    }

    private BaseCommand AttackClosest()
    {
        BaseDescription closest = null;
        foreach (var eon in Surroundings)
        {
            if (eon.Alignment != Description.Alignment && eon.Type == villager)
                if (closest == null || distance(Description, eon) < distance(Description, closest))
                    closest = eon;
        }

        if (distance(Description, closest) < InteractionDistance)
            return new Interaction
            {
                CurrentMood = Mood.Angry,
                Style = InteractionStyle.Attack,
                TargetID = closest.EntityID
            };
        else
            return new Movement
            {
                CurrentMood = Mood.Angry,
                Style = MovementStyle.Run,
                xCoord = (int)closest.xCoord,
                yCoord = (int)closest.yCoord
            };
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

            if (EnemyClose())
                yield return AttackClosest();

            else if (Description.Energy * 100.0 / Description.MaxEnergy < 30)
                yield return SeekFood();

            else
                yield return Idle();
            //yield return new AiProtocol.Command.Movement(MovementStyle.Run, 10, 10);
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

