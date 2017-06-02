namespace ActorModel
{
    public class PlayMovieMessage
    {
        public PlayMovieMessage(string titleName)
        {
            TitleName = titleName;
        }

        public string TitleName { get; }
    }
}