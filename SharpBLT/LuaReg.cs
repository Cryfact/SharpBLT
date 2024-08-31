
namespace SharpBLT
{
    public struct LuaReg
    {
        public string Name;
        public Lua.LuaCallback Function;

        public LuaReg(string name, Lua.LuaCallback func)
        {
            Name = name;
            Function = func;
        }
    }
}
