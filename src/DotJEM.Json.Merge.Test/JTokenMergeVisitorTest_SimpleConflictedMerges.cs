using System.Collections;
using DotJEM.NUnit.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using static DotJEM.Json.Merge.Test.TestUtil;

namespace DotJEM.Json.Merge.Test
{
    [TestFixture]
    // ReSharper disable once InconsistentNaming
    public class JTokenMergeVisitorTest_SimpleConflictedMerges 
    {
        [TestCaseSource(nameof(ConflictedMerges))]
        public void Merge_WithConflicts_DisallowsUpdate(JToken update, JToken conflict, JToken parent, JToken expected)
        {
            IJsonMergeVisitor service = new JsonMergeVisitor();

            IMergeResult result = service.Merge(update, conflict, parent);

            Assert.That(result, ObjectHas.Property<MergeResult>(x => x.HasConflicts).True
                                & ObjectHas.Property<MergeResult>(x => x.Conflicts).EqualTo(expected));
        }

        public IEnumerable ConflictedMerges
        {
            get
            {
                yield return CASE(
                    "{ prop: 'hey' }",
                    "{ prop: 'ho' }",
                    "{ prop: 'what' }",
                    "{ prop: { origin: 'what', update: 'hey', other: 'ho' } }"
                );

                yield return CASE(
                    "{ prop: { a: 42, b: 'hey' } }",
                    "{ prop: { a: 42, b: 'ho' } }",
                    "{ prop: { a: 42, b: 'what' } }",
                    "{ 'prop.b': { origin: 'what', update: 'hey', other: 'ho' } }"
                );
            }
        }
    }
}