using System.Linq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace DotJEM.Json.Merge.Test
{
    public static class TestUtil
    {
        public static JToken JSON(object json)
        {
            if (json is string jsonStr)
                return JToken.Parse(jsonStr);

            if (json is JToken jsonToken)
                return jsonToken;

            return JToken.FromObject(json);
        }

        public static TestCaseData CASE(params string[] args)
        {
            return new TestCaseData(args.Select(JSON).Cast<object>().ToArray());
        }
    }
}