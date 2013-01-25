using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectLifeInsights.MVC;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace ProjectLifeInsights.Models
{
    using SystemProcess = Process;
    using ProjectLifeInsights.Services;
    using MongoDB.Bson.Serialization.Attributes;

    /// <summary>
    /// 
    /// </summary>
    public class ProcessInfo : Model
    {
        protected Process _process;

        /// <summary>
        /// 
        /// </summary>
        public String Title { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        [BsonIgnore]
        public SystemProcess Process
        {
            get
            {
                return _process;
            }
            set 
            {
                _process = value;

                this.ProcessName = value.ProcessName;
                this.ModuleName = value.MainModule.ModuleName;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public String ProcessName { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public String ModuleName { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="text"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern Int32 GetWindowText(IntPtr hWnd, StringBuilder text, Int32 count);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="pid"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out UInt32 pid);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="intPtr"></param>
        /// <returns></returns>
        internal static ProcessInfo From(IntPtr handle)
        {
            var result = new ProcessInfo();

            try
            {
                UInt32 pid;
                GetWindowThreadProcessId(handle, out pid);
                result.Process = Process.GetProcessById((int)pid);
            }
            catch (Exception)
            {
                
            }

            Int32 chars = 256;
            StringBuilder buff = new StringBuilder(chars);

            if (handle == IntPtr.Zero)
                result.Title = "No active window";
            else if (GetWindowText(handle, buff, chars) > 0) 
                result.Title = buff.ToString();
            else
                result.Title = String.Empty;
            

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override Boolean Equals(object obj)
        {
            var other = obj as ProcessInfo;
            if (other == null)
                return false;

            return other.ProcessName == this.ProcessName && other.Title == this.Title;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return (this.ProcessName.GetHashCode() ^ 127) * (this.Title.GetHashCode() ^ 255);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override MongoDB.Driver.MongoCollection GetCollection()
        {
            return MongoService.Instance.Database.GetCollection<ProcessInfo>("Process.Log");
        }
    }
}
