using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using System;
using AiProtocol.Command;
using AiProtocol.Descriptions;

public struct GeneticMaterial
{
    public int Strength;
    public int Agility;
    public int Intelligence;
    public AiProtocol.IBrainGenetics BrainGenetics;
}

public class VillagerBody : LivingEntityBody
{
    public VillageBody Home;
    public ToolKind Tool;
    int ToolLevel;
    public List<EntityBody> Equipment = new List<EntityBody>();
    public int Strength;
    public int Agility;
    public int Intelligence;

    #region DerivedAttributes
    public override float WalkingSpeed
    {
        get
        {
            return ((10 + Agility) * GameManager.Instance.Config.BaseSpeedMultiplier)
                / (1 + Equipment.Count);
        }
    }
    public override float SpeakingTime
    {
        get
        {
            return 30.0f / (10 + Intelligence);
        }
    }
    public override float AttackDuration
    {
        get
        {
            return 4.0f/(1+Agility);
        }
    }
    public override int AttackDamage
    {
        get
        {
            float mod = ToolLevel + 1;
            switch (Tool)
            {
                case ToolKind.None:
                    mod = 0;
                    break;
                case ToolKind.Axe:
                    break;
                case ToolKind.Sword:
                    mod *= 1.5f;
                    break;
                case ToolKind.Pole:
                    mod *= 0.5f;
                    break;
            }
            return Strength * 2 + (int)(mod+0.5f);
        }
    }
    public override float SeeingDistance
    {
        get
        {
            return GameManager.Instance.Config.DefaultSeeingDistance * (15 + Intelligence) * 0.05f;
        }
    }
    public override float Sneakiness
    {
        get
        {
            return 5.0f / (5 + Agility);
        }
    }
    #endregion

    #region Technical

    public void OnValidate()
    {
        switch (Alignment)
        {
            case Faction.Neutral:
                throw new Exception("A villager has an illegal alingment");
            default:
                break;
        }
    }

    public override IEnumerator Execute(BaseCommand command)
    {
        if (command is VillageInteraction)
            return WorkWithVillage(command as VillageInteraction);
        return base.Execute(command);
    }

    public override void Start()
    {
        base.Start();
        if (Home != null)
            Home.Population++;
        MaxEnergy = GameManager.Instance.Config.BaseEnergy + Strength * GameManager.Instance.Config.EnergyPerStrength;
        Energy = MaxEnergy;
        Name = EntityManager.DispenseName(Female) + " " +
               EntityManager.DispenseSurname();
    }


    #endregion
    #region Reactions
    

    public override void OnDeath()
    {
        base.OnDeath();
        if (Home != null)
            Home.Population--;
        //TODO: maybe we want him to scream or something
    }

    public override void TakeHit(int damage, EntityBody source)
    {
        base.TakeHit(damage, source);
        //TODO: maybe we want him to take a note of the attacker
    }

    public override bool GetPickedUp(EntityBody source)
    {
        return false;
        //TODO: maybe he should take a note that
        //something tried to pick him up
    }

    public override void GetGathered(EntityBody source)
    {
        //TODO: maybe he should take a note that
        //something tried to gather him
    }

    #endregion
    #region Actions
    public override IEnumerator Speak(Talk talk)
    {
        switch(talk.Style)
        {
            case TalkStyle.Speak:
                Energy -= 2;
                break;
            case TalkStyle.Yell:
                Energy -= 4;
                break;
        }
        yield return StartCoroutine(base.Speak(talk));
    }

    public override IEnumerator Move(Movement movement)
    {
        switch(movement.Style)
        {
            case MovementStyle.Walk:
                Energy -= 1;
                break;
            case MovementStyle.Run:
                Energy -= 2;
                break;
            case MovementStyle.Sneak:
                Energy -= 1;
                break;
        }
        yield return StartCoroutine(base.Move(movement));
    }

    public IEnumerator WorkWithVillage(VillageInteraction interaction)
    {
        VillageBody village = EntityManager.EntityHash[interaction.VillageID] as VillageBody;
        if (village == null ||
            DistanceTo(village) > GameManager.Instance.Config.MaxInteractionDistance)
            return Wait(new AiProtocol.Command.Wait());
        if (interaction is MagazinePush)
            return MagazinePush(village, interaction as MagazinePush);
        else if (interaction is MagazinePull)
            return MagazinePull(village, interaction as MagazinePull);
        else if (interaction is Procreate)
            return Procreate(village, interaction as Procreate);
        else if (interaction is PickTool)
            return PickTool(village, interaction as PickTool);
        else throw new ArgumentOutOfRangeException();
    }

    IEnumerator PickTool(VillageBody village, PickTool pickTool)
    {
        CurrentCommand = pickTool;
        IsSneaking = false;
        yield return WaitWithProgress(1f);

        SetToolActive(Tool, false);
        Tool = pickTool.Tool;
        ToolLevel = village.ToolLevel(Tool);
        SetToolActive(Tool, true);
        CurrentCommand = null;
        
    }

