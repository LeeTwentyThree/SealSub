using SealSubMod.Interfaces;
using TMPro;

namespace SealSubMod.MonoBehaviours.UI;

public class DepthUIManager : MonoBehaviour, IUIElement
{
    [SerializeField] CrushDamage crushDamage;
    [SerializeField] TextMeshProUGUI depthText;

    private int lastDisplayedDepth = -1;
    private int lastDisplayedCrushDepth = -1;
    public void UpdateUI()
    {

        int depth = (int)crushDamage.GetDepth();
        int crushDepth = (int)crushDamage.crushDepth;

        Color depthTextColor = Color.white;

        if (depth >= crushDepth)
        {
            depthTextColor = Color.red;
        }

        if (lastDisplayedDepth != depth || lastDisplayedCrushDepth != crushDepth)
        {
            lastDisplayedDepth = depth;
            lastDisplayedCrushDepth = crushDepth;
            depthText.text = string.Format("{0}m / {1}m", depth, crushDepth);
        }

        depthText.color = depthTextColor;
    }
}
