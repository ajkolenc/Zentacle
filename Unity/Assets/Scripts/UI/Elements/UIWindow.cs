using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIWindow : UIComponent {

    public CanvasGroup Visuals, Content;
    [SerializeField] float expandTime;
    [SerializeField] AnimationCurve expandCurve;
    RectTransform expandFrom;

    [System.Serializable]
    public class UITab {
        public Button Tab;
        public SettingsGroup Settings;
    }

    public List<UITab> Tabs;
    public UITab OpenTab {
        get; private set;
    }

    protected override void Start()
    {
        base.Start();
        if (Tabs.Count > 0){
            OpenTab = Tabs[0];
            ChangeTab(OpenTab);
        }
    }

    public void ChangeTabs(Button click){
        int index = -1;
        for (int i = 0; i < Tabs.Count; i++){
            UITab tab = Tabs[i];
            if (tab.Tab == click){
                index = i;
                break;
            }
        }

        if (index >= 0){
            ChangeTab(Tabs[index]);
        }
    }

    public void ChangeTab(UITab newTab){
        if (OpenTab.Tab != null){
            OpenTab.Tab.interactable = true;
        }
        OpenTab.Settings.Group.alpha = 0;
        OpenTab.Settings.Group.blocksRaycasts = false;

        if (newTab.Tab != null){            
            newTab.Tab.interactable = false;
        }
        newTab.Settings.Group.alpha = 1;
        newTab.Settings.Group.blocksRaycasts = true;
        OpenTab = newTab;
    }

    public virtual void Randomize(){
        foreach (Field f in OpenTab.Settings.Fields){
            randomizeField(f);
        }
        UIManager.TriggerRefresh();
    }

    void randomizeField(Field f){
        if (f.SliderInput != null){
            f.SliderInput.Randomize();
        }
    }
        
    public Coroutine Expand(RectTransform from){
        expandFrom = from;
        return Expand();
    }

    public override Coroutine Expand()
    {
        if (!IsExpanded){
            base.Expand();
            Visuals.blocksRaycasts = true;
            Content.blocksRaycasts = true;
            return StartCoroutine(scale(true));
        }
        return null;
    }

    public override Coroutine Contract()
    {
        if (IsExpanded){
            base.Contract();
            Visuals.blocksRaycasts = false;
            Content.blocksRaycasts = false;
            return StartCoroutine(scale(false));
        }
        return null;
    }

    IEnumerator scale(bool scaleIn){
        RectTransform parent = transform.parent.GetComponent<RectTransform>();
        Rect parentRect = parent.rect;

        bool isAnimatingFromTransform = expandFrom != null;
        Rect expandRect = isAnimatingFromTransform ? expandFrom.rect : new Rect();

        Vector2 startScale = new Vector2();
        startScale.x = expandRect.width / parentRect.width;
        startScale.y = expandRect.height / parentRect.height;

        Vector2 endScale = Vector2.one;

        Vector3 startPosition = isAnimatingFromTransform ? expandFrom.position : parent.position;
        Vector3 endPosition = parent.position;

        Timer t = new Timer(expandTime);
        while (!t.IsFinished()){
            yield return 0;
            float time = t.Percent();
            if (scaleIn){
                transform.position = Vector3.Lerp(startPosition, endPosition, expandCurve.Evaluate(time));
                transform.localScale = Vector2.Lerp(startScale, endScale, expandCurve.Evaluate(time));
                Visuals.alpha = Mathf.Min(1f, time * 10);
                Content.alpha = Mathf.Max(0f, 1.0f * time - 0.0f);
            }
            else {
                transform.position = Vector3.Lerp(startPosition, endPosition, 1f - expandCurve.Evaluate(time));
                transform.localScale = Vector2.Lerp(startScale, endScale, 1f - expandCurve.Evaluate(time));
                Visuals.alpha = Mathf.Min(1f, 1 - time);
                Content.alpha = Mathf.Max(0f, 1.9f * (1 - time) - 0.9f);
            }
        }

        if (!scaleIn){
            expandFrom = null;
        }
    }
}