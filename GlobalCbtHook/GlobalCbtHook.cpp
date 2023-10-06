// GlobalCbtHook.cpp
//   by Chris Wilson

#include "stdafx.h"
#include <windows.h>
#include "GlobalCbtHook.h"

HHOOK hookCbt = NULL;
HHOOK hookShell = NULL;
HHOOK hookKeyboard = NULL;
HHOOK hookMouse = NULL;
HHOOK hookKeyboardLL = NULL;
HHOOK hookMouseLL = NULL;
HHOOK hookCallWndProc = NULL;
HHOOK hookGetMsg = NULL;

//
// Store the application instance of this module to pass to
// hook initialization. This is set in DLLMain().
//
HINSTANCE g_appInstance = NULL;

typedef void (CALLBACK *HookProc)(int code, WPARAM w, LPARAM l);

static LRESULT CALLBACK CbtHookCallback(int code, WPARAM wparam, LPARAM lparam);
static LRESULT CALLBACK ShellHookCallback(int code, WPARAM wparam, LPARAM lparam);
static LRESULT CALLBACK KeyboardHookCallback(int code, WPARAM wparam, LPARAM lparam);
static LRESULT CALLBACK MouseHookCallback(int code, WPARAM wparam, LPARAM lparam);
static LRESULT CALLBACK KeyboardLLHookCallback(int code, WPARAM wparam, LPARAM lparam);
static LRESULT CALLBACK MouseLLHookCallback(int code, WPARAM wparam, LPARAM lparam);
static LRESULT CALLBACK CallWndProcHookCallback(int code, WPARAM wparam, LPARAM lparam);
static LRESULT CALLBACK GetMsgHookCallback(int code, WPARAM wparam, LPARAM lparam);

bool InitializeCbtHook(int threadID, HWND destination)
{
	if (g_appInstance == NULL)
	{
		return false;
	}

	if (GetProp(GetDesktopWindow(), "WILSON_HOOK_HWND_CBT") != NULL)
	{
		SendNotifyMessage((HWND)GetProp(GetDesktopWindow(), "WILSON_HOOK_HWND_CBT"), RegisterWindowMessage("WILSON_HOOK_CBT_REPLACED"), 0, 0);
	}

	SetProp(GetDesktopWindow(), "WILSON_HOOK_HWND_CBT", destination);

	hookCbt = SetWindowsHookEx(WH_CBT, (HOOKPROC)CbtHookCallback, g_appInstance, threadID);
	return hookCbt != NULL;
}

void UninitializeCbtHook()
{
	if (hookCbt != NULL)
		UnhookWindowsHookEx(hookCbt);
	hookCbt = NULL;
}

static LRESULT CALLBACK CbtHookCallback(int code, WPARAM wparam, LPARAM lparam)
{
	if (code >= 0)
	{
		HWND dstWnd = (HWND)GetProp(GetDesktopWindow(), "WILSON_HOOK_HWND_CBT");
		UINT msg = 0;

		if (code == HCBT_ACTIVATE)
			msg = RegisterWindowMessage("WILSON_HOOK_HCBT_ACTIVATE");
		else if (code == HCBT_CREATEWND)
			msg = RegisterWindowMessage("WILSON_HOOK_HCBT_CREATEWND");
		else if (code == HCBT_DESTROYWND)
			msg = RegisterWindowMessage("WILSON_HOOK_HCBT_DESTROYWND");
		else if (code == HCBT_MINMAX)
		{
			msg = RegisterWindowMessage("WILSON_HOOK_HCBT_MINMAX");
		}
		else if (code == HCBT_MOVESIZE)
			msg = RegisterWindowMessage("WILSON_HOOK_HCBT_MOVESIZE");
		else if (code == HCBT_SETFOCUS)
			msg = RegisterWindowMessage("WILSON_HOOK_HCBT_SETFOCUS");
		else if (code == HCBT_SYSCOMMAND)
			msg = RegisterWindowMessage("WILSON_HOOK_HCBT_SYSCOMMAND");

		

		if (msg != 0)
			SendNotifyMessage(dstWnd, msg, wparam, lparam);
	}

	return CallNextHookEx(hookCbt, code, wparam, lparam);
}

