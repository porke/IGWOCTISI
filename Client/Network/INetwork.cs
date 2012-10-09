﻿namespace Client.Network
{
    using System;
    using System.Net;
    using Model;

    public interface INetwork
    {
        void Initialize(Client client);
        void Release();
        void Update(double delta, double time);

        IAsyncResult BeginConnect(string hostname, int port, AsyncCallback asyncCallback, object asyncState);
        void EndConnect(IAsyncResult asyncResult);
        IAsyncResult BeginLogin(string login, string password, AsyncCallback asyncCallback, object asyncState);
        void EndLogin(IAsyncResult asyncResult);
        IAsyncResult BeginDisconnect(AsyncCallback asyncCallback, object asyncState);
        void EndDisconnect(IAsyncResult asyncResult);
        IAsyncResult BeginGetGameList(AsyncCallback asyncCallback, object asyncState);
        void EndGetGameList(IAsyncResult asyncResult);

        IAsyncResult BeginReceiveGameState(AsyncCallback asyncCallback, object asyncState);
        GameState EndReceiveGameState(IAsyncResult asyncResult);
        IAsyncResult BeginSendCommands(UserCommands commands, AsyncCallback asyncCallback, object asyncState);
        void EndSendCommands(IAsyncResult asyncResult);
    }
}
