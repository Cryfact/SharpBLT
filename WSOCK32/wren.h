#pragma once
#include <wren.hpp>


struct wren_functions
{
	void (*wrenInitConfiguration)(WrenConfiguration* config);

	WrenVM* (*wrenNewVM)(WrenConfiguration* config);

	WrenInterpretResult(*wrenInterpret)(WrenVM* vm, const char* module,
		const char* source);
};


void init_wren_functions(wren_functions* pFuncs);
