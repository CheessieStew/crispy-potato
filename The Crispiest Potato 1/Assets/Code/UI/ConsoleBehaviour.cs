using AiProtocol.Command;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConsoleBehaviour : MonoBehaviour {

	void Start() {
		
	}
	
    public void Submit()
    {
        if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
        {
            print("SUBMIT");
            var input = GetComponent<InputField>();
            BrainWrapper villagerBrain = null;
            if (GameManager.Instance.SelectionController.Selected != null)
                villagerBrain = GameManager.Instance.SelectionController.Selected.GetComponent<BrainWrapper>();
            string msg = "";
            if (villagerBrain != null)
            {
                BaseCommand res = null;

                try
                {
                    res = ParseCommand(input.text);
                    msg = "";
                }
                catch (Exception e)
                {
                    msg = e.Message;
                }

                if (res != null)
                {
                    villagerBrain.Brain.SetNextAction(res);
                }
                else if (GameManager.Instance.Config.CheatsOn)
                {
                    try
                    {
                        ParseCheat(input.text);
                        msg = "";
                    }
                    catch (Exception e)
                    {
                        msg += e.Message;
                    }
                }
            }
            else if (GameManager.Instance.Config.CheatsOn)
                try
                {
                    ParseCheat(input.text);
                    msg = "";
                    print("cheat parsed");
                }
                catch (Exception e)
                {
                    msg = e.Message;
                }

            if (msg != "")
                print("Can't parse \"" + input.text + "\"\n" + msg);
            else
                print("OK");
            //todo - put feedback above the console instead of printing it
            input.text = "";
        }
    }

    static void ParseCheat(string text)
    {
        string[] tokens = text.ToLowerInvariant().Split(' ');
        if (tokens.Length < 1)
            throw new Exception("Can't parse cheat");
        switch(tokens[0])
        {
            case "takedamage":
                if (tokens.Length != 2)
                    throw new Exception("TakeDamage needs 1 argument");
                int dmg;
                if (int.TryParse(tokens[1], out dmg))
                    GameManager.Instance.SelectionController.Selected.TakeHit(dmg, null);
                else
                    throw new Exception("TakeDamage argument needs to be an integer");
                return;
            case "getgathered":
                if (tokens.Length != 1)
                    throw new Exception("GetGathered needs no arguments");
                print("lol");
                GameManager.Instance.SelectionController.Selected.GetGathered(null);
                return;
            default:
                throw new Exception("Cheat not recognized");

        }
    }

    static BaseCommand ParseCommand(string text)
    {
        string[] tokens = text.Split(' ');
        if (tokens.Length == 0)
            return null;
        BaseCommand res = null;
        string msg = "";

        try
        {
            res = ParseInteract(tokens, (InteractionStyle)System.Enum.Parse(typeof(InteractionStyle), tokens[0], true));
        }
        catch (Exception e )
        {
            msg += "Not an interaction: " + e.Message + "\n";
        }
        if (res != null)
            return res;

        try
        {
            res = ParseMove(tokens, (MovementStyle)System.Enum.Parse(typeof(MovementStyle), tokens[0], true));
        }
        catch (Exception e)
        {
            msg += "Not a movement: " + e.Message + "\n";
        }
        if (res != null)
            return res;

        try
        {
            res = ParsePickTool(tokens, (AiProtocol.Descriptions.ToolKind)System.Enum.Parse(typeof(AiProtocol.Descriptions.ToolKind), tokens[2], true));
        }
        catch (Exception e)
        { msg += "Not a PickTool: " + e.Message + "\n"; }
        if (res != null)
            return res;

        try
        { res = ParseMagazinePush(tokens); }
        catch (Exception e)
        { msg += "Not a MagazinePush: " + e.Message + "\n"; }
        if (res != null)
            return res;

        try
        {
            res = ParseMagazinePull(tokens, (AiProtocol.Descriptions.ResourceType)System.Enum.Parse(typeof(AiProtocol.Descriptions.ResourceType), tokens[2], true));
        }
        catch (Exception e)
        { msg += "Not a MagazinePull: " + e.Message + "\n"; }
        if (res != null)
            return res;


        try
        { res = ParseProcreate(tokens); }
        catch (Exception e)
        { msg += "Not a MagazinePush: " + e.Message + "\n"; }
        if (res != null)
            return res;

        throw new Exception(msg);
    }

    static Procreate ParseProcreate(string[] tokens)
    {
        if (tokens.Length < 2 || tokens[0].ToLowerInvariant() != "procreate")
            return null;
        var res = new Procreate();
        if (int.TryParse(tokens[1], out res.VillageID))
            return res;
        else return null;
    }

    static MagazinePush ParseMagazinePush(string[] tokens)
    {
        if (tokens.Length < 3 || tokens[0].ToLowerInvariant() != "magazinepush")
            return null;
        var res = new MagazinePush();
        if (int.TryParse(tokens[1], out res.VillageID)
            && int.TryParse(tokens[2], out res.TargetID))
            return res;
        else return null;
    }

    static MagazinePull ParseMagazinePull(string[] tokens, AiProtocol.Descriptions.ResourceType type)
    {
        if (tokens.Length < 3 || tokens[0].ToLowerInvariant() != "magazinepull")
            return null;
        var res = new MagazinePull();
        res.Type = type;
        if (int.TryParse(tokens[1], out res.VillageID))
            return res;
        else return null;
    }

    static PickTool ParsePickTool(string[] tokens, AiProtocol.Descriptions.ToolKind tool)
    {
        if (tokens.Length < 3 || tokens[0].ToLowerInvariant() != "picktool")
            return null;
        var res = new PickTool();
        res.Tool = tool;
        if (int.TryParse(tokens[1], out res.VillageID))
            return res;
        else return null;
    }

    static Interaction ParseInteract(string[] tokens, InteractionStyle style)
    {
        if (tokens.Length < 2)
            return null;
        var res = new Interaction();
        res.Style = style;

        if (int.TryParse(tokens[1], out res.TargetID))
            return res;
        else return null;
    }

    static Movement ParseMove(string[] tokens, MovementStyle style)
    {
        if (tokens.Length < 3)
            return null;
        var res = new Movement();
        res.Style = style;

        if (float.TryParse(tokens[1], out res.xCoord) &&
            float.TryParse(tokens[2], out res.yCoord))
            return res;
        else return null;
    }

    // Update is called once per frame
    void Update () {
		
	}
}
