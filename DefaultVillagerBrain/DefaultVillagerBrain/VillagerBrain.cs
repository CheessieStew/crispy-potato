using System;
using System.Linq;
using System.Collections.Generic;
using AiProtocol.Command;
using AiProtocol.Descriptions;
using AiProtocol;

using static HiveManager;


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
        
    private List<BaseDescription> Surroundings;
    public void See(IEnumerable<BaseDescription> descriptions)
    {
        Surroundings = new List<BaseDescription>(descriptions);

        foreach (var eon in descriptions)
            MyHive.Notify(eon);
    }

    private BodilyFunctions Description;
    private Hive MyHive;


    public void Feel(BodilyFunctions functions)
    {
        Description = functions;
        MyHive = AllHives[Description.Alignment];
    }

    private bool EnemyClose()
    {
        foreach (var eon in Surroundings)
        {
            if (eon.Alignment != Description.Alignment && (eon.GenericName == EntityType.Villager
                || eon.GenericName == EntityType.Tiger))
                return true;
        }

        return false;
    }

    private double distance(BaseDescription a, BaseDescription b)
    {
        double dx = a.xCoord - b.xCoord, dy = a.yCoord - b.yCoord;
        return Math.Sqrt(dx * dx + dy * dy);
    }

    private double distance(BaseDescription a, int xCoord, int yCoord)
    {
        double dx = a.xCoord - xCoord, dy = a.yCoord - yCoord;
        return Math.Sqrt(dx * dx + dy * dy);
    }

    private bool canInteract(BaseDescription target)
    {
        return distance(target, Description) - Description.Size - target.Size < rules.MaxInteractionDistance;
    }

    private BaseDescription findClosest(IEnumerable<BaseDescription> list)
    {
        BaseDescription closest = null;
        foreach (var eon in list)
            if (closest == null || distance(Description, eon) < distance(Description, closest))
                closest = eon;

        return closest;
    }

    private BaseCommand AttackClosest()
    {
        var closest = findClosest(from eon in Surroundings
                                              where eon.Alignment != Description.Alignment 
                                                && (eon.GenericName == EntityType.Villager ||
                                                    eon.GenericName == EntityType.Tiger)
                                              select eon) as LivingDescription;

        if (canInteract(closest))
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

    private BaseCommand SeekFood()
    {
        var food_inside = from food in Description.Inventory where food.GenericName == EntityType.Food select food;
        if (food_inside.Count() > 0)
            return new Interaction
            {
                CurrentMood = Mood.Happy,
                Style = InteractionStyle.Eat,
                TargetID = food_inside.ElementAt(0).EntityID
            };

        var closest = findClosest(from eon in Surroundings
                                  where eon.GenericName == EntityType.Food
                                  select eon) as ResourceDescription;

        if (closest != null)
        {
            if (canInteract(closest))
                return new Interaction
                {
                    CurrentMood = Mood.Happy,
                    Style = InteractionStyle.Eat,
                    TargetID = closest.EntityID
                };

            else
                return new Movement
                {
                    CurrentMood = Mood.Happy,
                    Style = MovementStyle.Run,
                    xCoord = closest.xCoord,
                    yCoord = closest.yCoord
                };
        }

        VillageDescription myVillage = MyHive.MyVillage;
        if (canInteract(myVillage))
            return new MagazinePull
            {
                CurrentMood = Mood.Happy,
                VillageID = myVillage.EntityID,
                Type = ResourceType.Food
            };

        else
            return new Movement
            {
                CurrentMood = Mood.Angry,
                Style = MovementStyle.Walk,
                xCoord = myVillage.xCoord,
                yCoord = myVillage.yCoord
            };
    }

    private IEnumerable<BaseCommand> Explore()
    {
        int tx = Generator.Next(-100, 100) + (int)Description.xCoord;
        int ty = Generator.Next(-100, 100) + (int)Description.yCoord;

        while (distance(Description, tx, ty) > 3)
            yield return new Movement
            {
                CurrentMood = Mood.Happy,
                Style = MovementStyle.Walk,
                xCoord = tx,
                yCoord = ty
            };
    }

    private IEnumerable<BaseCommand> AquireFood()
    {
        var explore = Explore().GetEnumerator();

        while (true)
        {
            if (Description.Inventory.Count() > 2)
            {
                if (canInteract(MyHive.MyVillage))
                {
                    while (Description.Inventory.Count() > 0)
                        yield return new MagazinePush
                        {
                            CurrentMood = Mood.Happy,
                            VillageID = MyHive.MyVillage.EntityID,
                            TargetID = Description.Inventory.ElementAt(0).EntityID
                        };

                    yield break;
                }
                else
                    yield return new Movement
                    {
                        CurrentMood = Mood.Sad,
                        Style = MovementStyle.Walk,
                        xCoord = MyHive.MyVillage.xCoord,
                        yCoord = MyHive.MyVillage.yCoord
                    };
            }
            else 
            {
                var closestFood = findClosest(from food in Surroundings
                                              where food.GenericName == EntityType.Food
                                              select food);
                if (closestFood != null)
                {
                    if (!canInteract(closestFood))
                        yield return new Movement
                        {
                            CurrentMood = Mood.Sad,
                            Style = MovementStyle.Walk,
                            xCoord = closestFood.xCoord,
                            yCoord = closestFood.yCoord
                        };
                    else
                        yield return new Interaction
                        {
                            CurrentMood = Mood.Angry,
                            Style = InteractionStyle.PickUp,
                            TargetID = closestFood.EntityID
                        };
                }
                else
                {
                    var closest = findClosest(from lake in Surroundings
                                              where lake.GenericName == EntityType.Lake
                                              select lake);
                    if (closest != null)
                    {
                        if (!canInteract(closest))
                            yield return new Movement
                            {
                                CurrentMood = Mood.Sad,
                                Style = MovementStyle.Walk,
                                xCoord = closest.xCoord,
                                yCoord = closest.yCoord
                            };
                        else
                            yield return new Interaction
                            {
                                CurrentMood = Mood.Angry,
                                Style = InteractionStyle.Gather,
                                TargetID = closest.EntityID
                            };
                    }
                    else
                        yield return cycle(ref explore, Explore());
                }
            }
        }
    }

    private IEnumerable<BaseCommand> AquireWood()
    {
        var explore = Explore().GetEnumerator();

        while (true)
        {
            if (Description.Inventory.Count() > 2)
            {
                if (canInteract(MyHive.MyVillage))
                {
                    while (Description.Inventory.Count() > 0)
                        yield return new MagazinePush
                        {
                            CurrentMood = Mood.Happy,
                            VillageID = MyHive.MyVillage.EntityID,
                            TargetID = Description.Inventory.ElementAt(0).EntityID
                        };

                    yield break;
                }
                else
                    yield return new Movement
                    {
                        CurrentMood = Mood.Sad,
                        Style = MovementStyle.Walk,
                        xCoord = MyHive.MyVillage.xCoord,
                        yCoord = MyHive.MyVillage.yCoord
                    };
            }
            else
            {
                var closestWood = findClosest(from wood in Surroundings
                                              where wood.GenericName == EntityType.Wood
                                              select wood);
                if (closestWood != null)
                {
                    if (!canInteract(closestWood))
                        yield return new Movement
                        {
                            CurrentMood = Mood.Sad,
                            Style = MovementStyle.Walk,
                            xCoord = closestWood.xCoord,
                            yCoord = closestWood.yCoord
                        };
                    else
                        yield return new Interaction
                        {
                            CurrentMood = Mood.Angry,
                            Style = InteractionStyle.PickUp,
                            TargetID = closestWood.EntityID
                        };
                }
                else
                {
                    var closest = findClosest(from tree in Surroundings
                                              where tree.GenericName == EntityType.Tree
                                              select tree);

                    if (closest != null)
                    {
                        if (!canInteract(closest))
                            yield return new Movement
                            {
                                CurrentMood = Mood.Sad,
                                Style = MovementStyle.Walk,
                                xCoord = closest.xCoord,
                                yCoord = closest.yCoord
                            };
                        else
                            yield return new Interaction
                            {
                                CurrentMood = Mood.Angry,
                                Style = InteractionStyle.Gather,
                                TargetID = closest.EntityID
                            };
                    }
                    else
                        yield return cycle(ref explore, Explore());
                }
            }
        }
    }

    private IEnumerable<BaseCommand> Idle()
    {
        var myVillage = MyHive.MyVillage;
        while (!canInteract(myVillage))
            yield return new Movement
            {
                CurrentMood = Mood.Happy,
                Style = MovementStyle.Walk,
                xCoord = myVillage.xCoord,
                yCoord = myVillage.yCoord
            };

        if (Generator.NextDouble() < 0.001)
        {
            yield return new Procreate
            {
                CurrentMood = Mood.Sad,
                VillageID = myVillage.EntityID
            };
        }
        else
        {
            if (myVillage.FoodInBank * 100.0 / myVillage.FoodLimit < 50 || Generator.NextDouble() < 0.5)
            {
                yield return new PickTool
                {
                    CurrentMood = Mood.Sad,
                    VillageID = myVillage.EntityID,
                    Tool = ToolKind.Pole
                };

                foreach (var cmd in AquireFood())
                    yield return cmd;
            }
            else
            {
                yield return new PickTool
                {
                    CurrentMood = Mood.Sad,
                    VillageID = myVillage.EntityID,
                    Tool = ToolKind.Axe
                };

                foreach (var cmd in AquireWood())
                    yield return cmd;
            }
        }
    }

    private T cycle<T>(ref IEnumerator<T> it, IEnumerable<T> resetter)
    {
        if (!it.MoveNext())
        {
            it = resetter.GetEnumerator();
            it.MoveNext();
        }

        
        return it.Current;
    }

    // This should be an infinite collection of commands
    // for the body to follow. See() and Feel() will be called
    // before each time a next command is needed
    public IEnumerable<BaseCommand> GetDecisions()
    {
        var idle = Idle().GetEnumerator();
        while (true)
        {
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

            if (EnemyClose())
                yield return AttackClosest();

            else if (Description.Energy * 100.0 / Description.MaxEnergy < 30)
                yield return SeekFood();

            else
            {
                yield return cycle(ref idle, Idle());
            }
            //yield return new AiProtocol.Command.Movement(MovementStyle.Run, 10, 10);
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

