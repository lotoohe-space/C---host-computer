using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Z001上位机
{
    interface SerialInterface
    {
         void OnDataReceived(object sender, SerialDataReceivedEventArgs e);
    }
}
