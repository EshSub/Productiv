using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Text;
using System.Runtime.InteropServices;
//using WindowsDesktop;
using System.Threading;
using System.Collections.Generic;
using Microsoft.Toolkit.Uwp.Notifications;
using System.Diagnostics;
//using System.Windows.Controls;
using System.Resources;
using System.Reflection.Metadata;
using Microsoft.Win32;
using Productiv;
using NuGet.Configuration;
using System.Configuration;
using WinRT;

namespace GlobalHooksTest
{
    public class Form1 : Form
    {
        private GroupBox groupBox1;
        private Button BtnInitCbt;
        private Button BtnUninitCbt;
        private GroupBox groupBox2;
        private Button BtnUninitShell;
        private Button BtnInitShell;
        private ListBox ListCbt;
        private ListBox ListShell;
        private Label LblMouse;
        private NotifyIcon notifyIcon1;
        private Button hideButton;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private GroupBox settingsBox;
        private GroupBox quickControlsBox;
        private Button SpreadWindowsBtn;
        private TabPage tabPage3;
        private CheckBox checkBox1;
        private Label LogoText;
        private Label LogoTM;
        private CheckBox debug;
        private Button ClearListsButton;
        private SplitContainer splitContainer1;
        private Label MadeWith;
        private LinkLabel GithubProfileLink;
        private Label FooterRight;
        private LinkLabel RepoLink;
        private GroupBox RoadmapGroup;
        private CheckedListBox RoadmapList;
        private ListBox listBox1;
        private Label Implemented;
        private ListBox listBox2;
        private Label Next;
        private Label MinimizeText;
        private Label ReportIssueHelpText;
        private Label HowToText;
        private TextBox textBox1;
        private TextBox TOBeDone;
        private Label White;
        private CheckBox AddToLast;
        private IContainer components;

        #region DLLs and variables

        // API calls to give us a bit more information about the data we get from the hook
        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder title, int size);
        [DllImport("user32.dll")]
        private static extern uint RealGetWindowClass(IntPtr hWnd, StringBuilder pszType, uint cchType);

        private GlobalHooks _GlobalHooks;

        delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        //IDictionary<IntPtr, VirtualDesktop> windowDesktopMap = new Dictionary<IntPtr, VirtualDesktop>();
        IDictionary<IntPtr, bool> windowStates = new Dictionary<IntPtr, bool>();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        [StructLayout(LayoutKind.Sequential)]
        struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public int showCmd;
            public POINT minPosition;
            public POINT maxPosition;
            public RECT normalPosition;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

        #endregion

        private void HideWindow()
        {

            this.Hide();
            this.ShowInTaskbar = false;
            notifyIcon1.Visible = true;
        }

        public Form1()
        {
            InitializeComponent();

            #region VD initializing and stuff

            EventHandlers.OnAppStart();

            _GlobalHooks = new GlobalHooks(this.Handle);

            // Set the hook events
            _GlobalHooks.CBT.Activate += new GlobalHooksTest.GlobalHooks.WindowEventHandler(_GlobalHooks_CbtActivate);
            _GlobalHooks.CBT.CreateWindow += new GlobalHooksTest.GlobalHooks.WindowEventHandler(_GlobalHooks_CbtCreateWindow);
            _GlobalHooks.CBT.DestroyWindow += new GlobalHooksTest.GlobalHooks.WindowEventHandler(_GlobalHooks_CbtDestroyWindow);
            _GlobalHooks.CBT.MinMax += new GlobalHooksTest.GlobalHooks.WindowEventHandler(_GlobalHooks_CbtMinMax);
            _GlobalHooks.Shell.WindowActivated += new GlobalHooksTest.GlobalHooks.WindowEventHandler(_GlobalHooks_ShellWindowActivated);
            _GlobalHooks.Shell.WindowCreated += new GlobalHooksTest.GlobalHooks.WindowEventHandler(_GlobalHooks_ShellWindowCreated);
            _GlobalHooks.Shell.WindowDestroyed += new GlobalHooksTest.GlobalHooks.WindowEventHandler(_GlobalHooks_ShellWindowDestroyed);
            _GlobalHooks.Shell.Redraw += new GlobalHooksTest.GlobalHooks.WindowEventHandler(_GlobalHooks_ShellRedraw);
            _GlobalHooks.MouseLL.MouseMove += new MouseEventHandler(MouseLL_MouseMove);

            _GlobalHooks.MouseLL.Start();

            _GlobalHooks.CBT.Start();
            _GlobalHooks.Shell.Start();

            #endregion

            ListCbt.Items.Add("Application.productName: " + Application.ProductName);
            ListCbt.Items.Add("Application.executablePath: " + Application.ExecutablePath);

            #region Run at startup config
            if (rkApp.GetValue(Application.ProductName) == null)
            {
                // The value doesn't exist, the application is not set to run at startup, Check box
                checkBox1.Checked = false;
                //lblInfo.Content = "The application doesn't run at startup";
            }
            else
            {
                // The value exists, the application is set to run at startup
                checkBox1.Checked = true;
                //lblInfo.Content = "The application runs at startup";
            }

            try
            {
                AddToLast.Checked = Convert.ToBoolean(ConfigurationManager.AppSettings["AddToLast"]);
            }
            catch
            {
                AddToLast.Checked = false;
            }
            #endregion

            HideWindow();
            this.WindowState = FormWindowState.Minimized;
        }

