using System;

namespace DotJEM.Json.Merge
{
    public class JsonMergeConflictException : Exception
    {
        public IMergeResult MergeResult { get; }

        public JsonMergeConflictException(IMergeResult result)
            : base(result.ToString())
        {
            MergeResult = result;
        }

        public JsonMergeConflictException(IMergeResult result, string message)
            : base(message)
        {
            MergeResult = result;
        }

        public JsonMergeConflictException(IMergeResult result, string message, Exception inner)
            : base(message, inner)
        {
            MergeResult = result;
        }
    }
}