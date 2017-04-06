using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AiProtocol.Command;
using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using AiProtocol.Descriptions;

public class TigerBody : LivingEntityBody
{
    public float Speed = 6;
    public float Damage = 15;
    public float AttackSpeed = 2;
    public float SneakMastery = 4;
    public override int AttackDamage
    {
        get { return (int)(Damage + 0.5f); }
    }
    public override float AttackDuration
    {
        get
        {
            return 4.0f / AttackSpeed;
        }
    }
    public override float WalkingSpeed
    {
        get
        {
            return Speed * GameManager.Instance.Config.BaseSpeedMultiplier;
        }
    }
    public override float Sneakiness
    {
        get
        {
            if (IsSneaking)
                return 1f / SneakMastery;
            return 1f;
        }
    }
    #region technical
    public override void Start()
    {
        base.Start();
        GameManager.Instance.Config.TigersOnMap++;
    }


    #endregion

    #region Reactions

    public override void OnDeath()
    {
        GameManager.Instance.Config.TigersOnMap--;
        base.OnDeath();
    }

    public override bool GetPickedUp(EntityBody source)
    {
        return false;
        //get angry or smth
    }

    public override void TakeHit(int damage, EntityBody source)
    {
        base.TakeHit(damage, source);
        //get angry or smth
    }



    public override void GetGathered(EntityBody source)
    {
        //get angry or smth
    }

    #endregion
    #region Actions
    public override IEnumerator PickUp(EntityBody target)
    {
        throw new NotImplementedException();
    }

    public override IEnumerator Drop(EntityBody target)
    {
        throw new NotImplementedException();
    }

    public override IEnumerator Eat(EntityBody target)
    {
        throw new NotImplementedException();
    }

    public override IEnumerator Gather(EntityBody target)
    {
        throw new NotImplementedException();
    }

    #endregion

}

