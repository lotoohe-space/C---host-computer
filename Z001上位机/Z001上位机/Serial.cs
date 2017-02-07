using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
namespace Z001上位机
{
    class Serial
    {
        public SerialPort _serialPort = null;
        public bool ReceiveEventFlag = false;  //接收事件是否有效 false表示有效

        public Serial()
        {
            _serialPort = new SerialPort("COM1", 9600, Parity.None, 8, StopBits.One);
        }
        public Serial(string comPortName, int baudRate, Parity parity, int dataBits, StopBits stopBits)
        {
            _serialPort = new SerialPort(comPortName, baudRate, parity, dataBits, stopBits);
            
        }
        public void setSerialPort(SerialInterface mSerialInterface)
        {
            if (_serialPort != null)
            {
                //设置触发DataReceived事件的字节数为1
                _serialPort.ReceivedBytesThreshold = 1;
                //接收到一个字节时，也会触发DataReceived事件
                _serialPort.DataReceived += new SerialDataReceivedEventHandler(mSerialInterface.OnDataReceived);
                //接收数据出错,触发事件
                _serialPort.ErrorReceived += null;
            }
        }
        /// <summary>
        /// 打开串口资源
        /// </summary>
        public bool openPort()
        {
            bool ok = false;
            //如果串口是打开的，先关闭
            if (_serialPort.IsOpen)
                _serialPort.Close();
            try
            {
                //打开串口
                _serialPort.Open();
                ok = true;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
            return ok;
        }
        /// <summary>
        /// 关闭串口资源,操作完成后,一定要关闭串口
        /// </summary>
        public void closePort()
        {
            //如果串口处于打开状态,则关闭
            if (_serialPort.IsOpen)
                _serialPort.Close();
        }

        /// <summary>
        /// 获取端口连接状态
        /// </summary>
        /// <returns></returns>
        public bool getIsOpen()
        {
            //如果串口是打开的，true,否则为false;
            bool flag = false;
            try { flag = _serialPort.IsOpen; }
            catch { }
            return flag;

        }
        /// <summary>
        /// 串口数据发送
        /// </summary>
        /// <param name="data"></param>
        public void SendData(string data)
        {
            if (_serialPort.IsOpen)
            {
                byte[] buf = System.Text.Encoding.GetEncoding("gb2312").GetBytes(data);
                _serialPort.Write(buf, 0, buf.Length);
            }
        }
        public void SendData(byte[] data, int offset, int count)
        {
            if (_serialPort.IsOpen)
            {
                _serialPort.Write(data, offset, count);
            }
        }

        /// <summary>
        /// 发送命令
        /// </summary>
        /// <param name="SendData"></param>
        /// <param name="ReceiveData"></param>
        /// <param name="Overtime"></param>
        /// <returns></returns>
        public int SendCommand(byte[] SendData, ref byte[] ReceiveData, int Overtime)
        {

            if (_serialPort.IsOpen)
            {
                ReceiveEventFlag = true;        //关闭接收事件
                _serialPort.DiscardInBuffer();         //清空接收缓冲区                 
                _serialPort.Write(SendData, 0, SendData.Length);
                int num = 0, ret = 0;
                while (num++ < Overtime)
                {
                    if (_serialPort.BytesToRead >= ReceiveData.Length) break;
                    System.Threading.Thread.Sleep(1);
                }
                if (_serialPort.BytesToRead >= ReceiveData.Length)
                    ret = _serialPort.Read(ReceiveData, 0, ReceiveData.Length);
                ReceiveEventFlag = false;       //打开事件
                return ret;
            }
            return -1;
        }
        //读取数据
        public int read(byte []buf,int offset,int count)
        {
            return _serialPort.Read(buf,offset,count);
        }
        /// <summary>
        /// 获取所有已连接短信猫设备的串口
        /// </summary>
        /// <returns></returns>
        public string[] serialsIsConnected()
        {
            List<string> lists = new List<string>();
            string[] seriallist = getSerials();
            return lists.ToArray();
        }
        /// <summary>
        /// 获得当前电脑上的所有串口资源
        /// </summary>
        /// <returns></returns>
        public static string[] getSerials()
        {
            return SerialPort.GetPortNames();
        }

    }
}
