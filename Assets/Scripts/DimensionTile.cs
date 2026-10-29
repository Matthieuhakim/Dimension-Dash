using UnityEngine;
using UnityEngine.Tilemaps;

// Authoring tile: what a cell is (floor, block, spike) and which dimension(s) it exists in.
// Painted on the level's single authoring tilemap; its sprite and colour are only an editor preview.
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

    public Kind TileKind => kind;
    public DimensionPresence PresentIn => presentIn;
    public TileBase RuntimeTile => runtimeTile;
}
