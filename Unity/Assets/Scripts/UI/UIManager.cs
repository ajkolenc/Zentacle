using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class UIManager : MonoBehaviour {

    static Dictionary<int, List<UIComponent>> activeUIElements = new Dictionary<int, List<UIComponent>>();
    public static event System.Action OnRefresh;
    static bool storedRefresh = false;

    public static void Register(UIComponent self){
        if (!activeUIElements.ContainsKey(self.Depth)){
            activeUIElements.Add(self.Depth, new List<UIComponent>());
        }
        activeUIElements[self.Depth].Add(self); 
    }

    public static void Expanded(UIComponent self){
        foreach (UIComponent ui in activeUIElements[self.Depth]){
            if (ui != self){
                ui.Contract();
            }
        }
    }

    public static void Contract(){
        foreach (int key in activeUIElements.Keys){
            foreach (UIComponent ui in activeUIElements[key]){
                ui.Contract();
            }
        }
    }

    public static void TriggerRefresh(){
        storedRefresh = true;
    }

    void Update(){
        if (Input.GetMouseButtonDown(0)){
        }    
    }

    void LateUpdate(){
        if (storedRefresh){
            OnRefresh();
            storedRefresh = false;
        }            
    }
}
