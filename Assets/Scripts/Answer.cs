using UnityEngine;

public enum AnswerType
{
    SwitchVideo,
    ContinueVideo,
}

public class Answer : MonoBehaviour
{
    [SerializeField] private GameObject answerObject;
    [SerializeField] private AnswerType answerType;

    public bool isCorrect;

    public void ResponseProcess(Zones zone)
    {
        if (!isCorrect)
        {
            zone.MakeMistake();
            return;
        }
        zone.TrueAnswer();
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
