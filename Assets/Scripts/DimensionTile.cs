using UnityEngine;
using UnityEngine.Tilemaps;

// Authoring tile: what a cell is (floor, block, spike) and which dimension(s) it exists in.
// Painted on the level's single authoring tilemap; its sprite and colour are only an editor preview, set to match
// how the cell looks at the level's start (Reality). With connected preview sprites, adjacent cells of the same
// tile join like the runtime ConnectedTiles.
// LevelTilemap copies its runtime tile onto the matching layer when the level loads.
[CreateAssetMenu(menuName = "Dimension Dash/Dimension Tile")]
public class DimensionTile : Tile
{
    public enum Kind { Floor, Block, Spike }

    [SerializeField]
    private Kind kind;
    [Tooltip("Ignored for Floor, which exists in both dimensions.")]
    [SerializeField]
    private DimensionPresence presentIn = DimensionPresence.Both;
    [Tooltip("Tile placed on the runtime layer. Keep it white: the layer tints it per dimension.")]
    [SerializeField]
    private TileBase runtimeTile;
    [Tooltip("Optional editor preview: 16 sprites by neighbour mask (N=1, E=2, S=4, W=8). Empty = use Sprite.")]
    [SerializeField]
    private Sprite[] connectedPreview;

    public Kind TileKind => kind;
    public DimensionPresence PresentIn => presentIn;
    public TileBase RuntimeTile => runtimeTile;

    private bool IsConnected => connectedPreview != null && connectedPreview.Length == ConnectedTile.VariantCount;

    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        if (IsConnected) ConnectedTile.RefreshWithNeighbours(position, tilemap, this);
        else base.RefreshTile(position, tilemap);
    }

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        base.GetTileData(position, tilemap, ref tileData);
        if (IsConnected) tileData.sprite = connectedPreview[ConnectedTile.NeighbourMask(position, tilemap, this)];
    }
}
