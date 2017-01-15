using System;


public class VillagerBot : IVillageBot
{
    public GameStateInfo Info
    {
        get;
        set;
    }

    public VillagerBot()
    {
        Info = new GameStateInfo();
    }

    public Action.BaseAction GetDesiredAction()
    {
        Action.BaseAction action = new Action.BaseAction();
        action.CurrentMood = Action.Mood.Happy;
        return action;
    }

}

