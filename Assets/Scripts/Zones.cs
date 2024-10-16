using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using RenderHeads.Media.AVProVideo;
using System.Collections;
using UnityEngine.Networking;

public class Zones : MonoBehaviour
{
    [Header("Basic Settings")]
    [SerializeField] private string folderCaption;
    [SerializeField] private bool hasStudyRegime;

    [Header("Links")]
    [SerializeField] private MediaPlayer mediaPlayer;
    [SerializeField] private TMP_Text allQuestionsText;
    [SerializeField] private TMP_Text wrongAnswersText;
    [SerializeField] private TMP_Text mainQuestion;
    [SerializeField] private GameObject modePanel;
    [SerializeField] private List<Texture2D> preloadedTextures = new List<Texture2D>();

    private List<Stage> stages = new List<Stage>(); 
    private int currentStageIndex;
    private int currentQuestionIndex;
    private int wrongAnswers = 0;
    private int allAnswers = 0, currentAnswers = 0;
    private float nextQuestionTime = 100;
    private string videoFolderPath;
    private bool isLastAnswer = false, isStarted = false;
    private bool haveDoneMistake = false;

    public Action<AnswerType, bool, int> OnChoosedAnswer;
    [HideInInspector] public Mode currentMode;

    private void Start()
    {
        currentStageIndex = 0;
        currentQuestionIndex = 0;
        mainQuestion.text = "";

        for (var i = 0; i < transform.childCount; i++)
        {
            var stage = transform.GetChild(i).GetComponent<Stage>();
            if (stage != null)
            {
                stages.Add(stage);
            }
        }
        if (!hasStudyRegime)
        {
            modePanel.SetActive(false);
            Invoke("Starter",1);
        }
        //StartCoroutine(PreloadTextures());

    }

    private void Starter()
    {
        SelectMode(0);
    }

    /*private IEnumerator PreloadTextures()
    {
#if UNITY_EDITOR
        videoFolderPath = Application.dataPath + "/" + folderCaption + "/";
#else
        videoFolderPath = "storage/emulated/0/" + folderCaption + "/";
#endif

        foreach (var stage in stages)
        {
            string imagePath = videoFolderPath + stage.videoCaption;

            if (imagePath.EndsWith(".jpg"))
            {
                UnityWebRequest www = UnityWebRequestTexture.GetTexture("file://" + imagePath);
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    Texture2D texture = DownloadHandlerTexture.GetContent(www);
                    preloadedTextures[stage.videoCaption] = texture;
                }
                else
                {
                    Debug.LogError("Failed to preload image: " + www.error);
                }
            }
        }
        if (!hasStudyRegime)
        {
            modePanel.SetActive(false);
            SelectMode(0);
        }
        Debug.Log("All textures preloaded.");
    }*/

    public void OpenModePanel()
    {
        modePanel.SetActive(true);
        if (mediaPlayer.Control.IsPlaying())
        {
            mediaPlayer.Pause();
        }
    }

    public void SelectMode(int selectedMode)
    {
        currentMode = (Mode)selectedMode;
        modePanel.SetActive(false);
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
        else if (!isLastAnswer)
        {
            PauseVideo();
        }
        else
        {
            mediaPlayer.Play();
        }
    }

    private Texture2D GetTextureByPath(string path)
    {
        foreach(var texture in preloadedTextures)
        {
            if(path.EndsWith(texture.name+".jpg"))
            {
                return texture;
            }
        }
        Debug.LogError("No Such texture with path " + path);
        return null;
    }

    private void PlayVideo()
    {
#if UNITY_EDITOR
        videoFolderPath = Application.dataPath + "/" + folderCaption + "/";
#else
videoFolderPath ="storage/emulated/0/"+folderCaption+"/";
#endif
        var videoPath = videoFolderPath + stages[currentStageIndex].videoCaption;
        if (videoPath.EndsWith(".jpg"))
        {
            mediaPlayer.GetComponent<ApplyToMesh>().DefaultTexture = GetTextureByPath(videoPath);
        }
        else
        {
            mediaPlayer.GetComponent<ApplyToMesh>().DefaultTexture = null;
            mediaPlayer.OpenMedia(new MediaPath(videoPath, MediaPathType.AbsolutePathOrURL));
            nextQuestionTime = stages[currentStageIndex].GetNextQuestionTime(currentQuestionIndex);
            Vector3 eulerRotation = new Vector3(0, stages[currentStageIndex].GetStartAngle(), 0);
            mediaPlayer.transform.rotation = Quaternion.Euler(eulerRotation);
            mediaPlayer.Play();
        }
        CountAnswers();
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
        print("Выбран ответ" + answer.gameObject.name);
        OnChoosedAnswer?.Invoke(answer.answerType, answer.isCorrect, answer.parentQuestion.GetIndexByAnswer(answer));
        answer.ResponseProcess(this);
    }

    public void MakeMistake()
    {
        wrongAnswers++;
        wrongAnswersText.text = "\nОшибок: " + wrongAnswers;
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
        ReturnVideo();
    }

    public void NextStage()
    {
        if (currentStageIndex != 0)
        {
            currentStageIndex++;
        }
        SetStage(currentStageIndex);
    }

    public void SetStage(int numStage)
    {
        if (numStage < stages.Count && numStage >= 0)
        {
            mediaPlayer.CloseMedia();
            stages[currentStageIndex].gameObject.SetActive(false);
            currentStageIndex = numStage;
            currentQuestionIndex = 0;
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
        try
        {
            if (!mediaPlayer.Control.IsPaused() && mediaPlayer.Control.GetCurrentTime() >= nextQuestionTime && !isLastAnswer || mediaPlayer.GetComponent<ApplyToMesh>().DefaultTexture != null)
            {
                PauseVideo();
            }
            if (mediaPlayer.Control.IsFinished())
            {
                NextStage();
            }
        }
        catch (Exception e)
        {
            //FPScounter.Print(e.ToString());
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
        allQuestionsText.text = "Ответов: " + currentAnswers + "/" + allAnswers;
        foreach (var stage in stages)
        {
            stage.gameObject.SetActive(false);
        }
        isStarted = false;
        OpenModePanel();
        mainQuestion.text = "";
    }
}


public enum Mode
{
    Study,
    Test,
    Exam
}

