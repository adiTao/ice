using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo
{
    /// <summary>
    /// 共用任務入口
    /// </summary>
    class TaskUtils
    {
        public static AsyncTaskQueue TaskQueue;

        public static async void AddTask(Task task)
        {
            var result = await TaskQueue.Run(() => task);
        }
    }
}
