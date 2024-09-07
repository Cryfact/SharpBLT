#include "wren.h"

void init_wren_functions(wren_functions* pFuncs)
{
	pFuncs->wrenInitConfiguration = wrenInitConfiguration;
	pFuncs->wrenInterpret = wrenInterpret;
	pFuncs->wrenNewVM = wrenNewVM;
	pFuncs->wrenGetVersionNumber = wrenGetVersionNumber;
	pFuncs->wrenFreeVM = wrenFreeVM;
	pFuncs->wrenCollectGarbage = wrenCollectGarbage;
	pFuncs->wrenMakeCallHandle = wrenMakeCallHandle;
	pFuncs->wrenCall = wrenCall;
	pFuncs->wrenReleaseHandle = wrenReleaseHandle;
	pFuncs->wrenGetSlotCount = wrenGetSlotCount;
	pFuncs->wrenEnsureSlots = wrenEnsureSlots;
	pFuncs->wrenGetSlotType = wrenGetSlotType;
	pFuncs->wrenGetSlotBool = wrenGetSlotBool;
	pFuncs->wrenGetSlotBytes = wrenGetSlotBytes;
	pFuncs->wrenGetSlotDouble = wrenGetSlotDouble;
	pFuncs->wrenGetSlotForeign = wrenGetSlotForeign;
	pFuncs->wrenGetSlotString = wrenGetSlotString;
	pFuncs->wrenGetSlotHandle = wrenGetSlotHandle;
	pFuncs->wrenSetSlotBool = wrenSetSlotBool;
	pFuncs->wrenSetSlotBytes = wrenSetSlotBytes;
	pFuncs->wrenSetSlotDouble = wrenSetSlotDouble;
	pFuncs->wrenSetSlotNewForeign = wrenSetSlotNewForeign;
	pFuncs->wrenSetSlotNewList = wrenSetSlotNewList;
	pFuncs->wrenSetSlotNewMap = wrenSetSlotNewMap;
	pFuncs->wrenSetSlotNull = wrenSetSlotNull;
	pFuncs->wrenSetSlotString = wrenSetSlotString;
	pFuncs->wrenSetSlotHandle = wrenSetSlotHandle;
	pFuncs->wrenGetListCount = wrenGetListCount;
	pFuncs->wrenGetListElement = wrenGetListElement;
	pFuncs->wrenSetListElement = wrenSetListElement;
	pFuncs->wrenInsertInList = wrenInsertInList;
	pFuncs->wrenGetMapCount = wrenGetMapCount;
	pFuncs->wrenGetMapContainsKey = wrenGetMapContainsKey;
	pFuncs->wrenGetMapValue = wrenGetMapValue;
	pFuncs->wrenSetMapValue = wrenSetMapValue;
	pFuncs->wrenRemoveMapValue = wrenRemoveMapValue;
	pFuncs->wrenGetVariable = wrenGetVariable;
	pFuncs->wrenHasVariable = wrenHasVariable;
	pFuncs->wrenHasModule = wrenHasModule;
	pFuncs->wrenAbortFiber = wrenAbortFiber;
	pFuncs->wrenGetUserData = wrenGetUserData;
	pFuncs->wrenSetUserData = wrenSetUserData;
}
