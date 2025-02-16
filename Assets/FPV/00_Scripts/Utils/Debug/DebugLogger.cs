using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class DebugLogger : MonoBehaviour
{
    [SerializeField] private bool useJobSystem;

    private void Update()
    {
        var startTime = Time.realtimeSinceStartup;

        if (useJobSystem)
        {
            var jobHandles = new NativeList<JobHandle>(Allocator.Temp);
            for (var i = 0; i < 100; i++)
            {
                var job = HardTaskJob();
                jobHandles.Add(job);
            }

            JobHandle.CompleteAll(jobHandles);
            jobHandles.Dispose();
        }
        else
        {
            for (var i = 0; i < 100; i++) HardTask();
        }

        Debug.Log((Time.realtimeSinceStartup - startTime) * 1000 + "ms");
    }

    private void HardTask()
    {
        float value = 0;
        for (var i = 0; i < 1000000; i++) value += i;
    }

    private JobHandle HardTaskJob()
    {
        var job = new HardTask();
        return job.Schedule();
    }
}

public struct HardTask : IJob
{
    public void Execute()
    {
        float value = 0;
        for (var i = 0; i < 1000000; i++) value += i;
    }
}