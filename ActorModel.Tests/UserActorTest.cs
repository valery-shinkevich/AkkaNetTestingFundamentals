namespace ActorModel.Tests
{
    using System;
    using Akka.Actor;
    using Akka.TestKit.TestActors;
    using Akka.TestKit.Xunit2;
    using Xunit;

    public class UserActorTest : TestKit
    {
        [Fact]
        public void ShouldHaveInitialState()
        {
            var actor = ActorOfAsTestActorRef<UserActor>(
                Props.Create(() => new UserActor(ActorOf(BlackHoleActor.Props))));
            Assert.Null(actor.UnderlyingActor.CurrentlyPlaying);
        }

        [Fact]
        public void ShouldUpdateCurrentlyPlayingState()
        {
            const string codenanTheBarbarian2 = "Codenan the Barbarian 2";
            var actor = ActorOfAsTestActorRef<UserActor>(
                Props.Create(() => new UserActor(ActorOf(BlackHoleActor.Props))));

            actor.Tell(new PlayMovieMessage(codenanTheBarbarian2));

            Assert.Equal(codenanTheBarbarian2, actor.UnderlyingActor.CurrentlyPlaying);
        }

        [Fact]
        public void ShouldSendNowPlayingMessage()
        {
            const string codenanTheBarbarian3 = "Codenan the Barbarian 3";
            var actor = ActorOfAsTestActorRef<UserActor>(
                Props.Create(() => new UserActor(ActorOf(BlackHoleActor.Props))));

            actor.Tell(new PlayMovieMessage(codenanTheBarbarian3));
            var message = ExpectMsgFrom<NowPlayingMessage>(actor);

            Assert.Equal(message.TitleName, codenanTheBarbarian3);
        }

        [Fact]
        public void ShouldLogPlayMovie()
        {
            var actor = ActorOf(Props.Create(() => new UserActor(ActorOf(BlackHoleActor.Props))));
            EventFilter
                .Info("Started playing Boolean Lies")
                .And
                .Info("Replaying to Sender")
                .Expect(2, () => actor.Tell(new PlayMovieMessage("Boolean Lies")));
        }

        [Fact]
        public void ShouldSendToDeadLetters()
        {
            var actor = ActorOf(Props.Create(() => new UserActor(ActorOf(BlackHoleActor.Props))));
            EventFilter.DeadLetter<PlayMovieMessage>(
                message => message.TitleName == "Boolean Lies"
            ).ExpectOne(() => actor.Tell(new PlayMovieMessage("Boolean Lies")));
        }

        [Fact]
        public void ShouldErrorOnUnknownMovie()
        {
            var actor = ActorOf(Props.Create(() => new UserActor(ActorOf(BlackHoleActor.Props))));
            EventFilter.Exception<NotSupportedException>("Unknown movie!").ExpectOne(
                () => actor.Tell(new PlayMovieMessage("")));
        }

        [Fact]
        public void ShouldPublishPlayingMovie()
        {
            var actor = ActorOf(Props.Create(() => new UserActor(ActorOf(BlackHoleActor.Props))));
            var subscriber = CreateTestProbe();
            Sys.EventStream.Subscribe(subscriber, typeof(NowPlayingMessage));

            actor.Tell(new PlayMovieMessage("Codenan the Barbarian"));
            subscriber.ExpectMsg<NowPlayingMessage>(
                message => message.TitleName == "Codenan the Barbarian");
        }

        [Fact]
        public void ShouldTerminated()
        {
            var actor = ActorOf(Props.Create(() => new UserActor(ActorOf(BlackHoleActor.Props))));

            Watch(actor);
            actor.Tell(PoisonPill.Instance);
            ExpectTerminated(actor);
        }

        [Fact]
        public void NonDeterministicMessageOrder()
        {
            var actor1 = ActorOf(() => new UserActor(null));
            var actor2 = ActorOf(() => new UserActor(null));

            actor1.Tell(new PlayMovieMessage("Codenan the Barbarian"));
            actor2.Tell(new PlayMovieMessage("Boolean Lies"));

            FishForMessage<NowPlayingMessage>(message => message.TitleName == "Boolean Lies");
            //or
            //IgnoreMessages(message => message is NowPlayingMessage &&
            //                          ((NowPlayingMessage) message).TitleName == "Codenan the Barbarian");
            //ExpectMsg<NowPlayingMessage>(message => message.TitleName == "Boolean Lies");
        }
    }
}