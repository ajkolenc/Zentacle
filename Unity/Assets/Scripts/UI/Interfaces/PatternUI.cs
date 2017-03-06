using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PatternUI : UIWindow {
    
    public static PatternUI Instance;
    void Awake(){
        Instance = this;
    }

    public InputField Name;

    [Header("Spoke Placement")]

    public SpiralSlider Count;
    public SpiralSlider Distance, PatternRotation, Rotation;

    [Header("Spoke Duplicates")]

    public SpiralSlider DuplicateCount;
    public SpiralSlider DuplicateLength, DuplicateArc, DuplicateCurl;

    SpiralPattern editingPattern;
    bool isLoading = false;

    protected override void Start(){
        base.Start();
        registerListeners(Count, Distance, 
            PatternRotation, Rotation, 
            DuplicateCount, DuplicateLength, DuplicateArc, DuplicateCurl);
    }

    void registerListeners(params SpiralSlider[] sliders){
        foreach (SpiralSlider s in sliders){
            s.OnSliderUpdated += (x) => { UpdatePattern(); };
        }
    }

    public override void Randomize()
    {
        base.Randomize();
        UpdatePattern();
    }

    public void LoadPattern(SpiralPattern pattern){
        isLoading = true;

        editingPattern = pattern;

        Name.text = pattern.name;

        Count.SetValue(pattern.Spokes);
        Distance.SetValue(pattern.SpokeDistance);

        PatternRotation.SetValue(pattern.PatternRotation);
        Rotation.SetValue(pattern.SpokeRotation);

        DuplicateCount.SetValue(pattern.SpiralsPerSpoke - 1);
        DuplicateLength.SetValue(pattern.SpokeLengthMultiplier * 100f);
        DuplicateArc.SetValue(pattern.SpokeArcMultiplier * 100f);
        DuplicateCurl.SetValue(pattern.SpokeCurlMultiplier * 100f);

        isLoading = false;
    }

    public void UpdatePattern(){
        if (!isLoading && editingPattern != null){
            editingPattern.Spokes = (int)Count.Value;
            editingPattern.SpokeDistance = Distance.Value;

            editingPattern.PatternRotation = PatternRotation.Value;
            editingPattern.SpokeRotation = Rotation.Value;

            editingPattern.SpiralsPerSpoke = ((int) DuplicateCount.Value) + 1;
            editingPattern.SpokeLengthMultiplier = DuplicateLength.Value / 100f;
            editingPattern.SpokeArcMultiplier = DuplicateArc.Value / 100f;
            editingPattern.SpokeCurlMultiplier = DuplicateCurl.Value / 100f;
            UIManager.TriggerRefresh();
        }
    }

    public void OnNameEdited(){
        if (editingPattern != null){
            editingPattern.name = Name.text;
            UIManager.TriggerRefresh();
        }
    }
}