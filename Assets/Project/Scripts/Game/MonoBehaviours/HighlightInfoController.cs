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
    private IHighlightable trackingBehaviour;

    [SerializeField] private float yOffset = 35;

    [SerializeField] private HighlightListingElement highlightListingElementPrefab;
    [SerializeField] private List<HighlightListingElement> highlightListingElements = new List<HighlightListingElement>();

    private void Awake()
    {
        DisableInfo();
        GameManager.OnHighlightChanged.AddListener(OnHighlightChanged);
    }

    private void OnHighlightChanged(IHighlightable contentBehaviour)
    {
        trackingBehaviour = contentBehaviour;
        if (trackingBehaviour != null && trackingBehaviour.IsHighlightable() == false)
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
        foreach (HighlightListingElement element in highlightListingElements)
            element.DisableHighlightListing();
    }

    private void EnableInfo()
    {
        foreach (HighlightListingElement element in highlightListingElements)
            element.DisableHighlightListing();

        List<ContentDisplayListing> listings = trackingBehaviour.GetDisplayListings();
        for (int i = 0; i < listings.Count; i++)
            if (i < highlightListingElements.Count)
                highlightListingElements[i].EnableHighlightListing(listings[i]);
    }
}
