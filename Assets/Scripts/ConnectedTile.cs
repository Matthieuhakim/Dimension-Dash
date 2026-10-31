using UnityEngine;
using UnityEngine.Tilemaps;

// Runtime tile that picks its sprite from its same-tilemap neighbours, so adjacent cells read as one slab
// (edges glow only on exposed sides). Sprite index = 4-bit mask of filled neighbours: N=1, E=2, S=4, W=8.
// Collision is unaffected by the sprite choice: keep colliderType = Grid for solid geometry.
[CreateAssetMenu(menuName = "Dimension Dash/Connected Tile")]
public class ConnectedTile : TileBase
{
    private static readonly Vector3Int[] Neighbours = { Vector3Int.up, Vector3Int.right, Vector3Int.down, Vector3Int.left };

    [Tooltip("16 sprites indexed by neighbour mask (N=1, E=2, S=4, W=8).")]
    [SerializeField]
    private Sprite[] sprites = new Sprite[16];
    [SerializeField]
    private Tile.ColliderType colliderType = Tile.ColliderType.Grid;

    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        tilemap.RefreshTile(position);
        foreach (Vector3Int offset in Neighbours)
        {
            if (tilemap.GetTile(position + offset) == this) tilemap.RefreshTile(position + offset);
        }
    }

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        int mask = 0;
        for (int i = 0; i < Neighbours.Length; i++)
        {
            if (tilemap.GetTile(position + Neighbours[i]) == this) mask |= 1 << i;
        }

        tileData.sprite = sprites[mask];
        tileData.color = Color.white;
        tileData.colliderType = colliderType;
        tileData.flags = TileFlags.LockColor;
        tileData.transform = Matrix4x4.identity;
    }
}
