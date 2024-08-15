using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    public float zoomSpeed = 1f;         // �� �ӵ� ����
    public float minOrthographicSize = 5f;  // �ּ� ī�޶� ũ��
    public float maxOrthographicSize = 20f; // �ִ� ī�޶� ũ��

    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        GetComponent<Camera>().orthographicSize -= scroll * zoomSpeed;
        GetComponent<Camera>().orthographicSize = Mathf.Clamp(GetComponent<Camera>().orthographicSize, minOrthographicSize, maxOrthographicSize);
    }
}
