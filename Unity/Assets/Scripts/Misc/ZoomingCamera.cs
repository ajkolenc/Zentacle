using UnityEngine;
using System.Collections;

public class ZoomingCamera : MonoBehaviour {

    [SerializeField] Camera cam;
    [SerializeField] float minZoom = 2f;

    [System.NonSerialized]
    public float MaxZoom = 0f, MinZoom = 0f;
    float oldMaxDistance = 0f;

	void Update () {
        float maxDistance = 0f;
        foreach (SpiralParticle particle in SpiralManager.Instance.particles){
            maxDistance = Mathf.Max(maxDistance, particle.transform.position.magnitude + 3f);
        }
        maxDistance = Mathf.Max(maxDistance, MinZoom);
        maxDistance = Mathf.Max(oldMaxDistance, maxDistance);
        oldMaxDistance = maxDistance;
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, maxDistance, 2 * Time.deltaTime);
	}

    public void SetBackground(Color c){
        cam.backgroundColor = c;
    }

    public void SetMinZoom(float zoom){
        oldMaxDistance = 0f;
        MinZoom = Mathf.Max(minZoom, zoom);
        cam.orthographicSize = MinZoom;
    }
}
