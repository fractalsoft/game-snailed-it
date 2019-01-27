using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeImageOnHealth : MonoBehaviour
{
    public static ChangeImageOnHealth instance;
    public Sprite Image100;
    public Sprite Image75;
    public Sprite Image50;
    public Sprite Image25;
    public Sprite full;
    public Sprite empty;
    public Image image;
    public Image slider;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    public void setHealth(float healthPercent)
    {
        if (image && Image100 && Image75 && Image50 && Image25)
        {
            if (healthPercent <= 25)
            {
                image.sprite = Image25;
            }
            else if (healthPercent <= 50)
            {
                image.sprite = Image50;
            }
            else if (healthPercent <= 75)
            {
                image.sprite = Image75;
            }
            else
            {
                image.sprite = Image100;
            }
        }
        if (slider && full && empty)
        {
            slider.fillAmount = healthPercent / 100;
        }
    }
}
