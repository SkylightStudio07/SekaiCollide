using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FarmingSystem : MonoBehaviour
{
    public Tilemap groundTilemap; // 기존의 타일맵 (경작 전)
    public Tilemap farmableTilemap; // 경작 후 농사 가능한 타일맵
    public TileBase farmTile; // 경작 후 사용할 농지 타일
    public float cultivationTime = 3f; // 경작 작업에 소요되는 시간

    // 유닛이 경작 명령을 받으면 호출
    public void CultivateLand(Vector3Int tilePosition)
    {
        // 경작 가능한 타일인지 확인
        if (CanCultivate(tilePosition))
        {
            StartCoroutine(CultivationProcess(tilePosition));
        }
        else
        {
            Debug.Log("이 위치는 경작할 수 없습니다.");
        }
    }

    // 경작 가능한지 확인 (기존 타일맵에서 특정 타일만 경작 가능하도록 설정)
    public bool CanCultivate(Vector3Int tilePosition)
    {
        TileBase tile = groundTilemap.GetTile(tilePosition);
        return tile != null; // 예시로 간단히 타일이 있는지 확인 (필요 시 특정 타일만 경작 가능하도록 추가 가능)
    }

    // 경작 과정
    System.Collections.IEnumerator CultivationProcess(Vector3Int tilePosition)
    {
        Debug.Log("경작 작업 시작...");
        yield return new WaitForSeconds(cultivationTime); // 경작 시간 대기

        // 기존 타일맵에서 해당 타일을 농지 타일맵으로 복사
        farmableTilemap.SetTile(tilePosition, farmTile);

        // 기존 타일맵에서 타일 제거 (경작이 완료된 후)
        groundTilemap.SetTile(tilePosition, null);

        Debug.Log("경작 작업 완료! 농지가 생성되었습니다.");
    }
}
