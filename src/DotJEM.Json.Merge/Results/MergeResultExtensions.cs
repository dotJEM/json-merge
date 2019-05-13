namespace DotJEM.Json.Merge
{
    public static class MergeResultExtensions
    {
        public static IMergeResult AddVersion(this IMergeResult self, long yourVersion, long otherVersion)
        {
            return new MergeResultWithVersion((MergeResult)self, yourVersion, otherVersion);
        }
    }
}