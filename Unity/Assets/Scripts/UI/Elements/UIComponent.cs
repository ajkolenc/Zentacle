using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public abstract class UIComponent : MonoBehaviour {
    
    [System.NonSerialized] public UIComponent Parent;
    public int Depth {
        get {
            int depth = 0;
            UIComponent parent = Parent;
            while (parent != null){
                depth++;
                parent = Parent.Parent;
            }
            return depth;
        }
    }
    [HideInInspector] public bool IsExpanded = false;
    [SerializeField] public bool CloseOnOtherSelected = true;
    protected RectTransform rect;
    protected int expandFrame;

    protected virtual void Start(){
        rect = GetComponent<RectTransform>();
        IsExpanded = false;
        UIManager.Register(this);
    }

    protected virtual void LateUpdate(){
        if (Input.GetMouseButtonUp(0)){
            GameObject selected = EventSystem.current.currentSelectedGameObject;
            onUIObjectSelected(selected);
        }
    }

    protected virtual void onUIObjectSelected(GameObject obj){
        if (CloseOnOtherSelected){
            Transform parent = null;
            if (obj != null)
            {
                parent = obj.transform;
            }
            while (parent != transform && parent != null){
                parent = parent.parent;
            }
            if (parent == null){
                if (IsExpanded && expandFrame != Time.frameCount){
                    Contract();
                }
            }
        }
    }

    public virtual Coroutine Expand(){
        if (!IsExpanded){
            IsExpanded = true;
            expandFrame = Time.frameCount;
        }
        return null;
    }

    public virtual Coroutine Contract(){
        if (IsExpanded){
            IsExpanded = false;
        }
        return null;
    }

    public void Close(){
        Contract();
    }
}
