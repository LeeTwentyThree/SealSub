using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SealSubMod.Patches;

[HarmonyPatch(typeof(uGUI_BlueprintsTab))]
internal class uGUI_BlueprintsTabPatches
{
    [HarmonyPatch(nameof(uGUI_BlueprintsTab.UpdateOrder))]
    [HarmonyPatch(new Type[0])]
    public static void Prefix()
    {
        if (uGUI_BlueprintsTab.groups.Remove(Plugin.SealGroup))
        {
            uGUI_BlueprintsTab.groups.Insert(uGUI_BlueprintsTab.groups.IndexOf(TechGroup.Cyclops) + 1, Plugin.SealGroup);
        }
    }
}
