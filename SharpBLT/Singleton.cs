
using System.Diagnostics.CodeAnalysis;

namespace SharpBLT
{
    public class Singleton<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] T>
    {
        public static T? ms_instance;

        public Singleton()
        { }

        public static T Instance()
        {
            ms_instance ??= Activator.CreateInstance<T>();

            return ms_instance;
        }
    }
}
