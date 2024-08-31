using System.Reflection;
using System.Runtime.InteropServices;

namespace SharpBLT
{
    public class Game
    {
        [UnmanagedFunctionPointer(Lua.DefaultCallingConvention)]
        private delegate IntPtr application_update_fn(IntPtr _this, long unk0);

#pragma warning disable CS0649
#pragma warning disable CS8618
        [FunctionPattern("48 89 5C 24 10 48 89 6C 24 18 48 89 74 24 20 57 41 56 41 57 48 83 EC 50 0F 29 74 24 40 0F 29 7C 24 30 48 8B F9")]
        [FunctionTarget(nameof(application_update_new), typeof(application_update_fn))]
        private static Hook<application_update_fn> ms_game_update_hook;

#pragma warning restore CS0649

        private static uint ms_main_thread_id;
        private static application_update_fn old_application_update;
        private static int ms_updates;
#pragma warning restore CS8618

        public static void Initialize()
        {
            ms_main_thread_id = Kernel32.GetCurrentThreadId();

            var fields = typeof(Game).GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            var methods = typeof(Game).GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var field in fields)
            {
                var patternAttr = field.GetCustomAttribute<FunctionPattern>();

                if (patternAttr == null)
                    continue;

                var functionTargetAttr = field.GetCustomAttribute<FunctionTarget>();

                var pattern = new SearchPattern(patternAttr.Pattern);

                var addr = pattern.Match(SearchRange.GetStartSearchAddress(), SearchRange.GetSearchSize());

                if (addr == IntPtr.Zero)
                    throw new Exception($"Failed to resolve Method '{field.Name}'");

                Logger.Instance().Log(LogType.Log, $"Address for '{field.Name}' found: 0x{addr.ToInt64():X8}");

                if (functionTargetAttr == null)
                {
                    field.SetValue(null, Marshal.GetDelegateForFunctionPointer(addr, field.FieldType));
                }
                else
                {
                    var method = methods.Where((x) => x.Name == functionTargetAttr.FunctionName).FirstOrDefault();

                    if (method == null)
                        throw new Exception($"No Method with Name '{functionTargetAttr.FunctionName}' implemented");

                    field.SetValue(null, Activator.CreateInstance(
                        field.FieldType, addr, method.CreateDelegate(functionTargetAttr.DelegateType)));
                }
            }

            ms_game_update_hook.Apply();
            old_application_update = ms_game_update_hook.OldFunction;
        }


        private static IntPtr application_update_new(IntPtr _this, long unk0)
        {
            if (Kernel32.GetCurrentThreadId() != ms_main_thread_id)
                return old_application_update(_this, unk0);

            if (ms_updates == 0)
            {
                // TODO: Implement This
                // if (!pd2hook::HTTPManager::GetSingleton()->AreLocksInit())
                //  pd2hook::HTTPManager::GetSingleton()->init_locks();

                ++ms_updates;
            }

            if (ms_updates > 1)
            {
                // TODO: Implement This
                // pd2hook::EventQueueMaster::GetSingleton().ProcessEvents();
            }


            ++ms_updates;
            return old_application_update(_this, unk0);
        }
    }
}
