﻿
using System.Text;

namespace AiProtocol.Command
{
    public enum MovementStyle
    {
        Run,
        Walk,
        Sneak
    }

    public enum TalkStyle
    {
        Speak,
        Yell
    }

    public enum InteractionStyle
    {
        Attack,
        Gather,
        Drop,
        PickUp,
        Eat
    }

    public enum Mood
    {
        Happy,
        Angry,
        Sad
    }

    public abstract class BaseCommand
    {
        public Mood CurrentMood;
    }

    public class Movement : BaseCommand
    {
        public MovementStyle Style;
        public float xCoord;
        public float yCoord;

        public Movement() { }
        public Movement(MovementStyle s, float x, float y)
        {
            Style = s;
            xCoord = x;
            yCoord = y;
        }

        public override string ToString()
        {
            StringBuilder res = new StringBuilder();
            res.Append(Style);
            res.Append(" ");
            res.Append(xCoord);
            res.Append(" ");
            res.Append(yCoord);
            return res.ToString();
        }
    }

    public class Talk : BaseCommand
    {
        public TalkStyle Style;
        public Words Words;

        public Talk ()
        {

        }

        public Talk(TalkStyle style, Words words)
        {
            Style = style;
            Words = words;
        }

        public override string ToString()
        {
            return Style.ToString() + " " + Words.ToString();
        }
    }

    public class Interaction : BaseCommand
    {
        public InteractionStyle Style;
        public int TargetID;

        public override string ToString()
        {
            return Style.ToString() + " " + TargetID.ToString();
        }
    }



    public abstract class VillageInteraction :  BaseCommand
    {
        public int VillageID;
    }

    public class MagazinePush : VillageInteraction
    {
        public int TargetID;
        public override string ToString()
        {
            return "Push " + TargetID + " to " + VillageID;
        }
    }

    public class MagazinePull : VillageInteraction
    {
        public Descriptions.ResourceType Type;
        public override string ToString()
        {
            return "Pull " + Type.ToString() + " from " + VillageID;
        }
    }

    public class Procreate : VillageInteraction
    {
        public override string ToString()
        {
            return "Procreate with " + VillageID;
        }
    }

    public class PickTool : VillageInteraction
    {
        public Descriptions.ToolKind Tool;
        public override string ToString()
        {
            return "Pick " + Tool.ToString() + " from " + VillageID;
        }
    }


    public class Wait : BaseCommand
    {
        public override string ToString()
        {
            return "Wait";
        }
    }
}