using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NationOverlay : MonoBehaviour
{
    public GameObject nationOverlayPrefab; // 나라를 나타낼 반투명 이미지 프리팹
    private GameObject currentOverlay;
    public string race; // 국가의 종족 정보
    void Start()
    {
        // 종족을 기본 값으로 설정하거나 다른 스크립트에서 설정해줍니다.
        race = "Human"; // 예시로 기본 값을 설정
    }
    public void CreateNationOverlay(Vector3 position)
    {
        // 나라를 표현할 반투명 이미지 생성
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
