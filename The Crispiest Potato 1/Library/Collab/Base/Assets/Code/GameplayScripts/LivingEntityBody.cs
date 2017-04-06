using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using AiProtocol.Descriptions;
using AiProtocol.Command;

public abstract class LivingEntityBody : EntityBody
{
    protected NavMeshAgent agent;
    public Faction Alignment;
    public bool Female;

    public BaseCommand CurrentCommand { get; protected set; }

    #region DerivedAttributes
    public float Progress { get; protected set; }
    public virtual float WalkingSpeed
    {
        get
        {
            return 4 * GameManager.Instance.BaseSpeedMultiplier;
        }
    }
    public virtual float SpeakingTime
    {
        get
        {
            return 2f;
        }
    }
    public virtual int AttackDamage
    {
        get
        {
            return 5;
        }
    }
    public virtual float AttackDuration
    {
        get { return 1.0f; }
    }
    public virtual float SeeingDistance
    {
        get { return GameManager.Instance.DefaultSeeingDistance; }
    }
    public bool IsSneaking { get; private set; }
    public virtual float Sneakiness
    {
        get { return 1.0f; }
    }
    #endregion

    #region Technical
    public override void Start()
    {
        base.Start();
        lastTime = Time.time;
        agent = GetComponent<NavMeshAgent>();
    }

    public virtual void Update()
    {
        if (CurrentCommand != null && CurrentCommand.CurrentMood == Mood.Angry)
            transform.Rotate(UnityEngine.Random.onUnitSphere, UnityEngine.Random.value);

    }

    public virtual IEnumerator Execute(BaseCommand command)
    {

        if (Health <= 0)
            yield break;
        if (command is Interaction)
            yield return Interact((Interaction)command);
        else if (command is Talk)
            yield return Speak((Talk)command);
        else if (command is Movement)
            yield return Move((Movement)command);
        else throw new ArgumentOutOfRangeException();
    }

    protected IEnumerator WaitWithProgress(float time)
    {
        float timer = 0;
        Progress = 0;
        while (timer < time)
        {
            timer += Time.deltaTime;
            Progress = timer / time;
            yield return null;
        }
    }
    #endregion

