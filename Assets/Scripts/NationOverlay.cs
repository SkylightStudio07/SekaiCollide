using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NationOverlay : MonoBehaviour
{
    public GameObject nationOverlayPrefab; // ���� ��Ÿ�� ������ �̹��� ������
    private GameObject currentOverlay;
    public string race; // ������ ���� ����
    void Start()
    {
        // ������ �⺻ ������ �����ϰų� �ٸ� ��ũ��Ʈ���� �������ݴϴ�.
        race = "Human"; // ���÷� �⺻ ���� ����
    }
    public void CreateNationOverlay(Vector3 position)
    {
        // ���� ǥ���� ������ �̹��� ����
        currentOverlay = Instantiate(nationOverlayPrefab, position, Quaternion.identity);
    }

    public void ExpandNationOverlay(float expansionRate)
    {
        if (currentOverlay != null)
        {
            currentOverlay.transform.localScale += new Vector3(expansionRate, expansionRate, 0f);
        }
    }
}
