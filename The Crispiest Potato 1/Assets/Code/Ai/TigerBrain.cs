using System;
using System.Collections.Generic;
using System.Linq;
using AiProtocol.Descriptions;
using AiProtocol.Command;
using UnityEngine;
using AiProtocol;

public class TigerBrain : AiProtocol.IBrain
{

    public void SetRules(AiProtocol.GameRules rules)
    {

    }

    public void SetNextAction(BaseCommand action)
    {

    }

    public void Initialize(AiProtocol.IBrainGenetics whatever)
    {

    }

    private class EoI
    {
        public int EntityID;
        public Vector2 Position;
    }

    private EoI closestSeen = null;
    private EoI closestHeard = null;
    private Vector2 target;


    public IEnumerable<BaseCommand> GetDecisions()
    {
        generateTarget();

        while (true)
        {

            Vector2 position = new Vector2(description.xCoord, description.yCoord);

            if (closestHeard != null && Vector2.Distance(closestHeard.Position, position) < 0.75f * GameManager.Instance.Config.DefaultSeeingDistance)
                closestHeard = null;

            if (closestSeen != null)
            {
                if (Vector2.Distance(closestSeen.Position, position) - description.Size - EntityManager.EntityHash[closestSeen.EntityID].Size < GameManager.Instance.Config.MaxInteractionDistance)
                    yield return new Interaction
                    {
                        CurrentMood = Mood.Angry,
                        Style = InteractionStyle.Attack,
                        TargetID = closestSeen.EntityID
                    };
                else
                    yield return new Movement
                    {
                        CurrentMood = Mood.Angry,
                        Style = MovementStyle.Run,
                        xCoord = (int)closestSeen.Position.x,
                        yCoord = (int)closestSeen.Position.y
                    };
            }
            else if (closestHeard != null)
            {
                yield return new Movement
                {
                    CurrentMood = Mood.Happy,
                    Style = MovementStyle.Sneak,
                    xCoord = (int)closestHeard.Position.x,
                    yCoord = (int)closestHeard.Position.y
                };
            }
            else
            {
                float random = UnityEngine.Random.Range(0, 80);

                if (random < 1)
                {
                    yield return new Talk
                    {
                        Style = TalkStyle.Yell,
                        Words = new AiProtocol.Roar(),
                        CurrentMood = Mood.Angry,
                    };
                }
                else
                {
                    if (Vector2.Distance(position, target) < 3)
                        generateTarget();
                    
                    yield return new Movement
                    {
                        CurrentMood = Mood.Sad,
                        Style = MovementStyle.Walk,
                        xCoord = (int)target.x,
                        yCoord = (int)target.y
                    };
                }
            }
        }
    }

    public void Hear(Words words)
    {
        if (words as AiProtocol.Roar == null)
        {
            var myPos = new Vector2(description.xCoord, description.yCoord);

            var heardPos = EntityManager.FindById(words.TalkerID).Position;
            float dist = Vector2.Distance(heardPos, myPos);

            if (closestHeard == null || dist < Vector2.Distance(myPos, closestHeard.Position))
                closestHeard = new EoI { EntityID = words.TalkerID, Position = heardPos };

        }
    }

    public void See(IEnumerable<BaseDescription> descriptions)
    {
        Vector2 position = new Vector2(description.xCoord, description.yCoord);

        var victims = descriptions.Where(x =>
                x.Alignment != Faction.Neutral
                && x.Alignment != Faction.Carnivore);

        int closestID = -1;
        float closestValue = Single.PositiveInfinity;

        foreach (var desc in victims)
        {
            Vector2 point = new Vector2(desc.xCoord, desc.yCoord);
            float dist = Vector2.Distance(position, point);

            if (dist < closestValue)
            {
                closestValue = dist;
                closestID = desc.EntityID;
            }
        }

        if (closestID != -1)
            closestSeen = new EoI { EntityID = closestID, Position = EntityManager.FindById(closestID).Position };
        else
            closestSeen = null;
    }

    private BodilyFunctions description;

    public void Feel(BodilyFunctions functions)
    {
        description = functions;
    }

    private void generateTarget()
    {
        target = new Vector2(description.xCoord, description.yCoord) + UnityEngine.Random.insideUnitCircle * 200;
    }

    public IBrainGenetics GetGeneticMaterial()
    {
        return null;
    }
}