    #region Actions
    /// <summary>
    /// After SpeakingTime seconds,
    /// broadcast a message to all
    /// entities in range, triggering their Hear()
    /// </summary>
    public virtual IEnumerator Speak(Talk talk)
    {
        CurrentCommand = talk;
        float maxDistance = GameManager.Instance.MaxHearingDistance;
        switch (talk.Style)
        {
            case TalkStyle.Speak:
                IsSneaking = true;
                break;
            case TalkStyle.Yell:
                IsSneaking = false;
                maxDistance *= 4;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        yield return StartCoroutine(WaitWithProgress(SpeakingTime));

        int c = 0;
        foreach (EntityBody body in EntityManager.Entities)
        {
            if (DistanceTo(body) <= maxDistance)
            {
                LivingEntityBody b = body as LivingEntityBody;
                if (b != null)
                {
                    c++;
                    Animal a = b.GetComponent<Animal>();
                    if (a != null)
                        a.Hear(talk);
                }
            }
        }
        CurrentCommand = null;
    }

    /// <summary>
    /// Set a destination and speed, based on WalkingSpeed 
    /// and MovementStyle. Set speed to 0 after
    /// WalkingWindowSize seconds
    /// </summary>
    public virtual IEnumerator Move(Movement movement)
    {
        CurrentCommand = movement;
        agent.speed = WalkingSpeed;
        switch (movement.Style)
        {
            case MovementStyle.Run:
                agent.speed *= 2;
                IsSneaking = false;
                break;
            case MovementStyle.Walk:
                IsSneaking = false;
                break;
            case MovementStyle.Sneak:
                IsSneaking = true;
                agent.speed *= 0.5f;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        agent.Resume();
        agent.destination = GameManager.Instance.LevelOrigin.position + new Vector3(movement.xCoord, 0, movement.yCoord);
        yield return WaitWithProgress(GameManager.Instance.WalkingWindowSize);
        agent.speed = 0;
        agent.Stop();
        CurrentCommand = null;
    }


    /// <summary>
    /// Pick the right coroutine for the interaction
    /// </summary
    public virtual IEnumerator Interact(Interaction interaction)
    {

        CurrentCommand = interaction;
        var style = interaction.Style;
        var targetID = interaction.TargetID;
        var mood = interaction.CurrentMood;
        var target = EntityManager.EntityHash[targetID];
        if (DistanceTo(target) > GameManager.Instance.MaxInteractionDistance)
            throw new Exception("Too far to interact");

        switch (style)
        {
            case InteractionStyle.Attack:
                IsSneaking = false;
                yield return StartCoroutine(Attack(target));
                break;
        
            case InteractionStyle.Gather:
                IsSneaking = false;
                yield return StartCoroutine(Gather(target));
                break;
        
            case InteractionStyle.Drop:
                IsSneaking = true;
                yield return StartCoroutine(Drop(target));
                break;

            case InteractionStyle.PickUp:
                IsSneaking = true;
                yield return StartCoroutine(PickUp(target));
                break;
        
            case InteractionStyle.Eat:
                IsSneaking = true;
                yield return StartCoroutine(Eat(target));
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
        CurrentCommand = null;
    }

    // only Attack has a default implementation
    // the rest is pretty much only aplicable to Villagers
    public virtual IEnumerator Attack(EntityBody target)
    {
        //TODO: attack speed and damage based on stuff
        yield return StartCoroutine(WaitWithProgress(AttackDuration));
        if (target != null)
        {
            ProjectileBehaviour proj = 
                Instantiate<ProjectileBehaviour>(GameManager.Instance.Projectile,transform.position,Quaternion.identity);
            proj.target = target.gameObject;
           
            target.TakeHit(AttackDamage, this);
        }
    }

    public abstract IEnumerator Gather(EntityBody target);

    public abstract IEnumerator PickUp(EntityBody target);

    public abstract IEnumerator Drop(EntityBody target);

    public abstract IEnumerator Eat(EntityBody target);
    #endregion


    public virtual IEnumerable<BaseDescription> GetDescriptions()
    {

        foreach (var entityBody in EntityManager.Entities)
        {
            LivingEntityBody lb = entityBody as LivingEntityBody;
            float modifier = lb == null
                ? 1f : lb.Sneakiness;

            if (DistanceTo(entityBody) < SeeingDistance * modifier
                && entityBody != this)
            {
                yield return entityBody.Description;
            }
        }
    }

    public override BaseDescription Description
    {
        get
        {
            return BodilyFunctions;
        }
    }

    float lastTime;
    public virtual BodilyFunctions BodilyFunctions
    {
        get
        {
            float newTime = Time.time;
            float delta = newTime - lastTime;
            lastTime = newTime;
            return new BodilyFunctions
            {
                DeltaTime = delta,
                HP = Health / (float)MaxHealth,
                Energy = this.Energy / (float)MaxEnergy,
                lastCommand = CurrentCommand,
                EntityID = EntityID,
                xCoord = (Position.x),
                yCoord = (Position.y),
                Alignment = this.Alignment,
                IsFemale = Female
            };
        }
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
#if UNITY_EDITOR
        if (!UnityEditor.EditorApplication.isPlaying)
            return;
#endif
        if (GameManager.Instance == null)
            return;

        Gizmos.color = new Color(1f, .3f, .3f, .25f);
        Gizmos.DrawSphere(transform.position, GameManager.Instance.MaxHearingDistance);

        Gizmos.color = new Color(1f, .3f, .3f, .25f);
        Gizmos.DrawSphere(transform.position, GameManager.Instance.MaxHearingDistance * 4f);

        Gizmos.color = new Color(.1f, .1f, 1f, .4f);
        Gizmos.DrawSphere(transform.position, SeeingDistance);

        Gizmos.color = new Color(1f, 1f, 1f, .25f);
        Gizmos.DrawSphere(transform.position, GameManager.Instance.MaxInteractionDistance);


    }

}
