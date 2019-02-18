using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//[AddComponentMenu("NGUI/Interaction/Play Sound HYZ")]
public class PlaySoundButton : MonoBehaviour, IPointerClickHandler
{
    public enum Trigger
    {
        OnClick,
        //OnMouseOver,
        //OnMouseOut,
        //OnPress,
        //OnRelease,
        //Custom,
        //OnEnable,
        //OnDisable,
    }

    public enum UISoundType
    {
        Click,
    }

    public Trigger trigger = Trigger.OnClick;
    public UISoundType soundType = UISoundType.Click;

    private Button btn;

    bool IsCanPlay
    {
        get
        {
            if (!enabled) return false;

            
            return !btnNo;
        }
    }
    bool btnNo;

    private void Awake()
    {
        btn = GetComponent<Button>();
        if (btn == null)
        {
            btnNo = true;
            return;
        }
        btnNo = !btn.isActiveAndEnabled;

        //btn.onClick.AddListener(OnClick);
    }
    private void OnDestroy()
    {
        //if (btn == null) return;
        //btn.onClick.RemoveAllListeners();
        
    }
    void OnClick()
    {
        if (trigger == Trigger.OnClick)
            Play();
    }

    //void OnSelect(bool isSelected)
    //{
    //    if (canPlay && (!isSelected || UICamera.currentScheme == UICamera.ControlScheme.Controller))
    //        OnHover(isSelected);
    //}

    public void Play()
    {
        if (!IsCanPlay) return;
        if (soundType == UISoundType.Click)
        {
            //GameSoundPlayer.Instance.PlaySoundEffect("点击按钮");//.SoundEffectPlayer.Play("点击按钮");
            Debug.Log("点击 按钮 声音");
            GameSoundPlayer.Instance.PlaySoundEffect("Button");
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (btn != null)
        {
            if (btn.interactable)
            {
                Play();
            }
        }
    }
}
