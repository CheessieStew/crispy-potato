using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AiProtocol.Descriptions;
using UnityEngine;

public class ResourceBody : EntityBody
{
    public ResourceType Type;

    public override void Start()
    {
        base.Start();
        MaxEnergy = GameManager.Instance.Config.FishEnergeticValue;
        Energy = MaxEnergy;
    }

    public override BaseDescription Description
    {
        get
        {
            return new ResourceDescription
            {
                Size = this.Size,
                GenericName = this.GenericName,
                EntityID = EntityID,
                xCoord = Position.x,
                yCoord = Position.y,
                Alignment = Faction.Neutral,
                Type = this.Type,
                Value = Energy
            };
        }
    }

    public override bool GetEaten(EntityBody source)
    {
        return true;
    }

    public override void GetGathered(EntityBody source)
    {
        //nothing!
    }
}
