using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HighlightInfoController : MonoBehaviour
{
    [SerializeField] private HighlightInfoElement highlightInfoElementPrefab;
    [SerializeField] private Transform movementParent;
    [SerializeField] private List<HighlightInfoElement> highlightInfoElements = new List<HighlightInfoElement>();
    private ContentBehaviour trackingBehaviour;

    [SerializeField] private float yOffset = 35;

    private void Awake()
    {
        DisableInfo();
        GameManager.Instance.OnHighlightChanged.AddListener(OnHighlightChanged);
    }

    private void OnHighlightChanged(ContentBehaviour contentBehaviour)
    {
        trackingBehaviour = contentBehaviour;
        if (trackingBehaviour != null && trackingBehaviour.ContentData.Highlightable == false)
            trackingBehaviour = null;

        if (trackingBehaviour == null)
            DisableInfo();
        else
            EnableInfo();
    }

    private void Update()
    {
        if (trackingBehaviour != null && Physics.Raycast(GameManager.Player.ActiveCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity))
            movementParent.position = GameManager.Player.ActiveCamera.WorldToScreenPoint(hit.point);
    }

    private void DisableInfo()
    {
        foreach (HighlightInfoElement element in highlightInfoElements)
            element.DisableHighlightInfo();
    }

    private void EnableInfo()
    {
        foreach (HighlightInfoElement element in highlightInfoElements)
            element.DisableHighlightInfo();

        List<ContentDisplayInfo> infos = trackingBehaviour.GetContentDisplayInfos();
        for (int i = 0; i < infos.Count; i++)
            if (i < highlightInfoElements.Count)
                highlightInfoElements[i].EnableHighlightInfo(infos[i]);

    }
}
