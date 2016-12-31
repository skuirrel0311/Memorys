using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

public class CameraContoller : MonoBehaviour
{
    [SerializeField]
    public Transform targetObject = null;

    float distance = 19.0f;    //カメラとターゲットの距離
    [SerializeField]
    float rotationSpeedX = 150.0f;
    [SerializeField]
    float rotationSpeedY = 100.0f;

    /// <summary>
    /// 緯度
    /// </summary>
    [SerializeField]
    float latitude = 15.0f;
    /// <summary>
    /// 経度
    /// </summary>
    [SerializeField]
    float longitude = 180.0f;

    /// <summary>
    /// 緯度の最大値
    /// </summary>
    [SerializeField]
    float maxLatitude = 89.0f;
    /// <summary>
    /// 緯度の最小値
    /// </summary>
    [SerializeField]
    float minLatitude = -85.0f;
    /// <summary>
    /// Slerpを開始する緯度の値
    /// </summary>
    [SerializeField]
    float startSlerpLatitude = 30.0f;

    //カメラを制御するか？
    public bool IsWork = true;
    [SerializeField]
    bool CanMouseControl = false;

    void Start()
    {
        //ターゲットが入ってなかったらプレイヤーを探す
        if (targetObject == null)
        {
            targetObject = GameObject.FindGameObjectWithTag("Player").transform;
        }
        if(targetObject == null)
        {
            targetObject = GameObject.Find("Player").transform;
        }
    }

    void Update()
    {
        if (targetObject == null) return;
        
        if(distance>=5)
        {
            distance -= Time.deltaTime*7.0f;
        }

        if (!IsWork) return;
        //右スティックで回転
        Vector2 rightStick = GetInputVector();
        if (GameManager.I.IsPlayStop) rightStick = Vector2.zero;
        longitude += rightStick.x * rotationSpeedX * Time.deltaTime;
        // - はお好み
        latitude -= rightStick.y * rotationSpeedY * Time.deltaTime;

        //経度には制限を掛ける
        latitude = Mathf.Clamp(latitude, minLatitude, maxLatitude);
        longitude = longitude % 360.0f;
        
        SphereCameraControl();
    }

    Vector2 GetInputVector()
    {
        Vector2 rightStick = MyInputManager.GetAxis(MyInputManager.Axis.RightStick);

        if(rightStick == Vector2.zero)
        {
            if (Input.GetKey(KeyCode.LeftArrow)) rightStick.x = -1;
            if (Input.GetKey(KeyCode.RightArrow)) rightStick.x = 1;
            if (Input.GetKey(KeyCode.UpArrow)) rightStick.y = 1;
            if (Input.GetKey(KeyCode.DownArrow)) rightStick.y = -1;
        }

        if(CanMouseControl)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            rightStick.x = Input.GetAxis("MouseX");
            rightStick.y = Input.GetAxis("MouseY");
        }

        return rightStick;
    }

    void SphereCameraControl()
    {
        if (latitude < startSlerpLatitude)
        {
            //リープ開始
            Vector3 vec1 = SphereCoordinate(longitude, startSlerpLatitude, distance);
            //リープ終了時の座標
            Vector3 vec2 = SphereCoordinate(longitude, minLatitude, distance);
            vec2.y = 0.0f;
            
            float t;
            //(開始位置からの移動量) / (全体の移動量) = 0 ～ 1
            if (latitude >= 0.0f)
                t = (startSlerpLatitude - latitude) / (-minLatitude + startSlerpLatitude);
            else
                t = ((-latitude) + startSlerpLatitude) / (-minLatitude + startSlerpLatitude);
            
            transform.position = targetObject.position + Vector3.Slerp(vec1, vec2, t);
        }
        else
        {
            //カメラが地面にめり込まない場合は球体座標をそのまま使う
            transform.position = targetObject.position + SphereCoordinate(longitude, latitude, distance);
        }

        transform.LookAt(targetObject.position + (Vector3.up * 2.0f));
    }

    /// <summary>
    /// 指定した角度の球体座標を返します
    /// </summary>
    /// <param name="longitude">経度</param>
    /// <param name="latitude">緯度</param>
    /// <returns></returns>
    public static Vector3 SphereCoordinate(float longitude, float latitude, float distance)
    {
        Vector3 cameraPosition = Vector3.zero;

        //重複した計算
        float temp1 = distance * Mathf.Cos(latitude * Mathf.Deg2Rad);
        float temp2 = longitude * Mathf.Deg2Rad;

        cameraPosition.x = temp1 * Mathf.Sin(temp2);
        cameraPosition.y = distance * Mathf.Sin(latitude * Mathf.Deg2Rad);
        cameraPosition.z = temp1 * Mathf.Cos(temp2);

        return cameraPosition;
    }

    public IEnumerator SeeFellPlayer()
    {
        IsWork = false;
        float time = 0;
        while(time < 1.0f)
        {
            transform.LookAt(targetObject.transform);
            time += Time.deltaTime;
            yield return null;
        }
        IsWork = true;
        if (targetObject == null) yield return null;
        PlayerOverlap overlap = targetObject.GetComponent<PlayerOverlap>();

        //Hp分のダメージを与える=即死
        overlap.Damage(overlap.HP);
    }
}
