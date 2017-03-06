using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Synth : MonoBehaviour {

    public enum WaveType {Sine, Triangle, Saw, Custom}
    public enum ScaleType {Minor, MinorSuspended, MajorSuspended, Major}
    public static ScaleType CurrentScale = ScaleType.MajorSuspended;
    static Dictionary<ScaleType, int[]> scaleDegreeShifts = new Dictionary<ScaleType, int[]>() 
    {
        { 
            ScaleType.Major, 
            new int[] { 0, 1, 0, -1, 1, 0, 1, 0, 1, 0, -1, 1 }
        },
        { 
            ScaleType.MajorSuspended, 
            new int[] { 0, -1, 0, -1, 1, 0, 1, 0, -1, 1, 0, -1 }
        },
        { 
            ScaleType.MinorSuspended, 
            new int[] { 0, -1, 1, 0, 1, 0, -1, 1, 0, 1, 0, -1 }
        },
        { 
            ScaleType.Minor, 
            new int[] { 0, -1, 1, 0, 1, 0, 1, 0, -1, 1, 0, -1 }
        }
    };

    public WaveType Wave;
    public AudioClip CustomFile;
    public AnimationCurve CustomWave;
    public float Noise = 0f;
    [SerializeField] float volume = 1f;
    public AnimationCurve Envelope;
    public float EnvelopeTime;
    public AudioSource Source;

    public bool IsPlaying {
        get {
            bool playing = isPlaying;
            return playing;
        }
    }
    bool isPlaying = false;
    AudioClip generatedClip;
    int samples = 4410;
    static float fundamental = 440f;
    public float Pitch {
        get {
            return currentFrequency;
        }
    }
    [SerializeField] float currentFrequency = 440f;
    bool initialized = false;
    public float Volume {
        get {
            return targetVolume * volume;
        }
    }
    float targetVolume = 1f;
    float envelopeTimer = 10000f;

    void Start(){
        Init();
    }

    public void Init(){
        if (!initialized){
            initialized = false;
            if (Source == null){
                Source = gameObject.AddComponent<AudioSource>();
                Source.loop = true;
            }
            GenerateClip();
        }
    }

    void Update(){        
        Source.volume = targetVolume * Envelope.Evaluate(Mathf.Clamp01(envelopeTimer / EnvelopeTime)) * volume;
        envelopeTimer += Time.deltaTime;
    }

    static float scaleAdjust(float frequency){
        int totalSteps = Mathf.RoundToInt(12 * Mathf.Log(frequency / fundamental, 2f));
        int halfStepDistance = totalSteps % 12;
        int endAdjustment = halfStepDistance >= 0 ? 0 : -12;
        halfStepDistance -= endAdjustment;
        int direction = (int)Mathf.Sign(totalSteps);

        totalSteps += direction * scaleDegreeShifts[CurrentScale][halfStepDistance];

        totalSteps += endAdjustment;
        return Mathf.Pow(2f, totalSteps / 12f) * fundamental;
    }


    public void Play(float seconds, float volume = 1f){
        //Debug.Log("Play synth");
        SetVolume(volume);
        envelopeTimer = 0f;
        Source.Play();
        isPlaying = true;
        Invoke("stop", seconds);
    }

    public void PlayPitch(float frequency){
        PlayPitch(frequency, EnvelopeTime, targetVolume);
    }

    public void PlayPitch(float frequency, float volume){
        PlayPitch(frequency, EnvelopeTime, volume);
    }

    public void PlayPitch(float frequency, float seconds, float volume){
        SetPitch(frequency);
        Play(seconds, volume);
    }

    public void Stop(){
        if (isPlaying){
            stop();
        }
    }

    void stop(){
        CancelInvoke("stop");
        isPlaying = false;
        Source.Stop();
    }

    public void SetVolume(float volume){
        targetVolume = volume;
    }

    public void SetPitch(float frequency){ 
        currentFrequency = scaleAdjust(frequency);
        float newPitch = currentFrequency / fundamental;
        Source.pitch = newPitch;
    }

    public void GenerateClip(){
        if (generatedClip != null){
            Destroy(generatedClip);
        }
        generatedClip = CreateClip(this);
        Source.clip = generatedClip;
    }

    public static AudioClip CreateClip(Synth s){
        AudioClip generatedClip = null;
        if (s.Wave == WaveType.Custom && s.CustomFile != null){
            float[] clipData = new float[s.CustomFile.samples];

            generatedClip = AudioClip.Create("Synth", clipData.Length, 2, 44100, false);

            s.CustomFile.GetData(clipData, 0);

            generatedClip.SetData(clipData, 0);
            return generatedClip;
        }

        float[] audioData = new float[s.samples];

        generatedClip = AudioClip.Create("Synth", s.samples, 1, 44100, false);
        for (int i = 0; i < s.samples; i++){
            float time = (fundamental * generatedClip.length * Mathf.Clamp01(i / (float) s.samples)) % 1f;

            float amplitude = 0f;

            switch (s.Wave){
                case WaveType.Sine:
                    amplitude = Mathf.Sin(time * 2 * Mathf.PI);
                    break;
                case WaveType.Triangle:
                    amplitude = (1f - Mathf.Abs(0.5f - time) * 4);
                    break;
                case WaveType.Saw:
                    amplitude = (time - 0.5f) * 2;
                    break;
                case WaveType.Custom:
                    amplitude = s.CustomWave.Evaluate(time);
                    break;
            }
            amplitude += Random.Range(-1f, 1f) * s.Noise;
            audioData[i] = amplitude;
        }

        generatedClip.SetData(audioData, 0);
        return generatedClip;
    }
}
