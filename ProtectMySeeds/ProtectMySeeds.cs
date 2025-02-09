using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Reflection;
using UnityEngine;

namespace ProtectMySeeds
{
    [BepInPlugin(ModGUID, ModName, ModVersion)]
    public class ProtectMySeeds : BaseUnityPlugin
    {
        internal const string ModName = "ProtectMySeeds";
        internal const string ModVersion = "1.0.4";
        internal const string Author = "NullPointerCollection";
        internal const string ModGUID = "com.nullpointercollection.deepernorth";

        internal static ManualLogSource Log;
        internal Harmony harmony = new(ModGUID);
        ServerSync.ConfigSync configSync = new(ModGUID) { DisplayName = ModName, CurrentVersion = ModVersion, MinimumRequiredVersion = ModVersion, /*IsLocked = true,*/ ModRequired = true };

        public void Awake()
        {
            Log = Logger;

            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    [HarmonyPatch(typeof(Plant), nameof(Plant.Awake))]
    public static class PlantAwakePatch
    {
        public static void Postfix(Plant __instance)
        {
            //Debug.LogError("Awake: " + __instance + " - " + __instance.m_name + " - " + __instance.name + " - " + __instance.m_grownPrefabs);

            //Debug.LogError("Awake: " + ZNet.instance.GetTime());
            var plantSeed = __instance.GetComponent<Piece>().m_resources[0].m_resItem.gameObject;
            //var requirement = plantPiece.m_resources[0];

            var dropOnDestroyed = __instance.gameObject.GetComponent<DropOnDestroyed>() ?? __instance.gameObject.AddComponent<DropOnDestroyed>();
            if (dropOnDestroyed.m_dropWhenDestroyed.IsEmpty())
            {
                var dropTable = new DropTable();
                dropTable.m_drops.Add(new DropTable.DropData { m_item = plantSeed, m_stackMin = 1, m_stackMax = 1, m_dontScale = false, m_weight = 1 });
                //dropTable.m_drops.Add(new DropTable.DropData { m_item = ObjectDB.instance.GetItemPrefab("CarrotSeeds"), m_stackMin = 1, m_stackMax = 1, m_dontScale = false, m_weight = 1 });
                dropOnDestroyed.m_dropWhenDestroyed = dropTable;
            }
            bool test = __instance.gameObject.GetComponent<Pickable>().m_nview.HasOwner();

        }
    }

}
