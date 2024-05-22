using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;


public class Question : MonoBehaviour
{
    [SerializeField] private string questionText;
    public TMP_Text questionTextUI;
    [SerializeField] private List<Answer> answers = new List<Answer>();
    [SerializeField] private bool isLinked = false;

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

    public void SetQuestionText(bool isStudy)
    {
        questionTextUI.text = questionText;
        foreach (var answer in answers)
        {
            //answer.gameObject.SetActive(true);
            if(isStudy && !answer.isCorrect)
            {
                answer.gameObject.SetActive(false);
            }
        }
    }

    public void HideAnswer(int answerIndex)
    {
        foreach (var answer in answers)
        {
            if(answer.correctNum==answerIndex)
            {
                answer.gameObject.SetActive(false);
                return;
            }
        }
    }

    public int GetIndexByAnswer(Answer answer) => answers.IndexOf(answer);

    public Answer GetAnswerByIndex(int index) => answers[index];

    public bool HasLinked { get { return isLinked; } private set { } }
}


