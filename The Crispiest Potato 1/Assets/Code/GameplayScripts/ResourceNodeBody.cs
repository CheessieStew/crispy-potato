using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using AiProtocol.Descriptions;

//todo pomyslec nad mysleniem na threadpoolu
class ResourceNodeBody : EntityBody
{
    public bool ResourceReady = true;
    public Transform TreeBody;

    public ResourceType Type;
    public GameObject RegrowingPlaceholder;
    public GameObject DroppedResource = null;
    public int DropCount = 3;
    float RegrowingTime = 0;
    public float DropRange = 2;
    public float BaseGatheringTime = 3;

    [HideInInspector]
    public float RegrowingTimer;

    public override void Start()
    {
        base.Start();
        if (GenericName == EntityType.Tree)
            RegrowingTime = GameManager.Instance.Config.TreeRegrowingTime;
        else RegrowingTime = 0;
        if (RegrowingPlaceholder != null)
        {
            RegrowingPlaceholder = Instantiate(RegrowingPlaceholder, transform.position, transform.rotation);
            RegrowingPlaceholder.transform.parent = transform;
            RegrowingPlaceholder.SetActive(false);
        }
    }

    public override void GetGathered(EntityBody source)
    {
        if (!ResourceReady)
            return;
        Vector3 sourcepos;
        if (source != null) sourcepos = source.transform.position;
        else
        {
            Vector3 rand = UnityEngine.Random.onUnitSphere;
            rand.y = 0; rand.Normalize();
            sourcepos = transform.position + rand;
        }
        DropResources(DropCount, sourcepos);
        OnDeath();
    }

    public void DropResources(int count, Vector3 where)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 mod = UnityEngine.Random.onUnitSphere * 1;
            mod.y = 0;
            Instantiate(DroppedResource,
                transform.position + (where - transform.position).normalized * DropRange
                + mod,
                Quaternion.identity);
        }
    }

    public override bool GetPickedUp(EntityBody source)
    {
        return false;
    }

    public override void OnDeath()
    {
        if (RegrowingTime == 0)
        {
            Health = MaxHealth;
            return;
        }
        ResourceReady = false;
        GameManager.Instance.Forester.NewDepleted(this);
        RegrowingTimer = RegrowingTime;
        if (RegrowingPlaceholder != null)
            RegrowingPlaceholder.SetActive(true);
        TreeBody.gameObject.SetActive(false);
    }

    public void Regrown()
    {
        if (RegrowingPlaceholder != null)
            RegrowingPlaceholder.SetActive(false);
        RegrowingTimer = 0;
        TreeBody.gameObject.SetActive(true);
        Health = MaxHealth;
        ResourceReady = true;
    }

    public override BaseDescription Description
    {
        get
        {
            return new ResourceNodeDescription
            {
                Size = this.Size,
                GenericName = this.GenericName,
                EntityID = EntityID,
                xCoord = Position.x,
                yCoord = Position.y,
                Alignment = Faction.Neutral,
                Type = this.Type,
                TimeUntilReady = RegrowingTimer,
                RegrowingTime = this.RegrowingTime
            };
        }
    }
}
