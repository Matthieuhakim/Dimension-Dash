using UnityEngine;
using UnityEngine.Tilemaps;

// Runtime tile that picks its sprite from its same-tilemap neighbours, so adjacent cells read as one slab
// (edges glow only on exposed sides). Sprite index = 4-bit mask of filled neighbours: N=1, E=2, S=4, W=8.
// Collision is unaffected by the sprite choice: keep colliderType = Grid for solid geometry.
[CreateAssetMenu(menuName = "Dimension Dash/Connected Tile")]
public class ConnectedTile : TileBase
{
    public const int VariantCount = 16;

    private static readonly Vector3Int[] Neighbours = { Vector3Int.up, Vector3Int.right, Vector3Int.down, Vector3Int.left };

    [Tooltip("16 sprites indexed by neighbour mask (N=1, E=2, S=4, W=8).")]
    [SerializeField]
    private Sprite[] sprites = new Sprite[VariantCount];
    [SerializeField]
    private Tile.ColliderType colliderType = Tile.ColliderType.Grid;

    // Neighbour mask of the cells around position holding the same tile.
    public static int NeighbourMask(Vector3Int position, ITilemap tilemap, TileBase tile)
    {
        int mask = 0;
        for (int i = 0; i < Neighbours.Length; i++)
        {
            if (tilemap.GetTile(position + Neighbours[i]) == tile) mask |= 1 << i;
        }
        return mask;
    }

    // Refreshes a cell and its same-tile neighbours, whose masks depend on it.
    public static void RefreshWithNeighbours(Vector3Int position, ITilemap tilemap, TileBase tile)
    {
        tilemap.RefreshTile(position);
        foreach (Vector3Int offset in Neighbours)
        {
            if (tilemap.GetTile(position + offset) == tile) tilemap.RefreshTile(position + offset);
        }
    }

    public override void RefreshTile(Vector3Int position, ITilemap tilemap) => RefreshWithNeighbours(position, tilemap, this);

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        tileData.sprite = sprites[NeighbourMask(position, tilemap, this)];
        tileData.color = Color.white;
        tileData.colliderType = colliderType;
        tileData.flags = TileFlags.LockColor;
        tileData.transform = Matrix4x4.identity;
    }
}
