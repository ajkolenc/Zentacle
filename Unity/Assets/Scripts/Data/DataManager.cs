using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DataManager : MonoBehaviour {

    public static DataManager Instance;

    [SerializeField] bool clearDataOnStart;
    [SerializeField] SpiralScene defaultScene;
    [SerializeField] SpiralPattern defaultPattern;
    [SerializeField] SpiralParticle defaultParticle;

    public List<SpiralScene> PresetScenes = new List<SpiralScene>();
    [System.NonSerialized] public List<SpiralScene> SavedScenes = new List<SpiralScene>();
    [System.NonSerialized] public SpiralScene UnsavedScene;
    [System.NonSerialized] public object CopiedObject;

    const float autosaveTime = 30f;
    float saveTimer = 0.0f;

    void Awake(){
        Instance = this;
    }

    void Start(){
        Init();
    }

    void Update(){
        saveTimer += Time.deltaTime;
        if (saveTimer >= autosaveTime){
            SaveData();
            saveTimer -= autosaveTime;
        }
    }

    public void Init(){
        if (clearDataOnStart){
            PlayerPrefs.DeleteAll();
        }

        UnsavedScene = CreateScene();
        UnsavedScene.name = "Scene 1";

        LoadData();
        LoadStartScene();
    }

    public void LoadData(){
        int sceneCount = PlayerPrefs.GetInt("Scene Count", 0);
        for (int i = 0; i < sceneCount; i++){
            string sceneJson = PlayerPrefs.GetString("Saved Scene " + i);

            if (i >= SavedScenes.Count){
                SpiralScene scene = SerializableManager.Deserialize<SpiralScene>(sceneJson);
                SavedScenes.Add(scene);
            }
            else {
                SerializableManager.Deserialize<SpiralScene>(SavedScenes[i], sceneJson);
            }
        }

        string inProgress = PlayerPrefs.GetString("In Progress Scene", "");
        if (inProgress != ""){
            SerializableManager.Deserialize<SpiralScene>(UnsavedScene, inProgress);
        }
    }

    public void LoadStartScene(){
        int savedScene = PlayerPrefs.GetInt("Last Saved Scene", -1);
        if (savedScene >= 0 && savedScene < SavedScenes.Count){
            SceneUI.LinkedSavedScene = SavedScenes[savedScene];
        }
        SceneUI.Instance.LoadScene(UnsavedScene);
    }

    public void LoadDefaultScene(){
        SerializableManager.Deserialize<SpiralScene>(UnsavedScene, defaultScene.ToJSON());
        SceneUI.Instance.LoadScene(UnsavedScene);
    }

    public void SaveData(){
        PlayerPrefs.SetInt("Scene Count", SavedScenes.Count);
        for (int i = 0; i < SavedScenes.Count; i++){
            string json = SavedScenes[i].ToJSON();
            PlayerPrefs.SetString("Saved Scene " + i, json);
        }

        if (SceneUI.CurrentScene != null){
            int saveIndex = SavedScenes.IndexOf(SceneUI.LinkedSavedScene);
            PlayerPrefs.SetInt("Last Saved Scene", saveIndex);
            PlayerPrefs.SetString("In Progress Scene", UnsavedScene.ToJSON());
            PlayerPrefs.SetInt("Unsaved Changes", SceneUI.Instance.SceneSaveChanges.gameObject.activeSelf ? 1 : 0);
        }
    }

    public SpiralScene CreateScene(SpiralScene copy = null){
        SpiralScene baseScene = copy ?? defaultScene;
        SpiralScene scene = new GameObject(defaultScene.name).AddComponent<SpiralScene>();
        scene.transform.SetParent(transform, false);
        SerializableManager.Deserialize<SpiralScene>(scene, baseScene.ToJSON());
        return scene;
    }

    public SpiralPattern CreatePattern(){
        SpiralPattern pattern = Instantiate<SpiralPattern>(defaultPattern);
        pattern.transform.SetParent(transform, false);
        SerializableManager.Deserialize<SpiralPattern>(pattern, defaultPattern.ToJSON());
        return pattern;
    }

    public SpiralParticle CreateParticle(){
        SpiralParticle particle = Instantiate<SpiralParticle>(defaultParticle);
        particle.transform.SetParent(transform, false);
        SerializableManager.Deserialize<SpiralParticle>(particle, defaultParticle.ToJSON());
        return particle;
    }

    public void SaveScene(SpiralScene scene){
        if (scene == UnsavedScene){
            UnsavedScene = CreateScene();
            SavedScenes.Add(scene);
			SaveData();
        }
    }

    public void DeleteScene(int sceneIndex){
        SavedScenes.RemoveAt(sceneIndex);
    }

    void OnApplicationQuit(){
        SaveData();
    }
}
