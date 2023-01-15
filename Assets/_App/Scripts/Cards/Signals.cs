namespace Cards
{
    public struct LoadingCompleteSignal
    {
    }

    public struct LoadCommand
    {
        public LoadCommand(int loaderId)
        {
            LoaderId = loaderId;
        }

        public int LoaderId { get; }
    }

    public struct CancelLoadingCommand
    {
    }
}