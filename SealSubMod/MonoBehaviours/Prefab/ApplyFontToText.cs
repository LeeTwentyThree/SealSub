using SealSubMod.MonoBehaviours.Abstract;
using TMPro;

namespace SealSubMod.MonoBehaviours.Prefab;

internal class ApplyFontToText : PrefabModifier
{
    public override void OnAsyncPrefabTasksCompleted()
    {
        GetComponent<TextMeshProUGUI>().font = FontUtils.Aller_Rg;
    }
}
