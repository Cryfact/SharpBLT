namespace SharpBLT;

public struct LuaReg(string name, Lua.LuaCallback func)
{
    public string Name = name;
    public Lua.LuaCallback Function = func;
}
