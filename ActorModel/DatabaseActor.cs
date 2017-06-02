namespace ActorModel
{
    using System.Collections.ObjectModel;
    using Akka.Actor;

    public class DatabaseActor : ReceiveActor
    {
        private readonly IDatabaseGateway _databaseGateway;

        public DatabaseActor(IDatabaseGateway databaseGateway)
        {
            _databaseGateway = databaseGateway;

            Receive<GetInitialStatisticMessage>(message =>
            {
                var storedStats = _databaseGateway.GetStoredStatistics();
                Sender.Tell(new InitialStatisticsMessage(
                    new ReadOnlyDictionary<string, int>(storedStats)));
            });
        }
    }
}