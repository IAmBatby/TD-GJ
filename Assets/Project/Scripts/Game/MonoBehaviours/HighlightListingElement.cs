using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class HighlightListingElement : MonoBehaviour
{
    [SerializeField] private List<HighlightInfoElement> highlightInfoElementsList = new List<HighlightInfoElement>();

    public void EnableHighlightListing(ContentDisplayListing listing)
    {
        List<ContentDisplayInfo> infos = listing.GetDisplayInfos();
        for (int i = 0; i < infos.Count; i++)
            if (i < highlightInfoElementsList.Count)
                highlightInfoElementsList[i].EnableHighlightInfo(infos[i]);
    }

    public void DisableHighlightListing()
    {
        for (int i = 0; i < highlightInfoElementsList.Count; i++)
            highlightInfoElementsList[i].DisableHighlightInfo();
    }
}

