using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Z001上位机
{
    public partial class Main : Form, SerialInterface
    {
        Serial mSerialCom = null;
        public Main()
        {
            InitializeComponent();
            mSerialCom = new Serial();
            mSerialCom.setSerialPort(this);
        }

        private void label2_Click(object sender, EventArgs e)
        {
            
        }
        int[] bauds = { 2400,4800,9600,19200,38400,57600,115200};
        Parity[] stopString = { Parity.None, Parity.Odd, Parity.Mark, Parity.Space };
        int[] datBit = { 5, 6, 7, 8, 9 };
        StopBits[] stopBit = { StopBits.None, StopBits.One, StopBits.OnePointFive, StopBits.Two };
        private void Main_Load(object sender, EventArgs e)
        {
            //获取电脑上所有的串口
            string []Coms= Serial.getSerials();
            foreach(string comStr in Coms)
            {
                comboBox1.Items.Add(comStr);
            }
            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
            }
            //设置信息
            foreach (int n_baud in bauds)
            {
                comboBox2.Items.Add(n_baud+"");
            }
            comboBox2.SelectedIndex = 2;
            foreach (Parity nParity in stopString)
            {
                comboBox5.Items.Add(nParity + "");
            }
            comboBox5.SelectedIndex = 0;
            foreach (int datbit in datBit)
            {
                comboBox3.Items.Add(datbit + "");
            }
            comboBox3.SelectedIndex = 3;
            foreach (StopBits stopbit in stopBit)
            {
                comboBox4.Items.Add(stopbit + "");
            }
            comboBox4.SelectedIndex = 1;
        }



        private void button1_Click(object sender, EventArgs e)
        {
            if (!mSerialCom.getIsOpen())
            {
                mSerialCom._serialPort.PortName = comboBox1.Text;
                mSerialCom._serialPort.BaudRate = int.Parse(comboBox2.Text);
                if (comboBox4.Text.Equals(StopBits.None + ""))
                {
                    mSerialCom._serialPort.StopBits = StopBits.None;
                }
                else if (comboBox4.Text.Equals(StopBits.One + ""))
                {
                    mSerialCom._serialPort.StopBits = StopBits.One;
                }
                else if (comboBox4.Text.Equals(StopBits.OnePointFive + ""))
                {
                    mSerialCom._serialPort.StopBits = StopBits.OnePointFive;
                }
                else if (comboBox4.Text.Equals(StopBits.Two + ""))
                {
                    mSerialCom._serialPort.StopBits = StopBits.Two;
                }

                mSerialCom._serialPort.DataBits = int.Parse(comboBox3.Text);
                if (comboBox3.Text.Equals(Parity.None + ""))
                {
                    mSerialCom._serialPort.Parity = Parity.None;
                }
                else if (comboBox3.Text.Equals(Parity.Odd + ""))
                {
                    mSerialCom._serialPort.Parity = Parity.Odd;
                }
                else if (comboBox3.Text.Equals(Parity.Even + ""))
                {
                    mSerialCom._serialPort.Parity = Parity.Even;
                }
                else if (comboBox3.Text.Equals(Parity.Mark + ""))
                {
                    mSerialCom._serialPort.Parity = Parity.Mark;
                }
                else if (comboBox3.Text.Equals(Parity.Space + ""))
                {
                    mSerialCom._serialPort.Parity = Parity.Space;
                }
                 try
                {
              
                    mSerialCom.openPort();
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                button1.Text = "关闭";
            }else
            {
                mSerialCom.closePort();
                button1.Text = "打开";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            mSerialCom.SendData("@"+textBox1.Text+"&\n");
        }

        void SerialInterface.OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (mSerialCom._serialPort.BytesToRead <= 0)
            {
                return;
            }
            byte[] buf = new byte[mSerialCom._serialPort.BytesToRead];
            int readcount = 0;
            while (mSerialCom._serialPort.BytesToRead > 0)
            {
                readcount = mSerialCom.read(buf, 0, buf.Length);
            }
            string str = System.Text.Encoding.ASCII.GetString(buf);

            //读取串口中一个字节的数据  
            this.textBox2.Invoke(
             //在拥有此控件的基础窗口句柄的线程上执行委托Invoke(Delegate)  
             //即在textBox_ReceiveDate控件的父窗口form中执行委托.  
             new MethodInvoker(
                 /*表示一个委托，该委托可执行托管代码中声明为 void 且不接受任何参数的任何方法。 在对控件的 Invoke    方法进行调用时或需要一个简单委托又不想自己定义时可以使用该委托。*/
                 delegate {
                     textBox2.AppendText(str);
                 })
                 );     
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox2.Text = "";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            mSerialCom.SendData("@setopen 0 1 2&\n");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            mSerialCom.SendData("@setclose 0 1 2&\n");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //年周月日时分秒
            // MessageBox.Show("@settime" + DateTime.Now.ToString(" yy " + Convert.ToInt32(DateTime.Now.DayOfWeek) + " MM dd HH mm ss") + "&\n");
            mSerialCom.SendData("@settime" + DateTime.Now.ToString(" yy "+ Convert.ToInt32(DateTime.Now.DayOfWeek) + " MM dd HH mm ss") + "&\n");
           // mSerialCom.SendData("@settime" + DateTime.Now.ToString(" 16 0 11 07 13 11 00") + "&\n");

        }

        private void button7_Click(object sender, EventArgs e)
        {
            mSerialCom.SendData("@exitcon&\n");
        }

        private void button8_Click(object sender, EventArgs e)
        {
            mSerialCom.SendData("@save_cmd&\n");
        }
    }
}
