using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DropPrefabOnTilemap : MonoBehaviour
{
    public GameObject prefab; // 드롭할 프리팹
    public Tilemap tilemap; // 타일맵

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 마우스 왼쪽 클릭
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPosition.z = 0;

            Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);
            Vector3 cellCenterPosition = tilemap.GetCellCenterWorld(cellPosition);

            Instantiate(prefab, cellCenterPosition, Quaternion.identity);
        }
    }
}