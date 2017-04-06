using AiProtocol.Descriptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.AI;

public abstract class EntityBody : MonoBehaviour
{
    public int MaxHealth;
    public int MaxEnergy;
    public string Name;
    public float Size;
    public EntityType GenericName;

    public Vector2 Position
    {
        get
        {
            return
                new Vector2(transform.position.x - GameManager.Instance.LevelOrigin.position.x,
                    transform.position.z - GameManager.Instance.LevelOrigin.position.z);
            
        }
    }

    public float DistanceTo(EntityBody other)
    {
        float res =(transform.position - other.transform.position)
            .magnitude - Size - other.Size;
        return res > 0 ? res : 0;
    }

    public int EntityID { get; private set; } 
    public int Health { get; protected set; }
    public int Energy { get; protected set; }
    public bool PickedUp { get; protected set; }

    GameObject selectRing;

    #region Technical
    public virtual void Start()
    {
        selectRing = this.transform.Find("SelectRing").gameObject;
        Register();
        Health = MaxHealth;
        Energy = MaxEnergy;
        PickedUp = false;
    }

    void Register()
    {
        if (EntityID == 0)
            EntityID = EntityManager.DispenseID();
        EntityManager.EntityHash[EntityID] = this;
    }

    void UnRegister()
    {
        EntityManager.EntityHash.Remove(EntityID);
    }

    public void Selected(bool b)
    {
        if (selectRing != null)
            selectRing.SetActive(b);
    }

    #endregion

    #region Reactions
    public virtual void OnDeath()
    {
        UnRegister();
        Destroy(gameObject);

    }

    public virtual void TakeHit(int damage, EntityBody source)
    { 
        Health -= damage;
        if (Health <= 0)
            OnDeath();
    }

    public virtual bool GetPickedUp(EntityBody source)
    {
        if (PickedUp)
            return false;
        return PickedUp = true;
    }

    public virtual void GetDropped(EntityBody source)
    {
        if (PickedUp)
            PickedUp = false;
    }

    public virtual bool GetEaten(EntityBody source)
    {
        return false;
    }

    public abstract void GetGathered(EntityBody source);

    #endregion

    public abstract BaseDescription Description { get; }

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, Size);
    }

}

