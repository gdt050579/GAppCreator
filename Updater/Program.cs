using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using GAppCreator;

namespace GAppCreator
{
    static class Program
    {
        static void ShowError(ErrorsContainer ec)
        {
            if (ec.HasErrors())
            {
                string s = "";
                for (int tr = 0; tr < ec.GetCount(); tr++)
                {
                    ErrorsContainer.ErrorInfo ei = ec.Get(tr);
                    s += ei.Error + "\n";
                    if (ei.Exception != null)
                        s += ei.Exception + "\n";
                }
                s = s.Trim();
                MessageBox.Show(s);
            }
        }
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // args[1] = parola
            string[] args = Environment.GetCommandLineArgs();
            string app_path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if ((Path.GetFileName(app_path).ToLower() != "update") || (args.Length!=2))
            {
                MessageBox.Show("This aplication should only be executed from the GACCreator framework !");
                return;
            }
            GZipPackage zp = new GZipPackage();
            ErrorsContainer ec = new ErrorsContainer();
            int count = 0;
            for (int tr = 0; tr < 3; tr++)
            {
                Process[] p = Process.GetProcesses();
                foreach (Process proc in p)
                {
                    string pname;
                    try { pname = proc.ProcessName.ToLower(); }
                    catch (Exception) { pname = ""; }
                    if (pname == "gappcreator")
                        count++;
                }
                if (count == 0)
                    break;
                System.Threading.Thread.Sleep(5000); // 5 secunde
            }
            if (count > 0)
            {
                MessageBox.Show("GAppCreator is still running. Please stop all instances of GAppCreator except one and try the update again.");
                return;
            }
            if (zp.Uncompress(args[1], Path.Combine(app_path, "update.dat"), Path.GetDirectoryName(app_path), ec) == false)
            {
                ShowError(ec);
                return;
            }
            // totul e ok - rulez 
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.Arguments = "";
                psi.FileName = Path.Combine(Path.GetDirectoryName(app_path), "GAppCreator.exe");
                psi.WorkingDirectory = Path.GetDirectoryName(app_path);
                Process p = Process.Start(psi);
                if (p == null)
                {
                    MessageBox.Show("Unable to start: " + psi.FileName);
                    return;
                }
            }
            catch (Exception e)
            {
                ec.AddException("Unable to start process: " + Path.Combine(Path.GetDirectoryName(app_path), "GAppCreator.exe"),e);
                ShowError(ec);
            }

        }
    }
}
