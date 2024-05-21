using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingControl : MonoBehaviour
{
    [SerializeField] private float blackFadeTime = .3f;
    private Volume volume;
    private ChannelMixer channelMixer;
    private PixelisationVolume pixelisationVolume;
    private static PostProcessingControl _instance;
    private float blackFadeVal;
    private float pixelisationVal;
    private Tween tween;
    public static PostProcessingControl Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("The PostProcessingControl Singlton is NULL.");
            }
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        volume = GetComponent<Volume>();
        if (!volume.profile.TryGet(out channelMixer))
        {
            Debug.LogError("Channel Mixer not found in the Volume component.");
            return;
        }
        if (!volume.profile.TryGet(out pixelisationVolume))
        {
            Debug.LogError("Pixelisation Volume not found in the Volume component.");
            return;
        }
    }

    public void blackFadeIn(Action callBack = null, bool pixelate = true)
    {

        if (pixelate)
        {
            pixelisationVal = .01f;
            DOTween.To(() => this.pixelisationVal, x => this.pixelisationVal = x, 30f, blackFadeTime);
        }

        blackFadeVal = 200f;
        tween = DOTween.To(() => this.blackFadeVal, x => this.blackFadeVal = x, 0f, blackFadeTime);
        tween.OnUpdate(() => onBlackFadeUpdate(pixelate));
        tween.OnComplete(() => onBlackFadeComplete(callBack));

    }

    public void blackFadeOut(Action callBack = null, float delay = 0f, bool pixelate = true)
    {
        if (pixelate)
        {
            pixelisationVal = 30f;
            DOTween.To(() => this.pixelisationVal, x => this.pixelisationVal = x, .01f, blackFadeTime).SetDelay(delay);
        }

        blackFadeVal = 0f;
        tween = DOTween.To(() => this.blackFadeVal, x => this.blackFadeVal = x, 100f, blackFadeTime);
        tween.OnUpdate(() => onBlackFadeUpdate(pixelate));
        tween.OnComplete(() => onBlackFadeComplete(callBack));
        tween.SetDelay(delay);
    }

    void onBlackFadeUpdate(bool pixelate)
    {
        if (channelMixer != null)
        {
            channelMixer.redOutRedIn.Override(blackFadeVal);
            channelMixer.blueOutBlueIn.Override(blackFadeVal);
            channelMixer.greenOutGreenIn.Override(blackFadeVal);
        }

        if (pixelate)
        {
            pixelisationVolume.Pixelisation.Override(pixelisationVal);
        }
    }

    void onBlackFadeComplete(Action callBack = null)
    {
        if (callBack != null)
        {
            callBack();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
