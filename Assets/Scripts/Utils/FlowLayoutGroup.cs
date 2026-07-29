using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(RectTransform))]
public class FlowLayoutGroup : MonoBehaviour {
    [SerializeField] private float rowHeight;
    [SerializeField] private float horizontalSpacing;
    [SerializeField] private float verticalSpacing;

    private RectTransform rectTransform;

    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
    }

    private void CalculateLayout() {
        if (transform.childCount == 0) {
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, 0f);
            return;
        }

        Canvas.ForceUpdateCanvases();

        float containerWidth = rectTransform.rect.width;
        List<RectTransform> children = new List<RectTransform>();
        foreach(Transform child in transform) {
            if (child is RectTransform)
                children.Add(child as RectTransform);
        }

        float currentRowWidth = children[0].rect.width;
        List<KeyValuePair<int, float>> rowFinalIndexAndWidth = new List<KeyValuePair<int, float>>(); 
        for(int i = 1; i < children.Count; i++) {
            if (currentRowWidth + children[i].rect.width + horizontalSpacing > containerWidth) {
                rowFinalIndexAndWidth.Add(new KeyValuePair<int, float>(i - 1, currentRowWidth));
                currentRowWidth = children[i].rect.width;
            }
            else
                currentRowWidth += children[i].rect.width + horizontalSpacing;
        }
        rowFinalIndexAndWidth.Add(new KeyValuePair<int, float>(children.Count - 1, currentRowWidth));

        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, rowHeight * rowFinalIndexAndWidth.Count + (verticalSpacing * (rowFinalIndexAndWidth.Count - 1)));

        int startingIndex = 0;
        for (int i = 0; i < rowFinalIndexAndWidth.Count; i++) {
            float rowOffset = (containerWidth - rowFinalIndexAndWidth[i].Value) / 2;
            float elementOffset = rowOffset;
            float rowPositionY = -(i * rowHeight + (i * verticalSpacing));
            for (int j = startingIndex; j <= rowFinalIndexAndWidth[i].Key; j++) {
                children[j].localPosition = new Vector2(elementOffset, rowPositionY);
                elementOffset += children[j].rect.width + horizontalSpacing;
            }
            startingIndex = rowFinalIndexAndWidth[i].Key + 1;
        }
    }

    private void OnTransformChildrenChanged() {
        CalculateLayout();
    }
}
