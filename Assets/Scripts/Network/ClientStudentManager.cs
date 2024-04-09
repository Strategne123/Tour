using System;
using Mirror;
using UnityEngine;


public class ClientStudentManager : Singleton<ClientStudentManager>
{
    public Action<string> OnStudentNameChanged;
    
    public Student Student;

    public int id;

    public void CreateStudent()
    {
        gameObject.SetActive(true);
        Student = new Student(String.Empty, null);
        NetworkClient.RegisterHandler<StudentNameMessage>(SetName);
    }

    public void SetNetworkConnection(NetworkConnection connection)
    {
        Student.Connection = connection;
    }
    
    private void SetName(StudentNameMessage msg)
    {
        Student.Name = msg.name;
        OnStudentNameChanged?.Invoke(Student.Name);
    }
}
