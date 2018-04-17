using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
namespace ConfigManage.Model
{
    public class SysCfgsettingModel : System.ComponentModel.INotifyPropertyChanged
    {
        private bool mesEnable = true;
        private bool mesOfflineMode = false;
        private bool printerEnable = true;
        private bool mesAutodown = true; //MES投产
        private int mesDownTimeout = 20;//MES下线查询最大延迟时间
        private int rfidTimeout = 15;//RFID最长允许延迟
        private bool zeroFireEnabled = true;
        private bool[] nodeCheckEnables = new bool[10];
        public  bool PreStationCheck { get; set; }
        public bool MesEnable {
            get{ return mesEnable; } set
            { mesEnable = value; 
                OnPropertyChanged("MesEnable");
            } 
        }
        public bool ZeroFileEnabled
        {
            get { return zeroFireEnabled; }
            set { zeroFireEnabled = value; }
        }
        public bool MesOfflineMode {
            get { return mesOfflineMode; }
            set { mesOfflineMode = value;
            OnPropertyChanged("MesOfflineMode");
            }
        }
        public bool MesAutoDown
        {
            get { return mesAutodown; }
            set
            {
                mesAutodown = value;
                OnPropertyChanged("MesAutoDown");
            }
        }
        public bool PrienterEnable { 
            get { return printerEnable; } 
            set {
                printerEnable = value;
                OnPropertyChanged("PrienterEnable"); 
            }
        }
       
        public int MesDownTimeout
        {
            get { return mesDownTimeout; }
            set
            {
                if(value <0)
                {
                    value = 0;
                }
                if(value>600)
                {
                    value = 600;
                }
                mesDownTimeout = value;
                OnPropertyChanged("MesDownTimeout");
            }
        }
        public int RfidTimeout
        {
            get { return rfidTimeout; }
            set
            {
                if(value<0)
                {
                    value = 0;
                }
                if(value>60)
                {
                    value = 60;
                }
                rfidTimeout = value;
                OnPropertyChanged("RfidTimeout");

            }
        }
        public bool[] NodeCheckEnables
        {
            get { return nodeCheckEnables; }
            set { nodeCheckEnables = value; OnPropertyChanged("NodeCheckEnables"); }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public SysCfgsettingModel()
        {
            PreStationCheck = true;
            MesEnable = true;
            PrienterEnable = true;
            for(int i=0;i<nodeCheckEnables.Count();i++)
            {
                nodeCheckEnables[i] = true;
            }
        }
        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
