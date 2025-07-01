using System;

namespace Utilities
{
    public static class StringUtilities
    {
        public static int GetInt(this string[] args, int argsPos = 1, int fallbackValue = 0)
        {
            if (args.Length <= argsPos || !int.TryParse(args[argsPos], out int amount)) return fallbackValue;
            return amount;
        }

        public static T GetEnum<T>(this string[] args, int argsPos = 1, T fallbackValue = default) where T : struct
        {
            if (args.Length <= argsPos || !Enum.TryParse<T>(args[argsPos], true, out var e)) return fallbackValue;
            return e;
        }
    }
}