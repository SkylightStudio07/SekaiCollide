using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FarmingSystem : MonoBehaviour
{
    public Tilemap groundTilemap; // ������ Ÿ�ϸ� (���� ��)
    public Tilemap farmableTilemap; // ���� �� ��� ������ Ÿ�ϸ�
    public TileBase farmTile; // ���� �� ����� ���� Ÿ��
    public float cultivationTime = 3f; // ���� �۾��� �ҿ�Ǵ� �ð�

    // ������ ���� ����� ������ ȣ��
    public void CultivateLand(Vector3Int tilePosition)
    {
        // ���� ������ Ÿ������ Ȯ��
        if (CanCultivate(tilePosition))
        {
            StartCoroutine(CultivationProcess(tilePosition));
        }
        else
        {
            Debug.Log("�� ��ġ�� ������ �� �����ϴ�.");
        }
    }

    // ���� �������� Ȯ�� (���� Ÿ�ϸʿ��� Ư�� Ÿ�ϸ� ���� �����ϵ��� ����)
    public bool CanCultivate(Vector3Int tilePosition)
    {
        TileBase tile = groundTilemap.GetTile(tilePosition);
        return tile != null; // ���÷� ������ Ÿ���� �ִ��� Ȯ�� (�ʿ� �� Ư�� Ÿ�ϸ� ���� �����ϵ��� �߰� ����)
    }

    // ���� ����
    System.Collections.IEnumerator CultivationProcess(Vector3Int tilePosition)
    {
        Debug.Log("���� �۾� ����...");
        yield return new WaitForSeconds(cultivationTime); // ���� �ð� ���

        // ���� Ÿ�ϸʿ��� �ش� Ÿ���� ���� Ÿ�ϸ����� ����
        farmableTilemap.SetTile(tilePosition, farmTile);

        // ���� Ÿ�ϸʿ��� Ÿ�� ���� (������ �Ϸ�� ��)
        groundTilemap.SetTile(tilePosition, null);

        Debug.Log("���� �۾� �Ϸ�! ������ �����Ǿ����ϴ�.");
    }
}
