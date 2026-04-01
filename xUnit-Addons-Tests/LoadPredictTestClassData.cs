using System.Collections;

namespace xUnitAddonsTests
{
    public class LoadPredictTestClassData : IEnumerable<object[]>
    {
        private readonly List<object[]> _data = new List<object[]>
        {
            new object[] { "What are the requisites for carbon credits?", Scheme.ACCU },
            new object[] { "How do I calculate net emissions?", Scheme.SafeguardMechanism },
            new object[] { "What is the colour of a rose?", Scheme.None }
        };

        public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
