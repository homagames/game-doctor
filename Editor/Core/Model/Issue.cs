namespace HomaGames.GameDoctor
{
    public abstract class Issue
    {
        public float Progress;
        public bool Fixed;
        public abstract ImportanceType Importance { get; }
        public abstract void Fix();
    }
}