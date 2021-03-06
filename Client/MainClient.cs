﻿using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Threading.Tasks;
using MenuAPI;
using Client.Menus.InteractionMenu;
using Client.UIComponents;
using Client.HUD;
using Client.Gameplay;

namespace Client
{
    public class MainClient : BaseScript
    {
        private static InteractionMenu InteractionMenu { get; set; }
        private Playerlist playerList;
        private WastedScreen wastedScreen;
        public static PersonalVehicleController PersonalVehicleController { get; private set; }
        private static bool isRadarExtended = false;
        private static int radarTimer;
        public MainClient()
        {
            PersonalVehicleController = new PersonalVehicleController();

            InteractionMenu = new InteractionMenu();
            InteractionMenu.CreateMenu();

            API.StatSetInt((uint)API.GetHashKey("MP0_STAMINA"), 100, true);
            API.RegisterCommand("passive", new Action(Gameplay.PassiveMode.Enable), false);

            Client.Players.Colors.Setup();

            WorldContent.WeaponPickups.CreatePickups();

            HUD.Blips.Create();
            HUD.GamerTags.Create();

            API.SetRadarBigmapEnabled(false, false);

            playerList = new Playerlist();

            wastedScreen = new WastedScreen();

            Tick += OnTick;
        }

        private async Task OnTick()
        {
            HUD.Blips.Update();
            HUD.GamerTags.Update();
            RadarController();
            PersonalVehicleController.OnTick();
            playerList.Loop();
        }

        // Extend the radar by pressing Z
        private async void RadarController()
        {
            if (API.IsControlJustReleased(0, 20)) {
                await BaseScript.Delay(25);
                ToggleRadarBigMode();
            }

            // Minimize the radar automatically after 10 seconds
            if (isRadarExtended)
            {
                if (API.GetGameTimer() - radarTimer >= 10000)
                {
                    ToggleRadarBigMode();
                }
            } 
        }

        private void ToggleRadarBigMode()
        {
            isRadarExtended = !isRadarExtended;
            API.SetRadarBigmapEnabled(isRadarExtended, false);
            radarTimer = API.GetGameTimer();
        }
    }
}
