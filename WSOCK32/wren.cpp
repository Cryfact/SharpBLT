#include "wren.h"

void init_wren_functions(wren_functions* pFuncs)
{
	pFuncs->wrenInitConfiguration = wrenInitConfiguration;
	pFuncs->wrenInterpret = wrenInterpret;
	pFuncs->wrenNewVM = wrenNewVM;
}
