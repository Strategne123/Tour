using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vrs.Internal;

public class Stage : MonoBehaviour
{
    public string videoCaption;
    [SerializeField] private float startAngle;
    [SerializeField] private List<Question> questions = new List<Question>();

    private void Awake()
    {
        try { 
        questions.Clear();
        questions = GetComponentsInChildren<Question>().ToList<Question>();
        gameObject.SetActive(false);
        }
        catch (Exception e)
        {
            FPScounter.Print(e.ToString());
        }
    }

    private void Start()
    {
        try { 
        if(questions.Count > 1)
        {
            for(int i = 1; i < questions.Count; i++)
            {
                questions[i].gameObject.SetActive(false);
            }
        }
        }
        catch (Exception e)
        {
            FPScounter.Print(e.ToString());
        }
    }

    public int QuestionCount()
    {
        return questions.Count;
    }

    public void ShowQuestion(int numberQuestion)
    {
        questions[numberQuestion].gameObject.SetActive(true);
        questions[numberQuestion].SetQuestionText(false);
    }

    public void ShowAnswer(int numberQuestion)
    {
        questions[numberQuestion].gameObject.SetActive(true);
        questions[numberQuestion].SetQuestionText(true);
    }

    public void HideQuestion(int numberQuestion)
    {
        questions[numberQuestion].gameObject.SetActive(false);
    }

    public void HideAnswer(int numberAnswer)
    {
        questions[0].HideAnswer(numberAnswer);
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

    public bool HasLinkedQuestion()
    {
        return questions[0].HasLinked;
    }

    public Question GetQuestionAt(int index) => questions[index];
}
