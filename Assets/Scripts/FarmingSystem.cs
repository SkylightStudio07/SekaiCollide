using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FarmingSystem : MonoBehaviour
{
    public Tilemap groundTilemap; // Ÿ�ϸ�
    public Tile farmTile; // ��� Ÿ��

    // ������ �����Ƿ� �ؽü�Ʈ�� ó��
    private HashSet<Vector3Int> cultivatedTiles = new HashSet<Vector3Int>();

    public bool CanCultivate(Vector3Int tilePosition)
    {
        return groundTilemap.GetTile(tilePosition) != farmTile;
    }

    public void CultivateLand(Vector3Int tilePosition)
    {
        groundTilemap.SetTile(tilePosition, farmTile);
        cultivatedTiles.Add(tilePosition);
    }

    public bool IsCultivationComplete(Vector3Int tilePosition)
    {
        return cultivatedTiles.Contains(tilePosition);
    }
}
