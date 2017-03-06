using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpiralPattern : MonoBehaviour, ISerializable {

    public int Spokes = 2; 
    public float PatternRotation = 0f, SpokeRotation;
    public float  SpokeDistance = 0f;
    public int SpiralsPerSpoke = 1;
    public float SpokeLengthMultiplier = 1, SpokeArcMultiplier = -1, SpokeCurlMultiplier = -1f;

    public Material Generate(SpiralParticle particlePrefab, Color c, float depth = 0f){
        Material particleMaterial = null;
        for (int i = 0; i < Spokes; i++){
            
            Vector3 direction = Vector2.zero;
            direction.x = Mathf.Cos(Mathf.PI * 2f * (i / (float)Spokes) + PatternRotation * Mathf.Deg2Rad);
            direction.y = Mathf.Sin(Mathf.PI * 2f * (i / (float)Spokes) + PatternRotation * Mathf.Deg2Rad);

            for (int j = 0; j < SpiralsPerSpoke; j++){
                SpiralParticle particle = GameObject.Instantiate<SpiralParticle>(particlePrefab);
                if (particleMaterial == null){
                    particleMaterial = new Material(particle.Trail.sharedMaterial);
                    particleMaterial.color = c;
                }
                particle.SetMaterial(particleMaterial);
                particle.TargetLength *= Mathf.Pow(SpokeLengthMultiplier, j);
                particle.Velocity *= Mathf.Pow(SpokeLengthMultiplier, j);
                particle.Arc *= Mathf.Pow(SpokeArcMultiplier, j);
                particle.Curl *= Mathf.Pow(SpokeCurlMultiplier, j);
                particle.ChildRotation *= Mathf.Pow(Mathf.Sign(SpokeArcMultiplier), j);
                float selfRotation = SpokeRotation * Mathf.Pow(Mathf.Sign(SpokeArcMultiplier), j);

                Vector3 forward = Vector2.zero;
                forward.x = Mathf.Cos(Mathf.PI * 2f * (i / (float)Spokes) + (PatternRotation + selfRotation) * Mathf.Deg2Rad);
                forward.y = Mathf.Sin(Mathf.PI * 2f * (i / (float)Spokes) + (PatternRotation + selfRotation) * Mathf.Deg2Rad);

                particle.transform.rotation = Quaternion.LookRotation(Vector3.forward, forward);
                if (SpokeDistance > 0){
                    particle.transform.position += direction * SpokeDistance;
                }
                particle.transform.position -= Vector3.forward * depth;

                particle.Init();
            }
        }
        return particleMaterial;
    }

    public string ToJSON(){
        System.Text.StringBuilder json = new System.Text.StringBuilder("{");

        json.Append("\"name\":\"" + name + "\",");

        json.Append("\"spokes\":" + Spokes + ",");
        json.Append("\"spokeSpirals\":" + SpiralsPerSpoke + ",");
        json.Append("\"spokeDistance\":" + SpokeDistance + ",");

        json.Append("\"patternRotation\":" + PatternRotation + ",");
        json.Append("\"spokeRotation\":" + SpokeRotation + ",");

        json.Append("\"spokeLengthMulti\":" + SpokeLengthMultiplier + ",");
        json.Append("\"spokeArcMulti\":" + SpokeArcMultiplier + ",");
        json.Append("\"spokeCurlMulti\":" + SpokeCurlMultiplier + "}");

        return json.ToString();
    }

    public void FromJSON(Dictionary<string, object> jsonData){
        name = SerializableManager.GetStringValue(jsonData, "name");

        Spokes = SerializableManager.GetIntValue(jsonData, "spokes");
        SpiralsPerSpoke = SerializableManager.GetIntValue(jsonData, "spokeSpirals");
        SpokeDistance = SerializableManager.GetFloatValue(jsonData, "spokeDistance");

        PatternRotation = SerializableManager.GetFloatValue(jsonData, "patternRotation");
        SpokeRotation = SerializableManager.GetFloatValue(jsonData, "spokeRotation");

        SpokeLengthMultiplier = SerializableManager.GetFloatValue(jsonData, "spokeLengthMulti");
        SpokeArcMultiplier = SerializableManager.GetFloatValue(jsonData, "spokeArcMulti");
        SpokeCurlMultiplier = SerializableManager.GetFloatValue(jsonData, "spokeCurlMulti");
    }
}
