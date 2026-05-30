import * as signalR from '@microsoft/signalr';
import { useAuthStore } from '@/store/authStore';

// In production, VITE_SIGNALR_URL points to the Render backend.
// In development, Vite's proxy handles /hubs -> localhost:8080.
const HUB_URL = import.meta.env.VITE_SIGNALR_URL
  ? `${import.meta.env.VITE_SIGNALR_URL}/hubs/monitoring`
  : '/hubs/monitoring';

let connection: signalR.HubConnection | null = null;

export function getSignalRConnection() {
  if (!connection) {
    connection = new signalR.HubConnectionBuilder()
      .withUrl(HUB_URL, {
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
