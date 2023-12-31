using Nautilus.Assets.PrefabTemplates;
using Nautilus.Assets;
using Nautilus.Crafting;
using SealSubMod.Prefabs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nautilus.Assets.Gadgets;

namespace SealSubMod.Extensions;

public static class PrefabInfoExtensions
{
    public static PrefabInfo RegisterUpgradeModulePrefab(this PrefabInfo info, RecipeData recipe = null)
    {
        var prefab = new CustomPrefab(info);
        prefab.SetGameObject(new CloneTemplate(info, TechType.CyclopsHullModule1));
        prefab.SetPdaGroupCategory(Plugin.SealGroup, Plugin.SealModuleCategory);
        prefab.SetUnlock(SealSubPrefab.SealType);

        if(recipe == null) recipe = JsonUtils.GetRecipeFromJson(info.TechType);
        prefab.SetRecipe(recipe).WithFabricatorType(Plugin.SealFabricatorTree);
        prefab.SetEquipment(Plugin.SealModuleEquipmentType);
        prefab.Register();
        return prefab.Info;
    }
}