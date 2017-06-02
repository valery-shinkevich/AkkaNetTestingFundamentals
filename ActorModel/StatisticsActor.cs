namespace ActorModel
{
    using System.Collections.Generic;
    using Akka.Actor;

    public class StatisticsActor : ReceiveActor
    {
        private readonly IActorRef _databaseActor;

        public StatisticsActor(IActorRef databaseActor)
        {
            _databaseActor = databaseActor;

            Receive<InitialStatisticsMessage>(message =>
                PlayCounts = new Dictionary<string, int>(message.InitialPlayCounts));

            Receive<string>(s =>
            {
                if (PlayCounts.ContainsKey(s))
                    PlayCounts[s]++;
                else
                    PlayCounts.Add(s, 1);
            });
        }

        public Dictionary<string, int> PlayCounts { get; set; }

        protected override void PreStart()
        {
            _databaseActor.Tell(new GetInitialStatisticMessage());
        }
    }
}