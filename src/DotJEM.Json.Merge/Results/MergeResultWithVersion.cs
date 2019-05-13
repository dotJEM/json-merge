using Newtonsoft.Json.Linq;

namespace DotJEM.Json.Merge
{
    public class MergeResultWithVersion : IMergeResult
    {
        private readonly MergeResult inner;

        private readonly long originVersion;
        private readonly long otherVersion;

        public JObject Conflicts => BuildDiff(new JObject(), false);
        public bool HasConflicts => inner.HasConflicts;
        public JToken Origin => inner.Origin;
        public JToken Other => inner.Other;
        public JToken Update => inner.Update;

        public JToken Merged
        {
            get
            {
                if (HasConflicts)
                    throw new JsonMergeConflictException(this);

                return inner.Merged;
            }
        }

        public MergeResultWithVersion(MergeResult inner, long originVersion, long otherVersion)
        {
            this.inner = inner;
            this.originVersion = originVersion;
            this.otherVersion = otherVersion;
        }

        private JObject BuildDiff(JObject diff, bool includeResolvedConflicts)
        {
            diff["$version"] = new JObject
            {
                ["update"] = $"{originVersion}++",
                ["other"] = otherVersion,
                ["origin"] = originVersion
            };
            inner.BuildDiff(diff, includeResolvedConflicts);
            return diff;
        }

        public override string ToString()
        {
            if (HasConflicts)
            {
                return $"{inner} Latest version was {otherVersion}, update was at version {otherVersion}++.";
            }
            return $"{inner}, version is {otherVersion + 1}";
        }
    }
}