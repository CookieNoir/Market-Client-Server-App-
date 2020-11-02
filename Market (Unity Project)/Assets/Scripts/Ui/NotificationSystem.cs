using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NotificationSystem : UiMovement
{
    public static NotificationSystem instance;

    public enum NotificationTypes { positive, negative }
    public Text textObject;
    public MaskableGraphic background;
    [Header("Positive")]
    public Color positiveForegroundColor;
    public Color positiveBackgroundColor;
    [Header("Negative")]
    public Color negativeForegroundColor;
    public Color negativeBackgroundColor;

    private IEnumerator refreshVisibility;

    protected override void Awake()
    {
        if (instance == null)
        {
            instance = this;
            base.Awake();
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying component");
            Destroy(this);
        }
    }

    private void Start()
    {
        refreshVisibility = RefreshVisibility();
    }

    public void ChangeScale(bool value)
    {
        if (value)
        {
            background.rectTransform.anchorMax = new Vector3(0.7f, 0f);
        }
        else
        {
            background.rectTransform.anchorMax = new Vector3(1f, 0f);
        }
    }

    public void Notify(NotificationTypes type, string textString)
    {
        switch (type)
        {
            case NotificationTypes.positive:
                {
                    SetValues(textString, positiveForegroundColor, positiveBackgroundColor);
                    break;
                }
            case NotificationTypes.negative:
                {
                    SetValues(textString, negativeForegroundColor, negativeBackgroundColor);
                    break;
                }
        }
        StopCoroutine(refreshVisibility);
        refreshVisibility = RefreshVisibility();
        StartCoroutine(refreshVisibility);
    }

    private IEnumerator RefreshVisibility()
    {
        StopCoroutine(activeMovement);
        activeMovement = SmoothMove(GetComponent<RectTransform>().anchoredPosition, newPosition, movingFunction);
        StartCoroutine(activeMovement);
        yield return new WaitForSeconds(3f);
        StopCoroutine(activeMovement);
        activeMovement = SmoothMove(GetComponent<RectTransform>().anchoredPosition, defaultPosition, movingFunction);
        StartCoroutine(activeMovement);
    }

    private void SetValues(string textString, Color mainColor, Color backgroundColor)
    {
        textObject.text = textString;
        textObject.color = mainColor;
        background.color = backgroundColor;
    }
}
