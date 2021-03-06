﻿using Hucares.Server.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HucaresWF
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var mainForm = new HucaresForm(new CameraInfoClient(), new MissingPlateClient(), new DetectedPlateClient());
            mainForm.ExceptionEventHandler += new ExceptionEventMessageBoxObserver().HandleExceptionEvent;

            Application.Run(mainForm);
        }
    }
}
