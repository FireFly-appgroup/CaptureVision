using CaptureVision.NN;
using System;
using System.Security.Permissions;
using System.Threading;

namespace CaptureVision
{
    class Program
    {
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
        static void Main(string[] args)
        {
            AppDomain currDomain = AppDomain.CurrentDomain;
            currDomain.UnhandledException += currDomain_UnhandledException;
            //Data.SetNewData();
            NeuralNetwork.RunProcessing();
            Console.ReadKey();
        }
        static void currDomain_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            ExceptionHandler(e.Exception, sender);
        }

        static void currDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ExceptionHandler(e.ExceptionObject as Exception, sender);
        }

        static void ExceptionHandler(Exception exception, object sender)
        {
            if (exception == null)
            {
                var unknownEx = new Exception("Unknown exception");
                const string unknown = "Unknown exception";
                Console.WriteLine(unknown);
                ThreadAbort(sender);
                return;
            }
            Console.WriteLine(exception);
            ThreadAbort(sender);
        }

        private static void ThreadAbort(object sender)
        {
            try
            {
                if (!(sender is Thread thread))
                {
                    Console.WriteLine("Thread == null");
                    return;
                }
                thread.Abort();
            }
            catch (ThreadAbortException)
            {
                Console.WriteLine("ThreadAbortException");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}