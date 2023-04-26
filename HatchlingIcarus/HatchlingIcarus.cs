using OWML.Common;
using OWML.ModHelper;
using HarmonyLib;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HatchlingIcarus {
    public class HatchlingIcarus : ModBehaviour
    {
        //public static GameObject gravObject = null;
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
            //To the body
            if(mode == 0)
                Patches.gravDirection = Locator.GetPlayerTransform().up * -1;

            //To the camera
            else
                Patches.gravDirection = Locator.GetPlayerCamera().transform.forward;
        }

        /**
         * Just need to check for the player pressing K & move the gravity object
         */
        private void Update()
        {
            if (Locator.GetPlayerTransform() != null)
            {
                //Enable/disable the grav
                if (Keyboard.current[Key.K].wasPressedThisFrame)
                {
                    Patches.gravOn = !Patches.gravOn;

                    if (Patches.gravOn) //If it's on now, align it
                        AlignField();

                    //Do the notification
                    gravNotification.SetVisibility(Patches.gravOn);
                }

                if (activeAlign)
                {
                    AlignField();
                }
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
            force = config.GetSettingsValue<float>("Field Force");

            //Active or not
            activeAlign = config.GetSettingsValue<bool>("Active Alignment");

            //Active or not
            Patches.disableImpacts = config.GetSettingsValue<bool>("Disable Impact Damage");
        }
    }
}

