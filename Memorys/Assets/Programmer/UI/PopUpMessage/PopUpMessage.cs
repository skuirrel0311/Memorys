using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PopUpMessage : MonoBehaviour
{
    RectTransform canvasRect;
    [SerializeField]
    Image messagePrefab = null;

    [SerializeField]
    float UpperOffset =  2.0f;
    [SerializeField]
    protected Vector3 offset = Vector3.zero;
    [SerializeField]
    protected Vector3 origin;
    
    public bool IsViewMessage = false;

    void Start()
    {
        canvasRect = GameObject.Find("Canvas").GetComponent<RectTransform>();
        origin = transform.position;
        messagePrefab = (Image)Instantiate(messagePrefab, canvasRect.transform);
    }

    public void Update()
    {
        DrawMessage();
    }

    public virtual void DrawMessage()
    {
        messagePrefab.gameObject.SetActive(IsViewMessage);

        if (!IsViewMessage) return;

        messagePrefab.rectTransform.anchoredPosition = GetPopUpPosition();
    }

    protected virtual Vector2 GetPopUpPosition()
    {
        Vector2 popUpPosition = Camera.main.WorldToViewportPoint(origin + (Vector3.up * UpperOffset) + offset);
        popUpPosition.x = (popUpPosition.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f);
        popUpPosition.y = (popUpPosition.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f);
        return popUpPosition;
    }
}
