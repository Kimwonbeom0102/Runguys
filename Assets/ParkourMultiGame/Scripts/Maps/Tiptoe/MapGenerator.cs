using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("Map Settings")]
    public int rows = 12; // ���� �� ����
    public int columns = 10; // �� ���� Ÿ�� ����
    public GameObject tilePrefab; // Ÿ�� ������
    public float tileSpacing = 1.5f; // Ÿ�� �� �Ÿ� (X, Z ����)

    private Tile[,] tiles; // ������ Ÿ���� �����ϴ� 2D �迭
    private Vector2Int lastRealTile; // ���� ��¥ Ÿ���� ��ǥ

    private void Start()
    {
        if (tilePrefab == null)
        {
            Debug.LogError("Tile Prefab is not assigned in the inspector!");
            return;
        }

        GenerateMap();
    }

    private void GenerateMap()
    {
        // 2D �迭 �ʱ�ȭ
        tiles = new Tile[rows, columns];

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                // Ÿ�� ���� ��ġ ���
                Vector3 tilePosition = new Vector3(col * tileSpacing, 0, row * tileSpacing);

                // Ÿ�� ����
                GameObject tileObj = Instantiate(tilePrefab, tilePosition, Quaternion.identity);

                // Ÿ�� �θ� ����
                tileObj.transform.parent = this.transform;

                // Ÿ���� �̸��� ���� (������)
                tileObj.name = $"Tile_{row}_{col}";

                // Ÿ�� ��ũ��Ʈ ��������
                Tile tile = tileObj.GetComponent<Tile>();
                if (tile == null)
                {
                    Debug.LogError("Tile prefab does not have a Tile script attached!");
                    return;
                }

                // Ÿ�� �ʱ�ȭ �� �迭�� �߰�
                tile.isReal = false; // �⺻������ ��¥ Ÿ��
                tiles[row, col] = tile;
            }

            // ��¥ Ÿ�� ��ġ
            if (row == 0)
            {
                // ù ��° �࿡�� ������ Ÿ���� ��¥ Ÿ�Ϸ� ����
                int randomCol = Random.Range(0, columns);
                tiles[row, randomCol].isReal = true;
                lastRealTile = new Vector2Int(row, randomCol);
            }
            else if (row == rows - 1 || row == rows - 2)
            {
                // ������ �� �࿡���� �ݵ�� ��¥ Ÿ���� �ϳ��� ����
                PlaceSingleRealTile(row);
            }
            else
            {
                // ������ ���� ��¥ Ÿ���� ���� �� ��ġ ����
                PlaceMultipleRealTiles(row);
            }
        }

        Debug.Log($"Map generated with {rows * columns} tiles.");
    }

    private void PlaceSingleRealTile(int row)
    {
        // ���� ��¥ Ÿ�ϰ� �̾����� ���� (-1, 0, 1)
        int prevCol = lastRealTile.y;
        int newCol = Mathf.Clamp(prevCol + Random.Range(-1, 2), 0, columns - 1);

        // ���ο� ��¥ Ÿ�� ����
        tiles[row, newCol].isReal = true;

        // �� ��¥ Ÿ�� ��ǥ ����
        lastRealTile = new Vector2Int(row, newCol);
    }

    private void PlaceMultipleRealTiles(int row)
    {
        // �ּ� �ϳ��� �̾����� ��¥ Ÿ��
        PlaceSingleRealTile(row);

        // �߰��� ������ Ÿ�ϵ��� ��¥ Ÿ�Ϸ� ����
        int additionalRealTiles = Random.Range(1, columns); // 1~columns-1���� Ÿ��
        for (int i = 0; i < additionalRealTiles; i++)
        {
            int randomCol = Random.Range(0, columns);
            tiles[row, randomCol].isReal = true;
        }
    }

    public void ResetMap()
    {
        // ��� Ÿ�� �ʱ�ȭ
        foreach (Tile tile in tiles)
        {
            if (tile != null)
            {
                tile.ResetTile();
            }
        }
    }
}