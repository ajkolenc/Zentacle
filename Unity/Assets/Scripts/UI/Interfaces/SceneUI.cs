using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;

public class SceneUI : MonoBehaviour {

    public static SceneUI Instance;
    public static SpiralScene CurrentScene, LinkedSavedScene;

    [SerializeField] CanvasGroup visuals;
    public List<PatternGroupUI> Groups; 
    public InputField SceneTitle;
    public Button SceneDrop, SceneSaveChanges;
    public ScrollingDropDown SceneDropDown;
    public Button BackgroundEdit, BackgroundRandom, BackgroundCopy, BackgroundPaste;
    public ColorPicker BackgroundColor;
    public GameObject MusicGroup;
    public Toggle MusicToggle;
    public Image MusicIcon;
    bool preventSave = false;

    void Awake(){
        Instance = this;
        preventSave = PlayerPrefs.GetInt("Unsaved Changes", 0) == 0;
    }

    void Start(){
        MusicToggle.isOn = PlayerPrefs.GetInt("Music On", 1) > 0;
        if (Application.platform == RuntimePlatform.WebGLPlayer){
            MusicToggle.isOn = false;
            MusicGroup.gameObject.SetActive(false);
        }
        UIManager.OnRefresh += Refresh;
        BackgroundColor.OnColorChanged += onBackgroundColorEdited;
    }

    public void SetVisible(bool visible){
        visuals.blocksRaycasts = visible;
        StartCoroutine(lerpTo(visible ? 1f : 0f));
    }

    IEnumerator lerpTo(float alpha){
        float startAlpha = visuals.alpha;

        Timer t = new Timer(0.3f);
        while (!t.IsFinished()){
            yield return 0;
            visuals.alpha = Mathf.Lerp(startAlpha, alpha, t.Percent());
        }
    }

    public void Refresh(){

        RefreshSaveButton();
        preventSave = false;

        RefreshSceneOptions();
        BackgroundColor.SetColor(CurrentScene.BackgroundColor);
        BackgroundPaste.gameObject.SetActive(DataManager.Instance.CopiedObject is Color);

        foreach (PatternGroupUI ui in Groups){
            ui.Refresh();
        }
        SpiralManager.Instance.PlayScene(CurrentScene);
    }

    public void RefreshSaveButton(){
        SceneSaveChanges.gameObject.SetActive(LinkedSavedScene != null && !preventSave);
    }

    public void RefreshSceneOptions(){
        SceneDropDown.BeginOptions();

        List<string> presetOptions = new List<string>();
        foreach (SpiralScene preset in DataManager.Instance.PresetScenes){
            presetOptions.Add(preset.name);
        }

        SceneDropDown.AddOption("Presets", presetOptions, (x) =>
            {
                LinkedSavedScene = null;
                DataManager.Instance.UnsavedScene.Copy(DataManager.Instance.PresetScenes[x]);
                LoadScene(DataManager.Instance.UnsavedScene);
            });

        List<string> savedScenes = new List<string>();
        foreach (SpiralScene saved in DataManager.Instance.SavedScenes){
            savedScenes.Add(saved.name);
        }
        savedScenes.Add("[SAVE CURRENT]");

        SceneDropDown.AddOption("Saved Scenes", savedScenes, (x) => {
            if (x < DataManager.Instance.SavedScenes.Count){
                LinkedSavedScene = DataManager.Instance.SavedScenes[x];
                DataManager.Instance.UnsavedScene.Copy(LinkedSavedScene);
                LoadScene(DataManager.Instance.UnsavedScene);
            }
            else {
                OnSaveAsNewScene();
            }
            preventSave = true;
        }, (x) => {
            SpiralScene toRemove = DataManager.Instance.SavedScenes[x];
            if (toRemove == LinkedSavedScene){
                LinkedSavedScene = null;
                SceneSaveChanges.gameObject.SetActive(false);
            }
            DataManager.Instance.DeleteScene(x);
            UIManager.TriggerRefresh();
        });

        SceneDropDown.AddOption("[RESET SCENE]", (x) => {
            LinkedSavedScene = null;
            DataManager.Instance.LoadDefaultScene();
        });

        SceneDropDown.EndOptions();
    }

