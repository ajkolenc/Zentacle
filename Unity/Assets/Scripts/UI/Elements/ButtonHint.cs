using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonHint : MonoBehaviour {

    [SerializeField] AnimationCurve curve;
    [SerializeField] float scaleTime = 0.4f;
    Coroutine routine;

    public void Expand(){
        if (routine != null){
            StopCoroutine(routine);
        }
        routine = StartCoroutine(goToScale(Vector3.one));
    }

    public void Contract(){
        if (routine != null){
            StopCoroutine(routine);
        }
        routine = StartCoroutine(goToScale(Vector3.up));
    }

    IEnumerator goToScale(Vector3 scale){
        Vector3 start = transform.localScale;
        Timer t = new Timer(scaleTime);
        while (!t.IsFinished()){
            yield return 0;
            transform.localScale = Vector3.Lerp(start, scale, curve.Evaluate(t.Percent()));
        }
    }

    void OnDisable(){
        transform.localScale = Vector3.up;
    }
}
