using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AlyCommon
{
    public class SocketServer
    {
        private int _backlog;
        private IPEndPoint _ipEndPoint;
        private Socket _server;
        private List<Socket> _clients;

        public SocketServer(string ip, int port, int backlog)
        {
            if (ip == null || ip == string.Empty) { throw new ArgumentNullException("ip", "ip is null"); }
            if (port == 0) { throw new ArgumentNullException("port", "port is zero"); }
            if (backlog == 0) { throw new ArgumentException("Argument cannot be 0", "backlog"); }

            _backlog = backlog;
            _ipEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            _server = new Socket(_ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _clients = new List<Socket>();
        }

        public event Action<object, Socket> Accepted;

        public event Action<object, Socket, SocketMessage> Received;

        public event Action<object, Socket, SocketMessage> Sent;

        public event Action<object, EndPoint> ServerOff;

        public event Action<object, EndPoint> ClientOff;

        private void AcceptCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success)
            {

                SocketAsyncEventArgs rArgs = new SocketAsyncEventArgs();
                rArgs.Completed += ReceiveCompleted;
                rArgs.AcceptSocket = args.AcceptSocket;
                rArgs.SetBuffer(new byte[8912], 0, 8912);
                rArgs.UserToken = new MemoryStream();

                _clients.Add(args.AcceptSocket);
                if (Accepted != null) { Accepted.Invoke(this, args.AcceptSocket); }

                try
                {
                    if (!rArgs.AcceptSocket.ReceiveAsync(rArgs)) { ReceiveCompleted(this, rArgs); }

                    args.AcceptSocket = null;
                    if (!_server.AcceptAsync(args)) { AcceptCompleted(this, args); }
                }
                catch (NotSupportedException nse) { throw nse; }
                catch (ObjectDisposedException oe) { throw oe; }
                catch (SocketException se) { throw se; }
                catch (Exception e) { throw e; }
            }
            else
            {
                _clients.Remove(args.AcceptSocket);
                args.AcceptSocket.Close();
            }
        }

        private void ReceiveCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success && args.BytesTransferred > 0)
            {
                MemoryStream ms = args.UserToken as MemoryStream;
                ms.Write(args.Buffer, args.Offset, args.BytesTransferred);

                if (args.AcceptSocket.Available == 0)
                {
                    ApartMessage(args.AcceptSocket,ms,0);
                    ms.Seek(0,SeekOrigin.Begin);
                    ms.SetLength(0);
                }

                try
                {
                    if (!args.AcceptSocket.ReceiveAsync(args)) { ReceiveCompleted(this, args); }
                }
                catch (NotSupportedException nse) { throw nse; }
                catch (ObjectDisposedException oe) { throw oe; }
                catch (SocketException se) { throw se; }
                catch (Exception e) { throw e; }
            }
            else
            {
                if (!args.AcceptSocket.SafeHandle.IsClosed)
                {
                    if (ClientOff != null) { ClientOff.Invoke(this, args.AcceptSocket.RemoteEndPoint); }
                    _clients.Remove(args.AcceptSocket);
                    args.AcceptSocket.Shutdown(SocketShutdown.Both);
                    args.AcceptSocket.Close();
                }
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

                if (Sent != null) { Sent.Invoke(this, args.AcceptSocket, msg); }
            }
            else
            {
                if (!args.AcceptSocket.SafeHandle.IsClosed)
                {
                    if (ClientOff != null) { ClientOff.Invoke(this, args.AcceptSocket.RemoteEndPoint); }
                    _clients.Remove(args.AcceptSocket);
                    args.AcceptSocket.Shutdown(SocketShutdown.Both);
                    args.AcceptSocket.Close();
                }
            }
        }
        
        public void Start()
        {
            try
            {
                if (_server.SafeHandle.IsClosed) { _server = new Socket(_ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp); }
                _server.Bind(_ipEndPoint);
                _server.Listen(_backlog);

                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.Completed += AcceptCompleted;
                if (!_server.AcceptAsync(args)) { AcceptCompleted(this, args); }

            }
            catch (NotSupportedException nse) { throw nse; }
            catch (ObjectDisposedException oe) { throw oe; }
            catch (SocketException se) { throw se; }
            catch (Exception e) { throw e; }
        }

        public void Start(string ip, int port, int backlog)
        {
            if (ip == null || ip == string.Empty) { throw new ArgumentNullException("ip", "ip is null"); }
            if (port == 0) { throw new ArgumentNullException("port", "port is zero"); }
            if (backlog == 0) { throw new ArgumentException("Argument cannot be 0", "backlog"); }
            _backlog = backlog;
            _ipEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            Start();
        }

        public void Stop()
        {
            if (!_server.SafeHandle.IsClosed)
            {
                _clients.ForEach(broker =>
                {
                    if (!broker.SafeHandle.IsClosed)
                    {
                        broker.Shutdown(SocketShutdown.Both);
                        broker.Close();
                    }
                });
                _clients.Clear();
                if (ServerOff != null) { ServerOff.Invoke(this, _server.LocalEndPoint); }
                _server.Close();
            }
        }

        public void Send(SocketMessage message)
        {
            if (message == null) { throw new ArgumentNullException("message"); }

            byte[] buffer = message.ToBytes();

            _clients.ForEach(client =>
            {
                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.Completed += CendCompleted;
                args.AcceptSocket = client;
                args.SetBuffer(buffer,0,buffer.Length);

                try
                {
                    if (!client.SendAsync(args)) { CendCompleted(this, args); }
                }
                catch (NotSupportedException nse) { throw nse; }
                catch (ObjectDisposedException oe) { throw oe; }
                catch (SocketException se) { throw se; }
                catch (Exception e) { throw e; }
            });
        }
        
        public void Send(Socket socket,SocketMessage message)
        {
            if (socket == null) { throw new ArgumentNullException("socket"); }
            if (message == null) { throw new ArgumentNullException("message"); }

            byte[] buffer = message.ToBytes();

            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += CendCompleted;
            args.AcceptSocket = socket;
            args.SetBuffer(buffer,0,buffer.Length);

            try
            {
                if (!socket.SendAsync(args)) { CendCompleted(this, args); }
            }
            catch (NotSupportedException nse) { throw nse; }
            catch (ObjectDisposedException oe) { throw oe; }
            catch (SocketException se) { throw se; }
            catch (Exception e) { throw e; }
        }

        #region Helper
        private void ApartMessage(Socket socket,MemoryStream ms, int offset)
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
                ApartMessage(socket,ms, offset);
            }
        }

        #endregion
    }
}
