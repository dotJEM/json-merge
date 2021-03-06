﻿using System.Collections;
using DotJEM.NUnit.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace DotJEM.Json.Merge.Test
{
    [TestFixture]
    // ReSharper disable once InconsistentNaming
    public class JTokenMergeVisitorTest_SimpleNonConflictedMerges 
    {
        [TestCaseSource(nameof(SimpleNonConflictedMerge))]
        public void Merge_WithoutConflicts_AllowsUpdate(JToken update, JToken conflict, JToken parent, JToken expected)
        {
            IJsonMergeVisitor service = new JsonMergeVisitor();

            IMergeResult result = service.Merge(update, conflict, parent);


            Assert.That(result,
                ObjectHas.Property<MergeResult>(x => x.HasConflicts).False
                & ObjectHas.Property<MergeResult>(x => x.Merged).EqualTo(expected));
        }
        public IEnumerable SimpleNonConflictedMerge
        {
            get
            {
                yield return TestUtil.CASE(
                    "{ prop: 'what' }",
                    "{ prop: 'what' }",
                    "{ prop: 'what' }",
                    "{ prop: 'what' }"
                );

                yield return TestUtil.CASE(
                    "{ prop: 'what' }",
                    "{ prop: 'x' }",
                    "{ prop: 'what' }",
                    "{ prop: 'x' }"
                );

                yield return TestUtil.CASE(
                    "{ prop: { a: 42 } }",
                    "{ prop: { a: 42, b: 'foo' } }",
                    "{ prop: { a: 42 } }",
                    "{ prop: { a: 42, b: 'foo' } }"
                );

                yield return TestUtil.CASE(
                    "{ prop: { a: 42 } }",
                    "{ prop: { a: 42, b: 'foo' } }",
                    "{ prop: { a: 42, b: 'foo' } }",
                    "{ prop: { a: 42 } }"
                );

                yield return TestUtil.CASE(
                    "{ prop: { a: 42, b: 'foo' } }",
                    "{ prop: { a: 42 } }",
                    "{ prop: { a: 42, b: 'foo' } }",
                    "{ prop: { a: 42 } }"
                );
            }
        }
    }
}