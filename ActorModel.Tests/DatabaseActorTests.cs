namespace ActorModel.Tests
{
    using System.Collections.Generic;
    using Akka.Actor;
    using Akka.TestKit.Xunit2;
    using FakeItEasy;
    using Xunit;

    public class DatabaseActorTests : TestKit
    {
        [Fact]
        public void ShouldReadStatsFromDatabase()
        {
            var statsData = new Dictionary<string, int>
            {
                {"Boolean Lies", 42},
                {"Codenan the Barbarian", 200}
            };

            var gateway = A.Fake<IDatabaseGateway>();
            A.CallTo(() => gateway.GetStoredStatistics()).Returns(statsData);
            var actor = ActorOf(Props.Create(() => new DatabaseActor(gateway)));

            actor.Tell(new GetInitialStatisticMessage());
            var received = ExpectMsg<InitialStatisticsMessage>();

            Assert.Equal(42, received.InitialPlayCounts["Boolean Lies"]);
            Assert.Equal(200, received.InitialPlayCounts["Codenan the Barbarian"]);
        }
    }
}