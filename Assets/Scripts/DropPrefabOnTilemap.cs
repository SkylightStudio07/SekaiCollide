using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DropPrefabOnTilemap : MonoBehaviour
{
    public GameObject prefab; // ����� ������
    public Tilemap tilemap; // Ÿ�ϸ�

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // ���콺 ���� Ŭ��
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPosition.z = 0;

            Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);
            Vector3 cellCenterPosition = tilemap.GetCellCenterWorld(cellPosition);

            Instantiate(prefab, cellCenterPosition, Quaternion.identity);
        }
    }
}