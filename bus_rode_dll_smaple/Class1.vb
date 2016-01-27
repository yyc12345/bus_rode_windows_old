Public Class main_dll

    '构造函数，对于一些属性进行必要的设置
    Public Sub New()

        DllDependBusRodeVersion = 8000
        DllWriter = "Smaple Name"
        DllRegoin = "China-Unknow-Unknow"
        DllVersion = "1.0.0.0"
        DllGetTick = 5000

    End Sub

    'dll所依赖的bus_rode的版本号，只有等于调用的bus_rode的版本号，该dll才会被加载
    Public DllDependBusRodeVersion As Integer
    'dll的作者
    Public DllWriter As String
    'dll所属地区，格式：国家-省/州-县/城市
    Public DllRegoin As String
    'dll的版本号
    Public DllVersion As String
    'dll每次获取实时资源所需要间隔的时间，用毫秒做单位，建议最少为5000(5秒)
    Public DllGetTick As Integer

    'dll在实际运行时所需要获取的线路数据的线路名
    Public DllUseBusLineName As String


    'dll中获取车辆实时位置的的主获取函数，所有获取操作需要写在这里
    Public Function GetResources() As String


        '根据你的需要使用一个String变量来替换 "" 进行返回
        Return ""

    End Function

End Class
