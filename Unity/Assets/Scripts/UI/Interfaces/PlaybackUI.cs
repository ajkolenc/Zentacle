using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Moments;

public class PlaybackUI : MonoBehaviour {

    public static PlaybackUI Instance;
    [SerializeField] CanvasGroup visuals;
    [SerializeField] Image background;
    [SerializeField] Recorder recorder;
    [SerializeField] RenderTexture spiralTexture;

    void Awake(){
        Instance = this;
    }

    public void SetBackground(Color c){
        background.color = c;
    }

    public void SetVisible(bool visible){
        visuals.alpha = visible ? 1f : 0f;
        visuals.blocksRaycasts = visible;
    }

    public void RecordGif(SpiralScene scene, System.Action callback){
        SetBackground(scene.BackgroundColor);
        SetVisible(true);
        recorder.Record();
        SpiralManager.Instance.PlayScene(scene, () => {
            SaveUI.Instance.StartSave(recorder, () => {
                callback();
            });
        });
    }

    public void TakeScreenshot(SpiralScene scene, System.Action callback){
        SetBackground(scene.BackgroundColor);
        SetVisible(true);
        SpiralManager.Instance.PlayScene(scene, () => {
            Texture2D screenshot = new Texture2D(spiralTexture.width, spiralTexture.height);
            RenderTexture.active = spiralTexture;
            screenshot.ReadPixels(new Rect(0, 0, spiralTexture.width, spiralTexture.height), 0, 0);
            SaveManager.SaveFile(screenshot.EncodeToPNG(), scene.name, ".png", "Image");
            callback();
        });
    }

    public void PlayAnimation(SpiralScene scene, System.Action callback = null){
        SetBackground(scene.BackgroundColor);
        SetVisible(true);
        SpiralManager.Instance.PlayScene(scene, callback);
    }

    public void StopAnimation(){
        SpiralManager.Instance.Clear();
    }
}