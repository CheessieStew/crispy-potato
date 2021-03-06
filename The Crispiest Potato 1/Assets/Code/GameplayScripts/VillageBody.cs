﻿using System;
using System.Collections;
using System.Collections.Generic;
using AiProtocol.Descriptions;
using UnityEngine;

public class VillageBody : EntityBody {

    public Transform TheCube;
    public Faction Alignment;
    public ResourceBody FoodPrefab;
    public ResourceBody WoodPrefab;
    int BasePopulation;
    int PopulationPerLevel;
    int WoodPerLevel;
    int FoodPerLevel;
    int FoodPerChild;
    float TimeBetweenBirths;
    bool ParentsRequired;
    int LowStrength;
    int HighStrength;
    int LowAgility;
    int HighAgility;
    int LowIntelligence;
    int HighIntelligence;

    public VillagerBody VillagerPrefab;

    public int VillageLevel;
    public int PopulationLimit { get { return BasePopulation + VillageLevel * PopulationPerLevel; } }
    public int Population;

    public int WoodLimit { get { return (VillageLevel + 1) * WoodPerLevel; } }
    public int WoodInBank;// { get; private set; }

    public int FoodLimit { get { return (VillageLevel + 1) * FoodPerLevel; } }
    public int FoodInBank;// { get; private set; }

    public int SwordsLevel { get { return VillageLevel; } }
    public int AxesLevel { get { return VillageLevel; } }
    public int FishingPolesLevel { get { return VillageLevel; } }

    public void LevelUp()
    {
        MaxHealth += 500;
        VillageLevel++;
        Health = MaxHealth;
        Transform newCube = Instantiate(TheCube, transform);
        newCube.RotateAround(newCube.position, Vector3.up, 180 * VillageLevel);
        newCube.position += Vector3.up * VillageLevel * 4;
    }

