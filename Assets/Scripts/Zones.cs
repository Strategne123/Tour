using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using RenderHeads.Media.AVProVideo;
using Vrs.Internal;

public class Zones : MonoBehaviour
{
    [SerializeField] private MediaPlayer mediaPlayer;
    [SerializeField] private List<Stage> stages = new List<Stage>();
    [SerializeField] private TMP_Text allQuestionsText;
    [SerializeField] private TMP_Text wrongAnswersText;
    [SerializeField] private TMP_Text mainQuestion;
    //[SerializeField] private GameObject modePanel;
    [SerializeField] private Mode mode;
    [SerializeField] private List<GameObject> correctLayers = new List<GameObject>();
    [SerializeField] private List<GameObject> parentLayers = new List<GameObject>();

    private int currentStageIndex;
    private int currentQuestionIndex;
    private int wrongAnswers = 0;
    private int allAnswers = 0,currentAnswers = 0;
    private float nextQuestionTime = 100;
    private string videoFolderPath;
    private bool isLastAnswer = false, isStarted = false;
    private int checkedAnswer = -1;
    private List<SpecialSequence> specialSequences = new List<SpecialSequence>();
    private int currentSequence = 0;

    

    public Action<AnswerType, bool, int> OnChoosedAnswer;

    private bool haveDoneMistake = false;


    [HideInInspector] public Mode currentMode;

    private void Start()
    {
        currentStageIndex = 0;
        currentQuestionIndex = 0;
        mainQuestion.text = "";
        SelectMode((int)mode);
        SequencesDetection();
    }

    private void SequencesDetection()
    {
        bool flag = false;
        for (var i=0; i<stages.Count;i++)
        {
            if (stages[i].HasLinkedQuestion() != flag)
            {
                flag = !flag;
                if(flag)
                {
                    specialSequences.Add(new SpecialSequence(i));
                }
                else
                {
                    var nextEl = i != stages.Count - 1 ? i + 1 : -1;
                    specialSequences[specialSequences.Count - 1].EndIndex = nextEl;
                }
            }
            else if (stages[i].HasLinkedQuestion())
            {
                specialSequences[specialSequences.Count - 1].Count++;
            }

        }
    }

    /*public void OpenModePanel()
    {
        modePanel.SetActive(true);
        if (mediaPlayer.Control.IsPlaying())
        {
            mediaPlayer.Pause();
        }
    }*/

    public void SelectMode(int selectedMode)
    {
        currentMode = (Mode)selectedMode;
        //modePanel.SetActive(false);
        if (mediaPlayer.Control.GetCurrentTime() < nextQuestionTime && !isLastAnswer)
        {
            if (!isStarted)
            {
                isStarted = true;
                PlayVideo();
            }
            else
            {
                mediaPlayer.Play();
            }
        }
        else if (!isStarted)
        {
            isStarted = true;
            PlayVideo();
        }
        else if(!isLastAnswer)
        {
            PauseVideo();
        }
        else
        {
            mediaPlayer.Play();
        }

    }

    private void PlayVideo()
    {
#if UNITY_EDITOR
        videoFolderPath = Application.dataPath + "/MedVideos/";
#else
videoFolderPath = "storage/emulated/0/MedVideos/";
#endif

        var videoPath = videoFolderPath + stages[currentStageIndex].videoCaption;
        mediaPlayer.OpenMedia(new MediaPath(videoPath, MediaPathType.AbsolutePathOrURL));
        nextQuestionTime = stages[currentStageIndex].GetNextQuestionTime(currentQuestionIndex);
        Vector3 eulerRotation = new Vector3(0, stages[currentStageIndex].GetStartAngle(), 0);
        mediaPlayer.transform.rotation = Quaternion.Euler(eulerRotation);
        CountAnswers();
        mediaPlayer.Play();
    }


    private void PauseVideo()
    {
        mediaPlayer.Pause();
        stages[currentStageIndex].HideQuestion(currentQuestionIndex);
        stages[currentStageIndex].gameObject.SetActive(true);
        if (currentMode == Mode.Study)
        {
            stages[currentStageIndex].ShowAnswer(currentQuestionIndex);
        }
        else
        {
            stages[currentStageIndex].ShowQuestion(currentQuestionIndex);
        }
        if(correctLayers.Count > 0 && currentSequence < correctLayers.Count)
        {
            try
            {
                parentLayers[currentSequence].SetActive(true);
            }
            catch { }
        }
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
        OnChoosedAnswer?.Invoke(answer.answerType, answer.isCorrect, answer.parentQuestion.GetIndexByAnswer(answer));
        answer.ResponseProcess(this);
    }


