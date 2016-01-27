using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bus_rode_dll
{
    public class main_dll
    {

        //构造函数，对于一些属性进行必要的设置
        public main_dll()
        {

            DllDependBusRodeVersion = 8000;
            DllWriter = "Smaple Name";
            DllRegoin = "China-Unknow-Unknow";
            DllVersion = "1.0.0.0";
            DllGetTick = 5000;

        }

        //dll所依赖的bus_rode的版本号，只有等于调用的bus_rode的版本号，该dll才会被加载
        public int DllDependBusRodeVersion;
        //dll的作者
        public string DllWriter;
        //dll所属地区，格式：国家-省/州-县/城市
        public string DllRegoin;
        //dll的版本号
        public string DllVersion;
        //dll每次获取实时资源所需要间隔的时间，用毫秒做单位，建议最少为5000(5秒)
        public int DllGetTick;

        //dll在实际运行时所需要获取的线路数据的线路名
        public string DllUseBusLineName;


        //dll中获取车辆实时位置的的主获取函数，所有获取操作需要写在这里
        public string GetResources()
        {


            //根据你的需要使用一个string变量来替换 "" 进行返回
            return "";

        }


    }
}
