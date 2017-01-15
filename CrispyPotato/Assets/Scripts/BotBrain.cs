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

    public Action.GameAction GetDesiredAction()
    {
        Action.GameAction action = new Action.GameAction();
        action.CurrentMood = Action.Mood.Happy;
        return action;
    }

}

