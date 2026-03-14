using System;

namespace xUnitAddons
{
    public static class Helpers
    {
        public static T? CreateInstance<T>(Type type)
            where T : IRunAsync
        {
            try
            {
                return (T)Activator.CreateInstance(type)!;
            }
            catch (Exception)
            {
                return default;
            }
        }
    }
}
