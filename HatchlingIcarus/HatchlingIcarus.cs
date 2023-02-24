using OWML.Common;
using OWML.ModHelper;
using HarmonyLib;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HatchlingIcarus {
    public class HatchlingIcarus : ModBehaviour
    {
        public static GameObject gravObject = null;
        private int mode; //0 is feet, 1 is cam
        public static float force;
        private bool activeAlign;
        public static HatchlingIcarus instance;
        public static ScreenPrompt gravNotification = new ScreenPrompt("Gravity Active");

        /**
         * Just need to load the patches
         */
        private void Start()
        {
            //Make all of the patches
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

            ModHelper.Console.WriteLine($"Hatchling Icarus is ready!");
            instance = this;
        }

        /**
         * Align the field to the player's rotation, taking the setting into accout
         */
        private void AlignField()
        {
            if(gravObject != null)
            {
                //To the body
                if(mode == 0)
                    gravObject.transform.rotation = Locator.GetPlayerTransform().rotation;

                //To the camera
                else
                {
                    gravObject.transform.rotation = Locator.GetPlayerCamera().transform.rotation;
                    Vector3 rotationAxis = new Vector3(1, 0, 0);
                    gravObject.transform.rotation *= Quaternion.AngleAxis(-90, rotationAxis);
                }
            }
        }

        /**
         * Just need to check for the player pressing P & move the gravity object
         */
        private void Update()
        {
            if (gravObject != null)
            {
                //Enable/disable the field
                if (Keyboard.current[Key.K].wasPressedThisFrame)
                {
                    gravObject.SetActive(!gravObject.activeSelf);
                    gravObject.GetComponent<DirectionalForceVolume>().SetAttachedBody(Locator.GetSunTransform().GetComponent<OWRigidbody>());
                    gravNotification.SetVisibility(gravObject.activeSelf);
                    AlignField();
                }

                //Move it to the player
                Transform playerTransform = Locator.GetPlayerTransform();
                gravObject.transform.position = playerTransform.position;
                if (activeAlign)
                    AlignField();
            }
        }

        /**
         * Retrieve settings for the mod
         */
        public override void Configure(IModConfig config)
        {
            //Alignment mode
            if (config.GetSettingsValue<string>("Alignment Mode").Equals("Body"))
                mode = 0;
            else
                mode = 1;

            //Force
            force = (config.GetSettingsValue<float>("Field Force") * 10.0f) + 2;
            if (gravObject != null)
                gravObject.GetComponent<DirectionalForceVolume>().SetFieldMagnitude(force);

            //Active or not
            activeAlign = config.GetSettingsValue<bool>("Active Alignment");

            //Active or not
            Patches.disableImpacts = config.GetSettingsValue<bool>("Disable Impact Damage");
        }
    }
}