bool InitializeShellHook(int threadID, HWND destination)
{
	if (g_appInstance == NULL)
	{
		return false;
	}

	if (GetProp(GetDesktopWindow(), "WILSON_HOOK_HWND_SHELL") != NULL)
	{
		SendNotifyMessage((HWND)GetProp(GetDesktopWindow(), "WILSON_HOOK_HWND_SHELL"), RegisterWindowMessage("WILSON_HOOK_SHELL_REPLACED"), 0, 0);
	}

	SetProp(GetDesktopWindow(), "WILSON_HOOK_HWND_SHELL", destination);

	hookShell = SetWindowsHookEx(WH_SHELL, (HOOKPROC)ShellHookCallback, g_appInstance, threadID);
	return hookShell != NULL;
}

void UninitializeShellHook()
{
	if (hookShell != NULL)
		UnhookWindowsHookEx(hookShell);
	hookShell = NULL;
}

static LRESULT CALLBACK ShellHookCallback(int code, WPARAM wparam, LPARAM lparam)
{
	if (code >= 0)
	{
		UINT msg = 0;

		if (code == HSHELL_ACTIVATESHELLWINDOW)
			msg = RegisterWindowMessage("WILSON_HOOK_HSHELL_ACTIVATESHELLWINDOW");
		else if (code == HSHELL_GETMINRECT)
			msg = RegisterWindowMessage("WILSON_HOOK_HSHELL_GETMINRECT");
		else if (code == HSHELL_LANGUAGE)
			msg = RegisterWindowMessage("WILSON_HOOK_HSHELL_LANGUAGE");
		else if (code == HSHELL_REDRAW)
			msg = RegisterWindowMessage("WILSON_HOOK_HSHELL_REDRAW");
		else if (code == HSHELL_TASKMAN)
			msg = RegisterWindowMessage("WILSON_HOOK_HSHELL_TASKMAN");
		else if (code == HSHELL_WINDOWACTIVATED)
			msg = RegisterWindowMessage("WILSON_HOOK_HSHELL_WINDOWACTIVATED");
		else if (code == HSHELL_WINDOWCREATED)
			msg = RegisterWindowMessage("WILSON_HOOK_HSHELL_WINDOWCREATED");
		else if (code == HSHELL_WINDOWDESTROYED)
			msg = RegisterWindowMessage("WILSON_HOOK_HSHELL_WINDOWDESTROYED");

		HWND dstWnd = (HWND)GetProp(GetDesktopWindow(), "WILSON_HOOK_HWND_SHELL");

		if (msg != 0)
			SendNotifyMessage(dstWnd, msg, wparam, lparam);
	}

	return CallNextHookEx(hookShell, code, wparam, lparam);
}

bool InitializeKeyboardHook(int threadID, HWND destination)
{
	if (g_appInstance == NULL)
	{
		return false;
	}

	if (GetProp(GetDesktopWindow(), "WILSON_HOOK_HWND_KEYBOARD") != NULL)
	{
		SendNotifyMessage((HWND)GetProp(GetDesktopWindow(), "WILSON_HOOK_HWND_KEYBOARD"), RegisterWindowMessage("WILSON_HOOK_KEYBOARD_REPLACED"), 0, 0);
	}

	SetProp(GetDesktopWindow(), "WILSON_HOOK_HWND_KEYBOARD", destination);

	hookKeyboard = SetWindowsHookEx(WH_KEYBOARD, (HOOKPROC)KeyboardHookCallback, g_appInstance, threadID);
	return hookKeyboard != NULL;
}

void UninitializeKeyboardHook()
{
	if (hookKeyboard != NULL)
		UnhookWindowsHookEx(hookKeyboard);
	hookKeyboard = NULL;
}

static LRESULT CALLBACK KeyboardHookCallback(int code, WPARAM wparam, LPARAM lparam)
{
	if (code >= 0)
	{
		UINT msg = 0;

		msg = RegisterWindowMessage("WILSON_HOOK_KEYBOARD");

		HWND dstWnd = (HWND)GetProp(GetDesktopWindow(), "WILSON_HOOK_HWND_KEYBOARD");

		if (msg != 0)
			SendNotifyMessage(dstWnd, msg, wparam, lparam);
	}

	return CallNextHookEx(hookKeyboard, code, wparam, lparam);
}

