using xUnitAddons;

namespace xUnitAddonsTests.Helpers
{
    public class MyBeforeAfterAsyncTestAttribute : BeforeAfterAsyncTestAttribute
    {
        public MyBeforeAfterAsyncTestAttribute(Type specificAttributeType, string stamp) : base(specificAttributeType, stamp)
        {
        }

        public MyBeforeAfterAsyncTestAttribute(Type specificAttribute, Type returnFunctionClassType,
                                                    string returnFunctionName, string stamp)
                                                    : base(specificAttribute, returnFunctionClassType, returnFunctionName, stamp)
        {
        }
    }
}
