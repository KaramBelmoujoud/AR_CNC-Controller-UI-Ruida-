using UnityEngine;
using UnityEngine.UI;
using System;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;
using System.Threading;
using System.Text.RegularExpressions;

public class python_runner : MonoBehaviour
{
    public InputField IpText, PortText, XText, YText;
    string ip;
    int port;
    public InputField x, y;
    string PacketData = null;
    DataTransfer dataTransfer;
    Position controlStick;
    [SerializeField]
    GameObject Data;
    [SerializeField]
    GameObject Stick;
    private UdpClient myClient;


    void Awake()
    {
        dataTransfer = Data.GetComponent<DataTransfer>();
        controlStick = Stick.GetComponent<Position>();
    }

    void Update()
    {
        if (controlStick.transform.position.x > 10 || controlStick.transform.position.x < -10)
        {
            Debug.Log(controlStick.transform.position.x);
        }
        else if (controlStick.transform.position.y > 10 || controlStick.transform.position.y < -10)
        {
            Debug.Log(controlStick.transform.position.y);
        }
    }

    public class Root
    {
        [JsonProperty("data.data")]
        public string DataData { get; set; }

        [JsonProperty("data.len")]
        public string DataLen { get; set; }
    }

    public void SaveIpPort()
    {
        ip = IpText.text;
        port = int.Parse(PortText.text);
    }

    
    [Obsolete]
    public void Connect()
    {
        if (Regex.IsMatch(ip, @"^([1-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])(\.([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])){3}$"))
        {
            myClient = new UdpClient(40200);
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ip), port);
            myClient.Connect(ep);
        }
        else
        {
            myClient = new UdpClient(40200);
            IPAddress ipAddress = Dns.Resolve(ip).AddressList[0];
            IPEndPoint ep = new IPEndPoint(ipAddress, port);
            myClient.Connect(ep);
        }
    }

    [HideInInspector]
    public int time = 0;
    [HideInInspector]
    public float X_pos = 0, Y_pos = 0;

    [Obsolete]
    public void SaveXY()
    {
        float Xex = float.Parse(x.text), Yex = float.Parse(y.text);
        float MX = float.Parse(XText.text), MY = float.Parse(YText.text);
        Connect();
        if (Xex > MX)
        {
            Xex = MY;
        }
        if (Yex > MY)
        {
            Yex = MY;
        }

        if (X_pos < Xex)
        {
            float Xpos = ((Xex - X_pos) / 60f) * 1000;
            X_pos = Xex;
            string jsontext = dataTransfer.Con_Left;
            var Dt = JsonConvert.DeserializeObject<Root[]>(jsontext);

            for (int j = 0; j < Dt.Length; j++)
            {
                var data = Dt[j];
                PacketData = data.DataData;
                PacketData = PacketData.Replace(":", "");
                //Debug.Log(PacketData);
                byte[] myData = new byte[PacketData.Length / 2];

                for (int i = 0, h = 0; h < PacketData.Length; i++, h += 2)
                {
                    myData[i] = (byte)Int32.Parse(PacketData.Substring(h, 2), System.Globalization.NumberStyles.HexNumber);
                }
                myClient.Send(myData, myData.Length);
                time++;
                if (time == Dt.Length - 1)
                {
                    //Debug.Log("LEFT " + Xpos);
                    Thread.Sleep((int)Xpos);
                }
            }
            time = 0;
        }
        else if (X_pos > Xex)
        {
            float Xpos = ((X_pos - Xex) / 60f) * 1000;
            X_pos = Xex;
            string jsontext = dataTransfer.Con_Right;
            var Dt = JsonConvert.DeserializeObject<Root[]>(jsontext);

            for (int j = 0; j < Dt.Length; j++)
            {
                var data = Dt[j];
                PacketData = data.DataData;
                PacketData = PacketData.Replace(":", "");
                //Debug.Log(PacketData);
                byte[] myData = new byte[PacketData.Length / 2];

                for (int i = 0, h = 0; h < PacketData.Length; i++, h += 2)
                {
                    myData[i] = (byte)Int32.Parse(PacketData.Substring(h, 2), System.Globalization.NumberStyles.HexNumber);
                }
                myClient.Send(myData, myData.Length);
                time++;
                //Debug.Log(time);
                if (time == Dt.Length - 1)
                {
                    //Debug.Log("RIGHT " + Xpos);
                    Thread.Sleep((int)Xpos);
                }
            }
            time = 0;
        }
        if (Y_pos < Yex)
        {
            float Ypos = ((Yex - Y_pos) / 60f) * 1000;
            Y_pos = Yex;
            string jsontext = dataTransfer.Con_Down;
            var Dt = JsonConvert.DeserializeObject<Root[]>(jsontext);

            for (int j = 0; j < Dt.Length; j++)
            {
                var data = Dt[j];
                PacketData = data.DataData;
                PacketData = PacketData.Replace(":", "");
                //Debug.Log(PacketData);
                byte[] myData = new byte[PacketData.Length / 2];

                for (int i = 0, h = 0; h < PacketData.Length; i++, h += 2)
                {
                    myData[i] = (byte)Int32.Parse(PacketData.Substring(h, 2), System.Globalization.NumberStyles.HexNumber);
                }
                myClient.Send(myData, myData.Length);
                time++;
                if (time == Dt.Length - 1)
                {
                    //Debug.Log("DOWN " + Ypos);
                    Thread.Sleep((int)Ypos);
                }
            }
            time = 0;
        }
        else if (Y_pos > Yex)
        {
            float Ypos = ((Y_pos - Yex) / 60f) * 1000;
            Y_pos = Yex;
            string jsontext = dataTransfer.Con_Up;
            var Dt = JsonConvert.DeserializeObject<Root[]>(jsontext);

            for (int j = 0; j < Dt.Length; j++)
            {
                var data = Dt[j];
                PacketData = data.DataData;
                PacketData = PacketData.Replace(":", "");
                //Debug.Log(PacketData);
                byte[] myData = new byte[PacketData.Length / 2];

                for (int i = 0, h = 0; h < PacketData.Length; i++, h += 2)
                {
                    myData[i] = (byte)Int32.Parse(PacketData.Substring(h, 2), System.Globalization.NumberStyles.HexNumber);
                }
                myClient.Send(myData, myData.Length);
                time++;
                if (time == Dt.Length - 1)
                {
                    //Debug.Log("UP " + Ypos);
                    Thread.Sleep((int)Ypos);
                }
            }
            time = 0;
        }
        myClient.Close();
    }

    [Obsolete]
    public void LEFT()
    {
        string jsontext = dataTransfer.Left;
        var Dt = JsonConvert.DeserializeObject<Root[]>(jsontext);
        Connect();
        // foreach (var data in Dt)
        for (int j = 0; j < Dt.Length; j++)
        {
            var data = Dt[j];
            PacketData = data.DataData;
            PacketData = PacketData.Replace(":", "");
            //Debug.Log(PacketData);
            byte[] myData = new byte[PacketData.Length / 2];

            for (int i = 0, h = 0; h < PacketData.Length; i++, h += 2)
            {
                myData[i] = (byte)Int32.Parse(PacketData.Substring(h, 2), System.Globalization.NumberStyles.HexNumber);
            }
            //Debug.Log(myData);
            myClient.Send(myData, myData.Length);
        }
        myClient.Close();
    }

    [Obsolete]
    public void RIGHT()
    {
        string jsontext = dataTransfer.Right;
        var Dt = JsonConvert.DeserializeObject<Root[]>(jsontext);
        Connect();
        // foreach (var data in Dt)
        for (int j = 0; j < Dt.Length; j++)
        {
            var data = Dt[j];
            PacketData = data.DataData;
            PacketData = PacketData.Replace(":", "");
            //Debug.Log(PacketData);
            byte[] myData = new byte[PacketData.Length / 2];

            for (int i = 0, h = 0; h < PacketData.Length; i++, h += 2)
            {
                myData[i] = (byte)Int32.Parse(PacketData.Substring(h, 2), System.Globalization.NumberStyles.HexNumber);
            }
            //Debug.Log(myData);
            myClient.Send(myData, myData.Length);
        }
        myClient.Close();
    }

    [Obsolete]
    public void UP()
    {
        string jsontext = dataTransfer.Up;
        var Dt = JsonConvert.DeserializeObject<Root[]>(jsontext);
        Connect();
        // foreach (var data in Dt)
        for (int j = 0; j < Dt.Length; j++)
        {
            var data = Dt[j];
            PacketData = data.DataData;
            PacketData = PacketData.Replace(":", "");
            //Debug.Log(PacketData);
            byte[] myData = new byte[PacketData.Length / 2];

            for (int i = 0, h = 0; h < PacketData.Length; i++, h += 2)
            {
                myData[i] = (byte)Int32.Parse(PacketData.Substring(h, 2), System.Globalization.NumberStyles.HexNumber);
            }
            //Debug.Log(myData);
            myClient.Send(myData, myData.Length);
        }
        myClient.Close();
    }

    [Obsolete]
    public void DOWN()
    {
        string jsontext = dataTransfer.Down;
        var Dt = JsonConvert.DeserializeObject<Root[]>(jsontext);
        Connect();
        // foreach (var data in Dt)
        for (int j = 0; j < Dt.Length; j++)
        {
            var data = Dt[j];
            PacketData = data.DataData;
            PacketData = PacketData.Replace(":", "");
            //Debug.Log(PacketData);
            byte[] myData = new byte[PacketData.Length / 2];

            for (int i = 0, h = 0; h < PacketData.Length; i++, h += 2)
            {
                myData[i] = (byte)Int32.Parse(PacketData.Substring(h, 2), System.Globalization.NumberStyles.HexNumber);
            }
            //Debug.Log(myData);
            myClient.Send(myData, myData.Length);
        }
        myClient.Close();
    }

    [Obsolete]
    public void HOME()
    {
        string jsontext = dataTransfer.Home;
        var Dt = JsonConvert.DeserializeObject<Root[]>(jsontext);
        Connect();
        for (int j = 0; j < Dt.Length; j++)
        {
            var data = Dt[j];
            PacketData = data.DataData;
            PacketData = PacketData.Replace(":", "");
            //Debug.Log(PacketData);
            byte[] myData = new byte[PacketData.Length / 2];

            for (int i = 0, h = 0; h < PacketData.Length; i++, h += 2)
            {
                myData[i] = (byte)Int32.Parse(PacketData.Substring(h, 2), System.Globalization.NumberStyles.HexNumber);
            }
            //Debug.Log(myData);
            myClient.Send(myData, myData.Length);
        }
        myClient.Close();
    }
    [HideInInspector]
    public int t = 0;

    [Obsolete]
    public void CON_LEFT()
    {
        string jsontext = dataTransfer.Con_Left;
        var Dt = JsonConvert.DeserializeObject<Root[]>(jsontext);
        Connect();
        for (int j = 0; j < Dt.Length; j++)
        {
            var data = Dt[j];
            PacketData = data.DataData;
            PacketData = PacketData.Replace(":", "");
            //Debug.Log(PacketData);
            byte[] myData = new byte[PacketData.Length / 2];

            for (int i = 0, h = 0; h < PacketData.Length; i++, h += 2)
            {
                myData[i] = (byte)Int32.Parse(PacketData.Substring(h, 2), System.Globalization.NumberStyles.HexNumber);
            }
            //Debug.Log(myData);
            myClient.Send(myData, myData.Length);
            t++;
            //Debug.Log(t);
            if (j == 2)
            {
                break;
            }
        }
        myClient.Close();
    }

    [Obsolete]
    public void CON_LEFT_STOP()
    {
        string jsontext = dataTransfer.Con_Left;
        var Dt = JsonConvert.DeserializeObject<Root[]>(jsontext);
        Connect();
        //Debug.Log(t);
        for (int j = t - 1; j < Dt.Length; j++)
        {
            var data = Dt[j];
            PacketData = data.DataData;
            PacketData = PacketData.Replace(":", "");
            //Debug.Log(PacketData);
            byte[] myData = new byte[PacketData.Length / 2];

            for (int i = 0, h = 0; h < PacketData.Length; i++, h += 2)
            {
                myData[i] = (byte)Int32.Parse(PacketData.Substring(h, 2), System.Globalization.NumberStyles.HexNumber);
            }
            //Debug.Log(myData);
            myClient.Send(myData, myData.Length);
        }
        t = 0;
        myClient.Close();
    }

    [Obsolete]
    public void CON_RIGHT()
    {
        string jsontext = dataTransfer.Con_Right;
        var Dt = JsonConvert.DeserializeObject<Root[]>(jsontext);
        Connect();
        for (int j = 0; j < Dt.Length; j++)
        {
            var data = Dt[j];
            PacketData = data.DataData;
            PacketData = PacketData.Replace(":", "");
            //Debug.Log(PacketData);
            byte[] myData = new byte[PacketData.Length / 2];

            for (int i = 0, h = 0; h < PacketData.Length; i++, h += 2)
            {
                myData[i] = (byte)Int32.Parse(PacketData.Substring(h, 2), System.Globalization.NumberStyles.HexNumber);
            }
            //Debug.Log(myData);
            myClient.Send(myData, myData.Length);
            t++;
            //Debug.Log(t);
            if (j == 2)
            {
                break;
            }
        }
        myClient.Close();
    }

    [Obsolete]
    public void CON_RIGHT_STOP()
    {
        string jsontext = dataTransfer.Con_Right;
        var Dt = JsonConvert.DeserializeObject<Root[]>(jsontext);
        Connect();
        //Debug.Log(t);
        for (int j = t - 1; j < Dt.Length; j++)
        {
            var data = Dt[j];
            PacketData = data.DataData;
            PacketData = PacketData.Replace(":", "");
            //Debug.Log(PacketData);
            byte[] myData = new byte[PacketData.Length / 2];

            for (int i = 0, h = 0; h < PacketData.Length; i++, h += 2)
            {
                myData[i] = (byte)Int32.Parse(PacketData.Substring(h, 2), System.Globalization.NumberStyles.HexNumber);
            }
            //Debug.Log(myData);
            myClient.Send(myData, myData.Length);
            
        }
        t = 0;
        myClient.Close();
    }

    [Obsolete]
    public void CON_UP()
    {
        string jsontext = dataTransfer.Con_Up;
        var Dt = JsonConvert.DeserializeObject<Root[]>(jsontext);
        Connect();
        for (int j = 0; j < Dt.Length; j++)
        {
            var data = Dt[j];
            PacketData = data.DataData;
            PacketData = PacketData.Replace(":", "");
            //Debug.Log(PacketData);
            byte[] myData = new byte[PacketData.Length / 2];

            for (int i = 0, h = 0; h < PacketData.Length; i++, h += 2)
            {
                myData[i] = (byte)Int32.Parse(PacketData.Substring(h, 2), System.Globalization.NumberStyles.HexNumber);
            }
            //Debug.Log(myData);
            myClient.Send(myData, myData.Length);
            t++;
            //Debug.Log(t);
            if (j == 2)
            {
                break;
            }
        }
        myClient.Close();
    }

    [Obsolete]
    public void CON_UP_STOP()
    {
        string jsontext = dataTransfer.Con_Up;
        var Dt = JsonConvert.DeserializeObject<Root[]>(jsontext);
        Connect();
        //Debug.Log(t);
        for (int j = t - 1; j < Dt.Length; j++)
        {
            var data = Dt[j];
            PacketData = data.DataData;
            PacketData = PacketData.Replace(":", "");
            //Debug.Log(PacketData);
            byte[] myData = new byte[PacketData.Length / 2];

            for (int i = 0, h = 0; h < PacketData.Length; i++, h += 2)
            {
                myData[i] = (byte)Int32.Parse(PacketData.Substring(h, 2), System.Globalization.NumberStyles.HexNumber);
            }
            //Debug.Log(myData);
            myClient.Send(myData, myData.Length);
        }
        t = 0;
        myClient.Close();
    }

    [Obsolete]
    public void CON_DOWN()
    {
        string jsontext = dataTransfer.Con_Down;
        var Dt = JsonConvert.DeserializeObject<Root[]>(jsontext);
        Connect();
        for (int j = 0; j < Dt.Length; j++)
        {
            var data = Dt[j];
            PacketData = data.DataData;
            PacketData = PacketData.Replace(":", "");
            //Debug.Log(PacketData);
            byte[] myData = new byte[PacketData.Length / 2];

            for (int i = 0, h = 0; h < PacketData.Length; i++, h += 2)
            {
                myData[i] = (byte)Int32.Parse(PacketData.Substring(h, 2), System.Globalization.NumberStyles.HexNumber);
            }
            //Debug.Log(myData);
            myClient.Send(myData, myData.Length);
            t++;
            //Debug.Log(t);
            if (j == 2)
            {
                break;
            }
        }
        myClient.Close();
    }

    [Obsolete]
    public void CON_DOWN_STOP()
    {
        string jsontext = dataTransfer.Con_Down;
        var Dt = JsonConvert.DeserializeObject<Root[]>(jsontext);
        Connect();
        //Debug.Log(t);
        for (int j = t - 1; j < Dt.Length; j++)
        {
            var data = Dt[j];
            PacketData = data.DataData;
            PacketData = PacketData.Replace(":", "");
            //Debug.Log(PacketData);
            byte[] myData = new byte[PacketData.Length / 2];

            for (int i = 0, h = 0; h < PacketData.Length; i++, h += 2)
            {
                myData[i] = (byte)Int32.Parse(PacketData.Substring(h, 2), System.Globalization.NumberStyles.HexNumber);
            }
            //Debug.Log(myData);
            myClient.Send(myData, myData.Length);
 
        }
        t = 0;
        myClient.Close();
    }

    [Obsolete]
    public void Stop_ALL()
    {
        string jsontext = dataTransfer.Emergency_Stop;
        var Dt = JsonConvert.DeserializeObject<Root[]>(jsontext);
        Connect();
        for (int j = 0; j < Dt.Length; j++)
        {
            var data = Dt[j];
            PacketData = data.DataData;
            PacketData = PacketData.Replace(":", "");
            //Debug.Log(PacketData);
            byte[] myData = new byte[PacketData.Length / 2];

            for (int i = 0, h = 0; h < PacketData.Length; i++, h += 2)
            {
                myData[i] = (byte)Int32.Parse(PacketData.Substring(h, 2), System.Globalization.NumberStyles.HexNumber);
            }
            //Debug.Log(myData);
            myClient.Send(myData, myData.Length);
        }
        myClient.Close();
    }
}