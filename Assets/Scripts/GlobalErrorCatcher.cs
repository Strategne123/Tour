using UnityEngine;
using System;

public class GlobalErrorCatcher : MonoBehaviour
{
    private static GlobalErrorCatcher _instance;

    private void Awake()
    {
        // ���������, ���� ��� ���� ��������� GlobalErrorCatcher � �����
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject); // ������ ������ ���������� ����� �������
        }
        else
        {
            Destroy(gameObject); // ������� ����������� ������
            return;
        }

        // ������������� �� ���������� ������� ��� ��������� ������
        Application.logMessageReceived += HandleLog;
    }

    // ����� ��� ��������� ���������� ������ � �����
    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Exception || type == LogType.Error)
        {
            Debug.LogError($"[GlobalErrorCatcher] Error: {logString}\nStack Trace: {stackTrace}");
            // ������ �������� ����� ������ ��� ������ ������ � ����
        }
    }

    private void OnDestroy()
    {
        Application.logMessageReceived -= HandleLog; // ������������ �� ������� ��� ����������� �������
    }
}
