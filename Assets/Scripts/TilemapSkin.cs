using UnityEngine;
using UnityEngine.Tilemaps;

// Visual-only copy of a runtime layer drawn with another tile (e.g. the daytime look of the blocks).
// Filled on load, after LevelTilemap has filled the source layer. Keep it empty in the scene and give it no
// collider; show or hide it per dimension with a DimensionColor.
[DefaultExecutionOrder(-40)]
[RequireComponent(typeof(Tilemap))]
public class TilemapSkin : MonoBehaviour
{
    [SerializeField]
    private Tilemap source;
    [SerializeField]
    private TileBase skinTile;

    private void Awake()
    {
        var tilemap = GetComponent<Tilemap>();
        tilemap.ClearAllTiles();

        foreach (Vector3Int cell in source.cellBounds.allPositionsWithin)
        {
            if (source.HasTile(cell)) tilemap.SetTile(cell, skinTile);
        }
    }
}
