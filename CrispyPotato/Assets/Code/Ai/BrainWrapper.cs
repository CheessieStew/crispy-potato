using System;
using System.Collections;
using System.Collections.Generic;
using AiProtocol.Command;
using AiProtocol.Descriptions;
using UnityEngine;
using System.Reflection;
using System.IO;
using UnityEngine.UI;
using AiProtocol;

public class BrainWrapper: MonoBehaviour, AiProtocol.IBrain
{
    public void SetRules(AiProtocol.GameRules rules)
    {
        Brain.SetRules(rules);
    }

    public AiProtocol.IBrain Brain
    {
        get; set;
    }

    public void SetNextAction(BaseCommand command)
    {
        Brain.SetNextAction(command);
    }

    public void Feel(BodilyFunctions functions)
    {
        Brain.Feel(functions);
    }

    public IEnumerable<BaseCommand> GetDecisions()
    {
        return Brain.GetDecisions();
    }

    public void Hear(Words words)
    {
        Brain.Hear(words);
    }

    void Start()
    {
        if (Brain == null)
            throw new Exception("NULL BRAIN OMG");
        SetRules(GameManager.Instance.Rules);
    }

    public void See(IEnumerable<BaseDescription> descriptions)
    {
        Brain.See(descriptions);
    }

    public void Initialize(IBrainGenetics genetics)
    {
        Brain.Initialize(genetics);
    }

    public IBrainGenetics GetGeneticMaterial()
    {
        return Brain.GetGeneticMaterial();
    }
}
