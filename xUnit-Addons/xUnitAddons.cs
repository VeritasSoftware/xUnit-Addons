using System;
using Xunit;

namespace xUnitAddons
{
    public interface IRunBeforeTest
    {
        Action Run { get; }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class MyTheoryAttribute : TheoryAttribute
    {
        private static string? _lastTestStamp = null;
        private static Type? _lastType = null;

        public MyTheoryAttribute(Type specificAttributeType, string stamp)
        {
            if (!typeof(IRunBeforeTest).IsAssignableFrom(specificAttributeType))
                throw new ArgumentException($"{specificAttributeType.Name} must implement ITestCondition");

            var specificAttribute = (IRunBeforeTest)Activator.CreateInstance(specificAttributeType)!;

            if (specificAttribute != null && specificAttribute.Run != null && (string.IsNullOrEmpty(_lastTestStamp)
                        || ((specificAttributeType != _lastType) && (Guid.Parse(stamp) != Guid.Parse(_lastTestStamp)))))
            {
                specificAttribute.Run();
                _lastTestStamp = stamp;
                _lastType = specificAttributeType;
            }
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class MyFactAttribute : FactAttribute
    {
        private static string? _lastTestStamp = null;
        private static Type? _lastType = null;

        public MyFactAttribute(Type specificAttributeType, string stamp)
        {
            if (!typeof(IRunBeforeTest).IsAssignableFrom(specificAttributeType))
                throw new ArgumentException($"{specificAttributeType.Name} must implement ITestCondition");

            var specificAttribute = (IRunBeforeTest)Activator.CreateInstance(specificAttributeType)!;

            if (specificAttribute != null && specificAttribute.Run != null && (string.IsNullOrEmpty(_lastTestStamp)
                        || ((specificAttributeType != _lastType) && (Guid.Parse(stamp) != Guid.Parse(_lastTestStamp)))))
            {
                specificAttribute.Run();
                _lastTestStamp = stamp;
                _lastType = specificAttributeType;
            }
        }
    }
}
