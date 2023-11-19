using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    // Start is called before the first frame update

    public Text controlsBtnText, musicBtnText, sfxBtnText;

    private string controlType;

    private bool isMusic, isSfx;
    void Start()
    {
        controlType = PrefManager.GetControlType();
        isMusic = PrefManager.IsMusicOn();
        isSfx = PrefManager.IsSfxOn();
        refresh();

    }
    private void refresh()
    {
        controlsBtnText.text = "Control: " + controlType;
        musicBtnText.text = "Music: " + getOnOff(isMusic);
        sfxBtnText.text = "SFX: "+getOnOff(isSfx);  
    }

    private string getOnOff(bool value)
    {
        return value ? "on" : "off";
    }
    public void flipControl()
    {
        //Debug.Log("flipped");
        controlType = controlType == Const.CONTROL_TILT ? Const.CONTROL_BUTTONS : Const.CONTROL_TILT;
        refresh();
        PrefManager.SetControlType  (controlType);
    }

    public void flipMusic()
    {
        isMusic = !isMusic;
        refresh();
        PrefManager.SetIsMusicOn(isMusic);
    }

    public void flipSfx()
    {
        isSfx = !isSfx;
        refresh();
        PrefManager.SetIsSfxOn(isSfx);
    }
}
