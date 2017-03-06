using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SettingsGroup : MonoBehaviour {

    public CanvasGroup Group;
    public Field[] Fields;
    Dictionary<string, Field> fields = new Dictionary<string, Field>();

    void Awake(){
        foreach (Field f in Fields){
            fields.Add(f.name, f);
        }
    }

    public Field GetField(string name){
        if (fields.ContainsKey(name)){
            return fields[name];
        }
        return null;
    }
}
