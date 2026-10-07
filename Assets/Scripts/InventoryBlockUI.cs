using System;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;
public class InventoryBlockUI : MonoBehaviour
{
    int _value;
    int _rotation;
    ItemManager manager;
    [SerializeField] EventTrigger _eventtrigger;
    [SerializeField] Image cornerup;
    [SerializeField] Image cornerleft;
    [SerializeField] Image cornerdown;
    [SerializeField] Image cornerright;
    [SerializeField] Image fullimage;
    [SerializeField] RawImage background;
    [SerializeField] Sprite unavailable; //add
    [SerializeField] Sprite broken; //add
    [SerializeField] Sprite sourceEngineMissingTexture; //replace
    /// <summary>
    /// Set border image values on block
    /// </summary>
    /// <param name="border">1 = up - 2 = down - 3 = left - 4 = right</param>
    public void SetBorder(ushort border, bool value)
    {
        switch (border)
        {
            case 1:
                cornerup.enabled = value;
                break;
            case 2:
                cornerdown.enabled = value;
                break;
            case 3:
                cornerleft.enabled = value;
                break;
            case 4:
                cornerright.enabled = value;
                break;
            default:
                throw new ArgumentOutOfRangeException("border value must be between 1 and 4");
        }
    }
    public int Value
    { 
        get { return _value; }
        set // Value being set means an update in the slot has happened
        { 
            _value = value;
            if (manager == null) { manager = ItemManager.Instance; }
            ChangeImage();
        }
    }
    public int Index
    {
        get;
        set;
    }
    public int Rotation
    {
        get { return _rotation; }
        set 
        {
            _rotation = value;
            SetRotation();
        }
    }
    public EventTrigger EventTrigger
    {
        get { return _eventtrigger; }
        set { _eventtrigger = value; }
    }

    public Image Cornerup { get => cornerup; set => cornerup = value; }
    public Image Cornerleft { get => cornerleft; set => cornerleft = value; }
    public Image Cornerdown { get => cornerdown; set => cornerdown = value; }
    public Image Cornerright { get => cornerright; set => cornerright = value; }

    public void SetBorderColor(Color value)
    {
        cornerdown.color = value;
        cornerup.color = value;
        cornerleft.color = value;
        cornerright.color = value;
    }

    public void SetBackgroundAlpha(float value)
    {
        Color temp = background.color;
        temp.a = value;
        background.color = temp;
    }
    void ChangeImage()
    {
        switch (Value)
        {
            case -3:

                break;

            case -2:

                break;

            case -1:
                fullimage.enabled = false;
                SetBorder(1, true);
                SetBorder(2, true);
                SetBorder(3, true);
                SetBorder(4, true);
                break;

            default:
                Sprite itemImg = manager.GetItem(Value).GetBlock(Index);
                fullimage.enabled = true;
                if (itemImg == null)
                {
                    fullimage.sprite = sourceEngineMissingTexture;
                }
                else
                {
                    fullimage.sprite = itemImg;
                }
                break;
        }
    }
    void SetRotation()
    {
        switch (_rotation) //change this whole fucking thing
        {
            case 1:
                if(fullimage.transform.eulerAngles.z != -90)
                {
                    fullimage.transform.Rotate(0, 0, 270, Space.Self);
                }
                break;
            case 2:
                break;
            case 3:
                fullimage.transform.Rotate(0, 0, 90, Space.Self);
                break;
            case 4:
                fullimage.transform.Rotate(0, 0, 180, Space.Self);
                break; 
        }
    }
}
