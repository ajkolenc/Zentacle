using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class SpiralScene : MonoBehaviour, ISerializable {

    [System.Serializable]
    public class PatternGroup {
        public bool Enabled;
        public SpiralPattern Pattern;
        public SpiralParticle Particle;
        public Color Color;
    }

    public Color BackgroundColor;
    public List<PatternGroup> Patterns = new List<PatternGroup>();

    public void Copy(SpiralScene sceneToCopy){
        SerializableManager.Deserialize<SpiralScene>(this, sceneToCopy.ToJSON());
    }

    public string ToJSON(){
        StringBuilder json = new StringBuilder("{");
        json.Append("\"name\":\"" + name + "\",");
        json.Append("\"background\":" + BackgroundColor.ToJSON() + ",");
        json.Append("\"groups\":[");
        for (int i = 0; i < 4; i++){
            PatternGroup group = i < Patterns.Count ? Patterns[i] : null;
            if (group == null){
                json.Append("{},");
            }
            else {
                json.Append("{");
                json.Append("\"enabled\":" + (group.Enabled ? 1 : 0) + ",");

                json.Append("\"pattern\":");
                json.Append(group.Pattern.ToJSON() + ",");

                json.Append("\"particle\":");
                json.Append(group.Particle.ToJSON() + ",");

                json.Append("\"color\":" + group.Color.ToJSON());
                json.Append("},");
            }
        }
        json.Remove(json.Length - 1, 1);
        json.Append("]}");
        return json.ToString();
    }

    public void FromJSON(Dictionary<string, object> jsonData){
        name = (string)jsonData["name"];
        BackgroundColor = SerializableManager.FromJSON(jsonData["background"]);
        foreach (PatternGroup g in Patterns){
            if (g != null){
                if (g.Pattern != null){
                    Destroy(g.Pattern.gameObject);
                }
                if (g.Particle != null){
                    Destroy(g.Particle.gameObject);
                }
            }
        }
        Patterns.Clear();
        List<object> groups = (List<object>)jsonData["groups"];
        for (int i = 0; i < 4; i++){
            PatternGroup p = null;

            if (i < groups.Count){

                Dictionary<string, object> group = (Dictionary<string, object>) groups[i];

                if (group.Count > 0){
                    p = new PatternGroup();

                    Dictionary<string, object> pattern = (Dictionary<string, object>)group["pattern"];
                    Dictionary<string, object> particle = (Dictionary<string, object>)group["particle"];

                    p.Enabled = SerializableManager.GetInt(group["enabled"]) > 0;

                    p.Pattern = DataManager.Instance.CreatePattern();
                    SerializableManager.Deserialize<SpiralPattern>(p.Pattern, pattern);

                    p.Particle = DataManager.Instance.CreateParticle();
                    SerializableManager.Deserialize<SpiralParticle>(p.Particle, particle);

                    p.Color = SerializableManager.FromJSON(group["color"]);
                }
            }
            Patterns.Add(p);
        }
    }
}
