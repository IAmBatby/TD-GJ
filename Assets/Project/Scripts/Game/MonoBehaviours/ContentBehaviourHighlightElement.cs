using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContentBehaviourHighlightElement : MonoBehaviour
{
    [SerializeField] private Transform parentTransform;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image iconGame;
    [SerializeField] private TextMeshProUGUI contentBehaviourName;

    private void Awake()
    {
        parentTransform.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (GameManager.Instance.HighlightedBehaviour == null)
        {
            parentTransform.gameObject.SetActive(false);
            return;
        }
        ScriptableContent highlightedContent = GameManager.Instance.HighlightedBehaviour.ContentData;
        if (highlightedContent.Highlightable == false) return;
        parentTransform.gameObject.SetActive(true);
        if (Physics.Raycast(GameManager.Player.ActiveCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity))
            transform.position = GameManager.Player.ActiveCamera.WorldToScreenPoint(hit.point);

        contentBehaviourName.SetText(highlightedContent.GetDisplayName());
        iconGame.sprite = highlightedContent.GetDisplayIcon();
        backgroundImage.color = highlightedContent.GetDisplayColor();
    }
}
