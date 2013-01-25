using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using ProjectLifeInsights.Models;
using System.Text;

namespace ProjectLifeInsights
{
    public partial class CollecterContext : Form
    {
        /// <summary>
        /// Returns a pointer to the active foreground window
        /// </summary>
        /// <returns></returns>
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        // The GetAsyncKeyState function determines
        // whether a key is up or down at the time 
        // the function is called, and whether
        // the key was pressed after a previous call 
        // to GetAsyncKeyState.
        // "vKey" Specifies one of 256 possible virtual-key codes. 
        // If the function succeeds, the return value specifies whether the key 
        // was pressed since the last call
        // to GetAsyncKeyState, and whether the key is 
        // currently up or down.
        // If the most significant bit is set, the key is down, 
        // and if the least significant bit is set, the key was pressed after 
        // the previous call to GetAsyncKeyState. 

        [DllImport("User32.dll")]
        private static extern short GetAsyncKeyState(
            System.Windows.Forms.Keys vKey); // Keys enumeration

        [DllImport("User32.dll")]
        private static extern short GetAsyncKeyState(
            System.Int32 vKey);

        protected StringBuilder keyBuffer;
        protected System.Timers.Timer timerKeyMine;
        protected System.Timers.Timer timerBufferFlush;


        protected NotifyIcon notifyIcon;
        protected ManualResetEventSlim exitEvent;
        protected TimeSpan executionDelay, exitDelay;
        protected Thread collectionThread;
        protected ProcessInfo previousInfo;
        private MenuItem pauseMenuItem, exitMenuItem;

        /// <summary>
        /// Gets the current active window title
        /// </summary>
        public static String GetActiveWindowTitle()
        {
            return ProcessInfo.From(GetForegroundWindow()).Title;
        }
        
        /// <summary>
        /// Gets the active process Information
        /// </summary>
        /// <returns></returns>
        public static String GetActiveProcessInformation()
        {
            return ProcessInfo.From(GetForegroundWindow()).Process.MainModule.ModuleName;
        }

        /// <summary>
        /// 
        /// </summary>
        public CollecterContext()
        {
            InitializeComponent();

            exitMenuItem = new MenuItem("Exit", OnExitClick);
            pauseMenuItem = new MenuItem("Pause", OnPauseClick);

            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = ProjectLifeInsights.Properties.Resources.TrayIcon;
            notifyIcon.ContextMenu = new ContextMenu(new MenuItem[] { pauseMenuItem, exitMenuItem });
            notifyIcon.Visible = true;
            pauseMenuItem.Checked = false;

            executionDelay = TimeSpan.FromSeconds(1);
            exitDelay = TimeSpan.FromSeconds(10);
            exitEvent = new ManualResetEventSlim(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void OnExitClick(Object sender, EventArgs args)
        {
            notifyIcon.Visible = false;
            
            exitEvent.Set();
            collectionThread.Join(exitDelay);

            Application.Exit();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void OnPauseClick(Object sender, EventArgs args) 
        {
            pauseMenuItem.Checked = !pauseMenuItem.Checked;
            timerKeyMine.Enabled = !pauseMenuItem.Checked;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            collectionThread = new Thread(RunService);
            collectionThread.Start();
        }

        /// <summary>
        /// Service loop
        /// </summary>
        private void RunService()
        {
            // Starts the service log
            ServiceLog.Start();

            keyBuffer = new StringBuilder();
            timerKeyMine = new System.Timers.Timer();
            timerKeyMine.Enabled = true;
            timerKeyMine.Elapsed += new System.Timers.ElapsedEventHandler(this.timerKeyMine_Elapsed);
            timerKeyMine.Interval = 10;
            
            timerBufferFlush = new System.Timers.Timer();
            timerBufferFlush.Enabled = true;
            timerBufferFlush.Elapsed += new System.Timers.ElapsedEventHandler(this.timerBufferFlush_Elapsed);
            timerBufferFlush.Interval = 30 * 60 * 1000; // 30 minutes

            while (true)
            {
                try
                {
                    // Exit when requested
                    if (exitEvent.Wait(executionDelay))
                        break;

                    // Execute
                    Environment.ExitCode = Execute();
                }
                catch (Exception e)
                {
                    // Save errors
                    var error = ServiceLog.Error(String.Format("An error of the type {0} occurred: {1}", e.GetType().Name, e.Message));
                    notifyIcon.ShowBalloonTip(1500, "RunService Exception", error.Message, ToolTipIcon.Error);
                }
            }

            timerKeyMine.Stop();
            timerBufferFlush.Stop();
            timerBufferFlush_Elapsed(this, null);

            // Stop the service log
            ServiceLog.Stop();
        }

        /// <summary>
        /// Executes in context and returns an ExitCode 
        /// </summary>
        /// <returns></returns>
        public Int32 Execute()
        {
            try
            {
                // Gets process info
                ProcessInfo info = ProcessInfo.From(GetForegroundWindow());
                if (!info.Equals(previousInfo))
                {
                    // Saves the info
                    info.Save();
                    previousInfo = info;
                }
            }
            catch (Exception e)
            {
                // Save errors
                var error = ServiceLog.Error(String.Format("An error of the type {0} occurred: {1}", e.GetType().Name, e.Message));
                notifyIcon.ShowBalloonTip(1500, "Excecute Exception", error.Message, ToolTipIcon.Info);
            }

            // Error code
            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void timerKeyMine_Elapsed(object sender,
                        System.Timers.ElapsedEventArgs e)
        {
            foreach (System.Int32 i in Enum.GetValues(typeof(Keys)))
            {
                if (GetAsyncKeyState(i) == -32767)
                {
                    var key = (Keys)i;
                    if (key == Keys.LShiftKey || key == Keys.RShiftKey || key == Keys.Shift || key == Keys.ShiftKey)
                        keyBuffer.Append(key.ToString().ToUpper());
                    else
                        keyBuffer.Append(key.ToString().ToLower());
                    keyBuffer.Append(" ");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void timerBufferFlush_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            KeyLog.Flush(keyBuffer.ToString());
            keyBuffer.Clear();
        }

    }
}
