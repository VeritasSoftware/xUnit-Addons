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

        private static Type? _specificAttributeType;
        private static int _noOfTests = 1;
        private static int _currentTestCount = 0;

        public BeforeAfterAsyncTestAttribute(Type specificAttributeType, string stamp, int noOfTests = 1) : base()
        {            
            _specificAttributeType = specificAttributeType;
            _noOfTests = noOfTests;

            var specificAttributeBefore = Helpers.CreateInstance<IRunBeforeAsync>(specificAttributeType);

            if (specificAttributeBefore != null && specificAttributeBefore.RunBefore != null && (string.IsNullOrEmpty(_lastTestStamp)
                        || ((specificAttributeType != _lastType) && (Guid.Parse(stamp) != Guid.Parse(_lastTestStamp)))))
            {
                specificAttributeBefore.RunBefore();

                _lastTestStamp = stamp;
                _lastType = specificAttributeType;
            }
        }

        public BeforeAfterAsyncTestAttribute(Type specificAttributeType, Type returnFunctionClassType, string returnFunctionName, string stamp, int noOfTests = 1) : base()
        {
            _specificAttributeType = specificAttributeType;
            _noOfTests = noOfTests;

            var specificAttributeBefore = Helpers.CreateInstance<IRunBeforeAsyncWithReturn>(specificAttributeType);

            if (specificAttributeBefore != null && specificAttributeBefore.RunBefore != null && (string.IsNullOrEmpty(_lastTestStamp)
                        || ((specificAttributeType != _lastType) && (Guid.Parse(stamp) != Guid.Parse(_lastTestStamp)))))
            {
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

        public override void After(MethodInfo methodUnderTest)
        {
            _currentTestCount++;

            if (_specificAttributeType == null || _currentTestCount < _noOfTests)
            {
                base.After(methodUnderTest);
                return;
            }

            var specificAttributeAfter = Helpers.CreateInstance<IRunAfterAsync>(_specificAttributeType);

            if (specificAttributeAfter != null && specificAttributeAfter.RunAfter != null)
            {
                specificAttributeAfter.RunAfter();
            }

            base.After(methodUnderTest);
        }
    }

    public sealed class MyBeforeAfterAsyncTestAttribute : BeforeAfterAsyncTestAttribute
    {
        public MyBeforeAfterAsyncTestAttribute(Type specificAttributeType, string stamp, int noOfTests = 1) : base(specificAttributeType, stamp, noOfTests)
        {
        }

        public MyBeforeAfterAsyncTestAttribute(Type specificAttribute, Type returnFunctionClassType,
                                                    string returnFunctionName, string stamp, int noOfTests = 1)
                                                    : base(specificAttribute, returnFunctionClassType, returnFunctionName, stamp, noOfTests)
        {
        }
    }

    public abstract class BaseCollectionFixture : IDisposable
    {
        public BaseCollectionFixture()
        {
            if (RunBefore !=  null)
                RunBefore();
        }

        public virtual Action? RunBefore { get; set; }

        public virtual Action? RunAfter { get; set; }

        public void Dispose()
        {
            if (RunAfter  != null)
                RunAfter();
        }
    }
}
