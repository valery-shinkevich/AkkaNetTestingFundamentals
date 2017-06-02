namespace ActorModel
{
    using System.Collections.ObjectModel;

    public class InitialStatisticsMessage
    {
        public InitialStatisticsMessage(ReadOnlyDictionary<string, int> initialPlayCounts)
        {
            InitialPlayCounts = initialPlayCounts;
        }

        public ReadOnlyDictionary<string, int> InitialPlayCounts { get; }
    }
}