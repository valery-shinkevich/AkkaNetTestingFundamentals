namespace ActorModel
{
    public class NowPlayingMessage
    {
        public NowPlayingMessage(string titleName)
        {
            TitleName = titleName;
        }

        public string TitleName { get; }
    }
}