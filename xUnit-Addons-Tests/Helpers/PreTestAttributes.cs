using xUnitAddons;

namespace xUnitAddonsTests.Helpers
{
    public class MyBeforeAfterAsyncTestAttribute : BeforeAfterAsyncTestAttribute
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
}
