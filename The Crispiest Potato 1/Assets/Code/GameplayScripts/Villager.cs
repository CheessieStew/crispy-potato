using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.CodeDom.Compiler;
using System.Reflection;
using Microsoft.CSharp;
using System;
using System.Text;
using System.IO;

public class Villager : Animal
{
    protected override void Start()
    {
        base.Start();
        if (((VillagerBody)Body).Female)
            Destroy(transform.Find("MaleRegalia").gameObject);
        else
            Destroy(transform.Find("FemaleRegalia").gameObject);


    }



}


