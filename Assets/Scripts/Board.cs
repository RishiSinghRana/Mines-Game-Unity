///<summary>
///Board.cs Script of Minesweeper Game
///</summary>
///<summary>
///Board.cs Script of Minesweeper Game
///</summary>
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class Board : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }

    public Tile tileUnknown;
    public Tile tileEmpty;
    public GameObject minePrefab;
    public GameObject explodedMinePrefab;
    public GameObject diamondPrefab;

    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();
    }

    public void Draw(CellGrid grid)
    {
        int width = grid.Width;
        int height = grid.Height;

        // Clear previous GameObjects
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // Draw tiles and instantiate GameObjects
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = grid[x, y];
                tilemap.SetTile(cell.position, GetTile(cell));

                Vector3 worldPosition = tilemap.GetCellCenterWorld(cell.position);

                if (cell.revealed)
                {
                    if (cell.type == Cell.Type.Empty)
                    {
                        Instantiate(diamondPrefab, worldPosition, Quaternion.identity, transform);
                    }
                    else if (cell.type == Cell.Type.Mine)
                    {
                        GameObject mineObject = cell.exploded ? explodedMinePrefab : minePrefab;
                        Instantiate(mineObject, worldPosition, Quaternion.identity, transform);
                    }
                }
            }
        }
    }

    private Tile GetTile(Cell cell)
    {
        return cell.revealed ? tileEmpty : tileUnknown;
    }
}
