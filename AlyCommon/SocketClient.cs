using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AlyCommon
{
    public class SocketClient
    {
        private IPEndPoint _ipEndPoint;
        private Socket _client;
        public SocketClient(string ip, int port)
        {
            if (ip == null || ip == string.Empty) { throw new ArgumentNullException("ip", "ip is null"); }
            if (port == 0) { throw new ArgumentNullException("port", "port is zero"); }

            _ipEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            _client = new Socket(_ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }

        public event Action<object, EndPoint> Disconnect;

        public event Action<object, Socket> Connected;

        public event Action<object, Socket, SocketMessage> Received;

        public event Action<object, Socket, SocketMessage> Sent;

        private void ConnectCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success)
            {
                if (Connected != null) { Connected.Invoke(this, _client); }
                try
                {
                    SocketAsyncEventArgs rArgs = new SocketAsyncEventArgs();
                    rArgs.Completed += ReceiveCompleted;
                    rArgs.SetBuffer(new byte[8912], 0, 8912);
                    rArgs.UserToken = new MemoryStream();
                    if (!_client.ReceiveAsync(rArgs)) { ReceiveCompleted(this, args); }
                }
                catch (NotSupportedException nse) { throw nse; }
                catch (ObjectDisposedException oe) { throw oe; }
                catch (SocketException se) { throw se; }
                catch (Exception e) { throw e; }
            }
            else
            {
                Stop();
            }
        }

        private void ReceiveCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success && args.BytesTransferred > 0)
            {
                MemoryStream ms = args.UserToken as MemoryStream;
                ms.Write(args.Buffer, args.Offset, args.BytesTransferred);

                if (_client.Available == 0)
                {
                    ApartMessage(_client, ms, 0);
                    ms.Seek(0, SeekOrigin.Begin);
                    ms.SetLength(0);
                }

                try
                {
                    if (!_client.ReceiveAsync(args)) { ReceiveCompleted(this, args); }
                }
                catch (NotSupportedException nse) { throw nse; }
                catch (ObjectDisposedException oe) { throw oe; }
                catch (SocketException se) { throw se; }
                catch (Exception e) { throw e; }
            }
            else
            {
                Stop();
            }
        }

        private void CendCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success && args.BytesTransferred > 0)
            {
                byte[] keyBytes = new byte[16];
                Array.Copy(args.Buffer, 0, keyBytes, 0, 16);
                Guid key = new Guid(keyBytes);

                byte[] lenghtBytes = new byte[4];
                Array.Copy(args.Buffer, 16, lenghtBytes, 0, 4);
                int length = BitConverter.ToInt32(lenghtBytes);

                byte[] msgBytes = new byte[length];
                Array.Copy(args.Buffer, 20, msgBytes, 0, args.BytesTransferred - 20);

                SocketMessage msg = new SocketMessage { Key = key, Body = msgBytes, Length = length };

                if (Sent != null) { Sent.Invoke(this, _client, msg); }
            }
            else
            {
                Stop();
            }
        }

        public void Start()
        {
            if (_client.SafeHandle.IsClosed || !_client.Connected)
            {
                _client = new Socket(_ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            }

            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += ConnectCompleted;
            args.RemoteEndPoint = _ipEndPoint;

            try
            {
                if (!_client.ConnectAsync(args))
                {
                    ConnectCompleted(this, args);
                }
            }
            catch (NotSupportedException nse) { throw nse; }
            catch (ObjectDisposedException oe) { throw oe; }
            catch (SocketException se) { throw se; }
            catch (Exception e) { throw e; }
        }

        public void Start(string ip, int port)
        {
            if (ip == null || ip == string.Empty) { throw new ArgumentNullException("ip", "ip is null"); }
            if (port == 0) { throw new ArgumentNullException("port", "port is zero"); }

            _ipEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);

            Start();
        }

        public void Stop()
        {
            if (!_client.SafeHandle.IsClosed || _client.Connected)
            {
                if (Disconnect != null) { Disconnect.Invoke(this, _client.RemoteEndPoint); }
                _client.Shutdown(SocketShutdown.Both);
                _client.Close();
            }
        }

        public void Send(SocketMessage message)
        {
            if (message == null) { throw new ArgumentNullException("message"); }

            byte[] buffer = message.ToBytes();

            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += CendCompleted;
            args.SetBuffer(buffer, 0, buffer.Length);

            try
            {
                if (!_client.SendAsync(args)) { CendCompleted(this, args); }
            }
            catch (NotSupportedException nse) { throw nse; }
            catch (ObjectDisposedException oe) { throw oe; }
            catch (SocketException se) { throw se; }
            catch (Exception e) { throw e; }
        }

        #region Helper

        private void ApartMessage(Socket socket, MemoryStream ms, int offset)
        {
            ms.Seek(offset, SeekOrigin.Begin);

            byte[] keyBytes = new byte[16];
            ms.Read(keyBytes, 0, 16);
            Guid key = new Guid(keyBytes);


            byte[] lenghtBytes = new byte[4];
            ms.Read(lenghtBytes, 0, 4);
            int length = BitConverter.ToInt32(lenghtBytes);

            byte[] msgBytes = new byte[length];
            ms.Read(msgBytes, 0, (int)length);

            SocketMessage msg = new SocketMessage { Key = key, Body = msgBytes, Length = length };

            if (Received != null) { Received.Invoke(this, socket, msg); }

            offset = offset + keyBytes.Length + lenghtBytes.Length + msgBytes.Length;

            if (ms.Length > offset)
            {
                ApartMessage(socket, ms, offset);
            }
        }

        #endregion
    }
}
