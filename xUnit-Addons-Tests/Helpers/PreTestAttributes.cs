using System.Reflection;
using xUnitAddons;

namespace xUnitAddonsTests.Helpers
{
    public class LoadModelBeforeTestAttribute : BeforeAfterAsyncTestAttribute
    {
        public LoadModelBeforeTestAttribute(Type specificAttributeType, string stamp) : base(specificAttributeType, stamp)
        {
        }

        public override void After(MethodInfo methodUnderTest)
        {
            // Clean up resources after the test, if necessary
        }
    }

    public class BuildLoadPredictDIContainerAttribute : BeforeAfterAsyncTestAttribute
    {
        public BuildLoadPredictDIContainerAttribute(Type specificAttribute, Type returnFunctionClassType,
                                                    string returnFunctionName, string stamp)
                                                    : base(specificAttribute, returnFunctionClassType, returnFunctionName, stamp)
        {
        }

        public override void After(MethodInfo methodUnderTest)
        {
            // Clean up resources after the test, if necessary
        }
    }

    public class BuildCreateModelDIContainerAttribute : BeforeAfterAsyncTestAttribute
    {
        public BuildCreateModelDIContainerAttribute(Type specificAttribute, Type returnFunctionClassType,
                                                    string returnFunctionName, string stamp)
                                                    : base(specificAttribute, returnFunctionClassType, returnFunctionName, stamp)
        {
        }

        public override void After(MethodInfo methodUnderTest)
        {
            // Clean up resources after the test, if necessary
        }
    }
}
