using AlyCommon;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using SocketMessage = AlyCommon.SocketMessage;

namespace AlyMq
{
    public partial class Broker : Form
    {
        private SocketClient _socketClient;

        public Broker()
        {
            InitializeComponent();

            _socketClient = new SocketClient("127.0.0.1", 6060);
            _socketClient.Connected += OnConnected;
            _socketClient.Received += OnRecevied;
            _socketClient.Sent += OnSent;
            _socketClient.Disconnect += OnDisconnect;
        }

        private void btnInit_Click(object sender, EventArgs e)
        {
            if (txtIp.Text.Trim() == string.Empty || txtPort.Text.Trim() == string.Empty)
            {
                txtLog.AppendText("Ip and Port is enpty");
            }
            else
            {
                if (btnInit.Text == "Start")
                {
                    _socketClient.Start(txtIp.Text.Trim(), int.Parse(txtPort.Text.Trim()));
                }
                else
                {
                    _socketClient.Stop();
                }
            }
        }

        private void OnDisconnect(object sender, EndPoint args)
        {
            txtLog.Invoke(new Action(() =>
            {
                txtLog.AppendText($"disconnect [{args}] ...\r\n");

                btnInit.Text = "Start";
                txtIp.Enabled = txtPort.Enabled = true;
                txtMsg.Enabled = btnSend.Enabled = false;
            }));
        }

        private void OnConnected(object sender, Socket socket)
        {
            txtLog.Invoke(new Action(() =>
            {
                btnInit.Text = "Stop";
                txtIp.Enabled = txtPort.Enabled = false;
                txtMsg.Enabled = btnSend.Enabled = true;

                txtLog.AppendText($"Server[{socket.RemoteEndPoint}] connected ...\r\n");
            }));
        }

        private void OnRecevied(object sender, Socket socket, SocketMessage message)
        {
            string dataString = Encoding.UTF8.GetString(message.Body);
            txtLog.Invoke(new Action(() =>
            {
                txtLog.AppendText($"Receviced from [{socket.RemoteEndPoint}]---->\r\n Key:{message.Key}\r\n Lehgth:{message.Length}\r\n Body: {dataString}\r\n");
            }));
        }

        private void OnSent(object sender, Socket socket, SocketMessage message)
        {
            string dataString = Encoding.UTF8.GetString(message.Body);
            txtLog.Invoke(new Action(() =>
            {
                txtLog.AppendText($"Sent to [{socket.RemoteEndPoint}]---->\r\n Key:{message.Key}\r\n Lehgth:{message.Length}\r\n Body: {dataString}\r\n");
            }));
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (txtMsg.Text.Trim() == string.Empty)
            {
                txtLog.AppendText("Cannot send an empty message ...\r\n");
            }
            else
            {
                byte[] bodyBytes = Encoding.UTF8.GetBytes(txtMsg.Text.Trim());
                SocketMessage message = new SocketMessage { Key=Guid.NewGuid(),Length= bodyBytes.Length, Body=bodyBytes };
                _socketClient.Send(message);
            }

        }
    }
}
