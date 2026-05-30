import * as signalR from '@microsoft/signalr';
import { useAuthStore } from '@/store/authStore';

let connection: signalR.HubConnection | null = null;

export function getSignalRConnection() {
  if (!connection) {
    connection = new signalR.HubConnectionBuilder()
      .withUrl('/hubs/monitoring', {
        accessTokenFactory: () => useAuthStore.getState().accessToken || '',
      })
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Warning)
      .build();
  }
  return connection;
}

export async function startSignalR() {
  const conn = getSignalRConnection();
  if (conn.state === signalR.HubConnectionState.Disconnected) {
    await conn.start();
    await conn.invoke('JoinMotorGroup');
  }
}

export async function stopSignalR() {
  if (connection?.state === signalR.HubConnectionState.Connected) {
    await connection.stop();
  }
}
