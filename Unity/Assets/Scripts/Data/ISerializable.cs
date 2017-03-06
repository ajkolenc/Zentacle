using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface ISerializable {
    
    string ToJSON();
    void FromJSON(Dictionary<string, object> jsonData);
}