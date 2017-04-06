using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Xml;
using Assets.Code.Managers;
using UnityEngine;
using UnityEngine.UI;

public class BrainClassFrontEnd : MonoBehaviour
{
    public string RelativePath { get; set; }

    public string ClassName
    {
        get
        {
            return BrainLoader.ClassName;
        }
        set
        {
            BrainLoader.ClassName = value;
        }
    }

    public bool Load()
    {
        BrainLoader.LoadAssembly(RelativePath);
        return Verify();
    }

    public bool Verify()
    {
        print(ClassName);
        print(RelativePath);

        var verifiers = new Func<bool>[]
        {
            () => BrainLoader.FileExists(RelativePath),
            BrainLoader.AssemblyLoaded,
            BrainLoader.BrainClassExists,
            BrainLoader.BrainClassCorrect
        };

        var texts = transform.Find("BrainClassInfo");
        var results = verifiers.Select((x, i) =>
        {
            var val = x.Invoke();
            var t = texts.transform.GetChild(i).GetChild(0).GetComponent<Text>();
            t.text = val.ToString().ToUpper();
            t.color = val ? Color.green : Color.red;
            t.fontStyle = FontStyle.Bold;
            return val;
        });
        return results.All(x => x);
    }
}