    public void OnSceneDropButton(){
        if (SceneDropDown.IsExpanded){
            SceneDropDown.Contract();
        }
        else {
            SceneDropDown.Expand();
        }
    }

    public void OnSceneTitleEdited(string newName){
        if (newName == ""){
            newName = "Scene";
        }

        if (CurrentScene.name != newName){
            CurrentScene.name = newName;
            UIManager.TriggerRefresh();
        }
    }

    public void OnSceneSaveChanges(){
        if (LinkedSavedScene != null){
            LinkedSavedScene.Copy(CurrentScene);
            SceneSaveChanges.gameObject.SetActive(false);
			DataManager.Instance.SaveData();
        }
    }
        
    public void OnSaveAsNewScene(){
        DataManager.Instance.SaveScene(CurrentScene);

        LinkedSavedScene = CurrentScene;
        DataManager.Instance.UnsavedScene.Copy(CurrentScene);
        LoadScene(DataManager.Instance.UnsavedScene);
    }

    public void OnExitApp(){
        Application.Quit();
    }

    public void LoadScene(SpiralScene scene){
        DataManager.Instance.CopiedObject = null;

        CurrentScene = scene;
        SceneTitle.text = scene.name;

        for (int i = 0; i < Groups.Count; i++){
            PatternGroupUI ui = Groups[i];
            if (i < scene.Patterns.Count){
                SpiralScene.PatternGroup g = scene.Patterns[i];
                ui.LoadGroup(g);
            }
            else {
                ui.LoadGroup(null);
            }
        }
    }

    public void OnImportDown(){
        #if UNITY_WEBGL
        Application.ExternalCall("importClickDown");
        #endif
    }

    public void OnImport(){
        SaveManager.OpenString((x) =>
            {
                if (x != ""){
                    try {
                        Dictionary<string, object> jsonData = (Dictionary<string, object>) MiniJSON.Json.Deserialize(x);
                        DataManager.Instance.UnsavedScene.FromJSON(jsonData);
                        LinkedSavedScene = null;
                        LoadScene(DataManager.Instance.UnsavedScene);
                    }
                    catch {
                        DataManager.Instance.LoadDefaultScene();
                    }
                }
            });
    }

    public void OnExport(){
        string sceneJSON = CurrentScene.ToJSON();
        SaveManager.SaveString(sceneJSON, CurrentScene.name, ".json", "Scene JSON");
    }

    public void OnBackgroundEdit(){
        BackgroundColor.Expand();
    }

    void onBackgroundColorEdited(Color newColor){
        if (CurrentScene.BackgroundColor != newColor){
            CurrentScene.BackgroundColor = newColor;
            RefreshSaveButton();
        }
        Camera.main.backgroundColor = newColor;
    }

    public void OnBackgroundCopy(){
        DataManager.Instance.CopiedObject = BackgroundColor.GetColor();
        UIManager.TriggerRefresh();
    }

    public void OnBackgroundPaste(){
        if (DataManager.Instance.CopiedObject is Color){
            Color copied = (Color)DataManager.Instance.CopiedObject;
            BackgroundColor.SetColor(copied);
            DataManager.Instance.CopiedObject = null;
            UIManager.TriggerRefresh();
        }
    }

    public void OnReplay(){
        SpiralManager.Instance.PlayScene(CurrentScene);
    }

    public void OnRecordGif(){
        SetVisible(false);
        PlaybackUI.Instance.RecordGif(CurrentScene, () =>
            {
                PlaybackUI.Instance.SetVisible(false);
                SetVisible(true);
            });
    }

    public void OnTakeScreenshot(){
        SetVisible(false);
        PlaybackUI.Instance.TakeScreenshot(CurrentScene, () =>
            {
                PlaybackUI.Instance.SetVisible(false);
                SetVisible(true);
            });
    }

    public void OnMusicToggle(){
        SpiralManager.Instance.UseSynths = MusicToggle.isOn;
        PlayerPrefs.SetInt("Music On", MusicToggle.isOn ? 1 : 0);
        Color newColor = MusicIcon.color;
        newColor.a = MusicToggle.isOn ? 1f : 0.3f;
        MusicIcon.color = newColor;
    }
}