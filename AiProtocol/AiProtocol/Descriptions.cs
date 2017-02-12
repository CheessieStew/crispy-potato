using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Whatever can be seen by an AI is defined here

namespace AiProtocol.Descriptions
{

    public enum ResourceType
    {
        Wood,
        Food
    }

    /// <summary>
    /// A Generic Name, each entity has one
    /// </summary>
    public enum EntityType
    {
        Tree,
        Wood,
        Lake,
        Food,
        Villager,
        Tiger,
        Village,
    }

    /// <summary>
    /// Used to differ a friend from a foe
    /// </summary>
    public enum Faction
    {
        Red,
        Blue,
        Carnivore,
        Herbivore,
        Neutral
    }

    public abstract class BaseDescription
    {
        public Faction Alignment;
        public int EntityID;
        public float xCoord;
        public float yCoord;
    }

    public class LivingDescription : BaseDescription
    {
        public int HP;
        public int MaxHP;
        public Command.BaseCommand lastCommand;
        public bool IsFemale;

    }

    public class VillageDescription : BaseDescription
    {
        public int HP;
        public int MaxHP;
        public int Level;
        public int WoodInBank;
        public int FoodInBank;
        public int WoodLimit;
        public int FoodLimit;
        public int Population;
        public int PopulationLimit;
    }

    public class ResourceNodeDescription : BaseDescription
    {
        public ResourceType Type;
        public float TimeUntilReady;
        public float RegrowingTime;
    }

    public class ResourceDescription : BaseDescription
    {
        public ResourceType Type;
        public int Value;
    }

    public class BodilyFunctions : LivingDescription
    {
        public int Energy;
        public int MaxEnergy;
        public int Strength;
        public int Agillity;
        public int Intelligence;
        public float DeltaTime;
        public IEnumerable<BaseDescription> Inventory;
        public ToolKind CurrentTool;
        public int ToolLevel;
    }

    public enum ToolKind
    {
        None,
        Axe,
        Sword,
        Pole,
    }
    
}
