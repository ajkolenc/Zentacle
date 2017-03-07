using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class DropDownOption : MonoBehaviour, IPointerEnterHandler {

    public Text Text;
    public Button Option, Delete;
    public Image SublistArrow;
    public System.Action OnHover, OnUnhover;


    #region IPointerEnterHandler implementation
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (OnHover != null){
            OnHover.Invoke();
        }
    }
    #endregion
}
