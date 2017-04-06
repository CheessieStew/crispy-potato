using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlsManager : Singleton<ControlsManager>
{
    public InputField Input;

    bool focusNotOnConsole = true;

    public bool FocusNotOnConsole
    {
        get
        {
            return (focusNotOnConsole);
        }
    }

    public void ConsoleFocus()
    {
        print("set to false");
        focusNotOnConsole = false;
    }

    public void ConsoleDeFocus()
    {
        print("set to true");
        focusNotOnConsole = true;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
