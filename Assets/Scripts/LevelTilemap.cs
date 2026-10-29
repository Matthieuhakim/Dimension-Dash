using System;
using UnityEngine;
using UnityEngine.Tilemaps;

// The level's single authoring tilemap. Everything is painted here (any kind, any dimension), so mixed sections
// can be selected and moved together. On load, each tile is copied onto the runtime layer for its kind and
// dimension (the layers carry physics and dimension behaviour), and this tilemap is hidden.
[DefaultExecutionOrder(-50)]
[RequireComponent(typeof(Tilemap), typeof(TilemapRenderer))]
public class LevelTilemap : MonoBehaviour
{
    [Serializable]
    private struct Layer
    {
        public DimensionTile.Kind kind;
        [Tooltip("Ignored for Floor.")]
        public DimensionPresence presentIn;
        public Tilemap tilemap;
    }

    [Tooltip("Runtime layers, filled on load. Keep them empty in the scene and at the same cell origin as this tilemap.")]
    [SerializeField]
    private Layer[] layers;

    private void Awake()
    {
        var source = GetComponent<Tilemap>();

        foreach (Layer layer in layers)
        {
            layer.tilemap.ClearAllTiles();
        }

        foreach (Vector3Int cell in source.cellBounds.allPositionsWithin)
        {
            TileBase placed = source.GetTile(cell);
            if (placed == null) continue;

            if (placed is not DimensionTile tile)
            {
                Debug.LogWarning($"{name}: tile '{placed.name}' at {cell} is not a DimensionTile; ignored.", this);
                continue;
            }

            Tilemap target = FindLayer(tile);
            if (target == null)
            {
                Debug.LogWarning($"{name}: no layer for {tile.TileKind} / {tile.PresentIn} (tile '{tile.name}' at {cell}).", this);
                continue;
            }

            target.SetTile(cell, tile.RuntimeTile);
            target.SetTransformMatrix(cell, source.GetTransformMatrix(cell));
        }

        foreach (Layer layer in layers)
        {
            RebuildColliders(layer.tilemap);
        }

        GetComponent<TilemapRenderer>().enabled = false;
    }

    private Tilemap FindLayer(DimensionTile tile)
    {
        foreach (Layer layer in layers)
        {
            if (layer.kind != tile.TileKind) continue;
            if (tile.TileKind == DimensionTile.Kind.Floor || layer.presentIn == tile.PresentIn) return layer.tilemap;
        }
        return null;
    }

    private static void RebuildColliders(Tilemap tilemap)
    {
        if (tilemap.TryGetComponent(out TilemapCollider2D tilemapCollider))
        {
            tilemapCollider.ProcessTilemapChanges();
        }
        if (tilemap.TryGetComponent(out CompositeCollider2D composite))
        {
            composite.GenerateGeometry();
        }
    }
}
