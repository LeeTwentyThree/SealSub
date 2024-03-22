namespace SealSubMod.MonoBehaviours.EditorUtils;

public class VisualizeTexture3DInEditor : MonoBehaviour
{
    public float metersPerPixel;
    public Texture3D texture;
    public bool invertVisualization;

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0.3f, 0f, 0.5f);
        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                for (int z = 0; z < texture.depth; z++)
                {
                    if (Mathf.Approximately(texture.GetPixel(x, y, z).a, invertVisualization ? 1f : 0f))
                        Gizmos.DrawCube(new Vector3(x, y, z) * metersPerPixel, Vector3.one * metersPerPixel);
                }
            }
        }
    }
}