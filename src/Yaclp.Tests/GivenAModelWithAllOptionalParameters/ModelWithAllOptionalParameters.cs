using Yaclp.Attributes;

namespace Yaclp.Tests.GivenAModelWithAllOptionalParameters
{
    public class ModelWithAllOptionalParameters
    {
        [ParameterIsOptional]
        public string Foo { get; set; }

        [ParameterIsOptional]
        public string Bar { get; set; }

        [ParameterIsOptional]
        public string Baz { get; set; }
    }
}