    public int ToolLevel(ToolKind tool)
    {
        switch(tool)
        {
            case ToolKind.None:
                return 0;
            case ToolKind.Axe:
                return AxesLevel;
            case ToolKind.Sword:
                return SwordsLevel;
            case ToolKind.Pole:
                return FishingPolesLevel;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public override BaseDescription Description
    {
        get
        {
            return new VillageDescription()
            {
                Size = this.Size,
                GenericName = this.GenericName,
                Alignment = this.Alignment,
                xCoord = this.Position.x,
                yCoord = this.Position.y,
                EntityID = this.EntityID,
                Level = VillageLevel
            };
        }
    }

    public void MagazinePush(EntityBody target)
    {
        ResourceBody b = target as ResourceBody;
        if (b == null)
            return;
        if (b.Type == ResourceType.Food)
            FoodInBank++;
        if (b.Type == ResourceType.Wood)
        {
            WoodInBank++;
            if (WoodInBank == WoodLimit)
            {
                WoodInBank = 0;
                LevelUp();
            }
        }
        b.OnDeath();
    }

    public override void OnDeath()
    {
        Time.timeScale = 0;
        GameObject.Find("UI/Canvas/TimeController").SetActive(false);
        GameObject.Find("UI/Canvas/Console").SetActive(false);
        GameObject.Find("UI/Canvas/EntityStats").SetActive(false);
        if (Alignment == Faction.Red)
            GameObject.Find("UI/Canvas/BlueWon").SetActive(true);
        else
            GameObject.Find("UI/Canvas/RedWon").SetActive(true);
        base.OnDeath();
    }

    public void MagazinePull(ResourceType type, LivingEntityBody who)
    {
        Vector3 mod = UnityEngine.Random.onUnitSphere;
        mod.y = 0;
        mod.Normalize();
        Vector3 location = who.transform.position + mod * 2;
        if (type == ResourceType.Wood && WoodInBank > 0)
        {
            Instantiate(WoodPrefab, location, Quaternion.identity);
            WoodInBank--;
        }
        if (type == ResourceType.Food && FoodInBank > 0)
        {
            Instantiate(FoodPrefab, location, Quaternion.identity);
            FoodInBank--;
        }
    }

    public bool WillAccept(EntityBody target)
    {
        ResourceBody b = target as ResourceBody;
        if (b == null)
            return false;
        if (b.Type == ResourceType.Food)
            return FoodInBank < FoodLimit;
        if (b.Type == ResourceType.Wood)
            return true;
        return false;
    }

    public override void Start()
    {
        base.Start();
        if (Alignment == Faction.Blue)
        {
            BasePopulation = GameManager.Instance.Config.BlueBasePopulation;
            PopulationPerLevel = GameManager.Instance.Config.BluePopulationPerLevel;
            WoodPerLevel = GameManager.Instance.Config.BlueWoodPerLevel;
            FoodPerLevel = GameManager.Instance.Config.BlueFoodPerLevel;
            FoodPerChild = GameManager.Instance.Config.BlueFoodPerChild;
            TimeBetweenBirths = GameManager.Instance.Config.BlueTimeBetweenBirths;
            ParentsRequired = GameManager.Instance.Config.BlueParentsRequired;
            LowStrength = GameManager.Instance.Config.BlueLowStrength;
            HighStrength = GameManager.Instance.Config.BlueHighStrength;
            LowAgility = GameManager.Instance.Config.BlueLowAgility;
            HighAgility = GameManager.Instance.Config.BlueHighAgility;
            LowIntelligence = GameManager.Instance.Config.BlueLowIntelligence;
            HighIntelligence = GameManager.Instance.Config.BlueHighIntelligence;
        }
        else
        {
            {
                BasePopulation = GameManager.Instance.Config.RedBasePopulation;
                PopulationPerLevel = GameManager.Instance.Config.RedPopulationPerLevel;
                WoodPerLevel = GameManager.Instance.Config.RedWoodPerLevel;
                FoodPerLevel = GameManager.Instance.Config.RedFoodPerLevel;
                FoodPerChild = GameManager.Instance.Config.RedFoodPerChild;
                TimeBetweenBirths = GameManager.Instance.Config.RedTimeBetweenBirths;
                ParentsRequired = GameManager.Instance.Config.RedParentsRequired;
                LowStrength = GameManager.Instance.Config.RedLowStrength;
                HighStrength = GameManager.Instance.Config.RedHighStrength;
                LowAgility = GameManager.Instance.Config.RedLowAgility;
                HighAgility = GameManager.Instance.Config.RedHighAgility;
                LowIntelligence = GameManager.Instance.Config.RedLowIntelligence;
                HighIntelligence = GameManager.Instance.Config.RedHighIntelligence;
            }
        }
        for (int i = Population; i <= (int)(0.75f * PopulationLimit); i++)
        {
            GiveBirth();
        }
    }


    public Queue<GeneticMaterial> MaleMaterials = new Queue<GeneticMaterial>();
    public Queue<GeneticMaterial> FemaleMaterials = new Queue<GeneticMaterial>();
    public void PutGeneticMaterial(VillagerBody body)
    {
        if (body.Female)
            FemaleMaterials.Enqueue(body.Genetics);
        else
            MaleMaterials.Enqueue(body.Genetics);
    }

    void GiveBirth()
    {
        Vector3 location = UnityEngine.Random.onUnitSphere;
        location.y = 0; location.Normalize();
        location *= Size * 1.1f;
        location += transform.position;
        VillagerBody newChild = Instantiate(VillagerPrefab, location, Quaternion.identity);
        newChild.transform.parent = transform.parent;
        newChild.Home = this;
        newChild.Alignment = Alignment;
        newChild.Female = UnityEngine.Random.value >= 0.5f;
        BrainWrapper newBrain = newChild.gameObject.AddComponent<BrainWrapper>();
        newBrain.Brain = BrainLoader.NewBrain();
        newBrain.SetRules(GameManager.Instance.Rules);

        if (FemaleMaterials.Count > 0 && MaleMaterials.Count > 0)
        {
            GeneticMaterial mother = FemaleMaterials.Dequeue();
            GeneticMaterial father = MaleMaterials.Dequeue();
            AiProtocol.IBrainGenetics childBrain = null;
            if (mother.BrainGenetics != null && father.BrainGenetics != null)
                childBrain = mother.BrainGenetics.Cross(father.BrainGenetics);
            newChild.Strength = (int)(mother.Strength + father.Strength + UnityEngine.Random.Range(-1f, 2f));
            newChild.Intelligence = (int)(mother.Intelligence + father.Intelligence + UnityEngine.Random.Range(-1f, 2f));
            newChild.Agility = (int)(mother.Agility + father.Agility + UnityEngine.Random.Range(-1f, 2f));
            newBrain.Initialize(childBrain);
        }
        else
        {
            newChild.Strength = (int)(UnityEngine.Random.Range(LowStrength, HighStrength) + 0.5f);
            newChild.Agility = (int)(UnityEngine.Random.Range(LowAgility, HighAgility) + 0.5f);
            newChild.Intelligence = (int)(UnityEngine.Random.Range(LowIntelligence, HighIntelligence) + 0.5f);
        }
    }

    float timer = 0;
    void Update()
    {
        
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            if (Population < PopulationLimit 
                && FoodInBank > FoodPerChild
                && (FemaleMaterials.Count > 0 
                    && MaleMaterials.Count > 0 
                    || !ParentsRequired)
                )
            {
                FoodInBank -= FoodPerChild;
                GiveBirth();
                timer = TimeBetweenBirths;
            }
        }
        
    }

    public override void GetGathered(EntityBody source)
    {
        
    }

    public override bool GetPickedUp(EntityBody source)
    {
        return false;
    }
}
