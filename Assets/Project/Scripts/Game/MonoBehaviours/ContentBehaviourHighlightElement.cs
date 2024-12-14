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
            parentTransform.gameObject.SetActive(false);
        else if (GameManager.Instance.HighlightedBehaviour.ContentData.Highlightable)
        {
            parentTransform.gameObject.SetActive(true);
            if (Physics.Raycast(GameManager.Player.ActiveCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity))
                transform.position = GameManager.Player.ActiveCamera.WorldToScreenPoint(hit.point);

            contentBehaviourName.SetText(GameManager.Instance.HighlightedBehaviour.ContentData.DisplayName);
            iconGame.sprite = GameManager.Instance.HighlightedBehaviour.ContentData.DisplayIcon;
            backgroundImage.color = GameManager.Instance.HighlightedBehaviour.ContentData.DisplayColor;
        }
    }
}
