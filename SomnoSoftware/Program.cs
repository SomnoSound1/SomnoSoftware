using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SomnoSoftware.Control;

namespace SomnoSoftware
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Controller controller = new Controller();
            //Application.Run(new View());
        }
    }
}
