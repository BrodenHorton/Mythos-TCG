using System;
using UnityEngine;

[RequireComponent (typeof(RectTransform))]
public partial class LogContainerUI : MonoBehaviour {
    public event EventHandler<FloatEventArgs> OnLogAdded;
    public event EventHandler<FloatEventArgs> OnLogRemoved;

    [SerializeField] private int maxLogCount;
    [SerializeField] private bool shouldForceExpandChildWidth;

    private RectTransform rectTransform;

    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Start() {
        TrimExcessLogs();
        UpdateContainer();
    }

    public void AddLog(LogUI logUI) {
        logUI.transform.parent = transform;
        UpdateContainer();
        OnLogAdded?.Invoke(this, new FloatEventArgs(logUI.GetComponent<RectTransform>().sizeDelta.y));
        TrimExcessLogs();
    }

    private void UpdateContainer() {
        float cumulativeHeight = 0f;
        for(int i = transform.childCount - 1; i >= 0; i--) {
            RectTransform childTransform = transform.GetChild(i).GetComponent<RectTransform>();
            if (childTransform == null)
                continue;

            childTransform.anchoredPosition = Vector2.zero;
            childTransform.pivot = Vector2.zero;
            childTransform.localPosition = new Vector3(0f, cumulativeHeight, 0f);
            cumulativeHeight += childTransform.sizeDelta.y;
            if(shouldForceExpandChildWidth)
                childTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, childTransform.sizeDelta.y);
        }
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, GetContainerHeight());
    }

    private void TrimExcessLogs() {
        if (transform.childCount <= maxLogCount)
            return;

        int excessLogCount = transform.childCount - maxLogCount;
        for(int i = 0; i < excessLogCount; i++) {
            float height =  transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.y;
            Transform child = transform.GetChild(0);
            child.parent = null;
            Destroy(child.gameObject);
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, GetContainerHeight());
            OnLogRemoved?.Invoke(this, new FloatEventArgs(height));
        }
    }

    private float GetContainerHeight() {
        float cumulativeHeight = 0f;
        for (int i = 0; i < transform.childCount; i++) {
            RectTransform childTransform = transform.GetChild(i).GetComponent<RectTransform>();
            if (childTransform == null)
                continue;

            cumulativeHeight += childTransform.sizeDelta.y;
        }

        return cumulativeHeight;
    }
}
