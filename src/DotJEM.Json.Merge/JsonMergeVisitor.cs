using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace DotJEM.Json.Merge
{
    public interface IJsonMergeVisitor
    {
        IMergeResult Merge(JToken update, JToken conflicted, JToken origin);
        IMergeResult Merge(JToken update, JToken conflicted, IJsonMergeContext context);
    }

    public class JsonMergeVisitor : IJsonMergeVisitor
    {
        private readonly HashSet<string> ignoredFields;

        public JsonMergeVisitor(params string[] ignoredFields)
            : this(ignoredFields.AsEnumerable())
        {
        }

        public JsonMergeVisitor(IEnumerable<string> ignoredFields)
        {
            this.ignoredFields = new HashSet<string>(ignoredFields);
        }

        public IMergeResult Merge(JToken update, JToken conflicted, JToken origin)
            => Merge(update, conflicted, new JsonMergeContext(update.DeepClone(), origin));

        public virtual IMergeResult Merge(JToken update, JToken conflicted, IJsonMergeContext context)
        {
            if (ignoredFields.Contains(update?.Path ?? conflicted?.Path))
                return context.Noop(update, conflicted);

            if (update == null && conflicted == null)
                return context.Noop(null, null);

            if (update == null || conflicted == null || update.Type != conflicted.Type)
                return context.Merge(update, conflicted);

            //TODO: (jmd 2015-11-23) Could change this into try cast into JValue, JObject, JArray -> Might be easier to read.
            //TODO: We need to take care of scenarios where the objects differ in type.
            switch (update.Type)
            {
                case JTokenType.Object:
                    return MergeObject((JObject)update, (JObject)conflicted, context);
                case JTokenType.Array:
                    return MergeArray((JArray)update, (JArray)conflicted, context);
                case JTokenType.Integer:
                case JTokenType.Float:
                case JTokenType.String:
                case JTokenType.Boolean:
                case JTokenType.Null:
                case JTokenType.Undefined:
                case JTokenType.Date:
                case JTokenType.Raw:
                case JTokenType.Bytes:
                case JTokenType.Guid:
                case JTokenType.Uri:
                case JTokenType.TimeSpan:
                    return MergeValue((JValue)update, (JValue)conflicted, context);
            }

            throw new ArgumentOutOfRangeException();
        }

        protected virtual IMergeResult MergeValue(JValue update, JValue other, IJsonMergeContext context)
        {
            return !JToken.DeepEquals(update, other)
                ? context.Merge(update, other)
                : context.Noop(update, other);
        }

        protected virtual IMergeResult MergeObject(JObject update, JObject other, IJsonMergeContext context)
        {
            IEnumerable<MergeResult> diffs = from key in UnionKeys(update, other)
                let diff = (MergeResult)Merge(update[key], other[key], context.Next(key))
                select diff;

            return context.Multiple(diffs, update, other);
        }

        protected virtual IMergeResult MergeArray(JArray update, JArray other, IJsonMergeContext context)
        {
            return !JToken.DeepEquals(update, other)
                ? context.Merge(update, other) 
                : context.Noop(update, other);
        }

        private IEnumerable<string> UnionKeys(IDictionary<string, JToken> update, IDictionary<string, JToken> other)
        {
            HashSet<string> keys = new HashSet<string>(update.Keys);
            keys.UnionWith(other.Keys);
            return keys;
        }

    }
}
