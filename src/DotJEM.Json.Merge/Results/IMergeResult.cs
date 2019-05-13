using Newtonsoft.Json.Linq;

namespace DotJEM.Json.Merge
{
    public interface IMergeResult
    {
        JToken Update { get; }
        JToken Merged { get; }
        JToken Other { get; }
        JToken Origin { get; }

        JObject Conflicts { get; }

        bool HasConflicts { get; }
    }
}