bool InitializeMouseHook(int threadID, HWND destination)
{
	if (g_appInstance == NULL)
	{
		return false;
	}

	if (GetProp(GetDesktopWindow(), "WILSON_HOOK_HWND_MOUSE") != NULL)
	{
		SendNotifyMessage((HWND)GetProp(GetDesktopWindow(), "WILSON_HOOK_HWND_MOUSE"), RegisterWindowMessage("WILSON_HOOK_MOUSE_REPLACED"), 0, 0);
	}

	SetProp(GetDesktopWindow(), "WILSON_HOOK_HWND_MOUSE", destination);

	hookMouse = SetWindowsHookEx(WH_MOUSE, (HOOKPROC)MouseHookCallback, g_appInstance, threadID);
	return hookMouse != NULL;
}

void UninitializeMouseHook()
{
	if (hookMouse != NULL)
		UnhookWindowsHookEx(hookMouse);
	hookMouse = NULL;
}

static LRESULT CALLBACK MouseHookCallback(int code, WPARAM wparam, LPARAM lparam)
{
	if (code >= 0)
	{
		UINT msg = 0;

		msg = RegisterWindowMessage("WILSON_HOOK_MOUSE");

		HWND dstWnd = (HWND)GetProp(GetDesktopWindow(), "WILSON_HOOK_HWND_MOUSE");

		if (msg != 0)
			SendNotifyMessage(dstWnd, msg, wparam, lparam);
	}

	return CallNextHookEx(hookMouse, code, wparam, lparam);
}

bool InitializeKeyboardLLHook(int threadID, HWND destination)
{
	if (g_appInstance == NULL)
	{
		return false;
	}

	if (GetProp(GetDesktopWindow(), "WILSON_HOOK_HWND_KEYBOARDLL") != NULL)
	{
		SendNotifyMessage((HWND)GetProp(GetDesktopWindow(), "WILSON_HOOK_HWND_KEYBOARDLL"), RegisterWindowMessage("WILSON_HOOK_KEYBOARDLL_REPLACED"), 0, 0);
	}

	SetProp(GetDesktopWindow(), "WILSON_HOOK_HWND_KEYBOARDLL", destination);

	hookKeyboardLL = SetWindowsHookEx(WH_KEYBOARD_LL, (HOOKPROC)KeyboardLLHookCallback, g_appInstance, threadID);
	return hookKeyboardLL != NULL;
}

void UninitializeKeyboardLLHook()
{
	if (hookKeyboardLL != NULL)
		UnhookWindowsHookEx(hookKeyboardLL);
	hookKeyboardLL = NULL;
}

static LRESULT CALLBACK KeyboardLLHookCallback(int code, WPARAM wparam, LPARAM lparam)
{
	if (code >= 0)
	{
		UINT msg = 0;

		msg = RegisterWindowMessage("WILSON_HOOK_KEYBOARDLL");

		HWND dstWnd = (HWND)GetProp(GetDesktopWindow(), "WILSON_HOOK_HWND_KEYBOARDLL");

		if (msg != 0)
			SendNotifyMessage(dstWnd, msg, wparam, lparam);
	}

	return CallNextHookEx(hookKeyboardLL, code, wparam, lparam);
}

bool InitializeMouseLLHook(int threadID, HWND destination)
{
	if (g_appInstance == NULL)
	{
		return false;
	}

	if (GetProp(GetDesktopWindow(), "WILSON_HOOK_HWND_MOUSELL") != NULL)
	{
		SendNotifyMessage((HWND)GetProp(GetDesktopWindow(), "WILSON_HOOK_HWND_MOUSELL"), RegisterWindowMessage("WILSON_HOOK_MOUSELL_REPLACED"), 0, 0);
	}

	SetProp(GetDesktopWindow(), "WILSON_HOOK_HWND_MOUSELL", destination);

	hookMouseLL = SetWindowsHookEx(WH_MOUSE_LL, (HOOKPROC)MouseLLHookCallback, g_appInstance, threadID);
	return hookMouseLL != NULL;
}

void UninitializeMouseLLHook()
{
	if (hookMouseLL != NULL)
		UnhookWindowsHookEx(hookMouseLL);
	hookMouseLL = NULL;
}

static LRESULT CALLBACK MouseLLHookCallback(int code, WPARAM wparam, LPARAM lparam)
{
	if (code >= 0)
	{
		UINT msg = 0;

		msg = RegisterWindowMessage("WILSON_HOOK_MOUSELL");

		HWND dstWnd = (HWND)GetProp(GetDesktopWindow(), "WILSON_HOOK_HWND_MOUSELL");

		if (msg != 0)
			SendNotifyMessage(dstWnd, msg, wparam, lparam);
	}

	return CallNextHookEx(hookMouseLL, code, wparam, lparam);
}

