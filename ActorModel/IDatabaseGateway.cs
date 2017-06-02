namespace ActorModel
{
    using System.Collections.Generic;

    public interface IDatabaseGateway
    {
        IDictionary<string, int> GetStoredStatistics();
    }
}