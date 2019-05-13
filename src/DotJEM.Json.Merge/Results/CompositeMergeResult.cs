using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace DotJEM.Json.Merge
{
    public class CompositeMergeResult : MergeResult
    {
        private readonly List<MergeResult> diffs;

        public CompositeMergeResult(IEnumerable<MergeResult> diffs, JToken update, JToken other, JToken origin, JToken merged)
            : this(diffs.ToList(), update, other, origin, merged)
        {
        }

        protected CompositeMergeResult(List<MergeResult> diffs, JToken update, JToken other, JToken origin, JToken merged)
            : base(diffs.Any(f => f.HasConflicts), update, other, origin, merged)
        {
            this.diffs = diffs;
        }

        internal override JObject BuildDiff(JObject diff, bool includeResolvedConflicts)
        {
            return diffs.Aggregate(diff, (o, result) => result.BuildDiff(o, includeResolvedConflicts));
        }

        public override string ToString()
        {
            return HasConflicts
                ? $"{diffs.Count(m => m.HasConflicts)} was detected, first conflict was {diffs.FirstOrDefault(f => f.HasConflicts)}"
                : $"{diffs.Count} merge results without conflicts.";
        }
    }
}