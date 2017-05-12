using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Threading;
using log4net;
using Smellyriver.Utilities;
using System.Management;

namespace Smellyriver.TankInspector
{
	internal static class Program
    {
        private static readonly ILog Log = SafeLog.GetLogger(MethodBase.GetCurrentMethod().DeclaringType.Name);

	    private const string DumpDirectory = "dump";

	    private static bool _hasDumped = false;

	    private static readonly Type[] IgnoredFirstChanceExceptions = 
        {
            typeof( AmbiguousMatchException ),
        };

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
        public static void InitializeUnhandledExceptionHandler()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;

            currentDomain.FirstChanceException += Program.FirstChanceExceptionHandler;

            if (Debugger.IsAttached)
            {
                return; //do nothing when debugger attached
            }
            
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(Program.OnUnhandledExceptionThrown);

            Application.Current.DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(Program.OnDispatcherUnhandledExceptionThrown);

            if (!Directory.Exists(DumpDirectory))
            {
                Directory.CreateDirectory(DumpDirectory);
            }
        }

        private static readonly string[] Hardwares = 
        {
          "Win32_OperatingSystem",
          "Win32_Processor",
          "Win32_PhysicalMemory",
          "Win32_VideoController",     
        };



        public static void LogHardwareInfo()
        {
            try
            {
                foreach (var hardware in Hardwares)
                {
                    var searcher = new ManagementObjectSearcher(
                        "SELECT * FROM " + hardware);

                    var hardwareLog = SafeLog.GetLogger(hardware);

                    foreach (ManagementObject wmiHd in searcher.Get())
                    {
                        
                        PropertyDataCollection
                          searcherProperties = wmiHd.Properties;
                        foreach(PropertyData sp in searcherProperties)
                        {
                            hardwareLog.Info(sp.Name + ' ' + sp.Value);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("logging hardware info", ex);
            }
        }

        private static void FirstChanceExceptionHandler(object sender, FirstChanceExceptionEventArgs e)
        {
            var isInPotentialExceptionRegion = Diagnostics.IsInPotentialExceptionRegion;
            using (Diagnostics.PotentialExceptionRegion)
            {
                try
                {
                    if (!isInPotentialExceptionRegion && !IgnoredFirstChanceExceptions.Any((type) => e.Exception.GetType() == type))
                    {
                        Log.Error("an first chance exception occurred: ", e.Exception);
                        if (!Debugger.IsAttached)
                            Program.DumpProcess(true);
                    }
                }
                catch (Exception)
                {
                    //don't do anything, let'em go.
                }
            }
        }

        private static void OnDispatcherUnhandledExceptionThrown(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            if (_hasDumped) return;
            var dumpFileName = Program.DumpProcess();

            Log.Fatal("an unhandled exception occurre in the user interface : ", e.Exception);

            var trace = new StackTrace(e.Exception);
            MessageBox.Show(
	            $"An unhandled exception occurred in the user interface, application will be terminated.\nException information :\n {e.Exception.ToInformationString()}\nStackTrace :\n {trace.ToString()}\nCheck {dumpFileName} for more information.");
            Application.Current.Shutdown();
        }

        private static void OnUnhandledExceptionThrown(object sender, UnhandledExceptionEventArgs e)
        {
            if (_hasDumped) return;
            var dumpFileName = Program.DumpProcess();

            if (e.ExceptionObject is Exception)
            {
                Log.Fatal("an unhandled exception occurre :", (Exception)e.ExceptionObject);
            }

            MessageBox.Show(
	            $"An unhandled exception occurred, application will be terminated.\nCheck {dumpFileName} for more information.");
            Application.Current.Shutdown();
        }

        private static string DumpProcess(bool isFirstChance = false)
        {
            var time = DateTime.Now;

            string fileName;

            if(isFirstChance)
                fileName = Path.Combine(DumpDirectory, "tank_inspector_previous_first_chance.dmp");
            else
                fileName = Path.Combine(DumpDirectory, $"tank_inspector_{time.ToString("yyyy_MM_dd_HH_mm_ss")}.dmp");
                
            using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Write))
            {
                MiniDump.Write(fs.SafeFileHandle, MiniDump.Option.Normal);
            }
            _hasDumped = true;
            return fileName;
        }
    }
}
