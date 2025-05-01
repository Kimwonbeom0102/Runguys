using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("Map Settings")]
    public int rows = 12; // 맵의 행 개수
    public int columns = 10; // 한 줄의 타일 개수
    public GameObject tilePrefab; // 타일 프리팹
    public float tileSpacing = 1.5f; // 타일 간 거리 (X, Z 간격)

    private Tile[,] tiles; // 생성된 타일을 저장하는 2D 배열
    private Vector2Int lastRealTile; // 이전 진짜 타일의 좌표

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
        // 2D 배열 초기화
        tiles = new Tile[rows, columns];

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                // 타일 생성 위치 계산
                Vector3 tilePosition = new Vector3(col * tileSpacing, 0, row * tileSpacing);

                // 타일 생성
                GameObject tileObj = Instantiate(tilePrefab, tilePosition, Quaternion.identity);

                // 타일 부모 설정
                tileObj.transform.parent = this.transform;

                // 타일의 이름을 변경 (디버깅용)
                tileObj.name = $"Tile_{row}_{col}";

                // 타일 스크립트 가져오기
                Tile tile = tileObj.GetComponent<Tile>();
                if (tile == null)
                {
                    Debug.LogError("Tile prefab does not have a Tile script attached!");
                    return;
                }

                // 타일 초기화 및 배열에 추가
                tile.isReal = false; // 기본적으로 가짜 타일
                tiles[row, col] = tile;
            }

            // 진짜 타일 배치
            if (row == 0)
            {
                // 첫 번째 행에서 랜덤한 타일을 진짜 타일로 설정
                int randomCol = Random.Range(0, columns);
                tiles[row, randomCol].isReal = true;
                lastRealTile = new Vector2Int(row, randomCol);
            }
            else if (row == rows - 1 || row == rows - 2)
            {
                // 마지막 두 행에서는 반드시 진짜 타일이 하나만 존재
                PlaceSingleRealTile(row);
            }
            else
            {
                // 나머지 행은 진짜 타일을 여러 개 배치 가능
                PlaceMultipleRealTiles(row);
            }
        }

        Debug.Log($"Map generated with {rows * columns} tiles.");
    }

    private void PlaceSingleRealTile(int row)
    {
        // 이전 진짜 타일과 이어지는 범위 (-1, 0, 1)
        int prevCol = lastRealTile.y;
        int newCol = Mathf.Clamp(prevCol + Random.Range(-1, 2), 0, columns - 1);

        // 새로운 진짜 타일 설정
        tiles[row, newCol].isReal = true;

        // 새 진짜 타일 좌표 저장
        lastRealTile = new Vector2Int(row, newCol);
    }

    private void PlaceMultipleRealTiles(int row)
    {
        // 최소 하나는 이어지는 진짜 타일
        PlaceSingleRealTile(row);

        // 추가로 랜덤한 타일들을 진짜 타일로 설정
        int additionalRealTiles = Random.Range(1, columns); // 1~columns-1개의 타일
        for (int i = 0; i < additionalRealTiles; i++)
        {
            int randomCol = Random.Range(0, columns);
            tiles[row, randomCol].isReal = true;
        }
    }

    public void ResetMap()
    {
        // 모든 타일 초기화
        foreach (Tile tile in tiles)
        {
            if (tile != null)
            {
                tile.ResetTile();
            }
        }
    }
}