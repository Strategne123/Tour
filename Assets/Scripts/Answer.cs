using UnityEngine;

public enum AnswerType
{
    SwitchVideo,
    ContinueVideo,
}

public class Answer : MonoBehaviour
{
    [SerializeField] private GameObject answerObject;
    [SerializeField] public AnswerType answerType;
    

    [HideInInspector] public Question parentQuestion;
    public bool isCorrect;
    public int correctNum = -1;


    public void ResponseProcess(Zones zone)
    {
        if (!isCorrect)
        {
            zone.MakeMistake();
            gameObject.SetActive(false);
            return;
        }
        parentQuestion.correctAnswers--;
        if (parentQuestion.correctAnswers > 0 && !parentQuestion.HasLinked)
        {
            gameObject.SetActive(false);
            return;
        }
        parentQuestion.questionTextUI.text = "";
        zone.TrueAnswer(correctNum);
        if (answerType == AnswerType.ContinueVideo)
        {
            zone.NextQuestion();
        }
        else
        {
            zone.NextStage();
        }
    }
}
