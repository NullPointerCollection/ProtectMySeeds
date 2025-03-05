using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;

namespace ProtectMySeeds
{
    [BepInPlugin(ModGUID, ModName, ModVersion)]
    public class ProtectMySeeds : BaseUnityPlugin
    {
        internal const string ModName = "ProtectMySeeds";
        internal const string ModVersion = "1.0.2";
        internal const string Author = "NullPointerCollection";
        internal const string ModGUID = "com.nullpointercollection.protectmyseeds";
        internal static string ConnectionError = "";
        public static readonly ManualLogSource ProtectMySeedsLogger = BepInEx.Logging.Logger.CreateLogSource(ModName);
        internal Harmony harmony = new(ModGUID);

        public void Awake()
        {
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    [HarmonyPatch(typeof(Plant), nameof(Plant.Start))]
    public static class PlantAwakePatch
    {
        public static void Postfix(Plant __instance)
        {
            if (!__instance.GetComponent<Piece>() || __instance.gameObject.layer.Equals(13)) return;
            var dropOnDestroyed = __instance.GetComponent<DropOnDestroyed>() ?? __instance.gameObject.AddComponent<DropOnDestroyed>();
            if (dropOnDestroyed.m_dropWhenDestroyed.IsEmpty())
            {
                var plantResources = __instance.GetComponent<Piece>().m_resources;
                DropTable dropTable = new();
                int dropCount = 0;
                foreach (var resource in plantResources)
                {
                    dropTable.m_drops.Add(new DropTable.DropData { m_item = resource.m_resItem.gameObject, m_stackMin = resource.m_amount, m_stackMax = resource.m_amount, m_dontScale = true, m_weight = 1 });
                    dropCount += resource.m_amount;
                }
                dropTable.m_dropMin = dropCount;
                dropTable.m_dropMax = dropCount;
                dropTable.m_oneOfEach = true;
                dropOnDestroyed.m_dropWhenDestroyed = dropTable;
            }
        }
    }

    [HarmonyPatch(typeof(Pickable), nameof(Pickable.Awake))]
    public static class PickableAwakePatch
    {
        public static void Postfix(Pickable __instance)
        {
            if (__instance.m_respawnTimeMinutes > 0 || !__instance.GetComponent<Destructible>()) return;
            var dropOnDestroyed = __instance.GetComponent<DropOnDestroyed>() ?? __instance.gameObject.AddComponent<DropOnDestroyed>();
            if (dropOnDestroyed.m_dropWhenDestroyed.IsEmpty())
            {
                DropTable dropTable = new();
                dropTable.m_drops.Add(new DropTable.DropData { m_item = __instance.m_itemPrefab, m_stackMin = __instance.m_amount, m_stackMax = __instance.m_amount, m_dontScale = true, m_weight = 1 });
                dropTable.m_oneOfEach = true;
                dropOnDestroyed.m_dropWhenDestroyed = dropTable;
            }
        }
    }
}