    public void MakeMistake()
    {
            wrongAnswers++;
            wrongAnswersText.text = "\nОшибок: " + wrongAnswers;
    }

    public void TrueAnswer(int answerNumber = -1)
    {
        currentAnswers++;
        if(answerNumber >=0)
        {
            checkedAnswer = answerNumber;
        }
    }


    public void NextQuestion()
    {
        if (currentQuestionIndex < stages[currentStageIndex].QuestionCount() - 1)
        {
            stages[currentStageIndex].HideQuestion(currentQuestionIndex);
            TransferCorrectLayers();
            currentQuestionIndex++;
            nextQuestionTime = stages[currentStageIndex].GetNextQuestionTime(currentQuestionIndex);
        }
        else
        {
            isLastAnswer = true;
        }
        stages[currentStageIndex].gameObject.SetActive(false);
        ReturnVideo();
    }


    public void NextStage()
    {
        if (currentStageIndex < stages.Count - 1 || IsQuestionHasLinked())
        {
            mediaPlayer.CloseMedia();
            TransferCorrectLayers();
            stages[currentStageIndex].gameObject.SetActive(false);
            //currentStageIndex++;
            SelectNextIndex();
            currentQuestionIndex = 0;
            isLastAnswer = false;
            PlayVideo();
        }
        else
        {
            Debug.Log("Выход в меню");
        }
    }

    private void SelectNextIndex()
    {
        if(!IsQuestionHasLinked())
        {
            currentStageIndex++;
        }
        else
        {
            DisableCorrectAnswers();
            specialSequences[currentSequence].Count--;
            if (specialSequences[currentSequence].Count < 0)
            {
                currentStageIndex = specialSequences[currentSequence++].EndIndex-1;
            }
            else
            {
                currentStageIndex = checkedAnswer+1;
            }
        }
    }

    private bool IsQuestionHasLinked()
    {
        return stages[currentStageIndex].GetQuestionAt(currentQuestionIndex).HasLinked;
    }

    private void TransferCorrectLayers()
    {
        try
        {
            if (IsQuestionHasLinked())
            {
                correctLayers[checkedAnswer].SetActive(true);
                parentLayers[currentSequence].SetActive(false);
            }
            else
            {
                for (var i = 0; i < parentLayers[currentSequence].transform.childCount; i++)
                {
                    parentLayers[currentSequence].transform.GetChild(i).gameObject.SetActive(false);
                }
            }
        }
        catch { }
    }

    private void DisableCorrectAnswers()
    {
        for (var i = specialSequences[currentSequence].StartIndex; i < specialSequences[currentSequence].EndIndex-1; i++)
        {
            stages[i].HideAnswer(checkedAnswer);
        }
    }



    private void Update()
    {
        try
        {
            if (!mediaPlayer.Control.IsPaused() && mediaPlayer.Control.GetCurrentTime() >= nextQuestionTime && !isLastAnswer)
            {
                if(currentSequence<specialSequences.Count && IsQuestionHasLinked())
                {
                    if (specialSequences[currentSequence].Count<=0)
                    {
                        NextStage();
                        return;
                    }
                }
                PauseVideo();
            }
            if (mediaPlayer.Control.IsFinished())
            {
                NextStage();
            }
        }
        catch(Exception e)
        {
            FPScounter.Print(e.ToString());
        }
    }

    public void Quit()
    {
        Application.Quit();
    }


    public Question GetCurrentQuestion() => stages[currentStageIndex].GetQuestionAt(currentQuestionIndex);

    
    public void Restart()
    {
        isLastAnswer = false;
        mediaPlayer.Pause();
        stages[currentStageIndex].HideQuestion(currentQuestionIndex);
        currentAnswers = 0;
        currentQuestionIndex = 0;
        currentStageIndex = 0;
        wrongAnswers = 0;
        wrongAnswersText.text = "\nОшибок: " + wrongAnswers;
        allQuestionsText.text = "Ответов: " + currentAnswers + "/" + (allAnswers-2);
        foreach(var stage in stages)
        {
            stage.gameObject.SetActive(false);
        }
        isStarted = false;
        //OpenModePanel();
        mainQuestion.text = "";
    }
}

public enum Mode
{
    Study,
    Test,
    Exam
}

public class SpecialSequence
{
    public int StartIndex = 0;
    public int EndIndex = 0;
    public int Count = 0;

    public SpecialSequence(int start)
    {
        StartIndex = start;
        EndIndex = start + 1;
    }
}

