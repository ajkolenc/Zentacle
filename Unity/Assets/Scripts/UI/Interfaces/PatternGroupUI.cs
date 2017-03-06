using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PatternGroupUI : MonoBehaviour {

    public Toggle EnableToggle;
    public CanvasGroup GroupPanel;

    public InputField PatternName, ParticleName;

    public Button PatternEdit, PatternCopy, PatternPaste;
    public Button ParticleEdit, ParticleCopy, ParticlePaste;

    public Button ColorEdit, ColorRandom, ColorCopy, ColorPaste;
    public Image ColorDisplay;
    public ColorPicker Picker;

    public SpiralScene.PatternGroup CurrentGroup;

    public bool IsEnabled {
        get {
            return EnableToggle.isOn;
        }
    }

    void Start(){
        Picker.OnColorChanged += onColorEdited;
    }

    public void Refresh(){
        if (CurrentGroup != null){
            EnableToggle.isOn = CurrentGroup.Enabled;

            if (CurrentGroup.Pattern != null) {
                PatternName.text = CurrentGroup.Pattern.name;
            }
            else {
                PatternName.text = "(None)";
            }

            if (CurrentGroup.Particle != null){
                ParticleName.text = CurrentGroup.Particle.name;
            }
            else {
                ParticleName.text = "(None)";
            }

            Picker.SetColor(CurrentGroup.Color);
        }
        else {
            EnableToggle.isOn = false;
            PatternName.text = "(None)";
            ParticleName.text = "(None)";
            Picker.SetColor(Color.white);
        }

        PatternPaste.gameObject.SetActive(DataManager.Instance.CopiedObject is SpiralPattern);
        ParticlePaste.gameObject.SetActive(DataManager.Instance.CopiedObject is SpiralParticle);
        ColorPaste.gameObject.SetActive(DataManager.Instance.CopiedObject is Color);

        OnToggle(EnableToggle.isOn);
    }

    public void OnToggle(bool enabled){
        if (enabled){
            GroupPanel.alpha = 1f;
            GroupPanel.blocksRaycasts = true;
            GroupPanel.interactable = true;
        }
        else {
            GroupPanel.alpha = 0.2f;
            GroupPanel.blocksRaycasts = false;
            GroupPanel.interactable = false;
        }

        if (CurrentGroup == null && enabled) {
            SpiralScene.PatternGroup g = new SpiralScene.PatternGroup();
            g.Pattern = DataManager.Instance.CreatePattern();
            g.Particle = DataManager.Instance.CreateParticle();
            g.Color = Color.white;
            g.Enabled = enabled;
            int patternIndex = SceneUI.Instance.Groups.IndexOf(this);
            SceneUI.CurrentScene.Patterns[patternIndex] = g;
            LoadGroup(g);
        }

        if (CurrentGroup != null){
            if (CurrentGroup.Enabled != enabled){
                CurrentGroup.Enabled = enabled;
                UIManager.TriggerRefresh();
            }
        }
    }

    public void OnPatternNameChanged(string newName) {
        if (newName == ""){
            newName = "Pattern";
        }

        if (CurrentGroup.Pattern.name != newName){
            CurrentGroup.Pattern.name = newName;
            UIManager.TriggerRefresh();
        }
    }

    public void OnPatternEditButton(){
        PatternUI.Instance.LoadPattern(CurrentGroup.Pattern);
        PatternUI.Instance.Expand(PatternName.GetComponent<RectTransform>());
    }

    public void OnPatternCopy(){
        DataManager.Instance.CopiedObject = CurrentGroup.Pattern;
        UIManager.TriggerRefresh();
    }

    public void OnPatternPaste(){
        if (DataManager.Instance.CopiedObject is SpiralPattern){
            SpiralPattern copied = DataManager.Instance.CopiedObject as SpiralPattern;
            SerializableManager.Deserialize<SpiralPattern>(CurrentGroup.Pattern, copied.ToJSON());
            DataManager.Instance.CopiedObject = null;
            UIManager.TriggerRefresh();
        }
    }

    public void OnParticleNameChanged(string newName) {
        if (newName == ""){
            newName = "Particle";
        }
        if (CurrentGroup.Particle.name != newName){
            CurrentGroup.Particle.name = newName;
        }
    }

    public void OnParticleEditButton(){
        ParticleUI.Instance.LoadParticle(CurrentGroup.Particle);
        ParticleUI.Instance.Expand(ParticleName.GetComponent<RectTransform>());
    }

    public void OnParticleCopy(){
        DataManager.Instance.CopiedObject = CurrentGroup.Particle;
        UIManager.TriggerRefresh();
    }

    public void OnParticlePaste(){
        if (DataManager.Instance.CopiedObject is SpiralParticle){
            SpiralParticle copied = DataManager.Instance.CopiedObject as SpiralParticle;
            SerializableManager.Deserialize<SpiralParticle>(CurrentGroup.Particle, copied.ToJSON());
            DataManager.Instance.CopiedObject = null;
            UIManager.TriggerRefresh();
        }
    }

    public void OnColorEditButton(){
        if (!Picker.IsExpanded){
            Picker.Expand(ColorEdit.transform.parent as RectTransform);
        }
    }

    public void OnColorCopy(){
        DataManager.Instance.CopiedObject = Picker.GetColor();
        UIManager.TriggerRefresh();
    }

    public void OnColorPaste(){
        if (DataManager.Instance.CopiedObject is Color){
            Color copied = (Color)DataManager.Instance.CopiedObject;
            Picker.SetColor(copied);
            DataManager.Instance.CopiedObject = null;
            UIManager.TriggerRefresh();
        }
    }

    void onColorEdited(Color c){
        if (CurrentGroup != null){
            if (CurrentGroup.Color != c){
                CurrentGroup.Color = c;
                SpiralManager.Instance.SetColor(CurrentGroup, c);
                SceneUI.Instance.RefreshSaveButton();
            }
        }
        ColorDisplay.color = c;
    }

    public void LoadGroup(SpiralScene.PatternGroup group){
        CurrentGroup = group;

        UIManager.TriggerRefresh();
    }
}