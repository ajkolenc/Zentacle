using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class ColorPicker : UIWindow, IPointerDownHandler {

    [SerializeField] Image targetImage;
    [SerializeField] Image saturationValueArea;
    [SerializeField] RectTransform cursor;
    [SerializeField] Slider hueSlider;
    [SerializeField] int colorDisplayRes = 400;
    RectTransform cursorParent;
    Texture2D hueTexture;
    bool isSetting = false;
    float lastH = 0f;

    public Color Default;
    public event System.Action<Color> OnColorChanged;

    public Color GetColor(){
        float h = hueSlider.value;
        float s = cursor.anchoredPosition.x / cursorParent.rect.width;
        float v = cursor.anchoredPosition.y / cursorParent.rect.height;
        return Color.HSVToRGB(h,s,v);
    }

    void Awake(){
        cursorParent = cursor.parent as RectTransform;

        SetColor(Default, false);
    }

    #region IPointerDownHandler implementation

    public void OnPointerDown(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    #endregion

    public void SetColor(Color c, bool triggerEvent = true){
        isSetting = true;
        float h, s, v;
        Color.RGBToHSV(c, out h, out s, out v);
        if (s == 0f && v == 1f){
            h = lastH;
        }
        lastH = h;
        hueSlider.value = h;
        Vector2 pos = new Vector2(s * cursorParent.rect.width, v * cursorParent.rect.height);
        cursor.anchoredPosition = pos;

        Color maxSaturatedValue = Color.HSVToRGB(h, 1f, 1f);
        saturationValueArea.color = maxSaturatedValue;

        if (targetImage != null){
            targetImage.color = c;
        }
        isSetting = false;
        if (triggerEvent && OnColorChanged != null){
            OnColorChanged(c);
        }
    }

    public override void Randomize(){
        SetColor(Random.ColorHSV(0, 1, 0, 1, 0, 1, 1, 1));
    }

    public void OnSliderChange(Slider s){
        if (!isSetting){
            lastH = s.value;
            SetColor(GetColor());
        }
    }

    void Update(){
        if (IsExpanded && Input.GetMouseButton(0) && EventSystem.current.currentSelectedGameObject == cursorParent.gameObject){
            Vector2 mousePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(cursorParent, Input.mousePosition, null, out mousePos);
            mousePos += cursorParent.rect.max;
            mousePos.x = Mathf.Clamp(mousePos.x, 0, cursorParent.rect.width);
            mousePos.y = Mathf.Clamp(mousePos.y, 0, cursorParent.rect.height);
            cursor.anchoredPosition = mousePos;
            if (targetImage != null){
                targetImage.color = GetColor();
            }
            if (OnColorChanged != null){
                OnColorChanged(targetImage.color);
            }
        }
    }
}