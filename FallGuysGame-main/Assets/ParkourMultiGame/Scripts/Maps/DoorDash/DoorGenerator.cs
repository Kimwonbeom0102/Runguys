using UnityEngine;

public class DoorGenerator : MonoBehaviour
{
    public int startingDoors = 5; // ù ���� �� ����
    public GameObject[] openDoors; // ������ �� ������ �迭
    public GameObject[] closedDoors; // ������ �� ������ �迭
    public float spacing = 2f;    // �� �� ����
    public float rowOffset = 3f;  // �� �� ����

    void Start()
    {
        GenerateMap();
    }

    void GenerateMap()
    {
        int currentDoors = startingDoors; // ���� �࿡�� ������ �� ����
        int row = 0; // ���� ��ȣ

        while (currentDoors >= 3) // ���� ������ 3 �̻��� ���� ���� ����
        {
            GenerateRow(currentDoors, row);
            currentDoors--; // ���� ������ ����
            row++; // ���� ������ �̵�
        }
    }

    void GenerateRow(int doorCount, int row)
    {
        Vector3 rowPosition = new Vector3(0, 0, row * rowOffset);

        // ���� ��ġ�� ���� �� ���¸� ����
        bool[] doorStates = GenerateDoorStates(doorCount);

        for (int i = 0; i < doorCount; i++)
        {
            Vector3 doorPosition = rowPosition + new Vector3(i * spacing - ((doorCount - 1) * spacing) / 2, 0, 0);

            // ������ �� �Ǵ� ������ �� ����
            GameObject doorPrefab = doorStates[i] ? GetRandomPrefab(openDoors) : GetRandomPrefab(closedDoors);
            Instantiate(doorPrefab, doorPosition, Quaternion.identity);
        }
    }

    bool[] GenerateDoorStates(int doorCount)
    {
        bool[] doorStates = new bool[doorCount];
        int openDoorIndex = Random.Range(0, doorCount); // �ּ� 1���� ������ ��
        int closedDoorIndex;

        // ������ ���� ������ �ϳ� ���� (������ ���� �ٸ� �ε���)
        do
        {
            closedDoorIndex = Random.Range(0, doorCount);
        } while (closedDoorIndex == openDoorIndex);

        // �� ���� �ʱ�ȭ
        for (int i = 0; i < doorCount; i++)
        {
            if (i == openDoorIndex)
            {
                doorStates[i] = true; // ������ ��
            }
            else if (i == closedDoorIndex)
            {
                doorStates[i] = false; // ������ ��
            }
            else
            {
                // ������ ���� �����ϰ� �����ų� ����
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