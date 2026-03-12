using System;
using Xunit.Sdk;

namespace xUnitAddons
{
    public interface IRunBeforeTest
    {
        Action Run { get; }
    }

    public abstract class BeforeAsyncAfterSyncTestAttribute : BeforeAfterTestAttribute
    {
        private static string? _lastTestStamp = null;
        private static Type? _lastType = null;

        public BeforeAsyncAfterSyncTestAttribute(Type specificAttributeType, string stamp) : base()
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
