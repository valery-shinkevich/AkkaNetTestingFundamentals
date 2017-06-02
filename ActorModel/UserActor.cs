namespace ActorModel
{
    using System;
    using Akka.Actor;
    using Akka.Event;

    public class UserActor : ReceiveActor
    {
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly IActorRef _stats;

        private readonly ILoggingAdapter _log = Context.GetLogger();
        public UserActor(IActorRef stats)
        {
            _stats = stats;
            Receive<PlayMovieMessage>(message =>
            {
                if (string.IsNullOrWhiteSpace(message.TitleName))
                {
                    throw new NotSupportedException("Unknown movie!");
                }

                _log.Info("Started playing {0}", message.TitleName);
                CurrentlyPlaying = message.TitleName;

                _log.Info("Replaying to Sender");
                Sender.Tell(new NowPlayingMessage(CurrentlyPlaying));
                _stats.Tell(CurrentlyPlaying);

                Context.ActorSelection("/user/audit").Tell(message);
                Context.System.EventStream.Publish(new NowPlayingMessage(CurrentlyPlaying));
            });
        }

        public string CurrentlyPlaying { get; private set; }
    }
}