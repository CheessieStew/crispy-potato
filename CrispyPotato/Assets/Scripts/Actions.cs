
namespace Action
{
    public enum WalkingStyle
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

    public enum InteractStyle
    {
        Attack,
        Gather,
        PickUp
    }

    public enum Mood
    {
        Happy,
        Angry,
        Sad
    }

    public class BaseAction
    {
        public Mood CurrentMood;
    }

    public class Movement : BaseAction
    {
        public WalkingStyle Style;
        public int xCoord;
        public int yCoord;
    }

    public class Talk : BaseAction
    {
        public TalkStyle Style;
        public object Words;
    }

    public class Interact : BaseAction
    {
        public InteractStyle Style;
        public int TargetID;
    }
}