using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class ScrollingDropDown : UIComponent, IPointerDownHandler {

    [SerializeField] ScrollingDropDown childListPrefab;
    [SerializeField] DropDownOption optionTemplate;
    [SerializeField] RectTransform content;
    [SerializeField] float maxHeight;
    [SerializeField] float dropTime;
    [HideInInspector][SerializeField] List<DropDownOption> options = new List<DropDownOption>();
    [HideInInspector][SerializeField] List<ScrollingDropDown> subLists = new List<ScrollingDropDown>();
    bool refresh = false;
    bool doCache = true;
    int refreshOptionIndex = 0;
    List<System.Action> cachedData = new List<System.Action>();


    protected override void Start()
    {
        base.Start();
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0f);
    }

    #region IPointerDownHandler implementation

    public void OnPointerDown(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    #endregion

    public void BeginOptions(){
        if (doCache){
            cachedData.Clear();
        }
        refresh = true;
        refreshOptionIndex = 0;
    }

    public void EndOptions(){
        refresh = false;
        for (int i = options.Count - 1; i >= refreshOptionIndex; i--){
            DropDownOption b = options[i];
            Destroy(b.gameObject);
            if (subLists[i] != null){
                Destroy(subLists[i].gameObject);
            }
            options.RemoveAt(i);
            subLists.RemoveAt(i);
        }
    }

    public override Coroutine Expand(){
        if (!IsExpanded){
            doCache = false;
            if (cachedData.Count > 0){
                List<System.Action> oldCache = new List<System.Action>(cachedData);
                cachedData.Clear();
                foreach (System.Action cache in oldCache){
                    cache.Invoke();
                }
                Canvas.ForceUpdateCanvases();
            }
            base.Expand();
            IsExpanded = true;
            UIManager.Expanded(this);
            return StartCoroutine(goToHeight(Mathf.Min(maxHeight, content.sizeDelta.y)));
        }
        return null;
    }

    public override Coroutine Contract(){
        if (IsExpanded){
            doCache = true;

            base.Contract();
            IsExpanded = false;
            foreach (ScrollingDropDown sub in subLists){
                if (sub != null){
                    sub.Contract();
                }
            }
            return StartCoroutine(goToHeight(0));
        }
        return null;
    }

    public void AddOption(string name, System.Action<int> onSelected, System.Action<int> onDeleted = null){
        if (doCache){
            cachedData.Add(() =>
                {
                    AddOption(name, onSelected, onDeleted);
                });
            return;
        }
        DropDownOption option = addOption(name);
        option.SublistArrow.enabled = false;

        int index = options.IndexOf(option);
        option.Option.onClick.AddListener(() =>
            {
                Contract();
                onSelected(index);
            });
        
        if (onDeleted != null) {
            option.Delete.onClick.AddListener(() =>
                {
                    onDeleted(index);
                });
        }
        else {
            option.Delete.gameObject.SetActive(false);
        }
    }

    public void AddOption(string name, IList<string> subOptions, System.Action<int> onSelected, System.Action<int> onDeleted = null){
        if (doCache){
            cachedData.Add(() =>
                {
                    AddOption(name, subOptions, onSelected, onDeleted);
                });
            return;
        }

        DropDownOption option = addOption(name);
        option.SublistArrow.enabled = true;

        ScrollingDropDown subList = addSublist();

        option.Delete.gameObject.SetActive(false);

        option.Option.onClick.AddListener(() =>
            {
                if (subList.IsExpanded) {
                    subList.Contract();
                }
                else {
                    subList.Expand();
                }
            });
        
        if (refresh){
            subList.BeginOptions();
        }

        for (int i = 0; i < subOptions.Count; i++){
            string subOption = subOptions[i];
            int index = i;
            if (onDeleted == null || subOption[0] == '['){
                subList.AddOption(subOption, (x) =>
                    {
                        Contract();
                        onSelected(index);
                    });
            }
            else {
                subList.AddOption(subOption, (x) =>
                    {
                        Contract();
                        onSelected(index);
                    }, (x) => {
                        onDeleted(index);
                    });
            }
        }

        if (refresh){
            subList.EndOptions();
        }
    }

    DropDownOption addOption(string name){
        DropDownOption newOption = null;
        if (refresh){
            if (refreshOptionIndex < options.Count){
                newOption = options[refreshOptionIndex];
                newOption.Option.onClick.RemoveAllListeners();
                newOption.Delete.gameObject.SetActive(true);
                newOption.Delete.onClick.RemoveAllListeners();
            }
        }
        refreshOptionIndex++;
        if (newOption == null){
            newOption = Instantiate<DropDownOption>(optionTemplate);   
            newOption.gameObject.SetActive(true);

            RectTransform r = newOption.GetComponent<RectTransform>();
            r.SetParent(content, false);
            options.Add(newOption);
            subLists.Add(null);
        }
        newOption.Text.text = name;
        return newOption;
    }

    ScrollingDropDown addSublist(){
        int index = refresh ? refreshOptionIndex - 1 : options.Count - 1;
        
        ScrollingDropDown dropDown = null;
        if (refresh){
            if (index < options.Count){
                dropDown = subLists[index];
            }
        }
        if (dropDown == null){
            dropDown = Instantiate<ScrollingDropDown>(childListPrefab);
            dropDown.Parent = this;
            RectTransform r = dropDown.GetComponent<RectTransform>();
            r.SetParent(options[index].transform, false);

            r.anchorMin = rect.pivot.y * Vector2.up + Vector2.right;
            r.anchorMax = r.anchorMin + Vector2.right;

            r.pivot = rect.pivot;
            r.anchoredPosition = Vector2.zero;
            r.GetComponent<Canvas>().sortingOrder++;
        }
        subLists[index] = dropDown;
        return dropDown;
    }

    public void Clear(){
        for (int i = 0; i < options.Count; i++){
            DropDownOption b = options[i];
            if (subLists[i] != null){
                Destroy(subLists[i].gameObject);
            }
            Destroy(b.gameObject);
        }
        options.Clear();
        subLists.Clear();
    }

    IEnumerator goToHeight(float newHeight){
        float startHeight = rect.sizeDelta.y;

        Timer t = new Timer(dropTime);
        while (!t.IsFinished()){
            yield return 0;
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Lerp(startHeight, newHeight, LerpUtility.HemiSpherical(t.Percent())));
        }
    }
}
