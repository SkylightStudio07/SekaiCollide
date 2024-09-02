using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FarmingSystem : MonoBehaviour
{
    public Tilemap groundTilemap; // 타일맵
    public Tile farmTile; // 농사 타일

    // 순서가 없으므로 해시세트로 처리
    private HashSet<Vector3Int> cultivatedTiles = new HashSet<Vector3Int>();

    public bool CanCultivate(Vector3Int tilePosition)
    {
        return groundTilemap.GetTile(tilePosition) != farmTile;
    }

    public void CultivateLand(Vector3Int tilePosition, Nation nation)
    {
        if (CanCultivate(tilePosition))
        {
            StartCoroutine(CultivationProcess(tilePosition, nation));
        }
        else
        {
            Debug.Log("경작 불가 위치");
        }
    }

    public bool IsCultivationComplete(Vector3Int tilePosition)
    {
        return cultivatedTiles.Contains(tilePosition);  
    }

    private IEnumerator CultivationProcess(Vector3Int tilePosition, Nation nation)
    {
        // 경작 작업 수행 중...
        Debug.Log("경작 작업 시작...");

        // 경작 시간 시뮬레이션 (예: 2초)
        yield return new WaitForSeconds(2f);

        // 타일이 농지로 변환 (타일맵에서 변경)
        groundTilemap.SetTile(tilePosition, farmTile);/* 농지 타일 */

        // 식량 생산 및 국가에 반영
        nation.AddFood(1f); // 타일 하나당 식량 1 단위 생산
        Debug.Log("식량 증가...");
    }
}
