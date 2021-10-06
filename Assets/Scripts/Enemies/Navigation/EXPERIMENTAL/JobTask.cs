using System.Runtime.InteropServices;
using Unity.Jobs;

namespace JobTask_syst
{
    public interface JobTask
    {
        void Execute();
    }

    public struct JobWrap : IJob
    {
        public GCHandle Handle;
        public void Execute()
        {
            JobTask task = (JobTask)Handle.Target;
            task.Execute();
        }
    }
}
