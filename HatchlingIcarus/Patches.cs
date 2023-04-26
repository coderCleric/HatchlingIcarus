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
        public static bool gravOn = false;
        public static Vector3 gravDirection = Vector3.up;

        /**
         * When toggled on, always tell the player's force detector that it should go a specific way
         * 
         * @param __instance The instance of the alignment force detector
         */
        [HarmonyPrefix]
        [HarmonyPatch(typeof(AlignmentForceDetector), nameof(AlignmentForceDetector.AccumulateAcceleration))]
        public static bool OverridePlayerForces(AlignmentForceDetector __instance)
        {
            if(gravOn && __instance.CompareTag("PlayerDetector"))
            {
                __instance._netAcceleration = gravDirection * HatchlingIcarus.force;
                if (HatchlingIcarus.force != 0)
                    __instance._aligmentAccel = gravDirection;
                else
                    __instance._aligmentAccel = Vector3.zero;
                return false;
            }
            return true;
        }

        /**
         * When the locator comes on, do some setup stuff
         */
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Locator), nameof(Locator.LocateSceneObjects))]
        public static void ResetOnLoad()
        {
            if (Locator.GetPlayerTransform() != null) //This tells if we're in a valid system or not
            {
                //Turn off the personal gravity
                gravOn = false;
                
                //Add the prompt
                Locator.GetPromptManager().AddScreenPrompt(HatchlingIcarus.gravNotification, PromptPosition.BottomCenter);
                HatchlingIcarus.gravNotification.SetVisibility(false);
            }
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
    }
}
