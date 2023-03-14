using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace BugerSpecialistAPI.Models
{
    public interface IWorkItem
    {
        void DoWork(Tuple<Queue<string>, object[]> input, out Tuple<Queue<string>, object[]> output, Action callback = null);
    }

    public abstract class WorkItem : IWorkItem
    {
        public void DoWork(Tuple<Queue<string>, object[]> input, out Tuple<Queue<string>, object[]> output, Action callback)
        {
            var methodInfo = input.Item1.Dequeue();

            var method = this.GetType().GetMethod(methodInfo);
            var methodResult = method.Invoke(this, new object[] { input.Item2 });

            output = new Tuple<Queue<string>, object[]>(input.Item1, new object[] { methodResult });
            if (callback != null)
            {
                callback();
            }
        }
    }

    public class CommentWorkItem: WorkItem
    {
        public bool SaveFile(object[] input)
        {
            Thread.Sleep(5000);
            var fileName = input[0] as string;
            byte[] content = input[1] as byte[];
            File.WriteAllBytes(fileName, content);
            return true;
        }
    }

    public class WorkItemManager
    {
        private Queue<IWorkItem> _freeWorkItemCollection;
        private static object _lockObject = new object();

        private static WorkItemManager _workItemManager = null;
        public static WorkItemManager Instance
        {
            get
            {
                lock (_lockObject)
                {
                    if (_workItemManager == null)
                    {
                        _workItemManager = new WorkItemManager();
                    }
                    return _workItemManager;
                }
            }
        }

        private WorkItemManager()
        {
            _freeWorkItemCollection = new Queue<IWorkItem>();

            _freeWorkItemCollection.Enqueue(new CommentWorkItem());
            _freeWorkItemCollection.Enqueue(new CommentWorkItem());
            _freeWorkItemCollection.Enqueue(new CommentWorkItem());
            _freeWorkItemCollection.Enqueue(new CommentWorkItem());
        }

        private bool TryDequeueFreeWorkItem(out IWorkItem item)
        {
            var result = false;
            item = null;
            lock (_lockObject)
            {
                try
                {
                    item = _freeWorkItemCollection.Dequeue() as IWorkItem;
                    result = true;
                }
                catch (Exception ex)
                {
                    result = false;
                }
            }
            return result;
        }

        private void EnQuqueFreeWorkItemCollection(IWorkItem item)
        {
            lock (_lockObject)
            {
                _freeWorkItemCollection.Enqueue(item);
            }
        }

        public object DoWork(Tuple<Queue<string>, object[]> input)
        {
            IWorkItem item = null;
            var tryCount = 0;
            while (tryCount < 10)
            {
                if (TryDequeueFreeWorkItem(out item))
                {
                    break;
                }
                tryCount++;
            }
            if(tryCount>=10)
            {
                throw new Exception("Error");
            }
            Tuple<Queue<string>, object[]> result = null;
            item.DoWork(input, out result, () => { EnQuqueFreeWorkItemCollection(item); });

            return result;
        }
    }
}