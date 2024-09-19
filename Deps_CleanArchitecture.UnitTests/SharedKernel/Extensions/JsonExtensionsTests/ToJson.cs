using Deps_CleanArchitecture.SharedKernel.Extensions;
using Xunit;

namespace Deps_CleanArchitecture.UnitTests.SharedKernel.Extensions.JsonExtensionsTests
{
    public class ToJson
    {
        internal class TestObj
        {
            public int Id { get; set; }
            public string NameProperty { get; set; }
        }

        [Theory]
        [InlineData(1, "AAAA", "{\"id\":1,\"nameProperty\":\"AAAA\"}")]
        public void CanTransformObjectToJson(int id, string name, string expected)
        {
            var obj = new TestObj
            {
                Id = id,
                NameProperty = name
            };

            var result = obj.ToJson();

            Assert.Equal(expected, result);
        }
    }
}
