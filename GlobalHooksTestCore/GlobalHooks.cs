using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace GlobalHooksTest
{
	public class GlobalHooks
	{
		public delegate void HookReplacedEventHandler();
		public delegate void WindowEventHandler(IntPtr Handle);
		public delegate void SysCommandEventHandler(int SysCommand, int lParam);
		public delegate void ActivateShellWindowEventHandler();
		public delegate void TaskmanEventHandler();
		public delegate void BasicHookEventHandler(IntPtr Handle1, IntPtr Handle2);
		public delegate void WndProcEventHandler(IntPtr Handle, IntPtr Message, IntPtr wParam, IntPtr lParam);

		// Functions imported from our unmanaged DLL
		[DllImport("GlobalCbtHook.dll")]
		private static extern bool InitializeCbtHook(int threadID, IntPtr DestWindow);
		[DllImport("GlobalCbtHook.dll")]
		private static extern void UninitializeCbtHook();
		[DllImport("GlobalCbtHook.dll")]
		private static extern bool InitializeShellHook(int threadID, IntPtr DestWindow);
		[DllImport("GlobalCbtHook.dll")]
		private static extern void UninitializeShellHook();
		[DllImport("GlobalCbtHook.dll")]
		private static extern void InitializeKeyboardHook(int threadID, IntPtr DestWindow);
		[DllImport("GlobalCbtHook.dll")]
		private static extern void UninitializeKeyboardHook();
		[DllImport("GlobalCbtHook.dll")]
		private static extern void InitializeMouseHook(int threadID, IntPtr DestWindow);
		[DllImport("GlobalCbtHook.dll")]
		private static extern void UninitializeMouseHook();
		[DllImport("GlobalCbtHook.dll")]
		private static extern void InitializeKeyboardLLHook(int threadID, IntPtr DestWindow);
		[DllImport("GlobalCbtHook.dll")]
		private static extern void UninitializeKeyboardLLHook();
		[DllImport("GlobalCbtHook.dll")]
		private static extern void InitializeMouseLLHook(int threadID, IntPtr DestWindow);
		[DllImport("GlobalCbtHook.dll")]
		private static extern void UninitializeMouseLLHook();
		[DllImport("GlobalCbtHook.dll")]
		private static extern void InitializeCallWndProcHook(int threadID, IntPtr DestWindow);
		[DllImport("GlobalCbtHook.dll")]
		private static extern void UninitializeCallWndProcHook();
		[DllImport("GlobalCbtHook.dll")]
		private static extern void InitializeGetMsgHook(int threadID, IntPtr DestWindow);
		[DllImport("GlobalCbtHook.dll")]
		private static extern void UninitializeGetMsgHook();

		// API call needed to retreive the value of the messages to intercept from the unmanaged DLL
		[DllImport("user32.dll")]
		private static extern int RegisterWindowMessage(string lpString);
		[DllImport("user32.dll")]
		private static extern IntPtr GetProp(IntPtr hWnd, string lpString);
		[DllImport("user32.dll")]
		private static extern IntPtr GetDesktopWindow();

		// Handle of the window intercepting messages
		private IntPtr _Handle;

		private CBTHook _CBT;
		private ShellHook _Shell;
		private KeyboardHook _Keyboard;
		private MouseHook _Mouse;
		private KeyboardLLHook _KeyboardLL;
		private MouseLLHook _MouseLL;
		private CallWndProcHook _CallWndProc;
		private GetMsgHook _GetMsg;

		public GlobalHooks(IntPtr Handle)
		{
			_Handle = Handle;

			_CBT = new CBTHook(_Handle);
			_Shell = new ShellHook(_Handle);
			_Keyboard = new KeyboardHook(_Handle);
			_Mouse = new MouseHook(_Handle);
			_KeyboardLL = new KeyboardLLHook(_Handle);
			_MouseLL = new MouseLLHook(_Handle);
			_CallWndProc = new CallWndProcHook(_Handle);
			_GetMsg = new GetMsgHook(_Handle);
		}

		~GlobalHooks()
		{
			_CBT.Stop();
			_Shell.Stop();
			_Keyboard.Stop();
			_Mouse.Stop();
			_KeyboardLL.Stop();
			_MouseLL.Stop();
			_CallWndProc.Stop();
			_GetMsg.Stop();
		}

		public void ProcessWindowMessage(ref System.Windows.Forms.Message m)
		{
			_CBT.ProcessWindowMessage(ref m);
			_Shell.ProcessWindowMessage(ref m);
			_Keyboard.ProcessWindowMessage(ref m);
			_Mouse.ProcessWindowMessage(ref m);
			_KeyboardLL.ProcessWindowMessage(ref m);
			_MouseLL.ProcessWindowMessage(ref m);
			_CallWndProc.ProcessWindowMessage(ref m);
			_GetMsg.ProcessWindowMessage(ref m);
		}

		#region Accessors

		public CBTHook CBT
		{
			get { return _CBT; }
		}

		public ShellHook Shell
		{
			get { return _Shell; }
		}

		public KeyboardHook Keyboard
		{
			get { return _Keyboard; }
		}

		public MouseHook Mouse
		{
			get { return _Mouse; }
		}

		public KeyboardLLHook KeyboardLL
		{
			get { return _KeyboardLL; }
		}

		public MouseLLHook MouseLL
		{
			get  { return _MouseLL; }
		}

		public CallWndProcHook CallWndProc
		{
			get { return _CallWndProc; }
		}

		public GetMsgHook GetMsg
		{
			get { return _GetMsg; }
		}

		#endregion

		public abstract class Hook
		{
			protected bool _IsActive = false;
			protected IntPtr _Handle;

			public Hook(IntPtr Handle)
			{
				_Handle = Handle;
			}

			public void Start()
			{
				if (!_IsActive)
				{
					_IsActive = true;
					OnStart();
				}
			}

			public void Stop()
			{
				if (_IsActive)
				{
					OnStop();
					_IsActive = false;
				}
			}

			~Hook()
			{
				Stop();
			}

			public bool IsActive
			{
				get { return _IsActive; }
			}

			protected abstract void OnStart();
			protected abstract void OnStop();
			public abstract void ProcessWindowMessage(ref System.Windows.Forms.Message m);
		}

		public class CBTHook : Hook
		{
			// Values retreived with RegisterWindowMessage
			private int _MsgID_CBT_HookReplaced;
			private int _MsgID_CBT_Activate;
			private int _MsgID_CBT_CreateWnd;
			private int _MsgID_CBT_DestroyWnd;
			private int _MsgID_CBT_MinMax;
			private int _MsgID_CBT_MoveSize;
			private int _MsgID_CBT_SetFocus;
			private int _MsgID_CBT_SysCommand;

			public event HookReplacedEventHandler HookReplaced;
			public event WindowEventHandler Activate;
			public event WindowEventHandler CreateWindow;
			public event WindowEventHandler DestroyWindow;
			public event WindowEventHandler MinMax;
			public event WindowEventHandler MoveSize;
			public event WindowEventHandler SetFocus;
			public event SysCommandEventHandler SysCommand;

			public CBTHook(IntPtr Handle) : base(Handle)
			{
			}

			protected override void OnStart()
			{
				// Retreive the message IDs that we'll look for in WndProc
				_MsgID_CBT_HookReplaced = RegisterWindowMessage("WILSON_HOOK_CBT_REPLACED");
				_MsgID_CBT_Activate = RegisterWindowMessage("WILSON_HOOK_HCBT_ACTIVATE");
				_MsgID_CBT_CreateWnd = RegisterWindowMessage("WILSON_HOOK_HCBT_CREATEWND");
				_MsgID_CBT_DestroyWnd = RegisterWindowMessage("WILSON_HOOK_HCBT_DESTROYWND");
				_MsgID_CBT_MinMax = RegisterWindowMessage("WILSON_HOOK_HCBT_MINMAX");
				_MsgID_CBT_MoveSize = RegisterWindowMessage("WILSON_HOOK_HCBT_MOVESIZE");
				_MsgID_CBT_SetFocus = RegisterWindowMessage("WILSON_HOOK_HCBT_SETFOCUS");
				_MsgID_CBT_SysCommand = RegisterWindowMessage("WILSON_HOOK_HCBT_SYSCOMMAND");

				// Start the hook
				InitializeCbtHook(0, _Handle);
			}

			protected override void OnStop()
			{
				UninitializeCbtHook();
			}

			public override void ProcessWindowMessage(ref System.Windows.Forms.Message m)
			{
				if (m.Msg == _MsgID_CBT_HookReplaced)
				{
					if (HookReplaced != null)
						HookReplaced();
				}
				else if (m.Msg == _MsgID_CBT_Activate)
				{
					if (Activate != null)
						Activate(m.WParam);
				}
				else if (m.Msg == _MsgID_CBT_CreateWnd)
				{
					if (CreateWindow != null)
						CreateWindow(m.WParam);
				}
				else if (m.Msg == _MsgID_CBT_DestroyWnd)
				{
					if (DestroyWindow != null)
						DestroyWindow(m.WParam);
				}
				else if (m.Msg == _MsgID_CBT_MinMax)
				{
					if (MinMax != null)
						MinMax(m.WParam);
				}
				else if (m.Msg == _MsgID_CBT_MoveSize)
				{
					if (MoveSize != null)
						MoveSize(m.WParam);
				}
				else if (m.Msg == _MsgID_CBT_SetFocus)
				{
					if (SetFocus != null)
						SetFocus(m.WParam);
				}
				else if (m.Msg == _MsgID_CBT_SysCommand)
				{
					if (SysCommand != null)
						SysCommand(m.WParam.ToInt32(), m.LParam.ToInt32());
				}
			}
		}

		public class ShellHook : Hook
		{
			// Values retreived with RegisterWindowMessage
			private int _MsgID_Shell_ActivateShellWindow;
			private int _MsgID_Shell_GetMinRect;
			private int _MsgID_Shell_Language;
			private int _MsgID_Shell_Redraw;
			private int _MsgID_Shell_Taskman;
			private int _MsgID_Shell_HookReplaced;
			private int _MsgID_Shell_WindowActivated;
			private int _MsgID_Shell_WindowCreated;
			private int _MsgID_Shell_WindowDestroyed;

			public event HookReplacedEventHandler HookReplaced;
			public event ActivateShellWindowEventHandler ActivateShellWindow;
			public event WindowEventHandler GetMinRect;
			public event WindowEventHandler Language;
			public event WindowEventHandler Redraw;
			public event TaskmanEventHandler Taskman;
			public event WindowEventHandler WindowActivated;
			public event WindowEventHandler WindowCreated;
			public event WindowEventHandler WindowDestroyed;

			public ShellHook(IntPtr Handle) : base(Handle)
			{
			}

			protected override void OnStart()
			{
				// Retreive the message IDs that we'll look for in WndProc
				_MsgID_Shell_HookReplaced = RegisterWindowMessage("WILSON_HOOK_SHELL_REPLACED");
				_MsgID_Shell_ActivateShellWindow = RegisterWindowMessage("WILSON_HOOK_HSHELL_ACTIVATESHELLWINDOW");
				_MsgID_Shell_GetMinRect = RegisterWindowMessage("WILSON_HOOK_HSHELL_GETMINRECT");
				_MsgID_Shell_Language = RegisterWindowMessage("WILSON_HOOK_HSHELL_LANGUAGE");
				_MsgID_Shell_Redraw = RegisterWindowMessage("WILSON_HOOK_HSHELL_REDRAW");
				_MsgID_Shell_Taskman = RegisterWindowMessage("WILSON_HOOK_HSHELL_TASKMAN");
				_MsgID_Shell_WindowActivated = RegisterWindowMessage("WILSON_HOOK_HSHELL_WINDOWACTIVATED");
				_MsgID_Shell_WindowCreated = RegisterWindowMessage("WILSON_HOOK_HSHELL_WINDOWCREATED");
				_MsgID_Shell_WindowDestroyed = RegisterWindowMessage("WILSON_HOOK_HSHELL_WINDOWDESTROYED");

				// Start the hook
				InitializeShellHook(0, _Handle);
			}

			protected override void OnStop()
			{
				UninitializeShellHook();
			}

			public override void ProcessWindowMessage(ref System.Windows.Forms.Message m)
			{
				if (m.Msg == _MsgID_Shell_HookReplaced)
				{
					if (HookReplaced != null)
						HookReplaced();
				}
				else if (m.Msg == _MsgID_Shell_ActivateShellWindow)
				{
					if (ActivateShellWindow != null)
						ActivateShellWindow();
				}
				else if (m.Msg == _MsgID_Shell_GetMinRect)
				{
					if (GetMinRect != null)
						GetMinRect(m.WParam);
				}
				else if (m.Msg == _MsgID_Shell_Language)
				{
					if (Language != null)
						Language(m.WParam);
				}
				else if (m.Msg == _MsgID_Shell_Redraw)
				{
					if (Redraw != null)
						Redraw(m.WParam);
				}
				else if (m.Msg == _MsgID_Shell_Taskman)
				{
					if (Taskman != null)
						Taskman();
				}
				else if (m.Msg == _MsgID_Shell_WindowActivated)
				{
					if (WindowActivated != null)
						WindowActivated(m.WParam);
				}
				else if (m.Msg == _MsgID_Shell_WindowCreated)
				{
					if (WindowCreated != null)
						WindowCreated(m.WParam);
				}
				else if (m.Msg == _MsgID_Shell_WindowDestroyed)
				{
					if (WindowDestroyed != null)
						WindowDestroyed(m.WParam);
				}
			}
		}

		public class KeyboardHook : Hook
		{
			// Values retreived with RegisterWindowMessage
			private int _MsgID_Keyboard;
			private int _MsgID_Keyboard_HookReplaced;

			public event HookReplacedEventHandler HookReplaced;
			public event BasicHookEventHandler KeyboardEvent;

			public KeyboardHook(IntPtr Handle) : base(Handle)
			{
			}

			protected override void OnStart()
			{
				// Retreive the message IDs that we'll look for in WndProc
				_MsgID_Keyboard = RegisterWindowMessage("WILSON_HOOK_KEYBOARD");
				_MsgID_Keyboard_HookReplaced = RegisterWindowMessage("WILSON_HOOK_KEYBOARD_REPLACED");

				// Start the hook
				InitializeKeyboardHook(0, _Handle);
			}
			protected override void OnStop()
			{
				UninitializeKeyboardHook();
			}

			public override void ProcessWindowMessage(ref System.Windows.Forms.Message m)
			{
				if (m.Msg == _MsgID_Keyboard)
				{
					if (KeyboardEvent != null)
						KeyboardEvent(m.WParam, m.LParam);
				}
				else if (m.Msg == _MsgID_Keyboard_HookReplaced)
				{
					if (HookReplaced != null)
						HookReplaced();
				}
			}
		}

		public class MouseHook : Hook
		{
			// Values retreived with RegisterWindowMessage
			private int _MsgID_Mouse;
			private int _MsgID_Mouse_HookReplaced;

			public event HookReplacedEventHandler HookReplaced;
			public event BasicHookEventHandler MouseEvent;

			public MouseHook(IntPtr Handle) : base(Handle)
			{
			}

			protected override void OnStart()
			{
				// Retreive the message IDs that we'll look for in WndProc
				_MsgID_Mouse = RegisterWindowMessage("WILSON_HOOK_MOUSE");
				_MsgID_Mouse_HookReplaced = RegisterWindowMessage("WILSON_HOOK_MOUSE_REPLACED");

				// Start the hook
				InitializeMouseHook(0, _Handle);
			}
			protected override void OnStop()
			{
				UninitializeMouseHook();
			}

			public override void ProcessWindowMessage(ref System.Windows.Forms.Message m)
			{
				if (m.Msg == _MsgID_Mouse)
				{
					if (MouseEvent != null)
						MouseEvent(m.WParam, m.LParam);
				}
				else if (m.Msg == _MsgID_Mouse_HookReplaced)
				{
					if (HookReplaced != null)
						HookReplaced();
				}
			}
		}

		public class KeyboardLLHook : Hook
		{
			// Values retreived with RegisterWindowMessage
			private int _MsgID_KeyboardLL;
			private int _MsgID_KeyboardLL_HookReplaced;

			public event HookReplacedEventHandler HookReplaced;
			public event BasicHookEventHandler KeyboardLLEvent;

			public KeyboardLLHook(IntPtr Handle) : base(Handle)
			{
			}

			protected override void OnStart()
			{
				// Retreive the message IDs that we'll look for in WndProc
				_MsgID_KeyboardLL = RegisterWindowMessage("WILSON_HOOK_KEYBOARDLL");
				_MsgID_KeyboardLL_HookReplaced = RegisterWindowMessage("WILSON_HOOK_KEYBOARDLL_REPLACED");

				// Start the hook
				InitializeKeyboardLLHook(0, _Handle);
			}

			protected override void OnStop()
			{
				UninitializeKeyboardLLHook();
			}

			public override void ProcessWindowMessage(ref System.Windows.Forms.Message m)
			{
				if (m.Msg == _MsgID_KeyboardLL)
				{
					if (KeyboardLLEvent != null)
						KeyboardLLEvent(m.WParam, m.LParam);
				}
				else if (m.Msg == _MsgID_KeyboardLL_HookReplaced)
				{
					if (HookReplaced != null)
						HookReplaced();
				}
			}
		}

		public class MouseLLHook : Hook
		{
			// Values retreived with RegisterWindowMessage
			private int _MsgID_MouseLL;
			private int _MsgID_MouseLL_HookReplaced;

			public event HookReplacedEventHandler HookReplaced;
			public event BasicHookEventHandler MouseLLEvent;
			public event MouseEventHandler MouseDown;
			public event MouseEventHandler MouseMove;
			public event MouseEventHandler MouseUp;

			private const int WM_MOUSEMOVE                    = 0x0200;
			private const int WM_LBUTTONDOWN                  = 0x0201;
			private const int WM_LBUTTONUP                    = 0x0202;
			private const int WM_LBUTTONDBLCLK                = 0x0203;
			private const int WM_RBUTTONDOWN                  = 0x0204;
			private const int WM_RBUTTONUP                    = 0x0205;
			private const int WM_RBUTTONDBLCLK                = 0x0206;
			private const int WM_MBUTTONDOWN                  = 0x0207;
			private const int WM_MBUTTONUP                    = 0x0208;
			private const int WM_MBUTTONDBLCLK                = 0x0209;
			private const int WM_MOUSEWHEEL                   = 0x020A;

			struct MSLLHOOKSTRUCT 
			{
				public System.Drawing.Point pt;
				public int mouseData;
				public int flags;
				public int time;
				public IntPtr dwExtraInfo;
			};

			public MouseLLHook(IntPtr Handle) : base(Handle)
			{
			}

			protected override void OnStart()
			{
				// Retreive the message IDs that we'll look for in WndProc
				_MsgID_MouseLL = RegisterWindowMessage("WILSON_HOOK_MOUSELL");
				_MsgID_MouseLL_HookReplaced = RegisterWindowMessage("WILSON_HOOK_MOUSELL_REPLACED");

				// Start the hook
				InitializeMouseLLHook(0, _Handle);
			}

			protected override void OnStop()
			{
				UninitializeMouseLLHook();
			}

			public override void ProcessWindowMessage(ref System.Windows.Forms.Message m)

			{
				if (m.Msg == _MsgID_MouseLL)
				{
					if (MouseLLEvent != null)
						MouseLLEvent(m.WParam, m.LParam);

					MSLLHOOKSTRUCT M = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(m.LParam, typeof(MSLLHOOKSTRUCT));

					if (m.WParam.ToInt32() == WM_MOUSEMOVE)
					{
						if (MouseMove != null)
							MouseMove(this, new MouseEventArgs(MouseButtons.None, 0, M.pt.X, M.pt.Y, 0));
					}
					else if (m.WParam.ToInt32() == WM_LBUTTONDOWN)
					{
						if (MouseDown != null)
							MouseDown(this, new MouseEventArgs(MouseButtons.Left, 0, M.pt.X, M.pt.Y, 0));
					}
					else if (m.WParam.ToInt32() == WM_RBUTTONDOWN)
					{
						if (MouseDown != null)
							MouseDown(this, new MouseEventArgs(MouseButtons.Right, 0, M.pt.X, M.pt.Y, 0));
					}
					else if (m.WParam.ToInt32() == WM_LBUTTONUP)
					{
						if (MouseUp != null)
							MouseUp(this, new MouseEventArgs(MouseButtons.Left, 0, M.pt.X, M.pt.Y, 0));
					}
					else if (m.WParam.ToInt32() == WM_RBUTTONUP)
					{
						if (MouseUp != null)
							MouseUp(this, new MouseEventArgs(MouseButtons.Right, 0, M.pt.X, M.pt.Y, 0));
					}
				}
				else if (m.Msg == _MsgID_MouseLL_HookReplaced)
				{
					if (HookReplaced != null)
						HookReplaced();
				}
			}
		}
		public class CallWndProcHook : Hook
		{
			// Values retreived with RegisterWindowMessage
			private int _MsgID_CallWndProc;
			private int _MsgID_CallWndProc_Params;
			private int _MsgID_CallWndProc_HookReplaced;

			public event HookReplacedEventHandler HookReplaced;
			public event WndProcEventHandler CallWndProc;

			private IntPtr _CacheHandle;
			private IntPtr _CacheMessage;

			public CallWndProcHook(IntPtr Handle) : base(Handle)
			{
			}

			protected override void OnStart()
			{
				// Retreive the message IDs that we'll look for in WndProc
				_MsgID_CallWndProc_HookReplaced = RegisterWindowMessage("WILSON_HOOK_CALLWNDPROC_REPLACED");
				_MsgID_CallWndProc = RegisterWindowMessage("WILSON_HOOK_CALLWNDPROC");
				_MsgID_CallWndProc_Params = RegisterWindowMessage("WILSON_HOOK_CALLWNDPROC_PARAMS");

				// Start the hook
				InitializeCallWndProcHook(0, _Handle);
			}

			protected override void OnStop()
			{
				UninitializeCallWndProcHook();
			}

			public override void ProcessWindowMessage(ref System.Windows.Forms.Message m)
			{
				if (m.Msg == _MsgID_CallWndProc)
				{
					_CacheHandle = m.WParam;
					_CacheMessage = m.LParam;
				}
				else if (m.Msg == _MsgID_CallWndProc_Params)
				{
					if (CallWndProc != null && _CacheHandle != IntPtr.Zero && _CacheMessage != IntPtr.Zero)
						CallWndProc(_CacheHandle, _CacheMessage, m.WParam, m.LParam);
					_CacheHandle = IntPtr.Zero;
					_CacheMessage = IntPtr.Zero;
				}
				else if (m.Msg == _MsgID_CallWndProc_HookReplaced)
				{
					if (HookReplaced != null)
						HookReplaced();
				}
			}
		}
		public class GetMsgHook : Hook
		{
			// Values retreived with RegisterWindowMessage
			private int _MsgID_GetMsg;
			private int _MsgID_GetMsg_Params;
			private int _MsgID_GetMsg_HookReplaced;

			public event HookReplacedEventHandler HookReplaced;
			public event WndProcEventHandler GetMsg;

			private IntPtr _CacheHandle;
			private IntPtr _CacheMessage;

			public GetMsgHook(IntPtr Handle) : base(Handle)
			{
			}

			protected override void OnStart()
			{
				// Retreive the message IDs that we'll look for in WndProc
				_MsgID_GetMsg_HookReplaced = RegisterWindowMessage("WILSON_HOOK_GETMSG_REPLACED");
				_MsgID_GetMsg = RegisterWindowMessage("WILSON_HOOK_GETMSG");
				_MsgID_GetMsg_Params = RegisterWindowMessage("WILSON_HOOK_GETMSG_PARAMS");

				// Start the hook
				InitializeGetMsgHook(0, _Handle);
			}

			protected override void OnStop()
			{
				UninitializeGetMsgHook();
			}

			public override void ProcessWindowMessage(ref System.Windows.Forms.Message m)
			{
				if (m.Msg == _MsgID_GetMsg)
				{
					_CacheHandle = m.WParam;
					_CacheMessage = m.LParam;
				}
				else if (m.Msg == _MsgID_GetMsg_Params)
				{
					if (GetMsg != null && _CacheHandle != IntPtr.Zero && _CacheMessage != IntPtr.Zero)
						GetMsg(_CacheHandle, _CacheMessage, m.WParam, m.LParam);
					_CacheHandle = IntPtr.Zero;
					_CacheMessage = IntPtr.Zero;
				}
				else if (m.Msg == _MsgID_GetMsg_HookReplaced)
				{
					if (HookReplaced != null)
						HookReplaced();
				}
			}
		}
	}
}
