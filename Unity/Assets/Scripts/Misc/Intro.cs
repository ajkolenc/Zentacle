using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Intro : MonoBehaviour {

    public static Intro Instance;

    public Text ZenText, TacleText;
    public CanvasGroup Visuals, CreateButton;
    public bool SkipIntro;
    SpiralScene startScene;

    void Awake(){
        Instance = this;
    }

    IEnumerator Start(){
        Color startColor = ZenText.color;
        startColor.a = 0f;

        ZenText.color = startColor;
        TacleText.color = startColor;

        PlaybackUI.Instance.SetBackground(SceneUI.CurrentScene.BackgroundColor);
        SpiralManager.Instance.Camera.SetBackground(SceneUI.CurrentScene.BackgroundColor);

        if (SkipIntro){
            OnCreate();
            yield break;
        }

        yield return new WaitForEndOfFrame();

        PlaybackUI.Instance.StopAnimation();

        Visuals.alpha = 1f;

        if (Application.platform == RuntimePlatform.WebGLPlayer){
            yield return new WaitForSeconds(1f);
        }

        PlaybackUI.Instance.PlayAnimation(SceneUI.CurrentScene);

        yield return new WaitForSeconds(1f);

        Color endColor = startColor;
        endColor.a = 1f;

        Timer t = new Timer(2f);
        while (!t.IsFinished()){
            yield return 0;
            ZenText.color = Color.Lerp(startColor, endColor, LerpUtility.Spherical(t.Percent()));
            TacleText.color = Color.Lerp(startColor, endColor, LerpUtility.Spherical(Mathf.Clamp01(t.Percent() * 1.4f - 0.4f)));
        }

        t.SetInterval(1f);
        t.Restart();
        CreateButton.interactable = true;
        CreateButton.blocksRaycasts = true;
        while (!t.IsFinished()){
            yield return 0;
            CreateButton.alpha = t.Percent();
        }
    }

    public void OnCreate(){
        Visuals.alpha = 0f;
        Visuals.blocksRaycasts = false;
        SceneUI.Instance.SetVisible(true);
    }
}