bool InitializeCallWndProcHook(int threadID, HWND destination)
{
	if (g_appInstance == NULL)
	{
		return false;
	}

	if (GetProp(GetDesktopWindow(), "WILSON_HOOK_HWND_CALLWNDPROC") != NULL)
	{
		SendNotifyMessage((HWND)GetProp(GetDesktopWindow(), "WILSON_HOOK_HWND_CALLWNDPROC"), RegisterWindowMessage("WILSON_HOOK_CALLWNDPROC_REPLACED"), 0, 0);
	}

	SetProp(GetDesktopWindow(), "WILSON_HOOK_HWND_CALLWNDPROC", destination);

	hookCallWndProc = SetWindowsHookEx(WH_CALLWNDPROC, (HOOKPROC)CallWndProcHookCallback, g_appInstance, threadID);
	return hookCallWndProc != NULL;
}

void UninitializeCallWndProcHook()
{
	if (hookCallWndProc != NULL)
		UnhookWindowsHookEx(hookCallWndProc);
	hookCallWndProc = NULL;
}

static LRESULT CALLBACK CallWndProcHookCallback(int code, WPARAM wparam, LPARAM lparam)
{
	if (code >= 0)
	{
		UINT msg = 0;
		UINT msg2 = 0;

		msg = RegisterWindowMessage("WILSON_HOOK_CALLWNDPROC");
		msg2 = RegisterWindowMessage("WILSON_HOOK_CALLWNDPROC_PARAMS");

		HWND dstWnd = (HWND)GetProp(GetDesktopWindow(), "WILSON_HOOK_HWND_CALLWNDPROC");
		
		CWPSTRUCT* pCwpStruct = (CWPSTRUCT*)lparam;

		if (msg != 0 && pCwpStruct->message != msg && pCwpStruct->message != msg2)
		{
			SendNotifyMessage(dstWnd, msg, (WPARAM)pCwpStruct->hwnd, pCwpStruct->message);
			SendNotifyMessage(dstWnd, msg2, pCwpStruct->wParam, pCwpStruct->lParam);
		}
	}

	return CallNextHookEx(hookCallWndProc, code, wparam, lparam);
}

bool InitializeGetMsgHook(int threadID, HWND destination)
{
	if (g_appInstance == NULL)
	{
		return false;
	}

	if (GetProp(GetDesktopWindow(), "WILSON_HOOK_HWND_GETMSG") != NULL)
	{
		SendNotifyMessage((HWND)GetProp(GetDesktopWindow(), "WILSON_HOOK_HWND_GETMSG"), RegisterWindowMessage("WILSON_HOOK_GETMSG_REPLACED"), 0, 0);
	}

	SetProp(GetDesktopWindow(), "WILSON_HOOK_HWND_GETMSG", destination);

	hookGetMsg = SetWindowsHookEx(WH_GETMESSAGE, (HOOKPROC)GetMsgHookCallback, g_appInstance, threadID);
	return hookGetMsg != NULL;
}

void UninitializeGetMsgHook()
{
	if (hookGetMsg != NULL)
		UnhookWindowsHookEx(hookGetMsg);
	hookGetMsg = NULL;
}

static LRESULT CALLBACK GetMsgHookCallback(int code, WPARAM wparam, LPARAM lparam)
{
	if (code >= 0)
	{
		UINT msg = 0;
		UINT msg2 = 0;

		msg = RegisterWindowMessage("WILSON_HOOK_GETMSG");
		msg2 = RegisterWindowMessage("WILSON_HOOK_GETMSG_PARAMS");

		HWND dstWnd = (HWND)GetProp(GetDesktopWindow(), "WILSON_HOOK_HWND_GETMSG");
		
		MSG* pMsg = (MSG*)lparam;

		if (msg != 0 && pMsg->message != msg && pMsg->message != msg2)
		{
			SendNotifyMessage(dstWnd, msg, (WPARAM)pMsg->hwnd, pMsg->message);
			SendNotifyMessage(dstWnd, msg2, pMsg->wParam, pMsg->lParam);
		}
	}

	return CallNextHookEx(hookGetMsg, code, wparam, lparam);
}
