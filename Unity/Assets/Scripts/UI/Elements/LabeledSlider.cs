using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[ExecuteInEditMode]
public class LabeledSlider : MonoBehaviour {

    public Slider Self;
    public Text Min, Max, Value;
    public string Append;
    [System.NonSerialized] public bool ValueShown = false;
    new Coroutine animation;

	void Start(){
		Value.raycastTarget = false;
	}

    void Update(){
        UpdateLabels();
    }

	public void UpdateLabels() {
        if (Self == null){
            return;
        }

        if (Min != null){
            Min.text = Self.minValue.ToString() + Append;
        }

        if (Max != null){
            Max.text = Self.maxValue.ToString() + Append;   
        }
	}

    public void SetValue(float value){
        Value.text = (value).ToString() + Append;
    }

    public Coroutine ShowValue(){
        if (animation != null){
            StopCoroutine(animation);
        }
        animation = StartCoroutine(scaleValue(Vector3.one));
        ValueShown = true;
        return animation;
    }

    public Coroutine HideValue(){
        if (animation != null){
            StopCoroutine(animation);
        }
        animation = StartCoroutine(scaleValue(Vector3.zero));
        ValueShown = false;
        return animation;
    }

    IEnumerator scaleValue(Vector3 target){
        Timer t = new Timer(0.25f);

        Vector3 start = Value.transform.parent.localScale;
        while (!t.IsFinished()){
            yield return 0;
            Value.transform.parent.localScale = Vector3.Lerp(start, target, LerpUtility.HemiSpherical(t.Percent()));
        }
    }
}