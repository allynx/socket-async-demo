using AlyCommon;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace AlyMq
{
    public partial class Adapter : Form
    {
        private SocketServer _socketServer;
        public Adapter()
        {
            InitializeComponent();

            _socketServer = new SocketServer("127.0.0.1", 6060, 200);
            _socketServer.Accepted += OnAccepted;
            _socketServer.Received += OnRecevied;
            _socketServer.Sent += OnSent;
            _socketServer.ServerOff += OnServerOff;
            _socketServer.ClientOff += OnClientOff;
        }

        private void btnInit_Click(object sender, EventArgs e)
        {
            if (txtIp.Text.Trim() == string.Empty || txtPort.Text.Trim() == string.Empty)
            {
                txtLog.AppendText("Ip or port is enpty ...\r\n");
            }
            else
            {
                if (btnInit.Text == "Start")
                {
                    btnInit.Text = "Stop";
                    txtIp.Enabled = txtPort.Enabled = false;
                    txtMsg.Enabled = btnAllSend.Enabled = lvBroker.Enabled = true;


                    _socketServer.Start(txtIp.Text, int.Parse(txtPort.Text), 200);
                }
                else
                {
                    btnInit.Text = "Start";
                    txtIp.Enabled = txtPort.Enabled = true;
                    txtMsg.Enabled = btnAllSend.Enabled = lvBroker.Enabled = btnOnlySend.Enabled = false;

                    _socketServer.Stop();
                }
            }
        }

        private void OnServerOff(object sender, EndPoint args)
        {
            txtLog.Invoke(new Action(() =>
            {
                lvBroker.Items.Clear();
                txtLog.AppendText($"Server [{args}] is stop listening ...\r\n");
            }));
        }
        private void OnClientOff(object sender, EndPoint args)
        {
            txtLog.Invoke(new Action(() =>
            {
                lvBroker.Items.RemoveByKey(args.ToString());
                txtLog.AppendText($"Client [{args}] is disconnect ...\r\n");
            }));
        }

        private void OnAccepted(object sender, Socket socket)
        {
            txtLog.Invoke(new Action(() =>
            {
                lvBroker.Items.Add(new ListViewItem
                {
                    Tag = socket,
                    Text = socket.RemoteEndPoint.ToString(),
                    Name = socket.RemoteEndPoint.ToString()
                });

                txtLog.AppendText($"Client[{socket.RemoteEndPoint}] connected ...\r\n");
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

        private void btnAllSend_Click(object sender, EventArgs e)
        {
            if (txtMsg.Text.Trim() != string.Empty)
            {
                byte[] msgBytes = Encoding.UTF8.GetBytes(txtMsg.Text.Trim());

                SocketMessage message = new SocketMessage { Key = Guid.NewGuid(), Body = msgBytes, Length = msgBytes.Length };

                _socketServer.Send(message);
            }
            else
            {
                txtLog.AppendText("Cannot send an empty message ...\r\n");
            }
        }

        private void btnOnlySend_Click(object sender, EventArgs e)
        {
            if (txtMsg.Text.Trim() != string.Empty)
            {
                byte[] msgBytes = Encoding.UTF8.GetBytes(txtMsg.Text.Trim());

                SocketMessage message = new SocketMessage { Key = Guid.NewGuid(), Body = msgBytes, Length = msgBytes.Length };

                _socketServer.Send(((sender as Button).Tag as Socket),message);
            }
            else
            {
                txtLog.AppendText("Cannot send an empty message ...\r\n");
            }
        }


        private void lvBroker_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selecteds = (sender as ListView).SelectedItems;
            if (selecteds.Count > 0)
            {
                btnOnlySend.Tag = (sender as ListView).SelectedItems[0].Tag;
                btnOnlySend.Enabled = btnInit.Text == "Start" ? false : true;
            }
            else
            {
                btnOnlySend.Enabled = false;
            }
        }
    }
}
