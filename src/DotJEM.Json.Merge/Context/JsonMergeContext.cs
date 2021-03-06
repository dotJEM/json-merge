﻿using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace DotJEM.Json.Merge
{
    public interface IJsonMergeContext
    {
        JToken Origin { get; }
        JToken Merged { get; }
        IMergeResult Noop(JToken update, JToken other);
        IMergeResult Multiple(IEnumerable<MergeResult> diffs, JObject update, JObject other);
        IMergeResult Merge(JToken update, JToken other);
        IJsonMergeContext Next(object key);
    }

    public class JsonMergeContext : IJsonMergeContext
    {
        private readonly object key;
        private readonly JToken parent;

        public JToken Merged { get; }
        public JToken Origin { get; }

        public JsonMergeContext(JToken merged, JToken origin)
        {
            Merged = merged;
            Origin = origin;
        }

        public JsonMergeContext(JToken merged, JToken origin, JToken parent, object key)
        {
            Merged = merged;
            Origin = origin;
            this.parent = parent;
            this.key = key;
        }

        public IMergeResult Noop(JToken update, JToken other)
        {
            return new MergeResult(false, update, other, Origin, Merged);
        }

        public IMergeResult Multiple(IEnumerable<MergeResult> diffs, JObject update, JObject other)
        {
            return new CompositeMergeResult(diffs, update, other, Origin, Merged);
        }

        public IMergeResult Merge(JToken update, JToken other)
        {
            if (JToken.DeepEquals(Origin, other))
            {
                //NOTE: (jmd 2015-11-23) The updated value here is already valid.
                return new MergeResult(false, update, other, Origin, Merged);
            }

            if (JToken.DeepEquals(Origin, update))
            {
                if (other == null)
                {
                    Merged.Parent.Remove();
                }
                else
                {
                    //NOTE: (jmd 2015-11-23) JSON.NET will automatically detect that other is from a different object, clone that at refresh it's parent.
                    //      So this might seem dirty, but it is not.
                    parent[key] = other;
                }
                return new MergeResult(false, update, other, Origin, Merged);
            }

            //NOTE: Conflict.
            return new MergeResult(true, update, other, Origin, Merged);
        }

        public IJsonMergeContext Next(object nextKey)
        {
            return new JsonMergeContext(Merged?[nextKey], Origin?[nextKey], Merged, nextKey);
        }
    }
}