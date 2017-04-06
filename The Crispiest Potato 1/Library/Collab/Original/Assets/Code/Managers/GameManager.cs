using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Code.Managers;
using UnityEngine;

class GameManager : Singleton<GameManager>, ICompositeManager
{
    protected GameManager() { } // guarantee this will be always a singleton only - can't use the constructor!
    #region Children Managers

    public bool ChildrenManagersInitiated { get; private set; }
    public Forester Forester { get; private set; }
    public SelectionController SelectionController { get; private set; }

    public void InitiateChildrenManagers()
    {
        Forester = new Forester();
        SelectionController = new SelectionController(); //jak tu coś urośnie to przerzucić do controls managera

        ChildrenManagersInitiated = true;
    }

    public void OperateChildrenManagers()
    {
        Forester.RegrowthTick();
        SelectionController.Operate();
    }
    #endregion

    [Header("Blue statistics")]
    public int BlueBasePopulation = 20;
    public int BluePopulationPerLevel = 10;
    public int BlueWoodPerLevel = 10;
    public int BlueFoodPerLevel = 10;
    public int BlueFoodPerChild = 4;
    public float BlueTimeBetweenBirths = 10f;
    public bool BlueParentsRequired;
    public int BlueLowStrength = 1;
    public int BlueHighStrength = 10;
    public int BlueLowAgility = 1;
    public int BlueHighAgility = 10;
    public int BlueLowIntelligence = 1;
    public int BlueHighIntelligence = 10;

    [Header("Red statistics")]
    public int RedBasePopulation = 20;
    public int RedPopulationPerLevel = 10;
    public int RedWoodPerLevel = 10;
    public int RedFoodPerLevel = 10;
    public int RedFoodPerChild = 4;
    public float RedTimeBetweenBirths = 10f;
    public bool RedParentsRequired;
    public int RedLowStrength = 1;
    public int RedHighStrength = 10;
    public int RedLowAgility = 1;
    public int RedHighAgility = 10;
    public int RedLowIntelligence = 1;
    public int RedHighIntelligence = 10;

    [Header("Tigers statistics")]
    public int TigerSpeed = 6;
    public int TigerDamage = 15;
    public int TigerAttackSpeed = 2;
    public int TigerSneakMastery = 4;
    public int TigersLimit = 15;
    public float TigerSpawnTimer = 20f;
    public int TigersOnMap = 0;

    [Header("Game rules")]
    public int FishEnergeticValue = 30;
    public float MaxInteractionDistance = 20f;
    public float MaxHearingDistance = 100f;
    public float DefaultSeeingDistance = 50f;
    public float WalkingWindowSize = 0.5f;
    public float BaseSpeedMultiplier = 1;
    public int BaseEnergy = 120;
    public int EnergyPerStrength = 15;
    public bool CheatsOn = false;
    public float TreeRegrowingTime = 40f;

    [Header("Other")]
    public ProjectileBehaviour Projectile;
    public Transform LevelOrigin;
    public static float TimeScale
    {
        get { return Time.timeScale; }
        set { Time.timeScale = value; }
    }

    void Start()
    {
        InitiateChildrenManagers();
        // TODO: MAKE SURE IT'S AFTER THE SETTINGS 
    }

    public static void SceneLoaded(bool IsMenu)
    {
        if (IsMenu)
        {
            Instance.Forester = null;
            Instance.SelectionController = null;
            Instance.ChildrenManagersInitiated = false;
        }
        else
            Instance.InitiateChildrenManagers();
        Instance.GenerateRules();
    }


    void Update()
    {
        if (ChildrenManagersInitiated)
            OperateChildrenManagers();
    }

    public AiProtocol.GameRules Rules { get; private set; }

    void GenerateRules()
    {

        AiProtocol.VillageRules redRules = new AiProtocol.VillageRules()
        {
            FoodPerChild = RedFoodPerChild,
            FoodPerLevel = RedFoodPerLevel,
            WoodPerLevel = RedWoodPerLevel,
            HighAgility = RedHighAgility,
            LowAgility = RedLowAgility,
            HighIntelligence = RedHighIntelligence,
            LowIntelligence = RedLowIntelligence,
            HighStrength = RedHighStrength,
            LowStrength = RedLowStrength,
            ParentsRequired = RedParentsRequired
        };
        AiProtocol.VillageRules blueRules = new AiProtocol.VillageRules()
        {
            FoodPerChild = BlueFoodPerChild,
            FoodPerLevel = BlueFoodPerLevel,
            WoodPerLevel = BlueWoodPerLevel,
            HighAgility = BlueHighAgility,
            LowAgility = BlueLowAgility,
            HighIntelligence = BlueHighIntelligence,
            LowIntelligence = BlueLowIntelligence,
            HighStrength = BlueHighStrength,
            LowStrength = BlueLowStrength,
            ParentsRequired = BlueParentsRequired
        };
        Rules = new AiProtocol.GameRules()
        {
            RedVillageRules = redRules,
            BlueVillageRules = blueRules,
            TimeWhenGenerated = Time.time,
            MaxHearingDistance = this.MaxHearingDistance,
            MaxInteractionDistance = this.MaxInteractionDistance,
            TreeRegrowingTime = this.TreeRegrowingTime,
            WalkingWindowSize = this.WalkingWindowSize
        };
        
    }
}
