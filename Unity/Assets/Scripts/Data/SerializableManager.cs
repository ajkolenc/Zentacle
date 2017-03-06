using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class SerializableManager {

    public static T Deserialize<T>(string json) where T : MonoBehaviour, ISerializable {
        Dictionary<string, object> jsonData = (Dictionary<string, object>) MiniJSON.Json.Deserialize(json);
        return Deserialize<T>(jsonData);
    }

    public static T Deserialize<T>(Dictionary<string, object> jsonData) where T : MonoBehaviour, ISerializable {        
        GameObject g = new GameObject();
        T obj = g.AddComponent<T>();
        return Deserialize<T>(obj, jsonData);
    }

    public static T Deserialize<T>(T obj, string json) where T : ISerializable {
        Dictionary<string, object> jsonData = (Dictionary<string, object>) MiniJSON.Json.Deserialize(json);
        return Deserialize<T>(obj, jsonData);
    }

    public static T Deserialize<T>(T obj, Dictionary<string, object> jsonData) where T : ISerializable {
        obj.FromJSON(jsonData);
        return obj;
    }

    public static string ToJSON(this Color self){
        return "[" + self.r + "," + self.g + "," + self.b + "," + self.a + "]";
    }

    public static Color FromJSON(object jsonData){
        List<object> colorData = (List<object>)jsonData;
        Color c = new Color();
        c.r = GetFloat(colorData[0]);
        c.g = GetFloat(colorData[1]);
        c.b = GetFloat(colorData[2]);
        c.a = GetFloat(colorData[3]);
        return c;
    }

    public static Vector3 DeserializeVector3(List<object> jsonObjects){
        Vector3 temp = Vector3.zero;
        temp.x = GetFloat(jsonObjects[0]);
        temp.y = GetFloat(jsonObjects[1]);
        temp.z = GetFloat(jsonObjects[2]);
        return temp;
    }

    public static Quaternion DeserializeQuaternion(List<object> jsonObjects){
        Quaternion temp = Quaternion.identity;
        temp.x = GetFloat(jsonObjects[0]);
        temp.y = GetFloat(jsonObjects[1]);
        temp.z = GetFloat(jsonObjects[2]);
        temp.w = GetFloat(jsonObjects[3]);
        return temp;
    }

    public static float GetFloat(object obj){
        if (obj.GetType() == typeof(System.Int64)){
            return (float) ((System.Int64) obj);
        }
        else {
            return (float) ((double) obj);
        }
    }

    public static int GetInt(object obj){
        if (obj.GetType() == typeof(System.Int64)){
            return (int) ((System.Int64) obj);
        }
        else {
            return (int) ((double) obj);
        }
    }

    public static string GetStringValue(Dictionary<string, object> jsonData, string key){
        if (jsonData.ContainsKey(key))
        {
            return (string)jsonData[key];
        }
        return "";
    }

    public static float GetFloatValue(Dictionary<string, object> jsonData, string key) {
        if (jsonData.ContainsKey(key))
        {
            return GetFloat(jsonData[key]);
        }
        return 0f;
    }

    public static int GetIntValue(Dictionary<string, object> jsonData, string key) {
        if (jsonData.ContainsKey(key))
        {
            return GetInt(jsonData[key]);
        }
        return 0;
    }

    public static T GetValue<T>(Dictionary<string, object> jsonData, string key){
        if (jsonData.ContainsKey(key)){
            return (T)jsonData[key];
        }
        else {
            return default(T);
        }
    }
}