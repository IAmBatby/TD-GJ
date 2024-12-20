using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ContentDisplayListing
{
    private List<ContentDisplayInfo> contentDisplayInfos;

    public ContentDisplayListing(params ContentDisplayInfo[] newDisplayInfos)
    {
        contentDisplayInfos = new List<ContentDisplayInfo>(newDisplayInfos);
    }

    public ContentDisplayListing(List<ContentDisplayInfo> contentDisplayInfos)
    {
        this.contentDisplayInfos = new List<ContentDisplayInfo>(contentDisplayInfos);
    }

    public void AddContentDisplayInfo(ContentDisplayInfo contentDisplayInfo) => contentDisplayInfos.Add(contentDisplayInfo);
    public void RemoveContentDisplayInfo(ContentDisplayInfo contentDisplayInfo) => contentDisplayInfos.Remove(contentDisplayInfo);

    public List<ContentDisplayInfo> GetDisplayInfos() => new List<ContentDisplayInfo>(contentDisplayInfos);
}
