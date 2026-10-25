
using System;
using System.Windows.Forms;

namespace com.Brainsoft.NeurPsy.Experiments
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            MatrixUtil.LogInit.Run();
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.

            Application.Run(new frmMain());
        }
    }
}