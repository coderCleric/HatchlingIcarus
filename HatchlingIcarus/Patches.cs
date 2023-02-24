using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace HatchlingIcarus
{
    [HarmonyPatch]
    public static class Patches
    {
        public static bool disableImpacts;

        /**
         * When the locator is activated, make the gravity field
         */
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Locator), nameof(Locator.LocateSceneObjects))]
        public static void MakeGravField()
        {
            //Add stuff for the gravity field
            GameObject gravField = new GameObject();
            gravField.SetActive(false);
            gravField.layer = LayerMask.NameToLayer("BasicEffectVolume");

            BoxCollider box = gravField.AddComponent<BoxCollider>();
            box.isTrigger = true;

            OWCollider OWCol = gravField.AddComponent<OWCollider>();
            OWCol._lodActivationMask = new DynamicOccupantMask();
            OWCol._lodActivationMask.ship = false;

            gravField.AddComponent<OWTriggerVolume>();

            DirectionalForceVolume forceVolume = gravField.AddComponent<DirectionalForceVolume>();
            forceVolume._priority = 99;
            forceVolume._alignmentPriority = 99;
            forceVolume._fieldDirection = Vector3.down;
            forceVolume._fieldMagnitude = HatchlingIcarus.force;

            HatchlingIcarus.gravObject = gravField;

            //Add the prompt
            Locator.GetPromptManager().AddScreenPrompt(HatchlingIcarus.gravNotification, PromptPosition.BottomCenter);
            HatchlingIcarus.gravNotification.SetVisibility(false);
        }

        /**
         * Disable impact damage if the option is on
         * 
         * @param __result The return value
         */
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerResources), nameof(PlayerResources.ApplyInstantDamage))]
        public static bool BlockImpactDamage(ref bool __result, InstantDamageType type)
        {
            if(disableImpacts && type == InstantDamageType.Impact)
            {
                __result = false;
                return false;
            }
            return true;
        }

        /**
         * Prevent the ship from being affected by the personal gravity field
         * 
         * @param __instance The calling volume
         * @param hitObj The object being added
         */
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ForceVolume), nameof(ForceVolume.OnEffectVolumeEnter))]
        public static bool BlockShipFromGrav(ForceVolume __instance, GameObject hitObj)
        {
            bool flag = __instance.gameObject == HatchlingIcarus.gravObject && hitObj == Locator.GetShipDetector();
            return !flag;
        }
    }
}
