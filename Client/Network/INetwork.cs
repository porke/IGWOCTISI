﻿namespace Client.Network
{
    using System;
    using System.Net;
    using Model;
    using System.Collections.Generic;

    public interface INetwork
    {
        void Initialize(GameClient client);
        void Release();
        void Update(double delta, double time);

        IAsyncResult BeginConnect(string hostname, int port, AsyncCallback asyncCallback, object asyncState);
        bool EndConnect(IAsyncResult asyncResult);
        IAsyncResult BeginLogin(string login, string password, AsyncCallback asyncCallback, object asyncState);
        bool EndLogin(IAsyncResult asyncResult);
        IAsyncResult BeginJoinGameLobby(int lobbyId, AsyncCallback asyncCallback, object asyncState);
        GameInfo EndJoinGameLobby(IAsyncResult asyncResult);
        IAsyncResult BeginDisconnect(AsyncCallback asyncCallback, object asyncState);
        void EndDisconnect(IAsyncResult asyncResult);
        IAsyncResult BeginGetGameList(AsyncCallback asyncCallback, object asyncState);
        List<LobbyInfo> EndGetGameList(IAsyncResult asyncResult);

        IAsyncResult BeginReceiveGameState(AsyncCallback asyncCallback, object asyncState);
        GameState EndReceiveGameState(IAsyncResult asyncResult);
        IAsyncResult BeginSendCommands(UserCommands commands, AsyncCallback asyncCallback, object asyncState);
        void EndSendCommands(IAsyncResult asyncResult);

        /// <summary>
        /// Arguments: username, chat message, time
        /// </summary>
        event Action<ChatMessage> OnChatMessageReceived;

        /// <summary>
        /// Argument: disconnection reason (may be empty)
        /// </summary>
        event Action<string> OnDisconnected;
    }
}
