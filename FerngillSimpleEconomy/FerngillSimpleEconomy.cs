﻿using fse.core.handlers;
using fse.core.models;
using fse.core.patches;
using fse.core.services;
using HarmonyLib;
using StardewModdingAPI;

namespace fse.core
{
	// ReSharper disable once UnusedType.Global
	public class FerngillSimpleEconomy : Mod
	{
		private EconomyService _economyService;

		public override void Entry(IModHelper helper)
		{
			ConfigModel.Instance = helper.ReadConfig<ConfigModel>();
			_economyService = new EconomyService(helper, Monitor);
			RegisterPatches(helper);
			RegisterHandlers(helper);
			helper.ConsoleCommands.Add("fse_reset", "Fully Resets Ferngill Simple Economy", (_, _) =>
			{
				_economyService.SetupForNewYear();
				_economyService.AdvanceOneDay();
			});
		}

		private void RegisterPatches(IModHelper helper)
		{
			var harmony = new Harmony(ModManifest.UniqueID);

			SelfRegisteringPatches.Initialize(helper, _economyService, Monitor);
			new ObjectPatches().Register(harmony);
			new ShopMenuPatches().Register(harmony);
		}

		private void RegisterHandlers(IModHelper helper)
		{
			new DayEndHandler(helper, Monitor, _economyService).Register();
			new SaveLoadedHandler(helper, Monitor, _economyService).Register();
			new GameLoadedHandler(helper, Monitor, ModManifest, _economyService).Register();
			new GameMenuLoadedHandler(helper, Monitor, _economyService).Register();
			new MultiplayerHandler(helper, Monitor, _economyService).Register();
		}
	}
}