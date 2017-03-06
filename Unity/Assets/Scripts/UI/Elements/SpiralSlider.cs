using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[ExecuteInEditMode]
public class SpiralSlider : MonoBehaviour {

    public event System.Action<SpiralSlider> OnSliderUpdated;
    public Slider Slider;
    public LabeledSlider SliderLabels;
    public float Min, Max, Default;
    public float RoundTo;
    bool isRounding = false;
    bool isDragging = false;
    bool isHovering = false;

    public float Value {
        get {
            return Slider.value;
        }
        set {
            SetValue(value);
        }
    }

    void Start(){
        SetValue(Default);
    }

    void Update(){
        if (Slider != null){            
            if (Slider.maxValue != Max){
                Slider.maxValue = Max;
            }        
            if (Slider.minValue != Min){
                Slider.minValue = Min;
            }
            if (Slider.wholeNumbers){
                Slider.wholeNumbers = false;
            }

            if (!Application.isPlaying){
                if (Slider.value != RoundAndClamp(Default)){
                    SetValue(Default);    
                }
            }
        }

    }

    public void Randomize(){
        SetValue(Mathf.Lerp(Min, Max, Random.value));
    }

    public void SetValue(float value){
        isRounding = true;
        Slider.value = RoundAndClamp(value);
        SliderValueChanged();
        isRounding = false;
    }

    public void SliderValueChanged() {
        SliderLabels.SetValue(RoundAndClamp(Slider.value));
    }

    public void SliderGrab(){
        isDragging = true;    
        if (!SliderLabels.ValueShown){
            SliderLabels.ShowValue();
        }
    }

    public void SliderUpdated(){
        isDragging = false;
        if (!isHovering && SliderLabels.ValueShown){
            SliderLabels.HideValue();
        }
        if (!isRounding){
            SetValue(RoundAndClamp(Slider.value));
            if (OnSliderUpdated != null){
                OnSliderUpdated(this);
            }
        }
    }

    public float RoundAndClamp(float value){
        float rounded = value;
        if (RoundTo > 0){
            rounded = Mathf.Round(value / RoundTo) * RoundTo;
        }
        return Mathf.Clamp(rounded, Min, Max);
    }

    public void HandleHover(){
        isHovering = true;
        if (!SliderLabels.ValueShown){
            SliderLabels.ShowValue();
        }
    }

    public void HandleUnhover(){
        isHovering = false;
        if (!isDragging){
            if (SliderLabels.ValueShown){
                SliderLabels.HideValue();
            }
        }
    }
}
