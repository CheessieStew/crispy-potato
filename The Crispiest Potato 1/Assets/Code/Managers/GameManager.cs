using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Code.Managers;
using UnityEngine;

[Serializable]
public class Config
{
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

    public Config()
    {
    }

    public static Config FromJSON(string json)
    {
        return JsonUtility.FromJson<Config>(json);
    }

    public static string ToJSON(Config config)
    {
        return JsonUtility.ToJson(config, true);
    }
}

[Serializable]
class GameManager : Singleton<GameManager>, ICompositeManager
{
    protected GameManager()
    {
        Config = staticConfig;
        if (Config == null) Config = new Config();
    } // guarantee this will be always a singleton only - can't use the constructor!
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

    #region Serialization

    public static Config staticConfig;

    #endregion

    [Header("Other")]
    public ProjectileBehaviour Projectile;
    public Transform LevelOrigin;

    [SerializeField]
    public Config Config;

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
        {
            Instance.LevelOrigin = GameObject.Find("Level").transform;
            Instance.InitiateChildrenManagers();
        }
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
            FoodPerChild = Config.RedFoodPerChild,
            FoodPerLevel = Config.RedFoodPerLevel,
            WoodPerLevel = Config.RedWoodPerLevel,
            HighAgility = Config.RedHighAgility,
            LowAgility = Config.RedLowAgility,
            HighIntelligence = Config.RedHighIntelligence,
            LowIntelligence = Config.RedLowIntelligence,
            HighStrength = Config.RedHighStrength,
            LowStrength = Config.RedLowStrength,
            ParentsRequired = Config.RedParentsRequired
        };
        AiProtocol.VillageRules blueRules = new AiProtocol.VillageRules()
        {
            FoodPerChild = Config.BlueFoodPerChild,
            FoodPerLevel = Config.BlueFoodPerLevel,
            WoodPerLevel = Config.BlueWoodPerLevel,
            HighAgility = Config.BlueHighAgility,
            LowAgility = Config.BlueLowAgility,
            HighIntelligence = Config.BlueHighIntelligence,
            LowIntelligence = Config.BlueLowIntelligence,
            HighStrength = Config.BlueHighStrength,
            LowStrength = Config.BlueLowStrength,
            ParentsRequired = Config.BlueParentsRequired
        };
        Rules = new AiProtocol.GameRules()
        {
            RedVillageRules = redRules,
            BlueVillageRules = blueRules,
            TimeWhenGenerated = Time.unscaledTime + 1,
            MaxHearingDistance = Config.MaxHearingDistance,
            MaxInteractionDistance = Config.MaxInteractionDistance,
            TreeRegrowingTime = Config.TreeRegrowingTime,
            WalkingWindowSize = Config.WalkingWindowSize
        };
        
    }
}
