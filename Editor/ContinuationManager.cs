using System;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;

/// <summary>
/// 출처 https://answers.unity.com/questions/221651/yielding-with-www-in-editor.html
/// </summary>
internal static class ContinuationManager
{
    private class Job
    {
        public Job(Func<bool> completed, Action continueWith)
        {
            Completed = completed;
            ContinueWith = continueWith;
        }
        public Func<bool> Completed { get; private set; }
        public Action ContinueWith { get; private set; }
    }

    private static readonly List<Job> jobs = new List<Job>();

    public static void Add(Func<bool> completed, Action continueWith)
    {
        if (!jobs.Any())
        {
            EditorApplication.update -= Update;
            EditorApplication.update += Update;
        }

        jobs.Add(new Job(completed, continueWith));
    }

    private static void Update()
    {
        for (int i = 0; i >= 0; --i)
        {
            var jobIt = jobs[i];
            if (jobIt.Completed())
            {
                jobIt.ContinueWith();
                jobs.RemoveAt(i);
            }
        }
        if (!jobs.Any()) EditorApplication.update -= Update;
    }
}
#endif