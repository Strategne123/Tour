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
    private int allAnswers = 0,currentAnswers = 0;
    private float nextQuestionTime = 100;
    private string videoFolderPath;
    private bool isLastAnswer = false;
    private bool haveDoneMistake = false;

    private void Start()
    {
        currentStageIndex = 0;
        currentQuestionIndex = 0;
        
        PlayVideo();
    }

    private void PlayVideo()
    {
#if UNITY_EDITOR
        videoFolderPath = Application.dataPath+"/";
#else
        videoFolderPath = "storage/emulated/0/TigerVideos/";
#endif

        var videoPath = videoFolderPath + stages[currentStageIndex].videoCaption;
        mediaPlayer.OpenMedia(new MediaPath(videoPath, MediaPathType.AbsolutePathOrURL));
        nextQuestionTime = stages[currentStageIndex].GetNextQuestionTime(currentQuestionIndex);
        var newQuaternion = mediaPlayer.transform.localRotation;
        newQuaternion.y = stages[currentStageIndex].GetStartAngle();
        mediaPlayer.transform.localRotation = newQuaternion;
        //mediaPlayer.Control.Seek(startTime);
        CountAnswers();
        mediaPlayer.Play();
    }


    private void PauseVideo()
    {
        mediaPlayer.Pause();
        stages[currentStageIndex].gameObject.SetActive(true);
        stages[currentStageIndex].ShowQuestion(currentQuestionIndex);
    }

    private void CountAnswers()
    {
        if (allAnswers == 0)
        {
            foreach (var stage in stages)
            {
                allAnswers += stage.GetQuestionsCount();
            }
        }
        allQuestionsText.text = "Ответов: " + currentAnswers + "/" + allAnswers;
    }

    private void ReturnVideo()
    {
        CountAnswers();
        mediaPlayer.Play();
    }


    public void ChooseAnswer(Answer answer)
    {
        print("Выбран ответ"+answer.gameObject.name);
        answer.ResponseProcess(this);
    }


    public void MakeMistake()
    {
        if (!haveDoneMistake)
        {
            wrongAnswers++;
            wrongAnswersText.text = "\nОшибок: " + wrongAnswers;
            haveDoneMistake = true;
        }
    }

    public void TrueAnswer()
    {
        currentAnswers++;
    }


    public void NextQuestion()
    {
        if (currentQuestionIndex < stages[currentStageIndex].QuestionCount() - 1)
        {
            stages[currentStageIndex].HideQuestion(currentQuestionIndex);
            currentQuestionIndex++;
            nextQuestionTime = stages[currentStageIndex].GetNextQuestionTime(currentQuestionIndex);

        }
        else
        {
            isLastAnswer = true;
        }
        stages[currentStageIndex].gameObject.SetActive(false);
        haveDoneMistake = false;
        ReturnVideo();
    }


    public void NextStage()
    {
        if (currentStageIndex < stages.Count - 1)
        {
            currentStageIndex++;
            currentQuestionIndex = 0;
            haveDoneMistake = false;
            isLastAnswer = false;
            PlayVideo();
        }
        else
        {
            Debug.Log("Выход в меню");
        }
    }


    private void Update()
    {
        if (!mediaPlayer.Control.IsPaused() && mediaPlayer.Control.GetCurrentTime() >= nextQuestionTime && !isLastAnswer)
        {
            PauseVideo();
        }
        if(mediaPlayer.Control.GetCurrentTime()==mediaPlayer.Info.GetDuration() && mediaPlayer.Info.GetDuration() > 0)
        {
            NextStage();
        }
    }

    public void Quit()
    {
        Application.Quit();
    }
}