    void SetToolActive(ToolKind tool, bool active)
    {
        switch (tool)
        {
            case ToolKind.Axe:
                transform.Find("Axe").gameObject.SetActive(active);
                break;
            case ToolKind.Sword:
                transform.Find("Sword").gameObject.SetActive(active);
                break;
            case ToolKind.Pole:
                transform.Find("Pole").gameObject.SetActive(active);
                break;
            case ToolKind.None:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    IEnumerator MagazinePush(VillageBody village, MagazinePush push)
    {
        EntityBody PushTarget = EntityManager.EntityHash[push.TargetID];
        CurrentCommand = push;
        IsSneaking = false;
        yield return WaitWithProgress(0.5f);
        if (!Equipment.Contains(PushTarget))
            yield break;
        if (!village.WillAccept(PushTarget))
            yield break;
        RemoveFromEq(PushTarget);
        village.MagazinePush(PushTarget);
        CurrentCommand = null;

    }

    IEnumerator MagazinePull(VillageBody village, MagazinePull pull)
    {
        CurrentCommand = pull;
        IsSneaking = false;
        yield return WaitWithProgress(0.5f);
        village.MagazinePull(pull.Type, this);
        CurrentCommand = null;

    }

    IEnumerator Procreate(VillageBody village, Procreate procreate)
    {
        CurrentCommand = procreate;
        IsSneaking = false;
        yield return WaitWithProgress(2f);
        village.PutGeneticMaterial(this);
        CurrentCommand = null;

    }

    public override IEnumerator Interact(Interaction interaction)
    {
        yield return StartCoroutine(base.Interact(interaction));
    }

    public override IEnumerator Attack(EntityBody target)
    {
        Energy -= 3;
        yield return StartCoroutine(base.Attack(target));
    }

    public override IEnumerator PickUp(EntityBody target)
    {
        yield return StartCoroutine(WaitWithProgress(0.1f));
        if (target.GetPickedUp(this))
        {
            Equipment.Add(target);
            target.transform.parent = transform;
            target.transform.localPosition = Vector3.up * 1 * (Equipment.Count + 1);
        }
    }

    void RemoveFromEq(EntityBody target)
    {
        if (!Equipment.Contains(target))
            return;
        bool lower = false;
        foreach (EntityBody b in Equipment)
        {
            if (lower)
                b.transform.localPosition += Vector3.down;
            else if (b == target)
                lower = true;
        }
        Equipment.Remove(target);
        
    }

    public override IEnumerator Drop(EntityBody target)
    {
        if (Equipment.Contains(target) && target.PickedUp)
        {
            yield return StartCoroutine(WaitWithProgress(0.1f));
            RemoveFromEq(target);
            target.transform.parent =
            GameObject.FindGameObjectWithTag("Entities").transform;
            Vector3 mod = UnityEngine.Random.onUnitSphere;
            mod.y = 0;
            target.GetDropped(this);
            target.transform.position = transform.position + mod.normalized * 2;
        }
        yield return StartCoroutine(WaitWithProgress(0.1f));

    }

    public override IEnumerator Gather(EntityBody target)
    {
        //TODO: based on stats, equipment and the specific target
        // time should be modified
        // also, gathering range stuff
        Energy -= 3;
        ResourceNodeBody targetResource = target as ResourceNodeBody;
        if (targetResource != null && !targetResource.ResourceReady)
        {
            yield return StartCoroutine(WaitWithProgress(1));
            yield break;
        }

        float time = targetResource != null ? targetResource.BaseGatheringTime : 4;
        int timeMod = 5;
        switch (target.GenericName)
        {
            case EntityType.Tree:
                timeMod += Strength;
                if (Tool == ToolKind.Axe)
                {
                    timeMod += ToolLevel;
                }
                else
                {
                    Energy -= 4;
                }
                break;
            case EntityType.Lake:
                timeMod += Agility;
                if (Tool == ToolKind.Pole)
                {
                    timeMod += ToolLevel;
                }
                else
                {
                    Energy -= 4;
                }
                break;
        }
        time *= 5.0f / timeMod;
        yield return StartCoroutine(WaitWithProgress(time));
        target.GetGathered(this);
    }

    //todo: actual eating
    public override IEnumerator Eat(EntityBody target)
    {
        if ((target.Position - Position).magnitude < GameManager.Instance.Config.MaxInteractionDistance
            && target.GetEaten(target))
        {
            yield return StartCoroutine(WaitWithProgress(0.1f));
            RemoveFromEq(target);
            Energy += target.Energy;
            Health += target.Energy;
            if (Energy > MaxEnergy)
                Energy = MaxEnergy;
            if (Health > MaxHealth)
                Health = MaxHealth;
            target.OnDeath();
        }
        else yield return StartCoroutine(WaitWithProgress(0.1f));
    }

    #endregion

    public override BodilyFunctions BodilyFunctions
    {
        get
        {
            var res = base.BodilyFunctions;
            res.Inventory = Equipment.Select(x => x.Description);
            res.Agility = Agility;
            res.Strength = Strength;
            res.Intelligence = Intelligence;
            res.CurrentTool = Tool;
            res.ToolLevel = ToolLevel;
            return res;
        }
    }

    public GeneticMaterial Genetics
    {
        get
        {
            return new GeneticMaterial
            {
                Strength = this.Strength,
                Agility = this.Agility,
                Intelligence = this.Intelligence,
                BrainGenetics = GetComponent<BrainWrapper>().GetGeneticMaterial()
            };
        }
    }
}


    