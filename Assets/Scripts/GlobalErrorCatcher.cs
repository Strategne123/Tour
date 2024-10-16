using UnityEngine;
using System;

public class GlobalErrorCatcher : MonoBehaviour
{
    private static GlobalErrorCatcher _instance;

    private void Awake()
    {
        // Проверяем, если уже есть экземпляр GlobalErrorCatcher в сцене
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject); // Делаем объект постоянным между сценами
        }
        else
        {
            Destroy(gameObject); // Удаляем дублирующий объект
            return;
        }

        // Подписываемся на глобальные события для обработки ошибок
        Application.logMessageReceived += HandleLog;
    }

    // Метод для обработки глобальных ошибок и логов
    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Exception || type == LogType.Error)
        {
            Debug.LogError($"[GlobalErrorCatcher] Error: {logString}\nStack Trace: {stackTrace}");
            // Можешь добавить здесь логику для записи ошибок в файл
        }
    }

    private void OnDestroy()
    {
        Application.logMessageReceived -= HandleLog; // Отписываемся от события при уничтожении объекта
    }
}
