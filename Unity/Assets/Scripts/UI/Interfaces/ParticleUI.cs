using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ParticleUI : UIWindow {

    public static ParticleUI Instance;
    void Awake(){
        Instance = this;
    }

    public InputField Name;

    [Header("Visuals")]
    public SpiralSlider Width;
    public SpiralSlider Tapering;

    [Header("Behavior")]
    public SpiralSlider Length;
    public SpiralSlider Arc, Curl;

    [Header("Offshoot Spawning")]
    public SpiralSlider OffshootCount;
    public SpiralSlider OffshootDepth, OffshootSpread, OffshootRotate;

    [Header("Offshoot Inheritance")]
    public SpiralSlider OffshootLengthMultiplier;
    public SpiralSlider OffshootArcMultiplier, OffshootCurlMultiplier;

    SpiralParticle editingParticle;
    bool isLoading = false;

    protected override void Start(){
        base.Start();
        registerListeners(
            Width, Tapering, Length, Arc, Curl,
            OffshootCount, OffshootDepth, OffshootSpread, OffshootRotate,
            OffshootLengthMultiplier, OffshootArcMultiplier, OffshootCurlMultiplier);
    }

    void registerListeners(params SpiralSlider[] sliders){
        foreach (SpiralSlider s in sliders){
            s.OnSliderUpdated += (x) => { UpdateParticle(); };
        }
    }

    public override void Randomize()
    {
        base.Randomize();
        UpdateParticle();
    }

    public void LoadParticle(SpiralParticle particle){
        editingParticle = particle;
        isLoading = true;

        Name.text = particle.name;

        Width.SetValue(particle.Width);
        Tapering.SetValue(particle.Tapering * 100f);

        Length.SetValue(particle.TargetLength);
        Arc.SetValue(particle.Arc);
        Curl.SetValue(particle.Curl);

        OffshootCount.SetValue(particle.TargetChildren);
        OffshootDepth.SetValue(particle.ChildDepthLimit);
        OffshootSpread.SetValue(particle.ChildSpread);
        OffshootRotate.SetValue(particle.ChildRotation);

        OffshootLengthMultiplier.SetValue(particle.ChildLengthMultiplier * 100f);
        OffshootArcMultiplier.SetValue(particle.ChildArcMultiplier * 100f);
        OffshootCurlMultiplier.SetValue(particle.ChildCurlMultiplier * 100f);
        isLoading = false;
    }

    public void UpdateParticle(){
        if (!isLoading && editingParticle != null){
            
            editingParticle.Width = Width.Value;
            editingParticle.Tapering = Tapering.Value / 100f;

            editingParticle.TargetLength = Length.Value;
            editingParticle.Arc = Arc.Value;
            editingParticle.Curl = Curl.Value;

            editingParticle.TargetChildren = (int) OffshootCount.Value;
            editingParticle.ChildDepthLimit = (int)OffshootDepth.Value;
            editingParticle.ChildSpread = OffshootSpread.Value;
            editingParticle.ChildRotation = OffshootRotate.Value;

            editingParticle.ChildLengthMultiplier = OffshootLengthMultiplier.Value / 100f;
            editingParticle.ChildArcMultiplier = OffshootArcMultiplier.Value / 100f;
            editingParticle.ChildCurlMultiplier = OffshootCurlMultiplier.Value / 100f;

            UIManager.TriggerRefresh();
        }
    }

    public void OnNameEdited(){
        if (editingParticle != null){
            editingParticle.name = Name.text;
            UIManager.TriggerRefresh();
        }
    }
}