using UnityEngine;
using System.Collections;
using System.IO;
#if UNITY_STANDALONE_WIN
using System.Windows.Forms;
#endif

public class SaveManager : MonoBehaviour {
    static System.Action<string> openCallback;

    public static void SaveFile(byte[] data, string filename, string extension, string filetype){
        #if UNITY_STANDALONE_WIN
        SaveFileDialog dialog = new SaveFileDialog();
        dialog.DefaultExt = filetype;
        dialog.FileName = filename + extension;
        dialog.Filter = filetype + " (*" + extension + ")|*" + extension;
        dialog.InitialDirectory = UnityEngine.Application.dataPath;
        if (dialog.ShowDialog() == DialogResult.OK){
            Stream file = dialog.OpenFile();
            file.Write(data, 0, data.Length);
            file.Close();
        }
        #elif UNITY_WEBGL
        Debug.Log(data.Length);
        UnityEngine.Application.ExternalCall("SaveBytes", filename + extension, System.Convert.ToBase64String(data));
        //SaveBytes(filename + extension, data, data.Length);
        #endif
    }

    public static void SaveString(string str, string filename, string extension, string filetype){
        #if UNITY_STANDALONE_WIN
        SaveFileDialog dialog = new SaveFileDialog();
        dialog.DefaultExt = filetype;
        dialog.FileName = filename + extension;
        dialog.Filter = filetype + " (*" + extension + ")|*" + extension;
        dialog.InitialDirectory = UnityEngine.Application.dataPath;
        if (dialog.ShowDialog() == DialogResult.OK){
            StreamWriter file = new StreamWriter(dialog.OpenFile());
            file.WriteLine(str);
            file.Close();
        }
        #elif UNITY_WEBGL
        UnityEngine.Application.ExternalCall("SaveJSON", filename + extension, str);
        //SaveJSON(filename + extension, str);
        #endif
    }

    public static void OpenString(System.Action<string> callback){
        #if UNITY_STANDALONE_WIN
        OpenFileDialog dialog = new OpenFileDialog();
        dialog.DefaultExt = ".json";
        dialog.Filter = "Scene JSON (*.json)|*.json";
        dialog.InitialDirectory = UnityEngine.Application.dataPath;
        if (dialog.ShowDialog() == DialogResult.OK){
            StreamReader file = new StreamReader(dialog.OpenFile());
            callback(file.ReadToEnd());
        }
        else {
            callback("");
        }
        #elif UNITY_WEBGL
        openCallback = callback;
        //UnityEngine.Application.ExternalCall("OpenJSON");
        //OpenJSON();
        #endif
    }

    public void ReceiveOpen(string json){
        if (openCallback != null){
            openCallback(json);
            openCallback = null;
        }
    }
}