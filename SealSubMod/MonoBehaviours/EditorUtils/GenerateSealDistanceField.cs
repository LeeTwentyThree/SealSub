using UnityEngine.Serialization;

namespace SealSubMod.MonoBehaviours.EditorUtils;

// This file helps with baking a texture3d for the seal's distance field, which is ESSENTIAL for flooding!
// DO NOT USE THIS AT RUNTIME!
public class GenerateSealDistanceField : MonoBehaviour
{
    public Bounds bounds;
    public DistanceField distanceField;
    public float pixelsPerUnit;

    [Header("Bake")] public bool generateTexture;

    [Header("Editor visualization")] public bool visualizeInEditor;
    [Tooltip("Percentage value representing the depth through the volume at which the cross section is visualized.")]
    [Range(0f, 1f)] public float crossSectionVisualizationDepth;
    [Tooltip("The cross section will be perpendicular to this axis.")]
    public Axis crossSectionAxis;

    private Vector3 Resolution => bounds.size * pixelsPerUnit;
    private Texture3D Texture3D => distanceField.texture;

    private void OnValidate()
    {
        if (generateTexture)
        {
            generateTexture = false;
            GenerateTexture();
            visualizeInEditor = false;
        }
    }

    public void GenerateTexture()
    {
        if (distanceField == null)
        {
            Debug.LogError($"No distance field found on object {this}! Cannot generate a texture.");
            return;
        }

        if (distanceField.texture == null || distanceField.texture.width != (int) Resolution.x ||
            distanceField.texture.height != (int) Resolution.y || distanceField.texture.depth != (int) Resolution.z)
        {
            distanceField.texture = new Texture3D((int) Resolution.x, (int) Resolution.y, (int) Resolution.z,
                TextureFormat.Alpha8, 1);
        }

        var checkBoxExtents = Vector3.Scale(bounds.extents, VectorReciprocal(Resolution));
        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                int interiorStartPixel = -1;
                int interiorEndPixel = -1;
                bool lastPixelOccupied = false;
                for (int x = 0; x < Resolution.x; x++)
                {
                    var occupied = Physics.CheckBox(bounds.center - bounds.extents + Vector3.Scale(new Vector3(x, y, z), Vector3.one * (1 / pixelsPerUnit)), checkBoxExtents);
                    if (occupied && interiorStartPixel == -1)
                    {
                        interiorStartPixel = x;
                    }
                    else if (lastPixelOccupied && !occupied)
                    {
                        interiorEndPixel = x;
                    }

                    lastPixelOccupied = occupied;
                }

                if (interiorStartPixel == -1)
                {
                    for (int x = 0; x < Resolution.x; x++)
                    {
                        Texture3D.SetPixel(x, y, z, Color.clear);
                    }
                }
                else
                {
                    for (int x = 0; x < Resolution.x; x++)
                    {
                        Texture3D.SetPixel(x, y, z,
                            x >= interiorStartPixel && x < interiorEndPixel ? Color.white : Color.clear);
                    }
                }
            }
        }

        Texture3D.Apply();
    }

    private void OnDrawGizmos()
    {
        if (!visualizeInEditor)
        {
            return;
        }

        Gizmos.color = new Color(0.3f, 0.3f, 0f, 0.2f);

        Gizmos.DrawCube(bounds.center, bounds.size);

        if (distanceField == null)
        {
            Debug.LogError("Cannot visualize an object with no distance field!");
            return;
        }

        if (Texture3D == null)
        {
            Debug.LogError("Cannot visualize a distance field with no a texture!");
            return;
        }

        Gizmos.color = new Color(0.3f, 0.4f, 1f, 0.2f);

        int d = (int) (Resolution[(int)crossSectionAxis] * crossSectionVisualizationDepth);
        for (int l = 0; l < Resolution[((int)crossSectionAxis + 1) % 3]; l++)
        {
            for (int w = 0; w < Resolution[((int)crossSectionAxis + 2) % 3]; w++)
            {
                var location = GetCrossSectionLocation(l, w, d, crossSectionAxis);
                var transparent = Mathf.Approximately(Texture3D.GetPixel(location.x, location.y, location.z).a, 1f);
                if (transparent)
                {
                    Gizmos.DrawCube(PixelToWorldCoordinate(location), Vector3.Scale(bounds.size, VectorReciprocal(Resolution)));
                }
            }
        }
    }

    private static Vector3Int GetCrossSectionLocation(int length, int width, int depth, Axis axis)
    {
        return axis switch
        {
            Axis.Right => new Vector3Int(depth, length, width),
            Axis.Up => new Vector3Int(length, depth, width),
            Axis.Forward => new Vector3Int(length, width, depth),
            _ => new Vector3Int()
        };
    }

    private Vector3 PixelToWorldCoordinate(Vector3 pixel) =>
        bounds.center - bounds.extents + Vector3.Scale(pixel, Vector3.one * (1 / pixelsPerUnit));

    private static Vector3 VectorReciprocal(Vector3 original) => new(1 / original.x, 1 / original.y, 1 / original.z);

    public enum Axis
    {
        Right,
        Up,
        Forward
    }
}