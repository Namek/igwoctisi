﻿namespace Client.State
{
    using System;
    using Model;
    using View.Play;
    using System.Collections.Generic;

    class PlayState : GameState
    {
        public Scene Scene { get; protected set; }

        private GameViewport _gameViewport;
        private GameHud _gameHud;

        private List<UserCommand> _commands = new List<UserCommand>();

        public PlayState(IGWOCTISI game, Map loadedMap)
            : base(game)
        {
            Scene = new Scene(loadedMap, new List<string>());
            Scene.Map = new Map("TestMap");

            var gameViewport = new GameViewport(this);
            _gameHud = new GameHud(this);

            ViewMgr.PushLayer(_gameViewport);
            ViewMgr.PushLayer(_gameHud);

            eventHandlers.Add("LeaveGame", LeaveGame);
            eventHandlers.Add("SendOrders", SendOrders);
            eventHandlers.Add("SelectPlanet", SelectPlanet);
        }

        #region View event handlers

        private void LeaveGame(EventArgs args)
        {
            Client.ChangeState(new LobbyState(Game));
        }

        private void SendOrders(EventArgs args)
        {
            // TODO: Send orders
            _commands.Clear();
        }

        private void SelectPlanet(EventArgs args)
        {
            var selectedPlanetArg = args as SelectPlanetArgs;
            _gameHud.UpdateSelectedPlanet(selectedPlanetArg.Planet);
        }

        #endregion

        #region Async netowrk callbacks

        #endregion
    }
}
