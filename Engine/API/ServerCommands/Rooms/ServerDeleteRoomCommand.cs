﻿using Engine.API.ClientCommands;
using Engine.Model.Server;
using System;
using System.Security;

namespace Engine.API.ServerCommands
{
  [SecurityCritical]
  class ServerDeleteRoomCommand :
    ServerCommand<ServerDeleteRoomCommand.MessageContent>
  {
    public const long CommandId = (long)ServerCommandId.DeleteRoom;

    public override long Id
    {
      [SecuritySafeCritical]
      get { return CommandId; }
    }

    [SecuritySafeCritical]
    public override void Run(MessageContent content, ServerCommandArgs args)
    {
      if (string.IsNullOrEmpty(content.RoomName))
        throw new ArgumentException("RoomName");

      if (string.Equals(content.RoomName, ServerModel.MainRoomName))
      {
        ServerModel.Api.SendSystemMessage(args.ConnectionId, "Вы не можете удалить основную комнату.");
        return;
      }

      if (!RoomExists(content.RoomName, args.ConnectionId))
        return;

      using (var context = ServerModel.Get())
      {
        var deletingRoom = context.Rooms[content.RoomName];
        if (!deletingRoom.Admin.Equals(args.ConnectionId))
        {
          ServerModel.Api.SendSystemMessage(args.ConnectionId, "Вы не являетесь администратором комнаты. Операция отменена.");
          return;
        }

        context.Rooms.Remove(deletingRoom.Name);

        var sendingContent = new ClientRoomClosedCommand.MessageContent { Room = deletingRoom };
        foreach (string user in deletingRoom.Users)
          ServerModel.Server.SendMessage(user, ClientRoomClosedCommand.CommandId, sendingContent);
      }
    }

    [Serializable]
    public class MessageContent
    {
      private string roomName;

      public string RoomName
      {
        get { return roomName; }
        set { roomName = value; }
      }
    }
  }
}
