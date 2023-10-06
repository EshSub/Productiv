#include "stdafx.h"
#include <windows.h>
#include <memory.h>

//
// Capture the application instance of this module to pass to
// hook initialization.
//
extern HINSTANCE g_appInstance;

BOOL APIENTRY DllMain(HINSTANCE hinstDLL, DWORD ul_reason_for_call, LPVOID lpReserved)
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
		//
		// Capture the application instance of this module to pass to
		// hook initialization.
		//
		if (g_appInstance == NULL)
		{
			g_appInstance = hinstDLL;
		}
		break;

	case DLL_THREAD_ATTACH:
		break;

	case DLL_THREAD_DETACH:
		break;

	case DLL_PROCESS_DETACH:
		break;

	default:
		OutputDebugString("That's weird.\n");
		break;
	}

	return TRUE;
}
