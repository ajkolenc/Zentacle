using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardShortcuts : MonoBehaviour {

	void Update () {
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)){
            if (Input.GetKeyDown(KeyCode.S)){
                if (SceneUI.LinkedSavedScene != null){
                    SceneUI.Instance.OnSceneSaveChanges();
                }
                else {
                    SceneUI.Instance.OnSaveAsNewScene();
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape)){
            Application.Quit();
        }
        else if (Input.GetKeyDown(KeyCode.P)){
            SceneUI.Instance.OnReplay();
        }
        else if (Input.GetKeyDown(KeyCode.G)){
            SceneUI.Instance.OnRecordGif();
        }
        else if (Input.GetKeyDown(KeyCode.S)){
            SceneUI.Instance.OnTakeScreenshot();
        }
        else if (Input.GetKeyDown(KeyCode.M)){
            if (Application.platform != RuntimePlatform.WebGLPlayer){
                SceneUI.Instance.MusicToggle.isOn = !SceneUI.Instance.MusicToggle.isOn;
            }
        }
        else if (Input.GetKeyDown(KeyCode.R)){
            if (PatternUI.Instance.IsExpanded){
                PatternUI.Instance.Randomize();
            }
            else if (ParticleUI.Instance.IsExpanded){
                ParticleUI.Instance.Randomize();
            }
        }
	}
}
