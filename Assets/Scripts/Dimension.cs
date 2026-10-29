// The dimension the world is currently in.
public enum Dimension { Reality, Shadow }

// Which dimension(s) an object exists in. Values match the old serialized Dimension enum
// (REALITY = 0, SHADOW = 1, BOTH = 2) so existing scene/prefab data keeps loading.
public enum DimensionPresence { Reality, Shadow, Both }
