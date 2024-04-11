using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Stage : MonoBehaviour
{
    public string videoCaption;
    [SerializeField] private float startAngle;
    [SerializeField] private List<Question> questions = new List<Question>();

    private void Awake()
    {
        questions.Clear();
        questions = GetComponentsInChildren<Question>().ToList<Question>();
        gameObject.SetActive(false);
    }

    public int QuestionCount()
    {
        return questions.Count;
    }

    public void ShowQuestion(int numberQuestion)
    {
        questions[numberQuestion].gameObject.SetActive(true);
    }

    public void HideQuestion(int numberQuestion)
    {
        questions[numberQuestion].gameObject.SetActive(false);
    }

    public float GetNextQuestionTime(int numberQuestion)
    {
        return questions[numberQuestion].timeToAppear;
    }

    public float GetStartAngle()
    {
        return startAngle;
    }

    public int GetQuestionsCount()
    {
        return questions.Count;
    }
}
