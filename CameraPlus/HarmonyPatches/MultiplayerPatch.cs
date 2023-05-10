using System.Collections.Generic;
using HarmonyLib;
using CameraPlus.Utilities;

namespace CameraPlus.HarmonyPatches
{
    [HarmonyPatch(typeof(MultiplayerSessionManager),"Start")]
    internal class MultiplayerSessionManagerPatch
    {
        public static MultiplayerSessionManager Instance { get; private set; }
        static void Postfix(MultiplayerSessionManager __Instance)
        {
            Instance = __Instance;
            MultiplayerSession.Init(Instance);
            Plugin.Log.Info($"Success Find SessionManager");
        }
    }

    [HarmonyPatch(typeof(MultiplayerLobbyAvatarPlaceManager), nameof(MultiplayerLobbyAvatarPlaceManager.Activate))]
    internal class MultiplayerLobbyAvatarPlaceManagerPatch
    {
        public static MultiplayerLobbyAvatarPlaceManager Instance { get; private set; }
        public static List<MultiplayerLobbyAvatarPlace> LobbyAvatarPlaces = null;
        static void Postfix(MultiplayerLobbyAvatarPlaceManager __Instance, List<MultiplayerLobbyAvatarPlace> ____allPlaces)
        {
            Instance = __Instance;
            LobbyAvatarPlaces = ____allPlaces;
#if DEBUG
            Plugin.Log.Notice("Got MultiplayerLobbyAvatarPlaceManager");
#endif
        }
    }

    [HarmonyPatch(typeof(MultiplayerLobbyController), "ActivateMultiplayerLobby")]
    internal class MultiplayerLobbyControllerPatch
    {
        public static MultiplayerLobbyController Instance { get; private set; }
        static void Postfix(MultiplayerLobbyController __Instance)
        {
            Instance = __Instance;
#if DEBUG
            Plugin.Log.Notice("Got MultiplayerLobbyController");
#endif
        }
    }
    [HarmonyPatch(typeof(MultiplayerPlayersManager), "BindPlayerFactories")]
    internal class MultiplayerPlayersManagerPatch
    {
        public static MultiplayerPlayersManager Instance { get; private set; }
        static void Postfix(MultiplayerPlayersManager __Instance)
        {
            Instance = __Instance;
#if DEBUG
            Plugin.Log.Notice("Got MultiplayerPlayersManager");
#endif
        }
    }
    [HarmonyPatch(typeof(MultiplayerScoreProvider),"Update")]
    internal class MultiplayerScoreProviderPatch
    {
        public static MultiplayerScoreProvider Instance = null;
        static void Postfix(MultiplayerScoreProvider __Instance)
        {
            if (Instance == null)
            {
#if DEBUG
                Plugin.Log.Notice("Got MultiplayerScoreProvider");
#endif
                Instance = __Instance;
            }
        }
    }
}