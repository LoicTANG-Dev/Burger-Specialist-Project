using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using BugerSpecialistAPI.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MockFunction
{
    [TestClass]
    public class MockFunctionClass
    {
        [TestMethod]
        public void TestSaveFile()
        {
            Action act = TestMethod;

            var canExit = false;
            act.BeginInvoke((IAsyncResult) => { canExit = true; }, null);

            while(!canExit)
            {
                Thread.Sleep(500);
            }
        }

        private void TestMethod()
        {
            var manager = WorkItemManager.Instance;
            byte[] filecontent = File.ReadAllBytes(@"../../../Test/Input/TestFile.txt");

            var e1 = new ManualResetEvent(false);
            e1.Reset();
            var thread1 = new Thread(() =>
            {
                try
                {
                    var workQueue1 = new Queue<string>();
                    workQueue1.Enqueue("SaveFile");
                    string fileSaveLocation = string.Format(@"../../../Test/Output/{0}/TestFile.txt", "User1");
                    WorkItemManager.Instance.DoWork(new Tuple<Queue<string>, object[]>(workQueue1, new object[] { fileSaveLocation, filecontent }));
                }
                finally
                {
                    e1.Set();
                }
            });

            var e2 = new ManualResetEvent(false);
            e2.Reset();
            var thread2 = new Thread(() =>
            {
                try
                {
                    var workQueue1 = new Queue<string>();
                    workQueue1.Enqueue("SaveFile");
                    string fileSaveLocation = string.Format(@"../../../Test/Output/{0}/TestFile.txt", "User2");
                    WorkItemManager.Instance.DoWork(new Tuple<Queue<string>, object[]>(workQueue1, new object[] { fileSaveLocation, filecontent }));
                }
                finally
                {
                    e2.Set();
                }
            });

            var e3 = new ManualResetEvent(false);
            e3.Reset();
            var thread3 = new Thread(() =>
            {
                try
                {
                    var workQueue1 = new Queue<string>();
                    workQueue1.Enqueue("SaveFile");
                    string fileSaveLocation = string.Format(@"../../../Test/Output/{0}/TestFile.txt", "User3");
                    WorkItemManager.Instance.DoWork(new Tuple<Queue<string>, object[]>(workQueue1, new object[] { fileSaveLocation, filecontent }));
                }
                finally
                {
                    e3.Set();
                }
            });

            thread1.Start();
            thread2.Start();
            thread3.Start();

            WaitHandle.WaitAll(new WaitHandle[] { e1, e2, e3 });
        }
    }
}
