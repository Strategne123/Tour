using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;


public class Question : MonoBehaviour
{
    [SerializeField] private string questionText;
    public TMP_Text questionTextUI;
    [SerializeField] private List<Answer> answers = new List<Answer>();

    [HideInInspector] public int correctAnswers = 0;

    public float timeToAppear;

    private void Awake()
    {
        answers.Clear();
        answers = GetComponentsInChildren<Answer>().ToList<Answer>();
        
        foreach (var answer in answers)
        {
            answer.parentQuestion = this;
            if(answer.isCorrect)
            {
                correctAnswers++;
            }
        }
    }

    public void SetQuestionText()
    {
        questionTextUI.text = questionText;
    }

    
}


