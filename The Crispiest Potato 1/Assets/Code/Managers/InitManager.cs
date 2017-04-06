using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Code.Managers
{
    class InitManager : MonoBehaviour
    {
        protected InitManager() { }
        // public GameObject SettingsDock;
        public BrainClassFrontEnd brainFront;
        public ConfigFrontEnd configFront;

        public static string HumanizeFieldName(string s)
        {
            StringBuilder builder = new StringBuilder();
            foreach (char c in s)
            {
                if (char.IsUpper(c) && builder.Length > 0)
                    builder.Append(' ');
                builder.Append(c);
            }
            return builder.ToString();
        }

        void Start()
        {
            if (!BrainLoader.FileExists("default_config.json"))
            {
                using (StreamWriter sw = new StreamWriter("default_config.json"))
                {
                    sw.Write(Config.ToJSON(new Config())); // bat dizajn
                }
            }
        }

        public void OKButtonClicked()
        {
            if (brainFront.Verify() && configFront.Verify())
            {
                SceneManager.LoadScene(1);
                ExportSettingsToGameManager();  
            }
        }

        public void EvaluateBrainInput()
        {
            configFront.Load();
            brainFront.Load();
        }

        public void ExportSettingsToGameManager()
        {
            GameManager.staticConfig = Config.FromJSON(configFront.jsonLoaded);
        }
    }

    

}

[Serializable]
public class SettingFloatDictionary : SerializableDictionary<int, UnityEngine.Object> { }

[Serializable]
public class SettingFloatHash : SettingFloatDictionary
{
    public override UnityEngine.Object this[int key]
    {
        get { return this[key, null]; }
        set { Insert(key, value, false); }
    }
}

