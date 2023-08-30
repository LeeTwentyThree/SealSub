namespace SealSubMod.MonoBehaviours;

internal class GayifierBehaviour : MonoBehaviour
{
    private static Color[] gayColors = new[]
    {
        new Color(1, 0f, 0),
        new Color(1, 0.5f, 0),
        new Color(1, 1, 0),
        new Color(0, 1, 0),
        new Color(0, 0, 1),
        new Color(0.5f, 0, 1),
        new Color(1, 0, 1),
    };
    private static float speed = 0.5f;

    private Renderer[] renderers;

    private Gradient gradient = new();

    private float _timeLastLoop = 0;
    private float TimeLastLoop
    {
        set
        {
            if (value >= 1)
                _timeLastLoop = 0;
            else
                _timeLastLoop = value;
        }
        get => _timeLastLoop;
    }

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>(true);

        var keys = new List<GradientColorKey>();
        var timeStep = 1f / gayColors.Length;

        var current = 0f;

        foreach(var gayColor in gayColors)
        {
            keys.Add(new GradientColorKey(gayColor, current));
            current += timeStep;
        }

        keys.Add(new GradientColorKey(gayColors[0], 1));//start returning to the original one at the end


        gradient.colorKeys = keys.ToArray();
    }
    private void Update()
    {
        TimeLastLoop += Time.deltaTime * speed;

        var color = gradient.Evaluate(TimeLastLoop);

        foreach(Renderer renderer in renderers)
        {
            foreach(var mat in renderer.materials)
            {
                mat.color = color;
            }
        }
    }
}
