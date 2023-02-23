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
        /**
         * When the locator is activated, make the gravity field
         */
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Locator), nameof(Locator.LocateSceneObjects))]
        public static void MakeGravField()
        {
            GameObject gravField = new GameObject();
            gravField.SetActive(false);
            gravField.layer = LayerMask.NameToLayer("BasicEffectVolume");

            BoxCollider box = gravField.AddComponent<BoxCollider>();
            box.isTrigger = true;

            OWCollider OWCol = gravField.AddComponent<OWCollider>();
            OWCol._lodActivationMask = new DynamicOccupantMask();

            gravField.AddComponent<OWTriggerVolume>();

            DirectionalForceVolume forceVolume = gravField.AddComponent<DirectionalForceVolume>();
            forceVolume._priority = 99;
            forceVolume._alignmentPriority = 99;
            forceVolume._fieldDirection = Vector3.down;
            forceVolume._fieldMagnitude = HatchlingIcarus.force;

            HatchlingIcarus.gravObject = gravField;
        }
    }
}