        #region COM imports

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        #endregion


        #region Minimize Form to system tray

        private void ImportStatusForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                notifyIcon1.Visible = true;
                notifyIcon1.ShowBalloonTip(1000);
                this.ShowInTaskbar = false;
            }
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.ShowInTaskbar = true;
            this.Show();
            notifyIcon1.Visible = false;
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Make sure we stop hooking before quitting! Otherwise, the hook procedures in GlobalCbtHook.dll
                // will keep being called, even after our application quits, which will needlessly degrade system
                // performance. And it's just plain sloppy.
                _GlobalHooks.CBT.Stop();
                _GlobalHooks.Shell.Stop();
                _GlobalHooks.MouseLL.Stop();

                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region wndProc prevent minimize

        private const int WM_SYSCOMMAND = 0x0112;
        private const int SC_MINIMIZE = 0xf020;

        protected override void WndProc(ref Message m)
        {
            // Check to see if we've received any Windows messages telling us about our hooks
            if (m.Msg == WM_SYSCOMMAND)
            {
                if (m.WParam.ToInt32() == SC_MINIMIZE)
                {
                    HideWindow();
                    m.Result = IntPtr.Zero;
                    return;
                }
            }
            if (_GlobalHooks != null)
                _GlobalHooks.ProcessWindowMessage(ref m);

            base.WndProc(ref m);
        }

        #endregion


        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new Container();
            var resources = new ComponentResourceManager(typeof(Form1));
            groupBox1 = new GroupBox();
            hideButton = new Button();
            ListCbt = new ListBox();
            BtnUninitCbt = new Button();
            BtnInitCbt = new Button();
            groupBox2 = new GroupBox();
            ListShell = new ListBox();
            BtnUninitShell = new Button();
            BtnInitShell = new Button();
            LblMouse = new Label();
            notifyIcon1 = new NotifyIcon(components);
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            MinimizeText = new Label();
            RoadmapGroup = new GroupBox();
            listBox2 = new ListBox();
            Next = new Label();
            Implemented = new Label();
            listBox1 = new ListBox();
            quickControlsBox = new GroupBox();
            SpreadWindowsBtn = new Button();
            settingsBox = new GroupBox();
            AddToLast = new CheckBox();
            checkBox1 = new CheckBox();
            tabPage2 = new TabPage();
            textBox1 = new TextBox();
            ReportIssueHelpText = new Label();
            HowToText = new Label();
            splitContainer1 = new SplitContainer();
            ClearListsButton = new Button();
            debug = new CheckBox();
            tabPage3 = new TabPage();
            TOBeDone = new TextBox();
            LogoText = new Label();
            LogoTM = new Label();
            MadeWith = new Label();
            GithubProfileLink = new LinkLabel();
            FooterRight = new Label();
            RepoLink = new LinkLabel();
            White = new Label();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            RoadmapGroup.SuspendLayout();
            quickControlsBox.SuspendLayout();
            settingsBox.SuspendLayout();
            tabPage2.SuspendLayout();
            ((ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            tabPage3.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBox1.AutoSize = true;
            groupBox1.Controls.Add(hideButton);
            groupBox1.Controls.Add(ListCbt);
            groupBox1.Controls.Add(BtnUninitCbt);
            groupBox1.Controls.Add(BtnInitCbt);
            groupBox1.Location = new Point(3, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(333, 434);
            groupBox1.TabIndex = 2;
            groupBox1.TabStop = false;
            groupBox1.Text = "CBT Hooks";
            // 
            // hideButton
            // 
            hideButton.Location = new Point(215, 20);
            hideButton.Name = "hideButton";
            hideButton.Size = new Size(93, 39);
            hideButton.TabIndex = 4;
            hideButton.Text = "Hide";
            hideButton.UseVisualStyleBackColor = true;
            hideButton.Click += hideButton_Click;
            // 
            // ListCbt
            // 
            ListCbt.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            ListCbt.ItemHeight = 15;
            ListCbt.Location = new Point(0, 68);
            ListCbt.Name = "ListCbt";
            ListCbt.Size = new Size(333, 304);
            ListCbt.TabIndex = 3;
            // 
            // BtnUninitCbt
            // 
            BtnUninitCbt.Location = new Point(107, 20);
            BtnUninitCbt.Name = "BtnUninitCbt";
            BtnUninitCbt.Size = new Size(102, 39);
            BtnUninitCbt.TabIndex = 2;
            BtnUninitCbt.Text = "Uninitialize CBT Hooks";
            BtnUninitCbt.Click += BtnUninitCbt_Click;
            // 
            // BtnInitCbt
            // 
            BtnInitCbt.Location = new Point(10, 20);
            BtnInitCbt.Name = "BtnInitCbt";
            BtnInitCbt.Size = new Size(91, 39);
            BtnInitCbt.TabIndex = 1;
            BtnInitCbt.Text = "Initialize CBT Hooks";
            BtnInitCbt.Click += BtnInitCbt_Click;
            // 
            // groupBox2
            // 
            groupBox2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBox2.AutoSize = true;
            groupBox2.Controls.Add(ListShell);
            groupBox2.Controls.Add(BtnUninitShell);
            groupBox2.Controls.Add(BtnInitShell);
            groupBox2.Location = new Point(3, 4);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(323, 422);
            groupBox2.TabIndex = 3;
            groupBox2.TabStop = false;
            groupBox2.Text = "Shell Hooks";
            // 
            // ListShell
            // 
            ListShell.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            ListShell.ItemHeight = 15;
            ListShell.Location = new Point(0, 69);
            ListShell.Name = "ListShell";
            ListShell.Size = new Size(317, 304);
            ListShell.TabIndex = 3;
            // 
            // BtnUninitShell
            // 
            BtnUninitShell.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            BtnUninitShell.Location = new Point(158, 19);
            BtnUninitShell.Name = "BtnUninitShell";
            BtnUninitShell.Size = new Size(145, 39);
            BtnUninitShell.TabIndex = 2;
            BtnUninitShell.Text = "Uninitialize Shell Hooks";
            BtnUninitShell.Click += BtnUninitShell_Click;
            // 
            // BtnInitShell
            // 
            BtnInitShell.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            BtnInitShell.Location = new Point(19, 19);
            BtnInitShell.Name = "BtnInitShell";
            BtnInitShell.Size = new Size(133, 39);
            BtnInitShell.TabIndex = 1;
            BtnInitShell.Text = "Initialize Shell Hooks";
            BtnInitShell.Click += BtnInitShell_Click;
            // 
            // LblMouse
            // 
            LblMouse.Location = new Point(19, 364);
            LblMouse.Name = "LblMouse";
            LblMouse.Size = new Size(327, 29);
            LblMouse.TabIndex = 4;
            // 
            // notifyIcon1
            // 
            notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
            notifyIcon1.BalloonTipText = "Double click notification icon to view settings";
            notifyIcon1.BalloonTipTitle = "Productiv running in background";
            notifyIcon1.Icon = (Icon)resources.GetObject("notifyIcon1.Icon");
            notifyIcon1.Text = "notifyIcon1";
            notifyIcon1.Visible = true;
            notifyIcon1.MouseDoubleClick += notifyIcon_MouseDoubleClick;
            // 
            // tabControl1
            // 
            tabControl1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabControl1.Appearance = TabAppearance.FlatButtons;
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Location = new Point(8, 86);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.ShowToolTips = true;
            tabControl1.Size = new Size(698, 497);
            tabControl1.TabIndex = 5;
            // 
            // tabPage1
            // 
            tabPage1.BackColor = Color.White;
            tabPage1.Controls.Add(MinimizeText);
            tabPage1.Controls.Add(RoadmapGroup);
            tabPage1.Controls.Add(quickControlsBox);
            tabPage1.Controls.Add(settingsBox);
            tabPage1.Location = new Point(4, 27);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(690, 466);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Home";
            // 
            // MinimizeText
            // 
            MinimizeText.AutoSize = true;
            MinimizeText.Font = new Font("Segoe UI", 9F, FontStyle.Italic, GraphicsUnit.Point);
            MinimizeText.Location = new Point(145, 435);
            MinimizeText.Name = "MinimizeText";
            MinimizeText.Size = new Size(379, 15);
            MinimizeText.TabIndex = 3;
            MinimizeText.Text = "You can minimze this window to keep Productiv running in background";
            // 
            // RoadmapGroup
            // 
            RoadmapGroup.Controls.Add(listBox2);
            RoadmapGroup.Controls.Add(Next);
            RoadmapGroup.Controls.Add(Implemented);
            RoadmapGroup.Controls.Add(listBox1);
            RoadmapGroup.Location = new Point(7, 202);
            RoadmapGroup.Name = "RoadmapGroup";
            RoadmapGroup.Size = new Size(677, 216);
            RoadmapGroup.TabIndex = 2;
            RoadmapGroup.TabStop = false;
            RoadmapGroup.Text = "Roadmap";
            // 
            // listBox2
            // 
            listBox2.BorderStyle = BorderStyle.None;
            listBox2.FormattingEnabled = true;
            listBox2.ItemHeight = 15;
            listBox2.Items.AddRange(new object[] { "Split virtual desktops : New virtual desktops for window groups", "Make the client app using WPF : more visually appealing UI", "Implement easy virtual desktop switching method for mouse users." });
            listBox2.Location = new Point(9, 151);
            listBox2.Name = "listBox2";
            listBox2.Size = new Size(661, 45);
            listBox2.TabIndex = 3;
            // 
            // Next
            // 
            Next.AutoSize = true;
            Next.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            Next.Location = new Point(8, 133);
            Next.Name = "Next";
            Next.Size = new Size(35, 15);
            Next.TabIndex = 2;
            Next.Text = "Next";
            // 
            // Implemented
            // 
            Implemented.AutoSize = true;
            Implemented.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            Implemented.Location = new Point(8, 25);
            Implemented.Name = "Implemented";
            Implemented.Size = new Size(133, 15);
            Implemented.TabIndex = 1;
            Implemented.Text = "Implemented features";
            Implemented.Click += label1_Click;
            // 
            // listBox1
            // 
            listBox1.BorderStyle = BorderStyle.None;
            listBox1.FormattingEnabled = true;
            listBox1.ItemHeight = 15;
            listBox1.Items.AddRange(new object[] { "Move to new vitual desktop on maxmize.", "Move to main (desktop 1) when minimized.", "Move to new virtual desktop when app launched in maximized mode.", "Move to main (desktop 1) when app is launched in minimzed mode in any virtual desktop.", "Remove virtual desktop when maximized window is closed." });
            listBox1.Location = new Point(9, 43);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(659, 75);
            listBox1.TabIndex = 0;
            // 
            // quickControlsBox
            // 
            quickControlsBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            quickControlsBox.Controls.Add(SpreadWindowsBtn);
            quickControlsBox.Location = new Point(7, 6);
            quickControlsBox.Name = "quickControlsBox";
            quickControlsBox.Size = new Size(677, 95);
            quickControlsBox.TabIndex = 1;
            quickControlsBox.TabStop = false;
            quickControlsBox.Text = "Quick Controls";
            // 
            // SpreadWindowsBtn
            // 
            SpreadWindowsBtn.Location = new Point(15, 31);
            SpreadWindowsBtn.Name = "SpreadWindowsBtn";
            SpreadWindowsBtn.Size = new Size(170, 37);
            SpreadWindowsBtn.TabIndex = 0;
            SpreadWindowsBtn.Text = "Spread Maximized windows";
            SpreadWindowsBtn.UseVisualStyleBackColor = true;
            SpreadWindowsBtn.Click += SpreadWindowsBtn_Click;
            // 
            // settingsBox
            // 
            settingsBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            settingsBox.Controls.Add(AddToLast);
            settingsBox.Controls.Add(checkBox1);
            settingsBox.Location = new Point(6, 107);
            settingsBox.Name = "settingsBox";
            settingsBox.Size = new Size(678, 89);
            settingsBox.TabIndex = 0;
            settingsBox.TabStop = false;
            settingsBox.Text = "Settings";
            // 
            // AddToLast
            // 
            AddToLast.AutoSize = true;
            AddToLast.Enabled = false;
            AddToLast.Location = new Point(126, 38);
            AddToLast.Name = "AddToLast";
            AddToLast.Size = new Size(243, 19);
            AddToLast.TabIndex = 1;
            AddToLast.Text = "Add new desktops at last (Coming soon!)";
            AddToLast.UseVisualStyleBackColor = true;
            AddToLast.CheckedChanged += AddToLast_CheckedChanged;
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new Point(16, 38);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(95, 19);
            checkBox1.TabIndex = 0;
            checkBox1.Text = "Start on boot";
            checkBox1.UseVisualStyleBackColor = true;
            checkBox1.CheckedChanged += checkBox1_CheckedChanged;
            // 
            // tabPage2
            // 
            tabPage2.BackColor = Color.White;
            tabPage2.Controls.Add(textBox1);
            tabPage2.Controls.Add(ReportIssueHelpText);
            tabPage2.Controls.Add(HowToText);
            tabPage2.Controls.Add(splitContainer1);
            tabPage2.Controls.Add(ClearListsButton);
            tabPage2.Controls.Add(debug);
            tabPage2.Location = new Point(4, 27);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(690, 466);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Debug";
            // 
            // textBox1
            // 
            textBox1.BorderStyle = BorderStyle.None;
            textBox1.Location = new Point(198, 22);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(483, 35);
            textBox1.TabIndex = 9;
            textBox1.Text = "Take a screenshot of the last elements of the below lists and submit an issue in the github link provide at the bottom right corner.";
            // 
            // ReportIssueHelpText
            // 
            ReportIssueHelpText.AutoSize = true;
            ReportIssueHelpText.Location = new Point(205, 27);
            ReportIssueHelpText.Name = "ReportIssueHelpText";
            ReportIssueHelpText.Size = new Size(0, 15);
            ReportIssueHelpText.TabIndex = 8;
            // 
            // HowToText
            // 
            HowToText.AutoSize = true;
            HowToText.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point);
            HowToText.Location = new Point(199, 5);
            HowToText.Name = "HowToText";
            HowToText.Size = new Size(132, 15);
            HowToText.TabIndex = 7;
            HowToText.Text = "How to report an issue.";
            // 
            // splitContainer1
            // 
            splitContainer1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            splitContainer1.Location = new Point(12, 61);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(groupBox1);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(groupBox2);
            splitContainer1.Size = new Size(672, 399);
            splitContainer1.SplitterDistance = 339;
            splitContainer1.TabIndex = 6;
            // 
            // ClearListsButton
            // 
            ClearListsButton.Location = new Point(85, 18);
            ClearListsButton.Name = "ClearListsButton";
            ClearListsButton.Size = new Size(75, 23);
            ClearListsButton.TabIndex = 5;
            ClearListsButton.Text = "Clear lists";
            ClearListsButton.UseVisualStyleBackColor = true;
            ClearListsButton.Click += ClearListsButton_Click;
            // 
            // debug
            // 
            debug.AutoSize = true;
            debug.Location = new Point(15, 20);
            debug.Name = "debug";
            debug.Size = new Size(61, 19);
            debug.TabIndex = 4;
            debug.Text = "Debug";
            debug.UseVisualStyleBackColor = true;
            debug.CheckedChanged += debug_CheckedChanged;
            debug.CheckStateChanged += debug_CheckStateChanged;
            // 
            // tabPage3
            // 
            tabPage3.BackColor = Color.White;
            tabPage3.Controls.Add(TOBeDone);
            tabPage3.Location = new Point(4, 27);
            tabPage3.Name = "tabPage3";
            tabPage3.Padding = new Padding(3);
            tabPage3.Size = new Size(690, 466);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "Licenses";
            // 
            // TOBeDone
            // 
            TOBeDone.BorderStyle = BorderStyle.None;
            TOBeDone.Location = new Point(294, 211);
            TOBeDone.Name = "TOBeDone";
            TOBeDone.Size = new Size(100, 16);
            TOBeDone.TabIndex = 0;
            TOBeDone.Text = "To be done";
            // 
            // LogoText
            // 
            LogoText.AutoSize = true;
            LogoText.BackColor = Color.Transparent;
            LogoText.FlatStyle = FlatStyle.Flat;
            LogoText.Font = new Font("Segoe UI", 27.75F, FontStyle.Regular, GraphicsUnit.Point);
            LogoText.ImageAlign = ContentAlignment.TopCenter;
            LogoText.Location = new Point(266, 23);
            LogoText.Margin = new Padding(0);
            LogoText.Name = "LogoText";
            LogoText.Size = new Size(200, 50);
            LogoText.TabIndex = 6;
            LogoText.Text = "Product(iv)";
            LogoText.TextAlign = ContentAlignment.MiddleCenter;
            LogoText.Click += LogoText_Click;
            // 
            // LogoTM
            // 
            LogoTM.AutoSize = true;
            LogoTM.Location = new Point(437, 26);
            LogoTM.Name = "LogoTM";
            LogoTM.Size = new Size(24, 15);
            LogoTM.TabIndex = 8;
            LogoTM.Text = "TM";
            // 
            // MadeWith
            // 
            MadeWith.AutoSize = true;
            MadeWith.Location = new Point(14, 586);
            MadeWith.Name = "MadeWith";
            MadeWith.Size = new Size(94, 15);
            MadeWith.TabIndex = 2;
            MadeWith.Text = "Made with ❤ by";
            MadeWith.Click += MadeWith_Click;
            // 
            // GithubProfileLink
            // 
            GithubProfileLink.AutoSize = true;
            GithubProfileLink.Location = new Point(107, 586);
            GithubProfileLink.Name = "GithubProfileLink";
            GithubProfileLink.Size = new Size(45, 15);
            GithubProfileLink.TabIndex = 3;
            GithubProfileLink.TabStop = true;
            GithubProfileLink.Text = "EshSub";
            GithubProfileLink.LinkClicked += GithubProfileLink_LinkClicked;
            // 
            // FooterRight
            // 
            FooterRight.AutoSize = true;
            FooterRight.Location = new Point(413, 586);
            FooterRight.Name = "FooterRight";
            FooterRight.Size = new Size(223, 15);
            FooterRight.TabIndex = 9;
            FooterRight.Text = "Love it? Give a star or Got issue? report @";
            // 
            // RepoLink
            // 
            RepoLink.AutoSize = true;
            RepoLink.Location = new Point(633, 586);
            RepoLink.Name = "RepoLink";
            RepoLink.Size = new Size(70, 15);
            RepoLink.TabIndex = 10;
            RepoLink.TabStop = true;
            RepoLink.Text = "Github repo";
            RepoLink.LinkClicked += RepoLink_LinkClicked;
            // 
            // White
            // 
            White.AutoSize = true;
            White.ForeColor = Color.White;
            White.Location = new Point(406, 66);
            White.Name = "White";
            White.Size = new Size(10, 15);
            White.TabIndex = 11;
            White.Text = " ";
            // 
            // Form1
            // 
            AutoScaleBaseSize = new Size(6, 16);
            AutoSize = true;
            BackColor = Color.White;
            ClientSize = new Size(715, 613);
            Controls.Add(White);
            Controls.Add(RepoLink);
            Controls.Add(FooterRight);
            Controls.Add(GithubProfileLink);
            Controls.Add(LogoTM);
            Controls.Add(MadeWith);
            Controls.Add(LogoText);
            Controls.Add(tabControl1);
            Controls.Add(LblMouse);
            MaximizeBox = false;
            MaximumSize = new Size(731, 652);
            MinimumSize = new Size(731, 652);
            Name = "Form1";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Productiv";
            Load += Form1_Load;
            Resize += ImportStatusForm_Resize;
            groupBox1.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            RoadmapGroup.ResumeLayout(false);
            RoadmapGroup.PerformLayout();
            quickControlsBox.ResumeLayout(false);
            settingsBox.ResumeLayout(false);
            settingsBox.PerformLayout();
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            splitContainer1.Panel2.PerformLayout();
            ((ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            tabPage3.ResumeLayout(false);
            tabPage3.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }
        #endregion

        [STAThread]
        static void Main()
        {
            Form1 form = new Form1();
            Application.Run(form);
        }

        private void BtnInitCbt_Click(object sender, EventArgs e)
        {
            _GlobalHooks.CBT.Start();
        }

        private void BtnUninitCbt_Click(object sender, EventArgs e)
        {
            _GlobalHooks.CBT.Stop();
        }

        private void BtnInitShell_Click(object sender, EventArgs e)
        {
            _GlobalHooks.Shell.Start();
        }

        private void BtnUninitShell_Click(object sender, EventArgs e)
        {
            _GlobalHooks.Shell.Stop();
        }



        #region Windows API Helper Functions

        private string GetWindowName(IntPtr Hwnd)
        {
            // This function gets the name of a window from its handle
            StringBuilder Title = new StringBuilder(256);
            GetWindowText(Hwnd, Title, 256);

            return Title.ToString().Trim();
        }

        private string GetWindowClass(IntPtr Hwnd)
        {
            // This function gets the name of a window class from a window handle
            StringBuilder Title = new StringBuilder(256);
            RealGetWindowClass(Hwnd, Title, 256);

            return Title.ToString().Trim();
        }

        #endregion

        private void _GlobalHooks_CbtActivate(IntPtr Handle)
        {
            if (debug.Checked)
            {
                ListCbt.Items.Add("Activate: " + GetWindowName(Handle));
            }
        }

        private void _GlobalHooks_CbtCreateWindow(IntPtr Handle)
        {
            if (debug.Checked)
            {
                ListCbt.Items.Add("Create: " + GetWindowName(Handle));
            }
        }

        private void _GlobalHooks_CbtDestroyWindow(IntPtr Handle)
        {
            //ListCbt.Items.Add("Destroy: " + GetWindowName(Handle) + Handle.ToString());

        }

        private void _GlobalHooks_CbtMinMax(IntPtr Handle)
        {
            if (Handle == null)
            {
                return;
            }
            if (debug.Checked)
            {
                ListShell.Items.Add("MinMax: " + Helpers.GetWindowName(Handle) + Helpers.GetWindowApplicationName(Handle) + Handle.ToString());

            }
            string windowName = Helpers.GetWindowName(Handle);

            if (windowName != null && Handle == GetForegroundWindow())
            {
                if (Helpers.IsZoomed(Handle))
                {
                    if (debug.Checked)
                    {
                        ListShell.Items.Add("Maximized: " + Helpers.GetWindowApplicationName(Handle) + Helpers.GetWindowName(Handle) + Handle.ToString());
                    }
                    EventHandlers.OnMaximize(Handle);
                }
                else
                {
                    if (debug.Checked)
                    {
                        ListShell.Items.Add("Minimized: " + Helpers.GetWindowApplicationName(Handle) + Helpers.GetWindowName(Handle) + Handle.ToString());
                    }
                    EventHandlers.OnMinimze();
                }
            }

        }

        private void _GlobalHooks_ShellWindowActivated(IntPtr Handle)
        {
            if (debug.Checked)
            {
                ListShell.Items.Add("Activated: " + GetWindowName(Handle) + Handle.ToString());
            }
        }

        private void _GlobalHooks_ShellWindowCreated(IntPtr Handle)
        {
            if (debug.Checked)
            {
                ListShell.Items.Add("Created: " + GetWindowName(Handle));
            }
            Thread.Sleep(200);
            if (Helpers.IsZoomed(Handle))
            {
                EventHandlers.OnCreateMaximized(Handle);
            }
            else
            {
                EventHandlers.OnCreateMinimized(Handle);
            }
            //VirtualDesktop.GetDesktops().
        }

        private void _GlobalHooks_ShellWindowDestroyed(IntPtr Handle)
        {
            if (debug.Checked)
            {
                ListCbt.Items.Add("Current Handle " + this.Handle.ToString() + " App hanlde " + Handle);
            }
            if (Handle == this.Handle)
            {
                return;
            }
            EventHandlers.OnDestroy(Handle);
        }

        private void _GlobalHooks_ShellRedraw(IntPtr Handle)
        {
            //ListShell.Items.Add("Redraw: " + GetWindowName(Handle));
        }

        private void MouseLL_MouseMove(object sender, MouseEventArgs e)
        {
            LblMouse.Text = "Mouse at: " + e.X + ", " + e.Y;
        }

        private void hideButton_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void SpreadWindowsBtn_Click(object sender, EventArgs e)
        {
            EventHandlers.OnSpreadMaximizedWindows();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                // Add the value in the registry so that the application runs at startup
                rkApp.SetValue(Application.ProductName, Application.ExecutablePath);
                //lblInfo.Content = "The application will run at startup";
            }
            else
            {
                // Remove the value from the registry so that the application doesn't start
                rkApp.DeleteValue(Application.ProductName, false);
                //lblInfo.Content = "The application will not run at startup";
            }
        }

        private void LogoTextiv_Click(object sender, EventArgs e)
        {

        }

        private void LogoText_Click(object sender, EventArgs e)
        {

        }

        private void debug_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void debug_CheckStateChanged(object sender, EventArgs e)
        {

        }

        private void ClearListsButton_Click(object sender, EventArgs e)
        {
            ListShell.Items.Clear();
            ListCbt.Items.Clear();
        }

        private void GithubProfileLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo("https://github.com/EshSub");
            sInfo.UseShellExecute = true;
            Process.Start(sInfo);

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void RepoLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo("https://github.com/EshSub/Productiv");
            sInfo.UseShellExecute = true;
            Process.Start(sInfo);
        }

        private void MadeWith_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void AddToLast_CheckedChanged(object sender, EventArgs e)
        {

            ConfigurationManager.AppSettings.Add("AddToLast", AddToLast.Checked.ToString());
        }
    }

    public class MyCustomApplicationContext : ApplicationContext
    {

        #region DLLs and variables

        // API calls to give us a bit more information about the data we get from the hook
        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder title, int size);
        [DllImport("user32.dll")]
        private static extern uint RealGetWindowClass(IntPtr hWnd, StringBuilder pszType, uint cchType);

        private GlobalHooks _GlobalHooks;

        delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        //IDictionary<IntPtr, VirtualDesktop> windowDesktopMap = new Dictionary<IntPtr, VirtualDesktop>();

        IDictionary<IntPtr, bool> windowStates = new Dictionary<IntPtr, bool>();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        [StructLayout(LayoutKind.Sequential)]
        struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public int showCmd;
            public POINT minPosition;
            public POINT maxPosition;
            public RECT normalPosition;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        #endregion

        #region Windows API Helper Functions

        private string GetWindowName(IntPtr Hwnd)
        {
            // This function gets the name of a window from its handle
            StringBuilder Title = new StringBuilder(256);
            GetWindowText(Hwnd, Title, 256);

            return Title.ToString().Trim();
        }

        private string GetWindowClass(IntPtr Hwnd)
        {
            // This function gets the name of a window class from a window handle
            StringBuilder Title = new StringBuilder(256);
            RealGetWindowClass(Hwnd, Title, 256);

            return Title.ToString().Trim();
        }

        #endregion

        private NotifyIcon notify_icon = new NotifyIcon();
        //public MyApplicationContext() {
        //    MenuItem configMenuItem = new MenuItem("Configuration", new EventHandler(ShowConfig));
        //    MenuItem exitMenuItem = new MenuItem("Exit", new EventHandler(Exit));

        //    NotifyIcon notifyIcon = new NotifyIcon();
        //    notifyIcon.Icon = TaskTrayApplication.Properties.Resources.AppIcon;
        //    notifyIcon.ContextMenu = new ContextMenu(new MenuItem[]
        //        { configMenuItem, exitMenuItem });
        //    notifyIcon.Visible = true;



        public MyCustomApplicationContext()
        {

            this.notify_icon.ContextMenuStrip = new ContextMenuStrip();
            this.notify_icon.Icon = new Icon("D:\\PROJECTS\\Global-CbtHook2\\Global-CbtHook\\GlobalHooksTestCore\\icon.ico");
            this.notify_icon.Visible = true;
            this.notify_icon.ContextMenuStrip.Items.Add("Properties", null, this.PropertiesClick);
            //this.notify_icon.ContextMenuStrip.Items.Add("Test2", null, this.MenuTest2_Click);
            this.notify_icon.ContextMenuStrip.Items.Add("Exit", null, this.Exit);

            #region VD initializing
            //VirtualDesktop.Configure();

            //// Instantiate our GlobalHooks object
            //_GlobalHooks = new GlobalHooks(this.Handle);

            //// Set the hook events
            //_GlobalHooks.CBT.Activate += new GlobalHooksTest.GlobalHooks.WindowEventHandler(_GlobalHooks_CbtActivate);
            //_GlobalHooks.CBT.CreateWindow += new GlobalHooksTest.GlobalHooks.WindowEventHandler(_GlobalHooks_CbtCreateWindow);
            //_GlobalHooks.CBT.DestroyWindow += new GlobalHooksTest.GlobalHooks.WindowEventHandler(_GlobalHooks_CbtDestroyWindow);
            //_GlobalHooks.CBT.MinMax += new GlobalHooksTest.GlobalHooks.WindowEventHandler(_GlobalHooks_CbtMinMax);
            //_GlobalHooks.Shell.WindowActivated += new GlobalHooksTest.GlobalHooks.WindowEventHandler(_GlobalHooks_ShellWindowActivated);
            //_GlobalHooks.Shell.WindowCreated += new GlobalHooksTest.GlobalHooks.WindowEventHandler(_GlobalHooks_ShellWindowCreated);
            //_GlobalHooks.Shell.WindowDestroyed += new GlobalHooksTest.GlobalHooks.WindowEventHandler(_GlobalHooks_ShellWindowDestroyed);
            //_GlobalHooks.Shell.Redraw += new GlobalHooksTest.GlobalHooks.WindowEventHandler(_GlobalHooks_ShellRedraw);
            //_GlobalHooks.MouseLL.MouseMove += new MouseEventHandler(MouseLL_MouseMove);

            //_GlobalHooks.MouseLL.Start();

            //_GlobalHooks.CBT.Start();
            //_GlobalHooks.Shell.Start();

            //EnumWindows(new EnumWindowsProc(EnumWindowsCallback), IntPtr.Zero);

            //int initcount = 0;

            //foreach (KeyValuePair<IntPtr, bool> entry in windowStates)
            //{
            //    if (entry.Value)
            //    {

            //        ListShell.Items.Add(entry.Key.ToString() + " " + entry.Value.ToString());
            //        try
            //        {
            //            var desktop = VirtualDesktop.Create();
            //            desktop.Name = GetWindowApplicationName(entry.Key);
            //            Thread.Sleep(10);
            //            VirtualDesktop.MoveToDesktop(entry.Key, desktop);
            //            if (windowDesktopMap.ContainsKey(Handle))
            //            {
            //                windowDesktopMap[Handle] = desktop;
            //            }
            //            else
            //            {
            //                windowDesktopMap.Add(Handle, desktop);
            //            }
            //        }
            //        catch (Exception e)
            //        {
            //            new ToastContentBuilder()
            //.AddText("Error occured")
            //.AddText("Form initializing")
            //.Show();
            //        }
            //        initcount++;
            //    }
            //}

            //new ToastContentBuilder()
            //.AddText("Moved maxmimzed desktops to virtual desktops")
            //.AddText("desktop Count" + initcount.ToString() + " handles count " + windowDesktopMap.Count.ToString())
            //.Show();

            //VirtualDesktop.Destroyed += onVDDestroy;

            #endregion
        }

        void PropertiesClick(object sender, EventArgs e)
        {
            // Hide tray icon, otherwise it will remain shown until user mouses over it
            this.notify_icon.Visible = false;

            Application.Exit();
        }

        void Exit(object sender, EventArgs e)
        {
            // Hide tray icon, otherwise it will remain shown until user mouses over it
            this.notify_icon.Visible = false;

            Application.Exit();
        }

        //void Exit(object sender, EventArgs e)
        //{
        //    // We must manually tidy up and remove the icon before we exit.
        //    // Otherwise it will be left behind until the user mouses over.
        //    notifyIcon.Visible = false;
        //    Application.Exit();
        //}
    }


}
