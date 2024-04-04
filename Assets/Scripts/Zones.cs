using System.Collections.Generic;
using UnityEngine;
using TMPro;
using RenderHeads.Media.AVProVideo;

public class Zones : MonoBehaviour
{
    [SerializeField] private MediaPlayer mediaPlayer;
    [SerializeField] private List<Stage> stages = new List<Stage>();
    [SerializeField] private TMP_Text allQuestionsText;
    [SerializeField] private TMP_Text wrongAnswersText;

    private int currentStageIndex;
    private int currentQuestionIndex;
    private int wrongAnswers = 0;
    private int allAnswers = 0;
    private float nextQuestionTime = 100;

#if UNITY_EDITOR
    private string videoFolderPath = Application.absoluteURL;
#else
    private string videoFolderPath = "storage/emulated/0/TigerVideos/";]
#endif

    private void Start()
    {
        currentStageIndex = 0;
        currentQuestionIndex = 0;
        PlayVideo();
    }

    private void PlayVideo()
    {
        var videoPath = videoFolderPath + stages[currentStageIndex].videoCaption;
        mediaPlayer.OpenMedia(new MediaPath(videoFolderPath, MediaPathType.AbsolutePathOrURL));
        //mediaPlayer.Control.Seek(startTime);
        mediaPlayer.Play();
    }


    private void PauseVideo()
    {
        mediaPlayer.Pause();
        stages[currentStageIndex].ShowQuestion(currentQuestionIndex);
    }


    private void ReturnVideo()
    {
        mediaPlayer.Play();
    }


    public void ChooseAnswer(Answer answer)
    {
        answer.ResponseProcess(this);
    }


    public void MakeMistake()
    {
        wrongAnswers++;
    }


    public void NextQuestion()
    {
        if (currentQuestionIndex < stages[currentStageIndex].QuestionCount() - 1)
        {
            currentQuestionIndex++;
            nextQuestionTime = stages[currentStageIndex].GetNextQuestionTime(currentQuestionIndex);
            ReturnVideo();
        }
        else
        {
            NextStage();
        }
    }


    public void NextStage()
    {
        if (currentStageIndex < stages.Count - 1)
        {
            currentStageIndex++;
            currentQuestionIndex = 0;
            PlayVideo();
        }
        else
        {
            Debug.Log("Выход в меню");
        }
    }


    void Update()
    {
        if (!mediaPlayer.Control.IsPaused() && mediaPlayer.Control.GetCurrentTime() >= nextQuestionTime)
        {
            PauseVideo();
        }
        if(mediaPlayer.Control.GetCurrentTime()==mediaPlayer.Info.GetDuration())
        {
            NextStage();
        }
    }
}

