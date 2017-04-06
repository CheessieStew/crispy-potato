using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ConfigFrontEnd : MonoBehaviour
{
    public string RelativePath { get; set; }
    public string jsonLoaded { get; private set; }
    private Config config;

    private bool pathExists = false;

    public bool Load()
    {
        if (!PathExists()) return false;
        using (StreamReader r = new StreamReader(RelativePath))
        {
            jsonLoaded = r.ReadToEnd();
        }
        return Verify();
    }

    private bool PathExists()
    {
        return pathExists = BrainLoader.FileExists(RelativePath);
    }

    public void CreateDefaultConfig()
    {
        using (StreamWriter sw = new StreamWriter("default_config.json"))
        {
            sw.Write(Config.ToJSON(new Config())); // bat dizajn
        }
    }

    private bool CheckConfigJSON()
    {
        if (!pathExists) return false;
        config = Config.FromJSON(jsonLoaded);
        return config != null;
    }

    public bool Verify()
    {
        print(RelativePath);

        var verifiers = new Func<bool>[]
        {
            PathExists,
            CheckConfigJSON
        };

        var texts = transform.Find("ConfigInfo");
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
