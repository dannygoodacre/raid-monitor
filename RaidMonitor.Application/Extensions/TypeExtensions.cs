namespace RaidMonitor.Application.Extensions;

internal static class TypeExtensions
{
    public static bool InheritsFromCommandHandler(this Type type)
    {
        var baseType = type.BaseType;

        while (baseType is not null)
        {
            if (baseType.IsGenericType)
            {
                var def = baseType.GetGenericTypeDefinition();

                if (def == typeof(CommandHandler<>) || def == typeof(CommandHandler<,>))
                {
                    return true;
                }
            }

            baseType = baseType.BaseType;
        }

        return false;
    }

    public static bool InheritsFromQueryHandler(this Type type)
    {
        var baseType = type.BaseType;

        while (baseType is not null)
        {
            if (baseType.IsGenericType)
            {
                var def = baseType.GetGenericTypeDefinition();

                if (def == typeof(QueryHandler<,>))
                {
                    return true;
                }
            }

            baseType = baseType.BaseType;
        }

        return false;
    }
}
