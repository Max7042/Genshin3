using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.VisualBasic.ApplicationServices;

namespace GenshinOverlay {
    static class Program {
        [STAThread]
        static void Main() {
            try {
                if(Environment.OSVersion.Version.Major >= 6) { 
                    User32.SetProcessDPIAware();
                }
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                SingleInstanceController Instance = new SingleInstanceController();
                Instance.Run(Environment.GetCommandLineArgs());
            } catch(Exception ex) {
                File.AppendAllText("_Error.txt", $"{DateTime.Now}\n{ex.ToString()}\n\n");
            }
        }
    }

    class SingleInstanceController : WindowsFormsApplicationBase {
        public SingleInstanceController() {
            IsSingleInstance = true;
        }

        protected override void OnCreateMainForm() {
            MainForm = new MainWindow();
            base.OnCreateMainForm();
        }

        public new void Run(string[] args) {
            base.Run(args);
        }
    }
}
