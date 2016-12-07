using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHasItem : MonoBehaviour
{
    public bool HasItem;

    GameObject item = null;

    void Start()
    {
    }

    void Update()
    {
        if (!HasItem) return;
    }

    //itemを持たせる
    public void ToHaveItem(GameObject item)
    {
        if (HasItem) return;

        this.item = item;
        Vector3 offset = transform.forward * -1.0f;
        item.transform.position = transform.position + (offset * 0.5f);
        item.transform.rotation = transform.rotation;
        item.transform.parent = transform;

        item.GetComponent<SphereCollider>().enabled = false;
        item.GetComponent<BoxCollider>().enabled = false;
        item.GetComponent<BreakMessage>().IsViewMessage = false;
        item.GetComponent<BreakMessage>().enabled = false;

        HasItem = true;
    }
    
    void OnTriggerStay(Collider col)
    {
        if (col.gameObject.tag != "Goal") return;

        if (!HasItem) return;

        if(MyInputManager.GetButtonDown(MyInputManager.Button.X))
        {
            GameManager.I.PutFloor();

            Destroy(item);
            item = null;
            HasItem = false;
        }
    }
}
