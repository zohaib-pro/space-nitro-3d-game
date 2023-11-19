
using System;
using System.Diagnostics;
using UnityEngine.UI;

public class Booster
{
    

    public Booster(float forTime, Slider slider)
    {
        this.forTime = forTime;
        this.slider = slider;
        this.currentTime = 0f;

        this.isActive = true;

        slider.gameObject.SetActive(true);
    }
    private float forTime;
    private float currentTime;

    public Boolean isActive { get; set; }

    public Slider slider { get; set; }

    public void update(float deltaTime)
    {
        if (isActive) {
            currentTime += deltaTime * 1000;
            float progress = getProgress();
            slider.value = progress;
            if (progress <= 0f )
            {
                slider.gameObject.SetActive(false);
                this.isActive = false;
            }
        }
    }

    public float getProgress()
    {
        return (forTime - currentTime) / forTime;
    }

}
