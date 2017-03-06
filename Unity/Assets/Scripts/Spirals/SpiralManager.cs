using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpiralManager : MonoBehaviour {

    public static SpiralManager Instance;
    System.Action animationCallback;

    public ZoomingCamera Camera;
    public SpiralPattern Pattern, DefaultPattern;
    public float PostSimulationTime = 1f;

    public bool UseSynths;
    public const float PitchDegree = 0.2f;
    public const float XPitchOffset = 1f;
    public const float YPitchOffset = 2f;
    public const float DistancePluck = 6f;
    public const float AnglePluck = 90f;
    public Synth SynthPrefab;

    public HashSet<SpiralParticle> particles = new HashSet<SpiralParticle>();
    Dictionary<SpiralScene.PatternGroup, Material> particleMaterials = new Dictionary<SpiralScene.PatternGroup, Material>();

    bool simulationFinish = false;
    bool playing = false, executingSimulation = false;
	float initZoom = 0f;
    SpiralPattern oldPattern;

    void Awake(){
		Instance = this;
    }

    void Start(){
        Synth[] synths = new Synth[SynthManager.SynthLimit];
        for (int i = 0; i < synths.Length; i++){
            synths[i] = SynthManager.Instance.RequestSynth(SynthPrefab);
        }
        for (int i = 0; i < synths.Length; i++){
            SynthManager.Instance.ReleaseSynth(synths[i]);
        }
    }

    public void RegisterParticle(SpiralParticle particle){
        if (playing){
            particles.Add(particle);
        }
        else {
            Destroy(particle.gameObject);
        }
    }


    public void PlayScene(SpiralScene scene, float delay = 0.1f){
        PlayScene(scene, null, delay);
    }

    public void PlayScene(SpiralScene scene, System.Action callback, float delay = 0.1f){
        StartCoroutine(playScene(scene, callback, delay));
    }

    IEnumerator playScene(SpiralScene scene, System.Action callback, float delay){
        if (playing){
            Clear();
        }

        playing = true;
        yield return new WaitForSeconds(delay);

        if (playing){
            executingSimulation = true;
            animationCallback = callback;

            Camera.SetBackground(scene.BackgroundColor);

            particleMaterials.Clear();

            float avgColor = (scene.BackgroundColor.r + scene.BackgroundColor.g + scene.BackgroundColor.b) / 3f;
            Synth.CurrentScale = (Synth.ScaleType) (Mathf.Min(3, Mathf.FloorToInt(4 * avgColor)));
            float startZoom = 0f;

            for (int i = 0; i < scene.Patterns.Count; i++){
                SpiralScene.PatternGroup group = scene.Patterns[i];
                if (group != null && group.Pattern != null && group.Particle != null && group.Enabled){
                    Material particleMat = group.Pattern.Generate(group.Particle, group.Color, i * 0.1f);
                    particleMaterials.Add(group, particleMat);
                    startZoom = Mathf.Max(group.Pattern.SpokeDistance, startZoom);
                }
            }
            Camera.SetMinZoom(startZoom);
        }
    }

    public void SetColor(SpiralScene.PatternGroup group, Color c){
        if (particleMaterials.ContainsKey(group)){
            particleMaterials[group].color = c;
        }
    }

	public void Clear(){
        CancelInvoke("endSimulation");
		playing = false;
		simulationFinish = false;
        executingSimulation = false;
		foreach (SpiralParticle p in particles){
            Destroy(p.gameObject);
		}
		particles.Clear();
	}

    void Update(){
        if (playing && executingSimulation){
            if (!simulationFinish){
                bool done = true;
                foreach (SpiralParticle p in particles){
                    if (p.Length < p.TargetLength){
                        done = false;
                        break;
                    }
                }

                if (done){
                    simulationFinish = true;
                    Invoke("endSimulation", PostSimulationTime);
                }
            }
		}
    }

	void endSimulation(){
        if (animationCallback != null){
            animationCallback();
            animationCallback = null;
        }
	}
}
