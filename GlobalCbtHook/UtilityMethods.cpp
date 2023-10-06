// /////////////////////////////////////////////////////////////
// File: UtilityMethods.cpp  Class: none
// Date: 2/25/2004			 Author: Michael Kennedy
// Language: C++			 Framework: Win32
//
// Copyright: Copyright (c) Michael Kennedy, 2004-2005
// /////////////////////////////////////////////////////////////
// License: See License.txt file included with application.
// Description: See compiled documentation (Managed Hooks.chm)
// /////////////////////////////////////////////////////////////

#include "stdafx.h"
#include <windows.h>
#include "SystemHookCore.h"

//bool GetMousePosition(WPARAM wparam, LPARAM lparam, int & x, int & y)
//{
//	MOUSEHOOKSTRUCT * pMouseStruct = (MOUSEHOOKSTRUCT *)lparam;
//
//	if (pMouseStruct == NULL)
//	{
//		return false;
//	}
//	
//	x = pMouseStruct->pt.x;
//	y = pMouseStruct->pt.y;
//
//	return true;
//}
//
//bool GetKeyboardReading(WPARAM wparam, LPARAM lparam, int & vkCode)
//{
//	KBDLLHOOKSTRUCT * pKeyboardStruct = (KBDLLHOOKSTRUCT *)lparam;
//
//	if (pKeyboardStruct == NULL)
//	{
//		return false;
//	}
//	
//	vkCode = pKeyboardStruct->vkCode;
//	return true;
//}
//
