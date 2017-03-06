using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

public class SynthManager : MonoBehaviour{

    public static SynthManager Instance;
    public const int SynthLimit = 64;
    static int createdSynths = 0;

    public Dictionary<Synth.WaveType, AudioClip> WaveClips = new Dictionary<Synth.WaveType, AudioClip>();
    Queue<Synth> synthPool = new Queue<Synth>();
    List<Synth> synthUseHistory = new List<Synth>();

    void Awake(){
        Instance = this;
    }
        
    public Synth RequestSynth(Synth prefab){
        return RequestSynth(prefab.Wave, prefab);
    }

    public Synth RequestSynth(Synth.WaveType waveType, Synth prefab = null) {
        Synth requestedSynth = null;

        if (synthPool.Count == 0 && createdSynths >= SynthLimit && synthUseHistory.Count > 0){
            Synth reclaimed = synthUseHistory[synthUseHistory.Count - 1];
            reclaimed.Stop();
            synthPool.Enqueue(reclaimed);
            synthUseHistory.RemoveAt(synthUseHistory.Count - 1);
        }

        if (synthPool.Count > 0){
            Synth recycled = synthPool.Dequeue();
            if (prefab != null){
                recycled.CustomFile = prefab.CustomFile;
                recycled.CustomWave = prefab.CustomWave;
                recycled.Envelope = prefab.Envelope;
                recycled.SetPitch(prefab.Pitch);
                recycled.SetVolume(prefab.Volume);
            }

            if (recycled.Wave != waveType){
                recycled.Wave = waveType;
                recycled.GenerateClip();
            }
            requestedSynth = recycled;
        }
        else {
            Synth newSynth = null;

            createdSynths++;

            if (prefab != null){
                newSynth = Instantiate<Synth>(prefab);
            }
            else {
                newSynth = new GameObject(waveType.ToString() + " Synth").AddComponent<Synth>();
                newSynth.Wave = waveType;
            }
            newSynth.transform.SetParent(transform, false);
            newSynth.transform.position = Vector3.zero;
            newSynth.Init();
            newSynth.SetPitch((prefab ?? newSynth).Pitch);
            newSynth.SetVolume(prefab.Volume);

            requestedSynth = newSynth;
        }
        if (requestedSynth != null){
            synthUseHistory.Add(requestedSynth);
        }
        return requestedSynth;
    }

    public void ReleaseSynth(Synth s, bool stopSynth = true){
        if (stopSynth){
            s.Stop();
        }
        synthPool.Enqueue(s);
        synthUseHistory.Remove(s);
    }
}
