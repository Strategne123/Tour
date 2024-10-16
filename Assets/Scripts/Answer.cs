using TMPro;
using UnityEngine;

public enum AnswerType
{
    SwitchVideo,
    ContinueVideo,
}

public class Answer : MonoBehaviour
{
    [SerializeField] private string answerText;
    [SerializeField] private GameObject answerObject;
    [SerializeField] public AnswerType answerType;
    [SerializeField] private int numNextVideo;

    [HideInInspector] public Question parentQuestion;
    public bool isCorrect;

    private void Start()
    {
        GetComponentInChildren<TMP_Text>().text = answerText;
    }

    public void ResponseProcess(Zones zone)
    {
        if (!isCorrect)
        {
            zone.MakeMistake();
            gameObject.SetActive(false);
            return;
        }
        parentQuestion.correctAnswers--;
        if (parentQuestion.correctAnswers > 0)
        {
            gameObject.SetActive(false);
            return;
        }
        parentQuestion.questionTextUI.text = "";
        zone.TrueAnswer();
        if (answerType == AnswerType.ContinueVideo)
        {
            zone.NextQuestion();
        }
        else
        {
            zone.SetStage(numNextVideo);
        }
    }
}
