#pragma once
#include <wren.hpp>


struct wren_functions
{
	void (*wrenInitConfiguration)(WrenConfiguration* config);

	WrenVM* (*wrenNewVM)(WrenConfiguration* config);

	WrenInterpretResult(*wrenInterpret)(WrenVM* vm, const char* module,
		const char* source);

	int (*wrenGetVersionNumber)();

	void (*wrenFreeVM)(WrenVM* vm);

	void (*wrenCollectGarbage)(WrenVM* vm);

	WrenHandle* (*wrenMakeCallHandle)(WrenVM* vm, const char* signature);

	WrenInterpretResult(*wrenCall)(WrenVM* vm, WrenHandle* method);

	void (*wrenReleaseHandle)(WrenVM* vm, WrenHandle* handle);

	int (*wrenGetSlotCount)(WrenVM* vm);

	void (*wrenEnsureSlots)(WrenVM* vm, int numSlots);

	WrenType(*wrenGetSlotType)(WrenVM* vm, int slot);

	bool (*wrenGetSlotBool)(WrenVM* vm, int slot);

	const char* (*wrenGetSlotBytes)(WrenVM* vm, int slot, int* length);

	double (*wrenGetSlotDouble)(WrenVM* vm, int slot);

	void* (*wrenGetSlotForeign)(WrenVM* vm, int slot);

	const char* (*wrenGetSlotString)(WrenVM* vm, int slot);
	
	WrenHandle* (*wrenGetSlotHandle)(WrenVM* vm, int slot);

	void (*wrenSetSlotBool)(WrenVM* vm, int slot, bool value);

	void (*wrenSetSlotBytes)(WrenVM* vm, int slot, const char* bytes, size_t length);

	void (*wrenSetSlotDouble)(WrenVM* vm, int slot, double value);

	void* (*wrenSetSlotNewForeign)(WrenVM* vm, int slot, int classSlot, size_t size);

	void (*wrenSetSlotNewList)(WrenVM* vm, int slot);

	void (*wrenSetSlotNewMap)(WrenVM* vm, int slot);

	void (*wrenSetSlotNull)(WrenVM* vm, int slot);

	void (*wrenSetSlotString)(WrenVM* vm, int slot, const char* text);

	void (*wrenSetSlotHandle)(WrenVM* vm, int slot, WrenHandle* handle);

	int (*wrenGetListCount)(WrenVM* vm, int slot);

	void (*wrenGetListElement)(WrenVM* vm, int listSlot, int index, int elementSlot);

	void (*wrenSetListElement)(WrenVM* vm, int listSlot, int index, int elementSlot);

	void (*wrenInsertInList)(WrenVM* vm, int listSlot, int index, int elementSlot);

	int (*wrenGetMapCount)(WrenVM* vm, int slot);

	bool (*wrenGetMapContainsKey)(WrenVM* vm, int mapSlot, int keySlot);

	void (*wrenGetMapValue)(WrenVM* vm, int mapSlot, int keySlot, int valueSlot);

	void (*wrenSetMapValue)(WrenVM* vm, int mapSlot, int keySlot, int valueSlot);

	void (*wrenRemoveMapValue)(WrenVM* vm, int mapSlot, int keySlot,
		int removedValueSlot);

	void (*wrenGetVariable)(WrenVM* vm, const char* module, const char* name,
		int slot);

	bool (*wrenHasVariable)(WrenVM* vm, const char* module, const char* name);

	bool (*wrenHasModule)(WrenVM* vm, const char* module);

	void (*wrenAbortFiber)(WrenVM* vm, int slot);

	void* (*wrenGetUserData)(WrenVM* vm);

	void (*wrenSetUserData)(WrenVM* vm, void* userData);
};


void init_wren_functions(wren_functions* pFuncs);
