using MonoMod.Utils;
using Nautilus.Crafting;
using Newtonsoft.Json;
using System;
using System.Reflection;

namespace SealSubMod;

public static class JsonUtils
{
    private static Dictionary<TechType, RecipeData> cachedRecipes = new();
    public static RecipeData GetRecipeFromJson(TechType recipeToGet, string filePath = null)
    {
        if (cachedRecipes.TryGetValue(recipeToGet, out var recipe)) return recipe;

        if (string.IsNullOrEmpty(filePath)) filePath = GetSealRecipeFile();
        
        var content = File.ReadAllText(filePath);

        Dictionary<TechType, RecipeData> recipes = null;
        try
        {
            recipes = JsonConvert.DeserializeObject<Dictionary<TechType, RecipeData>>(content);
        }
        catch(Exception e) { Plugin.Logger.LogError($"Error thrown when deserializing {filePath}, {e}"); }

        if(recipes == null) return null;

        cachedRecipes.AddRange(recipes);

        if(!recipes.TryGetValue(recipeToGet, out var recicicicicipe))
        {
            Plugin.Logger.LogError($"Couldn't find techtype {recipeToGet} in file {filePath}. If this techtype is from another mod, please specify a custom file path, as your recipe won't be included in the seal's default .json file");
            return new RecipeData(new Ingredient(TechType.Titanium, 2));
        }

        return recicicicicipe;
    }
    public static string GetSealRecipeFile()
    {
        return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Recipes.json");
    }
}
