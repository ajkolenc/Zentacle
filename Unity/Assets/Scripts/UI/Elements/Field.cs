using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class Field : MonoBehaviour {

    public SpiralSlider SliderInput;
    public CanvasGroup Group;        
    public List<Field> DependentFields = new List<Field>();
    public float DependentDeactivateValue = 0f;

    void Update(){
        if (DependentFields.Count > 0 && SliderInput != null && Group.interactable){
            if (SliderInput.Value == DependentDeactivateValue && AnyDependentActive()){
                foreach (Field f in DependentFields){
                    f.Group.alpha = 0.4f;
                    f.Group.interactable = false;
                    f.Group.blocksRaycasts = false;
                }
            }
            else if (SliderInput.Value != DependentDeactivateValue && AnyDependentInactive()){
                foreach (Field f in DependentFields){
                    f.Group.alpha = 1f;
                    f.Group.interactable = true;
                    f.Group.blocksRaycasts = true;
                }
            }
        }
    }

    public bool AnyDependentActive(){
        foreach (Field f in DependentFields){
            if (f != null && f.Group.interactable){
                return true;
            }
        }
        return false;
    }

    public bool AnyDependentInactive(){
        foreach (Field f in DependentFields){
            if (f != null && !f.Group.interactable){
                return true;
            }
        }
        return false;
    }
}
