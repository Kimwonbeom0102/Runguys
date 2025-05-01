using UnityEngine;

public class DoorGenerator : MonoBehaviour
{
    public int startingDoors = 5; // 첫 행의 문 개수
    public GameObject[] openDoors; // 열리는 문 프리팹 배열
    public GameObject[] closedDoors; // 닫히는 문 프리팹 배열
    public float spacing = 2f;    // 문 간 간격
    public float rowOffset = 3f;  // 행 간 간격

    void Start()
    {
        GenerateMap();
    }

    void GenerateMap()
    {
        int currentDoors = startingDoors; // 현재 행에서 생성할 문 개수
        int row = 0; // 행의 번호

        while (currentDoors >= 3) // 문의 개수가 3 이상일 때만 행을 생성
        {
            GenerateRow(currentDoors, row);
            currentDoors--; // 문의 개수를 줄임
            row++; // 다음 행으로 이동
        }
    }

    void GenerateRow(int doorCount, int row)
    {
        Vector3 rowPosition = new Vector3(0, 0, row * rowOffset);

        // 랜덤 배치를 위해 문 상태를 생성
        bool[] doorStates = GenerateDoorStates(doorCount);

        for (int i = 0; i < doorCount; i++)
        {
            Vector3 doorPosition = rowPosition + new Vector3(i * spacing - ((doorCount - 1) * spacing) / 2, 0, 0);

            // 열리는 문 또는 닫히는 문 생성
            GameObject doorPrefab = doorStates[i] ? GetRandomPrefab(openDoors) : GetRandomPrefab(closedDoors);
            Instantiate(doorPrefab, doorPosition, Quaternion.identity);
        }
    }

    bool[] GenerateDoorStates(int doorCount)
    {
        bool[] doorStates = new bool[doorCount];
        int openDoorIndex = Random.Range(0, doorCount); // 최소 1개의 열리는 문
        int closedDoorIndex;

        // 닫히는 문을 강제로 하나 선택 (열리는 문과 다른 인덱스)
        do
        {
            closedDoorIndex = Random.Range(0, doorCount);
        } while (closedDoorIndex == openDoorIndex);

        // 문 상태 초기화
        for (int i = 0; i < doorCount; i++)
        {
            if (i == openDoorIndex)
            {
                doorStates[i] = true; // 열리는 문
            }
            else if (i == closedDoorIndex)
            {
                doorStates[i] = false; // 닫히는 문
            }
            else
            {
                // 나머지 문은 랜덤하게 열리거나 닫힘
                doorStates[i] = Random.value > 0.5f;
            }
        }

        return doorStates;
    }

    GameObject GetRandomPrefab(GameObject[] prefabs)
    {
        int randomIndex = Random.Range(0, prefabs.Length);
        return prefabs[randomIndex];
    }
}