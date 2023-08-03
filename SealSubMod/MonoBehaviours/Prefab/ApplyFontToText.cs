using SealSubMod.Interfaces;
using TMPro;

namespace SealSubMod.MonoBehaviours.Prefab;

internal class ApplyFontToText : MonoBehaviour, IAsyncPrefabSetupOperation
{
    public IEnumerator SetupPrefabAsync()
    {
        GetComponent<TextMeshProUGUI>().font = FontUtils.Aller_Rg;
        yield break;
    }
}
