namespace ActorModel.Tests
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Akka.Actor;
    using Akka.TestKit.TestActors;
    using Akka.TestKit.Xunit2;
    using Xunit;

    public class IntegrationTests : TestKit
    {
        [Fact]
        public void UserActor_ShouldUpdatePlayCounts()
        {
            const string codenanTheBarbarian = "Codenan the Barbarian";
            var stats = ActorOfAsTestActorRef(() => new StatisticsActor(ActorOf(BlackHoleActor.Props)));
            var initialMovieStats = new Dictionary<string, int> {{codenanTheBarbarian, 17}};
            stats.Tell(new InitialStatisticsMessage(new ReadOnlyDictionary<string, int>(initialMovieStats)));

            var user = ActorOfAsTestActorRef<UserActor>(Props.Create(() => new UserActor(stats)));

            user.Tell(new PlayMovieMessage(codenanTheBarbarian));

            Assert.Equal(18, stats.UnderlyingActor.PlayCounts[codenanTheBarbarian]);
        }
    }
}