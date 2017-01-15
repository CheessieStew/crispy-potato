
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

    public class GameAction
    {
        public Mood CurrentMood;
    }

    public class MovementAction : GameAction
    {
        public WalkingStyle Style;
        public int xCoord;
        public int yCoord;
    }

    public class TalkAction : GameAction
    {
        public TalkStyle Style;
        public object Words;
    }

    public class InteractAction : GameAction
    {
        public InteractStyle Style;
        public int TargetID;
    }
}