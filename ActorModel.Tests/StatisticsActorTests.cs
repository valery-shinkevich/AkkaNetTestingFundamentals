namespace ActorModel.Tests
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Akka.TestKit;
    using Akka.TestKit.TestActors;
    using Akka.TestKit.Xunit2;
    using Xunit;

    public class StatisticsActorTests : TestKit
    {
        [Fact]
        public void ShouldHaveIntialPlayCountsValue()
        {
            var actor = new StatisticsActor(null);
            Assert.Null(actor.PlayCounts);
        }

        [Fact]
        public void ShouldReceiveIntialStatisticsMessage()
        {
            var actor = ActorOfAsTestActorRef(() => new StatisticsActor(ActorOf(BlackHoleActor.Props)));
            var initialMovieStats = new Dictionary<string, int> {{"Codenan the Barbarian", 45}};

            actor.Tell(new InitialStatisticsMessage(new ReadOnlyDictionary<string, int>(initialMovieStats)));

            Assert.Equal(45, actor.UnderlyingActor.PlayCounts["Codenan the Barbarian"]);
        }

        [Fact]
        public void ShouldUpdatePlayCounts()
        {
            var actor = ActorOfAsTestActorRef(() => new StatisticsActor(ActorOf(BlackHoleActor.Props)));
            const string codenanTheBarbarian4 = "Codenan the Barbarian 4";

            var initialMovieStats = new Dictionary<string, int> {{codenanTheBarbarian4, 45}};

            actor.Tell(new InitialStatisticsMessage(new ReadOnlyDictionary<string, int>(initialMovieStats)));
            actor.Tell(codenanTheBarbarian4);

            Assert.Equal(46, actor.UnderlyingActor.PlayCounts[codenanTheBarbarian4]);
        }

        [Fact]
        public void ShouldSetOneToPlayCountsForNewMovie()
        {
            var actor = ActorOfAsTestActorRef(() => new StatisticsActor(ActorOf(BlackHoleActor.Props)));
            const string codenanTheBarbarian5 = "Codenan the Barbarian 5";

            actor.Tell(new InitialStatisticsMessage(
                new ReadOnlyDictionary<string, int>(new Dictionary<string, int>())));
            actor.Tell(codenanTheBarbarian5);

            Assert.Equal(1, actor.UnderlyingActor.PlayCounts[codenanTheBarbarian5]);
        }

        [Fact]
        public void ShouldGetInitialStatsFromDatabase()
        {
            var mockDb = CreateTestProbe();
            var messageHandler = new DelegateAutoPilot((sender, message) =>
            {
                if (message is GetInitialStatisticMessage)
                {
                    var stats = new Dictionary<string, int>
                    {
                        {"Codenan the Barbarian", 42}
                    };
                    sender.Tell(new InitialStatisticsMessage(new ReadOnlyDictionary<string, int>(stats)), TestActor);
                }
                return AutoPilot.KeepRunning;
            });

            mockDb.SetAutoPilot(messageHandler);

            var actor = ActorOfAsTestActorRef(() => new StatisticsActor(mockDb));

            Assert.Equal(42, actor.UnderlyingActor.PlayCounts["Codenan the Barbarian"]);
        }

        [Fact]
        public void ShouldAskDatabaseForInitialStats()
        {
            var mockDb = CreateTestProbe();

            var actor = ActorOfAsTestActorRef(() => new StatisticsActor(mockDb));
            mockDb.ExpectMsg<GetInitialStatisticMessage>();
        }
    }
}