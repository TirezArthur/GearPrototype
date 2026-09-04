using UnityEngine;
using UnityEngine.UI;

public static class RectTransformExtension
{
    public static Canvas GetCanvas(this RectTransform rectTransform)
    {
        return rectTransform.root.GetComponentInChildren<Canvas>();
    }

    public static CanvasScaler GetCanvasScaler(this RectTransform rectTransform)
    {
        return rectTransform.root.GetComponentInChildren<CanvasScaler>();
    }
}
