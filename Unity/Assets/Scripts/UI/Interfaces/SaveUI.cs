using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SaveUI : MonoBehaviour {

    public static SaveUI Instance;
    public CanvasGroup Canvas;
    public RectTransform LoadingBar;
    public Text StatusText;
    public float FullWidth = 1010f;

    float currentProgress = 0f;
    float animProgress = 0f;
    byte[] fileData;
    bool saveFinished = false;
    System.Action callback;

    void Awake(){
        Instance = this;
    }

    void Update(){
        if (Canvas.blocksRaycasts){
            animProgress = Mathf.Lerp(animProgress, currentProgress, 0.5f);
            LoadingBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Lerp(0f, FullWidth, animProgress));
            if (!saveFinished && currentProgress == 1f){
                onSaveFinished();
                saveFinished = true;
            }
        }
    }

    public void StartSave(Moments.Recorder recorder, System.Action callback){
        this.callback = callback;

        Canvas.alpha = 1f;
        Canvas.blocksRaycasts = true;

        StatusText.text = "CREATING GIF...";

        currentProgress = 0f;
        animProgress = 0f;
        saveFinished = false;
        fileData = null;

        recorder.Pause();
        recorder.Save();
        recorder.OnFileSaveProgress += saveProgress;
        recorder.OnFileSaved += saveCompleted;
    }

    void saveProgress(int worker, float progress){
        currentProgress = progress;
    }

    void saveCompleted(int worker, byte[] fileBytes){
        fileData = fileBytes;
        currentProgress = 1f;
    }


    void onSaveFinished(){
        StatusText.text = "COMPLETED!";
        SaveManager.SaveFile(fileData, SceneUI.CurrentScene.name, ".gif", "Animation");
        Invoke("EndSave", 1f);
    }

    public void EndSave(){
        Canvas.alpha = 0f;
        Canvas.blocksRaycasts = false;
        callback();
    }
}