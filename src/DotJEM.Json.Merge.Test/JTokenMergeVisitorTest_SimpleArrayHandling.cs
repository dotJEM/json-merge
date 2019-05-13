﻿using System.Collections;
using DotJEM.NUnit.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using static DotJEM.Json.Merge.Test.TestUtil;

namespace DotJEM.Json.Merge.Test
{
    [TestFixture]
    // ReSharper disable once InconsistentNaming
    public class JTokenMergeVisitorTest_SimpleArrayHandling
    {
        [Test]
        public void Merge_ScrambledIntArray_IsConflicted()
        {
            JToken update = JSON("{ arr: [1,2,3] }");
            JToken conflict = JSON("{ arr: [2,3,1] }");
            JToken origin = JSON("{ arr: [3,1,2] }");
            JToken diff = JSON("{ arr: { origin: [3,1,2], other: [2,3,1], update: [1,2,3] } }");

            IJsonMergeVisitor visitor = new JsonMergeVisitor();

            IMergeResult result = visitor.Merge(update, conflict, origin);

            Assert.That(result, ObjectHas.Property<MergeResult>(x => x.HasConflicts).True
                                & ObjectHas.Property<MergeResult>(x => x.Conflicts).EqualTo(diff));
        }
        [Test]
        public void Merge_TypeMismatchArray_IsConflicted()
        {
            JToken update = JSON("{ arr: [1,2,3] }");
            JToken conflict = JSON("{ arr: [2,3,1] }");
            JToken origin = JSON("{ arr: 'foo' }");
            JToken diff = JSON("{ arr: { origin: 'foo', other: [2,3,1], update: [1,2,3] } }");

            IJsonMergeVisitor visitor = new JsonMergeVisitor();

            IMergeResult result = visitor.Merge(update, conflict, origin);

            Assert.That(result, ObjectHas.Property<MergeResult>(x => x.HasConflicts).True
                                & ObjectHas.Property<MergeResult>(x => x.Conflicts).Matches(JsonIs.EqualTo(diff)));
        }

        [TestCaseSource(nameof(NonConflictedMerges))]
        public void Merge_WithoutConflicts_Merges(JToken update, JToken conflict, JToken parent, JToken expected)
        {
            IJsonMergeVisitor service = new JsonMergeVisitor();

            IMergeResult result = service.Merge(update, conflict, parent);

            Assert.That(result, ObjectHas.Property<MergeResult>(x => x.HasConflicts).EqualTo(false)
                                & ObjectHas.Property<MergeResult>(x => x.Merged).Matches(JsonIs.EqualTo(expected)));
        }

        [TestCaseSource(nameof(ConflictedMerges))]
        public void Merge_WithConflicts_FailsMerge(JToken update, JToken conflict, JToken parent, JToken expected)
        {
            IJsonMergeVisitor service = new JsonMergeVisitor();

            IMergeResult result = service.Merge(update, conflict, parent);

            Assert.That(result, ObjectHas.Property<MergeResult>(x => x.HasConflicts).EqualTo(true));
            Assert.That(result.Conflicts, JsonIs.EqualTo(expected));
        }

        public IEnumerable ConflictedMerges
        {
            get
            {
                yield return CASE(
                    "{ arr: [1] }",
                    "{ arr: [2] }",
                    "{ arr: [3] }",
                    "{ arr: { update: [1], other: [2], origin: [3] } }"
                    );

                yield return CASE(
                    "{ arr: [1,2] }",
                    "{ arr: [2,1] }",
                    "{ arr: [3] }",
                    "{ arr: { update: [1,2], other: [2,1], origin: [3] } }"
                    );

                yield return CASE(
                    "{ arr: [1] }",
                    "{ arr: 42 }",
                    "{ arr: [3] }",
                    "{ arr: { update: [1], other: 42, origin: [3] } }"
                    );

                yield return CASE(
                    "{ arr: [1] }",
                    "{ arr: [2] }",
                    "{ }",
                    "{ arr: { update: [1], other: [2], origin: null } }"
                    );

                yield return CASE(
                    "{ arr: [1] }",
                    "{ arr: [1,2] }",
                    "{ arr: [2] }",
                    "{ arr: { update: [1], other: [1,2], origin: [2] } }"
                    );
            }
        }

        public IEnumerable NonConflictedMerges
        {
            get
            {
                //Note: All equals
                yield return CASE(
                    "{ arr: [1,2,3] }",
                    "{ arr: [1,2,3] }",
                    "{ arr: [1,2,3] }",
                    "{ arr: [1,2,3] }"
                    );

                //Note: Only update changed
                yield return CASE(
                    "{ arr: [2,3] }",
                    "{ arr: [1,2,3] }",
                    "{ arr: [1,2,3] }",
                    "{ arr: [2,3] }"
                    );

                //Note: Only conflict changed
                yield return CASE(
                    "{ arr: [1,2,3] }",
                    "{ arr: [2,3] }",
                    "{ arr: [1,2,3] }",
                    "{ arr: [2,3] }"
                    );

                //Note: Both changed, added item
                yield return CASE(
                    "{ arr: [1,2,3] }",
                    "{ arr: [1,2,3] }",
                    "{ arr: [2,3] }",
                    "{ arr: [1,2,3] }"
                    );

                //Note: Both changed, removed item
                yield return CASE(
                    "{ arr: [2,3] }",
                    "{ arr: [2,3] }",
                    "{ arr: [1,2,3] }",
                    "{ arr: [2,3] }"
                    );
            }
        }
    }
}