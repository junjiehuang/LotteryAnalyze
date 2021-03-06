﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace AirStudio
{
    static class Program
    {
        static public MainWindow mainForm = null;

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            //using(FormMain mainForm = new FormMain())
            mainForm = new MainWindow();
            {
                mainForm.Show();
                EngineInit();

                while (mainForm.Created)
                {
                    if (mainForm.WindowState == FormWindowState.Minimized)
                        System.Threading.Thread.Sleep(300);
                    else
                        System.Threading.Thread.Sleep(0);

                    EngineUpdate();
                    Application.DoEvents();
                }
                EngineRelease();
            }
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            Exception error = e.Exception as Exception;
            if (error != null)
            {
                String str = string.Format("ThreadException异常类型：{0}\r\n异常消息：{1}\r\n异常信息：{2}\r\n", error.GetType().Name, error.Message, error.StackTrace);
                if (MessageBox.Show(str + "\n按Retry继续,否则退出", "系统错误", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) == DialogResult.Cancel)
                    Application.Exit();
            }
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception error = e.ExceptionObject as Exception;
            if (error != null)
            {
                String str = string.Format("UnhandledException异常类型：{0}\r\n异常消息：{1}\r\n异常信息：{2}\r\n", error.GetType().Name, error.Message, error.StackTrace);
                if (MessageBox.Show(str + "\n按Retry继续,否则退出", "系统错误", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) == DialogResult.Cancel)
                    Application.Exit();
            }
        }

        static void EngineInit()
        {
            //Console.WriteLine("EngineInit");
            //AIR.EngineEditorWrapper.GetInstance().Init((uint)mainForm.Handle.ToInt32());
        }

        static void EngineUpdate()
        {
            //Console.WriteLine("EngineUpdate");
            //AIR.EngineEditorWrapper.GetInstance().Update();
        }

        static void EngineRelease()
        {
            //Console.WriteLine("EngineRelease");
            //AIR.EngineEditorWrapper.GetInstance().Shutdown();
        }
    }
}
