using System;
using System.Reflection;
using Xunit.Sdk;

namespace xUnitAddons
{
    public interface IRunAsync
    {
    }

    public interface IRunBeforeAsync : IRunAsync
    {
        Action? RunBefore { get; }
    }

    public interface IRunAfterAsync : IRunAsync
    {
        Action? RunAfter { get; }
    }

    public interface IRunBeforeAsyncWithReturn : IRunBeforeAsync
    {
        object? ReturnValue { get; set; }
    }

    public abstract class BeforeAfterAsyncTestAttribute : BeforeAfterTestAttribute
    {
        private static string? _lastTestStamp = null;
        private static Type? _lastType = null;

        public BeforeAfterAsyncTestAttribute(Type specificAttributeType, string stamp) : base()
        {
            var specificAttributeBefore = Helpers.CreateInstance<IRunBeforeAsync>(specificAttributeType);

            if (specificAttributeBefore != null && specificAttributeBefore.RunBefore != null && (string.IsNullOrEmpty(_lastTestStamp)
                        || ((specificAttributeType != _lastType) && (Guid.Parse(stamp) != Guid.Parse(_lastTestStamp)))))
            {
                var specificAttributeAfter = Helpers.CreateInstance<IRunAfterAsync>(specificAttributeType);

                if (specificAttributeAfter != null && specificAttributeAfter.RunAfter != null)
                {
                    specificAttributeAfter.RunAfter();
                }

                specificAttributeBefore.RunBefore();

                _lastTestStamp = stamp;
                _lastType = specificAttributeType;
            }
        }

        public BeforeAfterAsyncTestAttribute(Type specificAttributeType, Type returnFunctionClassType, string returnFunctionName, string stamp) : base()
        {
            var specificAttributeBefore = Helpers.CreateInstance<IRunBeforeAsyncWithReturn>(specificAttributeType);

            if (specificAttributeBefore != null && specificAttributeBefore.RunBefore != null && (string.IsNullOrEmpty(_lastTestStamp)
                        || ((specificAttributeType != _lastType) && (Guid.Parse(stamp) != Guid.Parse(_lastTestStamp)))))
            {

                var specificAttributeAfter = Helpers.CreateInstance<IRunAfterAsync>(specificAttributeType);

                if (specificAttributeAfter != null && specificAttributeAfter.RunAfter != null)
                {
                    specificAttributeAfter.RunAfter();
                }

                specificAttributeBefore.RunBefore();

                _lastTestStamp = stamp;
                _lastType = specificAttributeType;

                MethodInfo? returnStaticMethod = returnFunctionClassType.GetMethod(
                    returnFunctionName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static
                );

                if (returnStaticMethod != null)
                {
                    returnStaticMethod.Invoke(null, new object[] { specificAttributeBefore.ReturnValue! });
                }
            }
        }
    }
}
