namespace SharpBLT;

using System.Diagnostics.CodeAnalysis;

public class Singleton<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] T>
{
    private static T? ms_instance;

    public Singleton()
    { }

    public static T Instance()
    {
        ms_instance ??= Activator.CreateInstance<T>();

        return ms_instance;
    }
}
