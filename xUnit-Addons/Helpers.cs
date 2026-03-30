using System;

namespace xUnitAddons
{
    internal static class Helpers
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
