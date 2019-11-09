using UnityEngine;
using System.Collections;

public class _logs_printer : MonoBehaviour
{
    string s_logs;
    Queue Queue_logs = new Queue();


    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string s_log_string, string s_stack_trace, LogType type)
    {
        s_logs = s_log_string;
        string s_new_string = "\n [" + type + "] : " + s_logs;
        Queue_logs.Enqueue(s_new_string);
        if (type == LogType.Exception)
        {
            s_new_string = "\n" + s_stack_trace;
            Queue_logs.Enqueue(s_new_string);
        }
        s_logs = string.Empty;
        foreach (string s_log in Queue_logs)
        {
            s_logs += s_log;
        }
    }

    void OnGUI()
    {
        GUILayout.Label(s_logs);
    }
}