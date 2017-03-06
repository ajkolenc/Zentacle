using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpiralParticle : MonoBehaviour, ISerializable {

    public float TargetLength;
    public float Width = 1f, Tapering = 0.5f;

    public float Arc, Curl;
    [HideInInspector] public float CurlVelocity = 0f, CurlLength = 0f;

    public int TargetChildren;

    public float ChildSpread = 0f;
    public float ChildLengthMultiplier = 1f;
    public float ChildArcMultiplier = 1f, ChildCurlMultiplier = 1f;
    public float ChildRotation = 0f;
    public int ChildDepthLimit = 1;

    [System.NonSerialized]
    public int Children = 0;

    [System.NonSerialized]
    public float Length = 0;

    [System.NonSerialized]
    public float Velocity = 4;

    TrailRenderer trail;
    public TrailRenderer Trail {
        get {
            if (trail == null){
                trail = GetComponentInChildren<TrailRenderer>();
            }
            return trail;
        }
    }

    int growFrame = -1;

    float childTimer = 0, childT = 0;
    float lastT = 0f;

    float anglesTraveled;
    float lastSynthLength = 0.0f, lastSynthAngles = 0.0f;
    float pitchMultiplier = 1.0f;

    public void Init(){

        Color myColor = Trail.sharedMaterial.color;
        float avgColor = (myColor.r + myColor.g + myColor.b) / 3f;
        pitchMultiplier = Mathf.Pow(2f, Mathf.Min(2, Mathf.RoundToInt(3.5f * avgColor)) - 1);

        if (SpiralManager.Instance.UseSynths){
            Pluck();
        }

        childT = (1f - Mathf.Abs(ChildSpread)) / (TargetChildren);
        if (ChildSpread > 0){
            childTimer = -ChildSpread;
        }

        float tangent = (1 - Tapering);
        Trail.widthMultiplier = Width;
        Trail.widthCurve = new AnimationCurve(new Keyframe(0f, Tapering, 0f, tangent), new Keyframe(1f, 1f, tangent, 0f));
        TargetLength = Mathf.Lerp(TargetLength, Mathf.Max(Width / 2f, TargetLength), Mathf.Abs(Arc) / 360f);


        SpiralManager.Instance.RegisterParticle(this);
        growFrame = Time.frameCount + 1;

        Velocity = Mathf.Lerp(4, TargetLength, 0.5f);
    }

    public void SetMaterial(Material m){
        Trail.sharedMaterial = m;
    }

	void Update () {
        if (growFrame == Time.frameCount){
            float deltaTime = Mathf.Min(Velocity * Time.deltaTime, TargetLength - Length);
            Length += deltaTime;

            float t = (Length / TargetLength);
            float deltaT = t - lastT;
           
            CurlLength += deltaT;
            CurlVelocity += Curl * 1f * Mathf.Pow(1.3f + 0.3f * Mathf.Abs(Curl), 13f * t);
            if (Mathf.Abs(CurlVelocity) > 10000f){
                Length = TargetLength;
            }

            Vector3 oldPos = transform.position;
            Vector3 oldForward = transform.up;

            if ( Length >= TargetLength){
                transform.Rotate(Vector3.forward, Arc * deltaT, Space.Self);
                transform.Rotate(Vector3.forward, CurlVelocity * deltaT, Space.Self);
                transform.Translate(Vector3.up * deltaTime, Space.Self);
            }
            else {
                transform.Translate(Vector3.up * deltaTime, Space.Self);
                transform.Rotate(Vector3.forward, Arc * deltaT, Space.Self);
                transform.Rotate(Vector3.forward, CurlVelocity * deltaT, Space.Self);
            }

            anglesTraveled += Arc * deltaT;
            anglesTraveled += CurlVelocity * deltaT;

            if (SpiralManager.Instance.UseSynths){
                if ((Length - lastSynthLength) > SpiralManager.DistancePluck) {
                    lastSynthLength = Length;
                    Pluck();
                }

                if ((anglesTraveled - lastSynthAngles > SpiralManager.AnglePluck)){
                    lastSynthAngles = anglesTraveled;
                    Pluck();
                }
            }

            childTimer += deltaT;

            while (childTimer >= childT && Children < TargetChildren && ChildDepthLimit > 0){
                branch();
                childTimer -= childT;
            }

            if (Length >= TargetLength){
                while (Children < TargetChildren && ChildDepthLimit > 0){
                    branch();
                }
				growFrame = -1;
			}
            else {
                growFrame = Time.frameCount + 1;
            }
            lastT = t;
        }
	}

    public void Pluck(){
        float pitch = SpiralManager.YPitchOffset * 440f * pitchMultiplier * Mathf.Pow(2f, SpiralManager.PitchDegree * transform.position.y);
        float volume = Mathf.Pow(0.4f + 0.1f * Width * Mathf.Lerp(1f, Tapering, Length / TargetLength), 4);

        Synth y_synth = SynthManager.Instance.RequestSynth(SpiralManager.Instance.SynthPrefab);
        y_synth.Source.panStereo = transform.position.x / 10f;
        y_synth.PlayPitch(pitch, volume);
        SynthManager.Instance.ReleaseSynth(y_synth, false);
    }

    void branch(){
        Children++;

        SpiralParticle child = GameObject.Instantiate(this, transform.position, transform.rotation) as SpiralParticle;

        child.TargetChildren = TargetChildren;
        child.TargetLength = TargetLength * ChildLengthMultiplier;
        child.Width = Mathf.Lerp(Width, Width * Tapering, Length / TargetLength);

        child.Arc *= ChildArcMultiplier;
        child.Curl *= ChildCurlMultiplier;
        child.CurlVelocity *= ChildCurlMultiplier;
        child.ChildDepthLimit--;

        child.transform.Rotate(Vector3.forward, ChildRotation, Space.World);
        child.Init();
    }
        
    public string ToJSON(){
        System.Text.StringBuilder json = new System.Text.StringBuilder("{");

        json.Append("\"name\":\"" + name + "\",");

        json.Append("\"width\":" + Width + ",");
        json.Append("\"tapering\":" + Tapering + ",");

        json.Append("\"length\":" + TargetLength + ",");

        json.Append("\"arc\":" + Arc + ",");
        json.Append("\"curl\":" + Curl + ",");

        json.Append("\"children\":" + TargetChildren + ",");

        json.Append("\"childLengthMulti\":" + ChildLengthMultiplier + ",");
        json.Append("\"childArcMulti\":" + ChildArcMultiplier + ",");
        json.Append("\"childCurlMulti\":" + ChildCurlMultiplier + ",");

        json.Append("\"childDepth\":" + ChildDepthLimit + ",");
        json.Append("\"childSpread\":" + ChildSpread + ",");
        json.Append("\"childRotation\":" + ChildRotation + "}");

        return json.ToString();
    }

    public void FromJSON(Dictionary<string, object> jsonData){
        name = (string)jsonData["name"];

        Width = SerializableManager.GetFloatValue(jsonData, "width");
        Tapering = SerializableManager.GetFloatValue(jsonData, "tapering");

        TargetLength = SerializableManager.GetFloatValue(jsonData, "length");

        Arc = SerializableManager.GetFloatValue(jsonData, "arc");
        Curl = SerializableManager.GetFloatValue(jsonData, "curl");

        TargetChildren = SerializableManager.GetIntValue(jsonData, "children");

        ChildLengthMultiplier = SerializableManager.GetFloatValue(jsonData, "childLengthMulti");
        ChildArcMultiplier = SerializableManager.GetFloatValue(jsonData, "childArcMulti");
        ChildCurlMultiplier = SerializableManager.GetFloatValue(jsonData, "childCurlMulti");

        ChildDepthLimit = SerializableManager.GetIntValue(jsonData, "childDepth");
        ChildSpread = SerializableManager.GetFloatValue(jsonData, "childSpread");
        ChildRotation = SerializableManager.GetFloatValue(jsonData, "childRotation");
    }
}