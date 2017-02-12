namespace AiProtocol
{
    /// <summary>
    /// A class that all possible things any living entity might say should derive from.
    /// The default fields will be filled by the game when the actual Speak action is executed.
    /// </summary>
    public abstract class Words
    {
        public int sourceXCoords;
        public int sourceYCoords;
        public int talkerID;
    }

    /// <summary>
    /// The only Words a tiger will say
    /// </summary>
    public class Roar : Words
    {
        public override string ToString()
        {
            return "ROARRR!!!";
        }
    }

}