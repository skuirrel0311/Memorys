using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MapManager : MonoBehaviour
{
    protected Image playerImage, switchImage;
    Image[] enemyImages;
    GameObject[] enemyObjects;
    //GameObject[] destroyObjects;
    protected GameObject playerObj, switchObj;

    //動的に生成するImage
    [SerializeField]
    Image enemyImage = null;
    //[SerializeField]
    //Image destroyObjectImage = null;

    //表示するステージの大きさ
    [SerializeField]
    Vector2 drawingAreaSize = new Vector2(96.0f, 96.0f);
    Vector2 convertRate;

    [SerializeField]
    float showEnemyDistance = 30.0f;
    [SerializeField]
    bool isShowEnemy = true;

    public virtual void Start()
    {
        playerImage = transform.FindChild("Player").GetComponent<Image>();
        switchImage = transform.FindChild("Switch").GetComponent<Image>();

        playerObj = GameObject.FindGameObjectWithTag("Player");
        switchObj = GameObject.FindGameObjectWithTag("Target");

        //マップでの座標に変換するときのレート(四角形でも円形でもレートはこの計算であってる。)
        convertRate = new Vector2(GetComponent<RectTransform>().sizeDelta.x / drawingAreaSize.x, GetComponent<RectTransform>().sizeDelta.y / drawingAreaSize.y);

        if (!isShowEnemy) return;
        enemyObjects = GameObject.FindGameObjectsWithTag("Enemy");
        enemyImages = new Image[enemyObjects.Length];

        for (int i = 0; i < enemyImages.Length; i++)
        {
            enemyImages[i] = Instantiate(enemyImage).GetComponent<Image>();
            enemyImages[i].transform.parent = transform;
        }
    }

    public virtual void Update()
    {

    }

    protected void DrawEnemy(Vector3 centerPosition)
    {
        if (!isShowEnemy) return;
        for (int i = 0; i < enemyImages.Length; i++)
        {
            if (!IsNear(enemyObjects[i].transform.position, playerObj.transform.position, showEnemyDistance))
            {
                //遠かったら表示しない。
                enemyImages[i].gameObject.SetActive(false);
                continue;
            }
            enemyImages[i].gameObject.SetActive(true);
            enemyImages[i].rectTransform.anchoredPosition = ConvertMapPosition(enemyObjects[i].transform.position - centerPosition);
            enemyImages[i].rectTransform.localRotation = ConvertMapRotation(enemyObjects[i].transform.eulerAngles - centerPosition);
        }
    }

    //public void ChangedDestroyObject(List<GameObject> destroyObjects)
    //{
    //    this.destroyObjects = destroyObjects.ToArray();
    //}

    protected Vector2 ConvertMapPosition(Vector3 position)
    {
        return new Vector2(position.x * convertRate.x, position.z * convertRate.y);
    }

    protected Quaternion ConvertMapRotation(Vector3 rotation)
    {
        return Quaternion.Euler(0, 0, rotation.y);
    }

    //pos1とpos2の距離がdistanceより小さかったらtrueを返す
    protected bool IsNear(Vector3 position1, Vector3 position2, float distance)
    {
        return (position1 - position2).magnitude < distance;
    }
}
