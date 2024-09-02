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

    public void CultivateLand(Vector3Int tilePosition, Nation nation)
    {
        if (CanCultivate(tilePosition))
        {
            StartCoroutine(CultivationProcess(tilePosition, nation));
        }
        else
        {
            Debug.Log("���� �Ұ� ��ġ");
        }
    }

    public bool IsCultivationComplete(Vector3Int tilePosition)
    {
        return cultivatedTiles.Contains(tilePosition);  
    }

    private IEnumerator CultivationProcess(Vector3Int tilePosition, Nation nation)
    {
        // ���� �۾� ���� ��...
        Debug.Log("���� �۾� ����...");

        // ���� �ð� �ùķ��̼� (��: 2��)
        yield return new WaitForSeconds(2f);

        // Ÿ���� ������ ��ȯ (Ÿ�ϸʿ��� ����)
        groundTilemap.SetTile(tilePosition, farmTile);/* ���� Ÿ�� */

        // �ķ� ���� �� ������ �ݿ�
        nation.AddFood(1f); // Ÿ�� �ϳ��� �ķ� 1 ���� ����
        Debug.Log("�ķ� ����...");
    }
}
