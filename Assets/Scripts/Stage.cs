using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Stage : MonoBehaviour
{
    public string videoCaption;
    [SerializeField] private float startAngle;
    [SerializeField] private List<Question> questions = new List<Question>();

    public int QuestionCount()
    {
        return questions.Count;
    }

    public void ShowQuestion(int numberQuestion)
    {
        questions[numberQuestion].gameObject.SetActive(true);
    }

    public void HideQuestion(ref int numberQuestion)
    {
        questions[numberQuestion++].gameObject.SetActive(true);
    }

    public float GetNextQuestionTime(int numberQuestion)
    {
        return questions[numberQuestion].timeToAppear;
    }

    public float GetStartAngle()
    {
        return startAngle;
    }
}
