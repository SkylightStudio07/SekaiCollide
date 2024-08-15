using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    public float zoomSpeed = 1f;         // 줌 속도 조절
    public float minOrthographicSize = 5f;  // 최소 카메라 크기
    public float maxOrthographicSize = 20f; // 최대 카메라 크기

    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        GetComponent<Camera>().orthographicSize -= scroll * zoomSpeed;
        GetComponent<Camera>().orthographicSize = Mathf.Clamp(GetComponent<Camera>().orthographicSize, minOrthographicSize, maxOrthographicSize);
    }
}
