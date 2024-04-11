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

    public void ResponseProcess(Zones zone)
    {
        if (!isCorrect)
        {
            zone.MakeMistake();
            return;
        }
        zone.TrueAnswer();
        parentQuestion.correctAnswers--;
        if (parentQuestion.correctAnswers > 0)
        {
            return;
        }
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
