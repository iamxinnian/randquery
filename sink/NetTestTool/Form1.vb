Imports System
Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Data.SqlClient
Imports System.Threading
Imports ZqjDHForGraduationP
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.Security.Cryptography
Imports leftzeronecode
Imports System.IO

Public Class NetTestTool
    Dim mysocket As Socket = Nothing
    Dim mythread As Thread
    Dim mythread_Semaphore As New Threading.Semaphore(10, 10)
    Delegate Function Tothread(ByVal title As String, ByVal content As String) As Integer '声明委托
    Dim DicS As Dictionary(Of String, Socket) = New Dictionary(Of String, Socket) '创建Socket字典
    Dim DicC As Dictionary(Of Integer, TimeCheck) = New Dictionary(Of Integer, TimeCheck) '创建时间标记字典
    Dim DicT As Dictionary(Of String, Thread) = New Dictionary(Of String, Thread) '创建Thread字典
    Dim DicDH As Dictionary(Of String, DHTool) = New Dictionary(Of String, DHTool) '创建DH字典
    Public sqlconn As SqlConnection = New SqlConnection _
    ("Data Source=(local);Initial Catalog=graduation;Integrated Security=False;User ID=sa;Password=123456;") '连接数据库
    Dim recv_finish As Integer = 0
    Dim recv_buf_all As String = ""
    Dim dimension_data_nums As String = ""
    Dim collect_period_int As Integer = 0
    Dim data_num_str As String = ""
    Dim lux_min_str As String = ""
    Dim lux_max_str As String = ""
    Dim temp_min_str As String = ""
    Dim temp_max_str As String = ""
    Dim press_min_str As String = ""
    Dim press_max_str As String = ""
    Dim humi_min_str As String = ""
    Dim humi_max_str As String = ""
    Dim recv_richtext_length As Int32 = 0
    Dim recv_sock_num As UInt64 = 0
    Structure thread_buff
        Dim aeskey As String
        Dim json_ret As Object
        Dim sk As Socket
    End Structure
    Private Sub CearSendZone_Click(sender As Object, e As EventArgs) Handles CearSendZone.Click
        '清空输入
        startime_text.Text = ""
        endtime_text.Text = ""
        lux_min_text.Text = ""
        device_id.Text = ""
        collect_period.Text = ""
        lux_max_text.Text = ""
        tmp_min_text.Text = ""
        tmp_max_text.Text = ""
        press_min_text.Text = ""
        press_max_text.Text = ""
        humi_min_text.Text = ""
        humi_max_text.Text = ""
    End Sub

    Private Sub ClearText_Click(sender As Object, e As EventArgs) Handles ClearText.Click
        RT_RevText.Text = ""

    End Sub

    Public Sub OpenSocket()
        Dim ShowTitle As String = ""
        Dim ShowDate As String = ""
        While True
            Try
                Dim ss As Socket = mysocket.Accept()
                ss.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 1)
                Dim RevThread = New Thread(AddressOf ReciveData)
                RevThread.Start(ss)
                ShowTitle = "*****" + Format(DateTime.Now, "yyyy-MM-dd HH:mm:ss") + "--linked--" + ss.RemoteEndPoint.ToString + "*****" + vbCrLf
                ShowDate = ""
                Invok_Recv(ShowTitle, ShowDate) '委托调用
                Invok_Add_Client("add", ss.RemoteEndPoint.ToString)
                DicS.Add(ss.RemoteEndPoint.ToString, ss) '把连接到的客户端IP和端口存到键值里，加入socket字典
                DicT.Add(ss.RemoteEndPoint.ToString, RevThread) '把该客户端IP和端口存到键值里，加入Thread字典
            Catch ex As Exception
            End Try
        End While
    End Sub
    'Public Sub query_ret_deal(ByVal aeskey As String, ByVal json_ret As Object, ByVal s As Socket, ByVal Did As Integer)
    Public Sub query_ret_deal(ByVal thread_param As Object)
        Dim ShowTitle As String = ""
        Dim ShowDate As String = ""
        Dim restructure_index As String = ""
        Dim lux_data As String()
        Dim temp_data As String()
        Dim press_data As String()
        Dim humi_data As String()
        Try
            thread_param.json_ret.Item("devid").ToString()
            thread_param.json_ret.Item("time").ToString()
            thread_param.json_ret.Item("result").ToString()
            thread_param.json_ret.Item("data_index").ToString()
        Catch ex As Exception
            ShowTitle = "*****" + Format(DateTime.Now, "yyyy-MM-dd HH:mm:ss") + "<--" + thread_param.sk.RemoteEndPoint.ToString + " resualt of query erro infor*****" + vbCrLf
            ShowDate = "存在数据帧攻击！(-2)"
            Invok_Recv(ShowTitle, ShowDate) '委托调用
            Exit Sub
        End Try

        If thread_param.json_ret.Item("result").ToString() = "in limits" Then

            Try
                thread_param.json_ret.Item("lux_data").ToString()
                thread_param.json_ret.Item("temp_data").ToString()
                thread_param.json_ret.Item("press_data").ToString()
                thread_param.json_ret.Item("humi_data").ToString()
            Catch ex As Exception
                ShowTitle = "*****" + Format(DateTime.Now, "yyyy-MM-dd HH:mm:ss") + "<--" + thread_param.sk.RemoteEndPoint.ToString + " resualt of query erro infor*****" + vbCrLf
                ShowDate = "存在数据帧攻击！(-2)"
                Invok_Recv(ShowTitle, ShowDate) '委托调用
                Exit Sub
            End Try

            restructure_index = thread_param.json_ret.Item("devid").ToString + ","
            restructure_index = restructure_index + thread_param.json_ret.Item("time").ToString + ","
            '构建时间变量用于推算数据采集时间
            Dim dt As Date
            Try
                dt = Date.Parse(thread_param.json_ret.Item("time").ToString)
            Catch ex As Exception
                MessageBox.Show(thread_param.json_ret.Item("time").ToString)
            End Try


            Dim decripter_tool As New aes_decripter_strn
            decripter_tool.setkey(thread_param.aeskey)
            Dim colect_n As Integer = 0

            If thread_param.json_ret.Item("lux_data").ToString <> "NULL" Then
                Dim ret As String
                ret = decripter_tool.do_decripter(thread_param.json_ret.Item("lux_data").ToString)
                If ret <> "0" Then
                    ShowTitle = "*****" + Format(DateTime.Now, "yyyy-MM-dd HH:mm:ss") + "<--" + thread_param.sk.RemoteEndPoint.ToString + " resualt of query erro infor*****" + vbCrLf
                    ShowDate = "存在光照数据伪造攻击！"
                    Invok_Recv(ShowTitle, ShowDate) '委托调用
                    Exit Sub
                End If
                decripter_tool.split_tonstr()
                lux_data = decripter_tool.nstr
                colect_n = lux_data.Length
                Try
                    restructure_index = restructure_index + CInt(decripter_tool.str_min_max(0)).ToString + "," _
                    + CInt(decripter_tool.str_min_max(1)).ToString
                Catch ex As Exception
                    MessageBox.Show(decripter_tool.str_min_max(0) + decripter_tool.str_min_max(1))
                End Try
            End If

            If thread_param.json_ret.Item("temp_data").ToString <> "NULL" Then
                Dim ret As String
                ret = decripter_tool.do_decripter(thread_param.json_ret.Item("temp_data").ToString)
                If ret <> "0" Then
                    ShowTitle = "*****" + Format(DateTime.Now, "yyyy-MM-dd HH:mm:ss") + "<--" + thread_param.sk.RemoteEndPoint.ToString + " resualt of query erro infor*****" + vbCrLf
                    ShowDate = "存在温度数据伪造攻击！"
                    Invok_Recv(ShowTitle, ShowDate) '委托调用
                    Exit Sub
                End If
                decripter_tool.split_tonstr()
                temp_data = decripter_tool.nstr
                restructure_index = restructure_index + "," + CInt(decripter_tool.str_min_max(0)).ToString + "," _
                    + CInt(decripter_tool.str_min_max(1)).ToString
            End If

            If thread_param.json_ret.Item("press_data").ToString <> "NULL" Then
                Dim ret As String
                ret = decripter_tool.do_decripter(thread_param.json_ret.Item("press_data").ToString)
                If ret <> "0" Then
                    ShowTitle = "*****" + Format(DateTime.Now, "yyyy-MM-dd HH:mm:ss") + "<--" + thread_param.sk.RemoteEndPoint.ToString + " resualt of query erro infor*****" + vbCrLf
                    ShowDate = "存在大气压数据伪造攻击！"
                    Invok_Recv(ShowTitle, ShowDate) '委托调用
                    Exit Sub
                End If
                decripter_tool.split_tonstr()
                press_data = decripter_tool.nstr
                restructure_index = restructure_index + "," + CInt(decripter_tool.str_min_max(0)).ToString + "," _
                    + CInt(decripter_tool.str_min_max(1)).ToString
            End If

            If thread_param.json_ret.Item("humi_data").ToString <> "NULL" Then
                Dim ret As String
                ret = decripter_tool.do_decripter(thread_param.json_ret.Item("humi_data").ToString)
                If ret <> "0" Then
                    ShowTitle = "*****" + Format(DateTime.Now, "yyyy-MM-dd HH:mm:ss") + "<--" + thread_param.sk.RemoteEndPoint.ToString + " resualt of query erro infor*****" + vbCrLf
                    ShowDate = "存在湿度数据伪造攻击！"
                    Invok_Recv(ShowTitle, ShowDate) '委托调用
                    Exit Sub
                End If
                decripter_tool.split_tonstr()
                humi_data = decripter_tool.nstr
                restructure_index = restructure_index + "," + CInt(decripter_tool.str_min_max(0)).ToString + "," _
                    + CInt(decripter_tool.str_min_max(1)).ToString
            End If
            restructure_index = restructure_index + "," + colect_n.ToString
            Dim encode As String = decripter_tool.do_encripter(restructure_index)

            If Trim(encode) = Trim(thread_param.json_ret.Item("data_index").ToString) Then
                Dim flag As Byte = 0
                ShowTitle = "*****" + Format(DateTime.Now, "yyyy-MM-dd HH:mm:ss") + "<--" + thread_param.sk.RemoteEndPoint.ToString + " resualt of query *****" + vbCrLf
                For i = 0 To colect_n - 1 Step 1
                    If dimension_data_nums >= "1" Then
                        If (CInt(lux_data(i)) > CInt(lux_max_str)) Or (CInt(lux_data(i)) < CInt(lux_min_str)) Then
                            Continue For
                        End If
                    End If

                    If dimension_data_nums >= "2" Then
                        If CInt(data_num_str) = 8 Then
                            If (CInt(temp_data(i)) > CInt(temp_max_str)) Or (CInt(temp_data(i)) < CInt(temp_min_str)) Then
                                Continue For
                            End If
                        Else
                            If (CInt(temp_data(i)) > CInt(CDbl(temp_max_str) * 10)) Or (CInt(temp_data(i)) < CInt(CDbl(temp_min_str) * 10)) Then
                                Continue For
                            End If
                        End If
                    End If

                    If dimension_data_nums >= "3" Then
                        If CInt(data_num_str) = 8 Then
                            If (CInt(press_data(i)) > CInt(press_max_str)) Or (CInt(press_data(i)) < CInt(press_min_str)) Then
                                Continue For
                            End If
                        ElseIf CInt(data_num_str) = 16 Then
                            If (CInt(press_data(i)) > CInt(CDbl(press_max_str) * 100)) Or (CInt(press_data(i)) < CInt(CDbl(press_min_str) * 100)) Then
                                Continue For
                            End If
                        Else
                            If (CInt(press_data(i)) > CInt(CDbl(press_max_str) * 1000)) Or (CInt(press_data(i)) < CInt(CDbl(press_min_str) * 1000)) Then
                                Continue For
                            End If
                        End If
                    End If

                    If dimension_data_nums >= "4" Then
                        If CInt(data_num_str) = 8 Then
                            If (CInt(humi_data(i)) > CInt(humi_max_str)) Or (CInt(humi_data(i)) < CInt(humi_min_str)) Then
                                Continue For
                            End If
                        Else
                            If (CInt(humi_data(i)) > CInt(CDbl(humi_max_str) * 10)) Or (CInt(humi_data(i)) < CInt(CDbl(humi_min_str) * 10)) Then
                                Continue For
                            End If
                        End If
                    End If
                    flag = 1
                    If dimension_data_nums >= "4" Then
                        ShowDate = ShowDate + "devid:" + thread_param.json_ret.Item("devid").ToString + "  time:" + Format(dt.AddMilliseconds(-(collect_period_int * (colect_n - 1 - i))), _
                        "yyyy-MM-dd HH:mm:ss") + "  lux:" + CInt(lux_data(i)).ToString + "  temp:" + CInt(temp_data(i)).ToString + "  press:" + _
                        CInt(press_data(i)).ToString + "   humi:" + CInt(humi_data(i)).ToString + vbCrLf
                    ElseIf dimension_data_nums >= "3" Then
                        ShowDate = ShowDate + "devid:" + thread_param.json_ret.Item("devid").ToString + "  time:" + Format(dt.AddMilliseconds(-(collect_period_int * (colect_n - 1 - i))), _
                        "yyyy-MM-dd HH:mm:ss") + "  lux:" + CInt(lux_data(i)).ToString + "  temp:" + CInt(temp_data(i)).ToString + "  press:" + CInt(press_data(i)).ToString + vbCrLf
                    ElseIf dimension_data_nums >= "2" Then
                        ShowDate = ShowDate + "devid:" + thread_param.json_ret.Item("devid").ToString + "  time:" + Format(dt.AddMilliseconds(-(collect_period_int * (colect_n - 1 - i))), _
                        "yyyy-MM-dd HH:mm:ss") + "  lux:" + CInt(lux_data(i)).ToString + "  temp:" + CInt(temp_data(i)).ToString + vbCrLf
                    ElseIf dimension_data_nums >= "1" Then
                        ShowDate = ShowDate + "devid:" + thread_param.json_ret.Item("devid").ToString + "  time:" + Format(dt.AddMilliseconds(-(collect_period_int * (colect_n - 1 - i))), _
                        "yyyy-MM-dd HH:mm:ss") + "  lux:" + CInt(lux_data(i)).ToString + vbCrLf
                    End If

                Next

                Invok_Recv(ShowTitle, ShowDate) '委托调用

                If flag = 0 Then
                    ShowTitle = "*****" + Format(DateTime.Now, "yyyy-MM-dd HH:mm:ss") + "<--" + thread_param.sk.RemoteEndPoint.ToString + " resualt of query erro infor*****" + vbCrLf
                    ShowDate = "存在比较因子伪造攻击！"
                    Invok_Recv(ShowTitle, ShowDate) '委托调用
                End If
                'MessageBox.Show("ok!", "结果")
            Else
                ShowTitle = "*****" + Format(DateTime.Now, "yyyy-MM-dd HH:mm:ss") + "<--" + thread_param.sk.RemoteEndPoint.ToString + " resualt of query erro infor*****" + vbCrLf
                ShowDate = "存在索引数据伪造攻击！"
                Invok_Recv(ShowTitle, ShowDate) '委托调用
            End If
        ElseIf thread_param.json_ret.Item("result").ToString() = "not in limits" Then
            Dim decripter_tool As New aes_decripter_strn
            Dim d_index As String
            Dim nstr As String()
            decripter_tool.setkey(thread_param.aeskey)
            decripter_tool.do_decripter(thread_param.json_ret.Item("data_index").ToString())
            d_index = decripter_tool.clearcode
            nstr = d_index.Split(",")

            If CInt(data_num_str) = 8 Then

                If dimension_data_nums = "1" Then
                    If (CInt(nstr(2)) >= CInt(lux_min_str) And CInt(nstr(2)) <= CInt(lux_max_str)) And _
                    (CInt(nstr(3)) >= CInt(lux_min_str) And CInt(nstr(3)) <= CInt(lux_max_str)) Then
                        ShowTitle = "*****" + Format(DateTime.Now, "yyyy-MM-dd HH:mm:ss") + "<--" + thread_param.sk.RemoteEndPoint.ToString + " resualt of query erro infor*****" + vbCrLf
                        ShowDate = "存在比较因子伪造攻击！(-1)"
                        Invok_Recv(ShowTitle, ShowDate) '委托调用
                    End If
                End If

                If dimension_data_nums = "2" Then
                    If (CInt(nstr(2)) >= CInt(lux_min_str) And CInt(nstr(2)) <= CInt(lux_max_str)) And _
                    (CInt(nstr(3)) >= CInt(lux_min_str) And CInt(nstr(3)) <= CInt(lux_max_str)) And _
                    (CInt(nstr(4)) >= CInt(temp_min_str) And CInt(nstr(4)) <= CInt(temp_max_str)) And _
                    (CInt(nstr(5)) >= CInt(temp_min_str) And CInt(nstr(5)) <= CInt(temp_max_str)) Then
                        ShowTitle = "*****" + Format(DateTime.Now, "yyyy-MM-dd HH:mm:ss") + "<--" + thread_param.sk.RemoteEndPoint.ToString + " resualt of query erro infor*****" + vbCrLf
                        ShowDate = "存在比较因子伪造攻击！(-1)"
                        Invok_Recv(ShowTitle, ShowDate) '委托调用
                    End If
                End If

                If dimension_data_nums = "3" Then
                    If (CInt(nstr(2)) >= CInt(lux_min_str) And CInt(nstr(2)) <= CInt(lux_max_str)) And _
                    (CInt(nstr(3)) >= CInt(lux_min_str) And CInt(nstr(3)) <= CInt(lux_max_str)) And _
                    (CInt(nstr(4)) >= CInt(temp_min_str) And CInt(nstr(4)) <= CInt(temp_max_str)) And _
                    (CInt(nstr(5)) >= CInt(temp_min_str) And CInt(nstr(5)) <= CInt(temp_max_str)) And _
                    (CInt(nstr(6)) >= CInt(press_min_str) And CInt(nstr(6)) <= CInt(press_max_str)) And _
                    (CInt(nstr(7)) >= CInt(press_min_str) And CInt(nstr(7)) <= CInt(press_max_str)) Then
                        ShowTitle = "*****" + Format(DateTime.Now, "yyyy-MM-dd HH:mm:ss") + "<--" + thread_param.sk.RemoteEndPoint.ToString + " resualt of query erro infor*****" + vbCrLf
                        ShowDate = "存在比较因子伪造攻击！(-1)"
                        Invok_Recv(ShowTitle, ShowDate) '委托调用
                    End If
                End If

                If dimension_data_nums = "4" Then
                    If (CInt(nstr(2)) >= CInt(lux_min_str) And CInt(nstr(2)) <= CInt(lux_max_str)) And _
                    (CInt(nstr(3)) >= CInt(lux_min_str) And CInt(nstr(3)) <= CInt(lux_max_str)) And _
                    (CInt(nstr(4)) >= CInt(temp_min_str) And CInt(nstr(4)) <= CInt(temp_max_str)) And _
                    (CInt(nstr(5)) >= CInt(temp_min_str) And CInt(nstr(5)) <= CInt(temp_max_str)) And _
                    (CInt(nstr(6)) >= CInt(press_min_str) And CInt(nstr(6)) <= CInt(press_max_str)) And _
                    (CInt(nstr(7)) >= CInt(press_min_str) And CInt(nstr(7)) <= CInt(press_max_str)) And _
                    (CInt(nstr(8)) >= CInt(humi_min_str) And CInt(nstr(8)) <= CInt(humi_max_str)) And _
                    (CInt(nstr(9)) >= CInt(humi_min_str) And CInt(nstr(9)) <= CInt(humi_max_str)) Then
                        ShowTitle = "*****" + Format(DateTime.Now, "yyyy-MM-dd HH:mm:ss") + "<--" + thread_param.sk.RemoteEndPoint.ToString + " resualt of query erro infor*****" + vbCrLf
                        ShowDate = "存在比较因子伪造攻击！(-1)"
                        Invok_Recv(ShowTitle, ShowDate) '委托调用
                    End If
                End If

            ElseIf CInt(data_num_str) = 16 Then

                If dimension_data_nums = "1" Then
                    If (CInt(nstr(2)) >= CInt(lux_min_str) And CInt(nstr(2)) <= CInt(lux_max_str)) And _
                        (CInt(nstr(3)) >= CInt(lux_min_str) And CInt(nstr(3)) <= CInt(lux_max_str)) Then
                        ShowTitle = "*****" + Format(DateTime.Now, "yyyy-MM-dd HH:mm:ss") + "<--" + thread_param.sk.RemoteEndPoint.ToString + " resualt of query erro infor*****" + vbCrLf
                        'ShowDate = CInt(CDbl(temp_min_str) * 10).ToString + "  " + CInt(nstr(5)).ToString
                        ShowDate = "存在比较因子伪造攻击！(-2)"
                        Invok_Recv(ShowTitle, ShowDate) '委托调用
                    End If
                End If

                If dimension_data_nums = "2" Then
                    If (CInt(nstr(2)) >= CInt(lux_min_str) And CInt(nstr(2)) <= CInt(lux_max_str)) And _
                        (CInt(nstr(3)) >= CInt(lux_min_str) And CInt(nstr(3)) <= CInt(lux_max_str)) And _
                        (CInt(nstr(4)) >= CInt(CDbl(temp_min_str) * 10) And CInt(nstr(4)) <= CInt(CDbl(temp_max_str) * 10)) And _
                        (CInt(nstr(5)) >= CInt(CDbl(temp_min_str) * 10) And CInt(nstr(5)) <= CInt(CDbl(temp_max_str) * 10)) Then
                        ShowTitle = "*****" + Format(DateTime.Now, "yyyy-MM-dd HH:mm:ss") + "<--" + thread_param.sk.RemoteEndPoint.ToString + " resualt of query erro infor*****" + vbCrLf
                        'ShowDate = CInt(CDbl(temp_min_str) * 10).ToString + "  " + CInt(nstr(5)).ToString
                        ShowDate = "存在比较因子伪造攻击！(-2)"
                        Invok_Recv(ShowTitle, ShowDate) '委托调用
                    End If
                End If

                If dimension_data_nums = "3" Then
                    If (CInt(nstr(2)) >= CInt(lux_min_str) And CInt(nstr(2)) <= CInt(lux_max_str)) And _
                        (CInt(nstr(3)) >= CInt(lux_min_str) And CInt(nstr(3)) <= CInt(lux_max_str)) And _
                        (CInt(nstr(4)) >= CInt(CDbl(temp_min_str) * 10) And CInt(nstr(4)) <= CInt(CDbl(temp_max_str) * 10)) And _
                        (CInt(nstr(5)) >= CInt(CDbl(temp_min_str) * 10) And CInt(nstr(5)) <= CInt(CDbl(temp_max_str) * 10)) And _
                        (CInt(nstr(6)) >= CInt(CDbl(press_min_str) * 100) And CInt(nstr(6)) <= CInt(CDbl(press_max_str) * 100)) And _
                        (CInt(nstr(7)) >= CInt(CDbl(press_min_str) * 100) And CInt(nstr(7)) <= CInt(CDbl(press_max_str) * 100)) Then
                        ShowTitle = "*****" + Format(DateTime.Now, "yyyy-MM-dd HH:mm:ss") + "<--" + thread_param.sk.RemoteEndPoint.ToString + " resualt of query erro infor*****" + vbCrLf
                        'ShowDate = CInt(CDbl(temp_min_str) * 10).ToString + "  " + CInt(nstr(5)).ToString
                        ShowDate = "存在比较因子伪造攻击！(-2)"
                        Invok_Recv(ShowTitle, ShowDate) '委托调用
                    End If
                End If

                If dimension_data_nums = "4" Then
                    If (CInt(nstr(2)) >= CInt(lux_min_str) And CInt(nstr(2)) <= CInt(lux_max_str)) And _
                        (CInt(nstr(3)) >= CInt(lux_min_str) And CInt(nstr(3)) <= CInt(lux_max_str)) And _
                        (CInt(nstr(4)) >= CInt(CDbl(temp_min_str) * 10) And CInt(nstr(4)) <= CInt(CDbl(temp_max_str) * 10)) And _
                        (CInt(nstr(5)) >= CInt(CDbl(temp_min_str) * 10) And CInt(nstr(5)) <= CInt(CDbl(temp_max_str) * 10)) And _
                        (CInt(nstr(6)) >= CInt(CDbl(press_min_str) * 100) And CInt(nstr(6)) <= CInt(CDbl(press_max_str) * 100)) And _
                        (CInt(nstr(7)) >= CInt(CDbl(press_min_str) * 100) And CInt(nstr(7)) <= CInt(CDbl(press_max_str) * 100)) And _
                        (CInt(nstr(8)) >= CInt(CDbl(humi_min_str) * 10) And CInt(nstr(8)) <= CInt(CDbl(humi_max_str) * 10)) And _
                        (CInt(nstr(9)) >= CInt(CDbl(humi_min_str) * 10) And CInt(nstr(9)) <= CInt(CDbl(humi_max_str) * 10)) Then
                        ShowTitle = "*****" + Format(DateTime.Now, "yyyy-MM-dd HH:mm:ss") + "<--" + thread_param.sk.RemoteEndPoint.ToString + " resualt of query erro infor*****" + vbCrLf
                        'ShowDate = CInt(CDbl(temp_min_str) * 10).ToString + "  " + CInt(nstr(5)).ToString
                        ShowDate = "存在比较因子伪造攻击！(-2)"
                        Invok_Recv(ShowTitle, ShowDate) '委托调用
                    End If
                End If

            ElseIf CInt(data_num_str) >= 32 Then
                If dimension_data_nums = "1" Then
                    If (CInt(nstr(2)) >= CInt(lux_min_str) And CInt(nstr(2)) <= CInt(lux_max_str)) And _
                        (CInt(nstr(3)) >= CInt(lux_min_str) And CInt(nstr(3)) <= CInt(lux_max_str)) Then
                        ShowTitle = "*****" + Format(DateTime.Now, "yyyy-MM-dd HH:mm:ss") + "<--" + thread_param.sk.RemoteEndPoint.ToString + " resualt of query erro infor*****" + vbCrLf
                        ShowDate = "存在比较因子伪造攻击！(-3)"
                        Invok_Recv(ShowTitle, ShowDate) '委托调用
                    End If
                End If

                If dimension_data_nums = "2" Then
                    If (CInt(nstr(2)) >= CInt(lux_min_str) And CInt(nstr(2)) <= CInt(lux_max_str)) And _
                        (CInt(nstr(3)) >= CInt(lux_min_str) And CInt(nstr(3)) <= CInt(lux_max_str)) And _
                        (CInt(nstr(4)) >= CInt(CDbl(temp_min_str) * 10) And CInt(nstr(4)) <= CInt(CDbl(temp_max_str) * 10)) And _
                        (CInt(nstr(5)) >= CInt(CDbl(temp_min_str) * 10) And CInt(nstr(5)) <= CInt(CDbl(temp_max_str) * 10)) Then
                        ShowTitle = "*****" + Format(DateTime.Now, "yyyy-MM-dd HH:mm:ss") + "<--" + thread_param.sk.RemoteEndPoint.ToString + " resualt of query erro infor*****" + vbCrLf
                        ShowDate = "存在比较因子伪造攻击！(-3)"
                        Invok_Recv(ShowTitle, ShowDate) '委托调用
                    End If
                End If

                If dimension_data_nums = "3" Then
                    If (CInt(nstr(2)) >= CInt(lux_min_str) And CInt(nstr(2)) <= CInt(lux_max_str)) And _
                        (CInt(nstr(3)) >= CInt(lux_min_str) And CInt(nstr(3)) <= CInt(lux_max_str)) And _
                        (CInt(nstr(4)) >= CInt(CDbl(temp_min_str) * 10) And CInt(nstr(4)) <= CInt(CDbl(temp_max_str) * 10)) And _
                        (CInt(nstr(5)) >= CInt(CDbl(temp_min_str) * 10) And CInt(nstr(5)) <= CInt(CDbl(temp_max_str) * 10)) And _
                        (CInt(nstr(6)) >= CInt(CDbl(press_min_str) * 1000) And CInt(nstr(6)) <= CInt(CDbl(press_max_str) * 1000)) And _
                        (CInt(nstr(7)) >= CInt(CDbl(press_min_str) * 1000) And CInt(nstr(7)) <= CInt(CDbl(press_max_str) * 1000)) Then
                        ShowTitle = "*****" + Format(DateTime.Now, "yyyy-MM-dd HH:mm:ss") + "<--" + thread_param.sk.RemoteEndPoint.ToString + " resualt of query erro infor*****" + vbCrLf
                        ShowDate = "存在比较因子伪造攻击！(-3)"
                        Invok_Recv(ShowTitle, ShowDate) '委托调用
                    End If
                End If

                If dimension_data_nums = "4" Then
                    If (CInt(nstr(2)) >= CInt(lux_min_str) And CInt(nstr(2)) <= CInt(lux_max_str)) And _
                        (CInt(nstr(3)) >= CInt(lux_min_str) And CInt(nstr(3)) <= CInt(lux_max_str)) And _
                        (CInt(nstr(4)) >= CInt(CDbl(temp_min_str) * 10) And CInt(nstr(4)) <= CInt(CDbl(temp_max_str) * 10)) And _
                        (CInt(nstr(5)) >= CInt(CDbl(temp_min_str) * 10) And CInt(nstr(5)) <= CInt(CDbl(temp_max_str) * 10)) And _
                        (CInt(nstr(6)) >= CInt(CDbl(press_min_str) * 1000) And CInt(nstr(6)) <= CInt(CDbl(press_max_str) * 1000)) And _
                        (CInt(nstr(7)) >= CInt(CDbl(press_min_str) * 1000) And CInt(nstr(7)) <= CInt(CDbl(press_max_str) * 1000)) And _
                        (CInt(nstr(8)) >= CInt(CDbl(humi_min_str) * 10) And CInt(nstr(8)) <= CInt(CDbl(humi_max_str) * 10)) And _
                        (CInt(nstr(9)) >= CInt(CDbl(humi_min_str) * 10) And CInt(nstr(9)) <= CInt(CDbl(humi_max_str) * 10)) Then
                        ShowTitle = "*****" + Format(DateTime.Now, "yyyy-MM-dd HH:mm:ss") + "<--" + thread_param.sk.RemoteEndPoint.ToString + " resualt of query erro infor*****" + vbCrLf
                        ShowDate = "存在比较因子伪造攻击！(-3)"
                        Invok_Recv(ShowTitle, ShowDate) '委托调用
                    End If
                End If

            End If
        End If
    End Sub
    Public Sub query_ret(ByVal rev_buff As String, ByVal sk As Socket)
        Dim ShowDate As String = ""
        Dim ShowTitle As String = ""

        ShowTitle = "*****" + Format(DateTime.Now, "yyyy-MM-dd HH:mm:ss") + "<--" + sk.RemoteEndPoint.ToString + " resualt of query*****" + vbCrLf
        Dim recv_jobj As New JObject
        Try
            recv_jobj = JObject.Parse(rev_buff)
            recv_jobj.Item("time").ToString()
            recv_jobj.Item("devid").ToString()
        Catch ex As Exception
            ShowDate = "存在数据帧攻击(-1.2)！" + vbCrLf + rev_buff
            Invok_Recv(ShowTitle, ShowDate) '委托调用
            Exit Sub
        End Try

        Dim sqlconn1 As SqlConnection = New SqlConnection _
    ("Data Source=(local);Initial Catalog=graduation;Integrated Security=False;User ID=sa;Password=123456;") '连接数据库
        sqlconn1.Open()
        Dim cmd As New SqlCommand()
        cmd.Connection = sqlconn1
        cmd.CommandText = "SELECT id,aeskey FROM device WHERE startime<'" + recv_jobj.Item("time").ToString + "' AND (endtime>'" + _
            recv_jobj.Item("time").ToString + "' OR endtime IS NULL) AND devid='" + recv_jobj.Item("devid").ToString + "'"
        Dim dr As SqlDataReader

        dr = cmd.ExecuteReader()

        If dr.HasRows() = True Then
            Do While dr.Read() = True   '如果有记录，就循环打印符合条件的记录的address字段中内容

                Dim decripter_tool As New aes_decripter_strn
                Dim d_index As String
                Dim nstr As String()
                decripter_tool.setkey(dr.Item("aeskey"))
                decripter_tool.do_decripter(recv_jobj.Item("data_index").ToString())
                d_index = decripter_tool.clearcode
                nstr = d_index.Split(",")

                Dim tmptime As TimeSpan
                tmptime = Date.Parse(recv_jobj.Item("time")) - DicC(dr.Item("id")).currtime
                If tmptime.TotalSeconds >= ((2 * CInt(nstr(nstr.Length - 1)) * collect_period_int) / 1000) Then
                    ShowTitle = "*****" + Format(DateTime.Now, "yyyy-MM-dd HH:mm:ss") + "<--" + sk.RemoteEndPoint.ToString + " resualt of query erro infor*****" + vbCrLf
                    ShowDate = "在" + Format(DicC(dr.Item("id")).currtime, "yyyy-MM-dd HH:mm:ss") + "到" + recv_jobj.Item("time").ToString + "数据丢失"
                    Invok_Recv(ShowTitle, ShowDate) '委托调用
                End If
                DicC(dr.Item("id")).currtime = Date.Parse(recv_jobj.Item("time"))
                ShowDate = dr.Item("aeskey")

                Dim thread_buffsockeet As thread_buff
                thread_buffsockeet.aeskey = dr.Item("aeskey")
                thread_buffsockeet.json_ret = recv_jobj
                thread_buffsockeet.sk = sk
                mythread_Semaphore.WaitOne()
                Dim query_ret_thread As New Thread(AddressOf query_ret_deal)
                query_ret_thread.Start(thread_buffsockeet)
                mythread_Semaphore.Release()
            Loop
        Else
            MessageBox.Show("没有找到密钥！", "提示")
        End If
        dr.Close()
        sqlconn1.Close()

        'ShowDate = recv_buf_all
        'Invok_Recv(ShowTitle, ShowDate) '委托调用
    End Sub
    Public Sub ReciveData_Deal(ByVal buff_recv As String, ByVal s As Socket) '接收到的数据处理函数
        Dim ShowTitle As String = ""
        Dim ShowDate As String = ""

        Dim rcv_jobj As New JObject
        Try
            rcv_jobj = JObject.Parse(buff_recv)
            rcv_jobj.Item("type").ToString()
        Catch ex As Exception
            ShowDate = "存在数据帧攻击(-1)！" + vbCrLf + buff_recv
            Invok_Recv(ShowTitle, ShowDate) '委托调用
            Exit Sub
        End Try
        Dim objectStr As String = rcv_jobj.Item("type").ToString
        If objectStr = "request join net" Then
            Dim SendNum As Integer
            Dim sink_dh As ZqjDHForGraduationP.DHTool
            sink_dh = New ZqjDHForGraduationP.DHTool
            sink_dh.DHParameterGenerate()
            sink_dh.GenerateYa()
            Dim send_jobj As New JObject
            Try
                rcv_jobj.Item("devid").ToString()
                rcv_jobj.Item("socketfd").ToString()

            Catch ex As Exception
                ShowDate = "存在数据帧攻击(-2)！"
                Invok_Recv(ShowTitle, ShowDate) '委托调用
                Exit Sub
            End Try
            send_jobj.Add("type", "request join rck gpya")
            send_jobj.Add("devid", rcv_jobj.Item("devid").ToString)
            send_jobj.Add("dh_g", sink_dh.G)
            send_jobj.Add("dh_p", sink_dh.P)
            send_jobj.Add("dh_ya", sink_dh.Ya)
            send_jobj.Add("socketfd", rcv_jobj.Item("socketfd"))
            If DicDH.Keys.Contains(rcv_jobj.Item("devid").ToString) Then '判断键值是否存在
                DicDH.Remove(rcv_jobj.Item("devid").ToString)
            End If
            DicDH.Add(rcv_jobj.Item("devid").ToString, sink_dh)
            Dim sendstr As String
            sendstr = JsonConvert.SerializeObject(send_jobj)
            SendNum = s.Send(Encoding.UTF8.GetBytes(sendstr))
            If SendNum = 0 Then
                MessageBox.Show("发送失败！", "错误")
            End If

        End If

        If objectStr = "request join generat KEY" Then
            Dim send_jobj As New JObject
            Try
                rcv_jobj.Item("devid").ToString()
                rcv_jobj.Item("socketfd").ToString()
                rcv_jobj.Item("dh_yb").ToString()
                rcv_jobj.Item("dh_check").ToString()
                rcv_jobj.Item("time").ToString()
            Catch ex As Exception
                ShowDate = "存在数据帧攻击(-2)！"
                Invok_Recv(ShowTitle, ShowDate) '委托调用
                Exit Sub
            End Try
            send_jobj.Add("type", "check key status")
            send_jobj.Add("devid", rcv_jobj.Item("devid").ToString)
            send_jobj.Add("socketfd", rcv_jobj.Item("socketfd"))
            DicDH(rcv_jobj.Item("devid").ToString).KeyGenerateA(rcv_jobj.Item("dh_yb").ToString)

            Dim checkkey As String = ""
            Dim n As Integer = 0
            Dim strkey As String = DicDH(rcv_jobj.Item("devid").ToString).Key
            If Int(Len(strkey)) < 32 Then
                For i = 1 To 32 - Int(Len(strkey)) Step 1
                    strkey = strkey + "0"
                Next
            End If
            Dim encripter_tool As New aes_decripter_strn
            encripter_tool.setkey(strkey)
            checkkey = encripter_tool.do_encripter(rcv_jobj.Item("dh_yb").ToString)

            Dim dh_check As String = ""
            dh_check = rcv_jobj.Item("dh_check").ToString()
            If Trim(dh_check) = Trim(checkkey) Then
                sqlconn.Open()
                Dim sqlcmdstr As String
                sqlcmdstr = "UPDATE device SET endtime='" + rcv_jobj.Item("time").ToString() + "' WHERE id=(SELECT MAX(id) FROM device WHERE devid='" + rcv_jobj.Item("devid").ToString() + "')"
                Dim sqlcmd1 As New SqlCommand(sqlcmdstr, sqlconn)
                sqlcmd1.ExecuteNonQuery()
                sqlcmdstr = "INSERT INTO device (devid,aeskey,startime) VALUES ('" + rcv_jobj.Item("devid").ToString() + "','" + strkey + "','" + rcv_jobj.Item("time").ToString() + "')"
                Dim sqlcmd2 As New SqlCommand(sqlcmdstr, sqlconn)
                sqlcmd2.ExecuteNonQuery()
                sqlconn.Close()

                ShowTitle = "*****" + Format(DateTime.Now, "yyyy-MM-dd HH:mm:ss") + "<--" + s.RemoteEndPoint.ToString + "share key status*****" + vbCrLf
                ShowDate = "设备'" + rcv_jobj.Item("devid").ToString() + "' 密钥交换完成!"
                send_jobj.Add("key_status", "key ok")
            Else
                ShowDate = "The key change error"
                send_jobj.Add("key_status", "error")
            End If
            Invok_Recv(ShowTitle, ShowDate) '委托调用
            If DicDH.Keys.Contains(rcv_jobj.Item("devid").ToString) Then '判断键值是否存在
                DicDH.Remove(rcv_jobj.Item("devid").ToString) '建立密钥后暂存在字典的中间数据释放
            End If
            Dim sendstr As String
            Dim SendNum As Integer
            sendstr = JsonConvert.SerializeObject(send_jobj)
            SendNum = s.Send(Encoding.UTF8.GetBytes(sendstr))
            If SendNum = 0 Then
                MessageBox.Show("发送失败！", "错误")
            End If
        End If

        If objectStr = "private data query result" Then
            query_ret(buff_recv, s)

        End If
        rcv_jobj = Nothing

    End Sub

    Public Sub ReciveData(ByVal s As Socket) '接收socket数据
        Dim timeout As Byte = 0
        Dim bytesRec As Integer
        Dim ShowTitle As String = ""
        Dim ShowDate As String = ""
        While (True)
            Dim bytes(1024) As Byte '用来存储接收到的字节
            timeout = 0
            Try
                bytesRec = s.Receive(bytes)

            Catch ex As Exception '处理服务器端口关闭
                Try
                    mysocket.Poll(50, Net.Sockets.SelectMode.SelectRead) '检查socket是否异常
                Catch ex1 As Exception
                    s.Dispose()
                    s.Close()
                    Thread.CurrentThread.Abort()
                End Try
                timeout = 1
            End Try
            If timeout = 0 Then
                If Encoding.UTF8.GetString(bytes, 0, bytesRec).Length = 0 Then '是否是断开信号
                    ShowTitle = "*****" + Format(DateTime.Now, "yyyy-MM-dd HH:mm:ss") + "--unlinked--" + s.RemoteEndPoint.ToString + "*****" + vbCrLf
                    ShowDate = ""
                    Invok_Recv(ShowTitle, ShowDate) '委托调用
                    Invok_Add_Client("remove", s.RemoteEndPoint.ToString)
                    DicS.Remove(s.RemoteEndPoint.ToString) '把已经断开的客户端从SOCKET列表里删除
                    DicT.Remove(s.RemoteEndPoint.ToString) '把已经断开的客户端从Thread列表里删除

                    s.Dispose()
                    s.Close()
                    Thread.CurrentThread.Abort()
                End If
                '接收到数据处理区
                Dim buf_tmp As String = ""
                Dim buf_tmp_len As Integer = 0
                Dim recv_single_buff As String()
                Dim recv_single_buff_len As Integer = 0

                'Sleep(10)
                buf_tmp = Encoding.ASCII.GetString(bytes, 0, bytesRec).ToString
                recv_buf_all = recv_buf_all + buf_tmp
                If InStr(recv_buf_all, "}") <> 0 Then
                    recv_single_buff = recv_buf_all.Split("}")
                    recv_single_buff_len = recv_single_buff.Length
                    For i = 0 To recv_single_buff_len - 2 Step 1
                        recv_single_buff(i) = recv_single_buff(i) + "}"
                        ReciveData_Deal(recv_single_buff(i), s)
                    Next
                    recv_buf_all = recv_single_buff(recv_single_buff_len - 1)
                End If

            End If
        End While
    End Sub
    Private Sub StartStop_Click(sender As Object, e As EventArgs) Handles StartStop.Click
        If StartStop.Text = "启动" Then
            If TB_LocalIP.Text = "" Or TB_Port.Text = "" Then
                MessageBox.Show("IP地址或端口号本能为空！", "提示：")
                Return
            End If
            If Val(TB_Port.Text) < 0 Or Val(TB_Port.Text) > 65535 Then
                MessageBox.Show("端口号不在合法区间！", "提示：")
                Return
            End If
            mysocket = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp) '使用TCP协议
            Dim localEndPoint As New IPEndPoint(IPAddress.Parse(TB_LocalIP.Text), Val(TB_Port.Text)) '指定IP和端口
            Try
                mysocket.Bind(localEndPoint) '绑定到该Socket
            Catch ex As Exception
                MessageBox.Show("请检查本地主机IP和端口！", "错误：")
                Return
            End Try
            mysocket.Listen(100) '侦听最多接收100个连接
            mythread = New Thread(AddressOf OpenSocket)
            mythread.Start()
            StartStop.Text = "停止"
        ElseIf StartStop.Text = "停止" Then
            mysocket.Dispose()
            mysocket.Close()
            mythread.Abort()
            CB_Client.Items.Clear()
            CB_Client.Items.Add("ALL Client")
            DicS.Clear()
            DicT.Clear()
            StartStop.Text = "启动"
        End If

    End Sub
    Private Function Invok_Recv(ByVal title As String, ByVal content As String) As Integer '委托定义初始化
        Dim procshow As New Tothread(AddressOf PrintRevData)
        Me.Invoke(procshow, title, content)
        Return 0
    End Function
    Private Function Invok_Add_Client(ByVal title As String, ByVal content As String) As Integer '委托定义初始化
        Dim AddClient As New Tothread(AddressOf AddClientFun)
        Me.Invoke(AddClient, title, content)
        Return 0
    End Function
    Private Function AddClientFun(ByVal cmd As String, ByVal content As String) As Integer '委托功能实现
        If cmd = "add" Then
            CB_Client.Items.Add(content)
        ElseIf cmd = "remove" Then
            CB_Client.Items.Remove(content)
        End If
        Return 0
    End Function
    Private Function PrintRevData(ByVal title As String, ByVal content As String) As Integer '委托功能实现
        RT_RevText.SelectionColor = Color.Black
        RT_RevText.AppendText(title)
        RT_RevText.SelectionColor = Color.Blue
        RT_RevText.AppendText(content)
        RT_RevText.AppendText(vbCrLf)
        Return 0
    End Function

    Private Sub NetTestTool_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If StartStop.Text = "停止" Then
            mysocket.Dispose()
            mysocket.Close()
            mythread.Abort()

        End If

    End Sub

    Private Sub NetTestTool_Load(sender As Object, e As EventArgs) Handles Me.Load
        CB_Client.DropDownStyle = ComboBoxStyle.DropDownList
        dimension_select.DropDownStyle = ComboBoxStyle.DropDownList
        data_num.DropDownStyle = ComboBoxStyle.DropDownList
        Timer1.Start()
    End Sub

    Private Sub NetTestTool_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        selsect_config.Top = (Me.Height - 373 - 121) / 4 + server_config.Height + 5
        server_config.Top = (Me.Height - 373 - 121) / 4
        RT_RevText.Height = Me.Height - 75
        RT_RevText.Width = Me.Width - 265

    End Sub

    Private Sub generate_querycmd_send(ByVal devid As String, ByVal aeskey As String, ByVal startime As String, ByVal endtime As String)
        Dim SendNum As Integer = 0
        Dim ShowTitle As String = ""
        Dim ShowDate As String = ""
        Dim Send_Query_Cmd As String = ""
        Dim Send_Query_Cmd_Jobj As New JObject
        Dim work_lzerone As New LZerOne_toHmacString
        work_lzerone.set_parameter(aeskey, data_num.Text)

        Send_Query_Cmd_Jobj.Add("type", "private data query cmd")
        Send_Query_Cmd_Jobj.Add("devid", devid)
        Send_Query_Cmd_Jobj.Add("star_time", startime)
        Send_Query_Cmd_Jobj.Add("end_time", endtime)

        '处理光照强度
        If dimension_select.Text >= "1" Then
            work_lzerone.calc_zerone_hmacstring(lux_min_text.Text)
            Send_Query_Cmd_Jobj.Add("lux_min_one", work_lzerone.get_comparer_one())
            Send_Query_Cmd_Jobj.Add("lux_min_zero", work_lzerone.get_comparer_zero())
            work_lzerone.calc_zerone_hmacstring(lux_max_text.Text)
            Send_Query_Cmd_Jobj.Add("lux_max_one", work_lzerone.get_comparer_one())
            Send_Query_Cmd_Jobj.Add("lux_max_zero", work_lzerone.get_comparer_zero())
        End If

        '处理温度
        If dimension_select.Text >= "2" Then
            Dim temp_min_int As Integer = 0
            Dim temp_max_int As Integer = 0
            Dim temp_f As Double
            If data_num.Text = "8" Then
                temp_min_int = CInt(tmp_min_text.Text)
                temp_max_int = CInt(tmp_max_text.Text)
            Else
                temp_f = CDbl(tmp_min_text.Text)
                temp_f = temp_f * 10
                temp_min_int = CInt(temp_f)
                temp_f = CDbl(tmp_max_text.Text)
                temp_f = temp_f * 10
                temp_max_int = CInt(temp_f)
            End If
            work_lzerone.calc_zerone_hmacstring(temp_min_int.ToString)
            Send_Query_Cmd_Jobj.Add("temp_min_one", work_lzerone.get_comparer_one())
            Send_Query_Cmd_Jobj.Add("temp_min_zero", work_lzerone.get_comparer_zero())

            work_lzerone.calc_zerone_hmacstring(temp_max_int.ToString)
            Send_Query_Cmd_Jobj.Add("temp_max_one", work_lzerone.get_comparer_one())
            Send_Query_Cmd_Jobj.Add("temp_max_zero", work_lzerone.get_comparer_zero())
        End If

        '处理大气压
        If dimension_select.Text >= "3" Then
            Dim press_min_int As Integer = 0
            Dim press_max_int As Integer = 0
            Dim press_f As Double
            If data_num.Text = "8" Then
                press_min_int = CInt(press_min_text.Text)
                press_max_int = CInt(press_max_text.Text)
            ElseIf data_num.Text = "16" Then
                press_f = CDbl(press_min_text.Text)
                press_f = press_f * 100
                press_min_int = CInt(press_f)
                press_f = CDbl(press_max_text.Text)
                press_f = press_f * 100
                press_max_int = CInt(press_f)
            Else
                press_f = CDbl(press_min_text.Text)
                press_f = press_f * 1000
                press_min_int = CInt(press_f)
                press_f = CDbl(press_max_text.Text)
                press_f = press_f * 1000
                press_max_int = CInt(press_f)
            End If
            work_lzerone.calc_zerone_hmacstring(press_min_int.ToString)
            Send_Query_Cmd_Jobj.Add("press_min_one", work_lzerone.get_comparer_one())
            Send_Query_Cmd_Jobj.Add("press_min_zero", work_lzerone.get_comparer_zero())

            work_lzerone.calc_zerone_hmacstring(press_max_int.ToString)
            Send_Query_Cmd_Jobj.Add("press_max_one", work_lzerone.get_comparer_one())
            Send_Query_Cmd_Jobj.Add("press_max_zero", work_lzerone.get_comparer_zero())
        End If

        '处理湿度
        If dimension_select.Text = "4" Then
            Dim humi_min_int As Integer = 0
            Dim humi_max_int As Integer = 0
            Dim humi_f As Double
            If data_num.Text = "8" Then
                humi_min_int = CInt(humi_min_text.Text)
                humi_max_int = CInt(humi_max_text.Text)
            Else
                humi_f = CDbl(humi_min_text.Text)
                humi_f = humi_f * 10
                humi_min_int = CInt(humi_f)
                humi_f = CDbl(humi_max_text.Text)
                humi_f = humi_f * 10
                humi_max_int = CInt(humi_f)

            End If
            work_lzerone.calc_zerone_hmacstring(humi_min_int.ToString)
            Send_Query_Cmd_Jobj.Add("humi_min_one", work_lzerone.get_comparer_one())
            Send_Query_Cmd_Jobj.Add("humi_min_zero", work_lzerone.get_comparer_zero())

            work_lzerone.calc_zerone_hmacstring(humi_max_int.ToString)
            Send_Query_Cmd_Jobj.Add("humi_max_one", work_lzerone.get_comparer_one())
            Send_Query_Cmd_Jobj.Add("humi_max_zero", work_lzerone.get_comparer_zero())

        End If
        Send_Query_Cmd = JsonConvert.SerializeObject(Send_Query_Cmd_Jobj)

        '负责发送数据
        If CB_Client.Text = "ALL Client" Then
            If CB_Client.Items.Count > 1 Then
                For i As Integer = 1 To (CB_Client.Items.Count - 1)
                    SendNum = DicS(CB_Client.GetItemText(CB_Client.Items(i))).Send(Encoding.UTF8.GetBytes(Send_Query_Cmd))
                    If SendNum = 0 Then
                        MessageBox.Show("发送失败！", "错误")
                    Else
                        'ShowTitle = "*****" + Format(DateTime.Now, "yyyy-MM-dd HH:mm:ss") + "-->" + "send to strong " + DicS(CB_Client.GetItemText(CB_Client.Items(i))).RemoteEndPoint.ToString + " information*****" + vbCrLf
                        'ShowDate = Send_Query_Cmd
                        'PrintRevData(ShowTitle, ShowDate)
                    End If
                Next
            Else
                MessageBox.Show("没有存储节点上线！", "提示")
            End If
        Else
            If CB_Client.Text <> "" Then
                SendNum = DicS(CB_Client.Text).Send(Encoding.UTF8.GetBytes(Send_Query_Cmd))
                If SendNum = 0 Then
                    MessageBox.Show("发送失败！", "错误")
                Else
                    'ShowTitle = "*****" + Format(DateTime.Now, "yyyy-MM-dd HH:mm:ss") + "-->" + "send to strong " + DicS(CB_Client.Text).RemoteEndPoint.ToString + " information*****" + vbCrLf
                    'ShowDate = Send_Query_Cmd
                    'PrintRevData(ShowTitle, ShowDate)
                End If
            End If
        End If

    End Sub
    'Public Shared Sub Sleep(ByVal Interval)
    '    Dim __time As DateTime = DateTime.Now
    '    Dim __Span As Int64 = Interval * 10000 '因为时间是以100纳秒为单位。
    '    While (DateTime.Now.Ticks - __time.Ticks < __Span)
    '        Application.DoEvents()
    '    End While
    'End Sub
    Private Sub SentButton_Click(sender As Object, e As EventArgs) Handles SentButton.Click
        '发送查询命令
        If startime_text.Text.Length <> 19 Or endtime_text.Text.Length <> 19 Then
            MessageBox.Show("输入时间格式不合法！", "提示：")
            Exit Sub
        End If

        If dimension_select.Text >= "1" Then
            If lux_min_text.Text = "" Or lux_max_text.Text = "" Then
                MessageBox.Show("光照强度范围不能为空", "错误：")
                Exit Sub
            End If
        End If

        If dimension_select.Text >= "2" Then
            If tmp_min_text.Text = "" Or tmp_max_text.Text = "" Then
                MessageBox.Show("温度范围不能为空", "错误：")
                Exit Sub
            End If
        End If

        If dimension_select.Text >= "3" Then
            If press_min_text.Text = "" Or press_max_text.Text = "" Then
                MessageBox.Show("大气压范围不能为空", "错误：")
                Exit Sub
            End If
        End If

        If dimension_select.Text = "4" Then
            If humi_min_text.Text = "" Or humi_max_text.Text = "" Then
                MessageBox.Show("湿度范围不能为空", "错误：")
                Exit Sub
            End If
        End If

        sqlconn.Open()
        Dim cmd As New SqlCommand()
        cmd.Connection = sqlconn
        If device_id.Text = "" Then
            cmd.CommandText = "SELECT * FROM device WHERE (startime>='" + startime_text.Text + _
                "' AND startime<'" + endtime_text.Text + "' ) OR (endtime>'" + _
                startime_text.Text + "' AND endtime<='" + endtime_text.Text + "') OR (startime<='" + startime_text.Text + _
                "' AND endtime>='" + endtime_text.Text + "' )"
        Else
            cmd.CommandText = "SELECT * FROM device WHERE devid='" + device_id.Text + _
            "' AND ((startime>='" + startime_text.Text + _
                "' AND startime<'" + endtime_text.Text + "' ) OR (endtime>'" + _
                startime_text.Text + "' AND endtime<='" + endtime_text.Text + "') OR (startime<='" + startime_text.Text + _
                "' AND endtime>='" + endtime_text.Text + "' ))"
        End If
        Dim dr As SqlDataReader
        dr = cmd.ExecuteReader()
        If dr.HasRows() = True Then
            Do While dr.Read() = True   '如果有记录，就循环打印符合条件的记录的address字段中内容
                If Not (DicC.ContainsKey(CInt(dr.Item("id")))) Then
                    Dim TimeZoneTmp As New TimeCheck
                    DicC.Add(CInt(dr.Item("id")), TimeZoneTmp)
                End If
                Dim cmd_start_time As String
                Dim cmd_end_time As String
                If dr.Item("startime") >= startime_text.Text Then
                    cmd_start_time = dr.Item("startime")
                Else
                    cmd_start_time = startime_text.Text
                End If

                If IsDBNull(dr.Item("endtime")) Then
                    cmd_end_time = endtime_text.Text
                Else
                    If dr.Item("endtime") >= endtime_text.Text Then
                        cmd_end_time = endtime_text.Text
                    Else
                        cmd_end_time = dr.Item("endtime")
                    End If
                End If

                DicC(CInt(dr.Item("id"))).currtime = cmd_start_time
                DicC(CInt(dr.Item("id"))).endtime = cmd_end_time
                generate_querycmd_send(dr.Item("devid"), dr.Item("aeskey"), cmd_start_time, cmd_end_time)
                'Sleep(100)
            Loop
        Else
            MessageBox.Show("密钥数据库没有符合要求的数据！", "提示")
        End If
        dr.Close()
        sqlconn.Close()

    End Sub

    Private Sub dimension_select_SelectedIndexChanged(sender As Object, e As EventArgs) Handles dimension_select.SelectedIndexChanged
        If dimension_select.Text = "4" Then
            lux_min_text.Enabled = True
            lux_max_text.Enabled = True
            tmp_min_text.Enabled = True
            tmp_max_text.Enabled = True
            press_min_text.Enabled = True
            press_max_text.Enabled = True
            humi_min_text.Enabled = True
            humi_max_text.Enabled = True
        End If

        If dimension_select.Text = "3" Then
            lux_min_text.Enabled = True
            lux_max_text.Enabled = True
            tmp_min_text.Enabled = True
            tmp_max_text.Enabled = True
            press_min_text.Enabled = True
            press_max_text.Enabled = True
            humi_min_text.Enabled = False
            humi_min_text.Text = ""
            humi_max_text.Enabled = False
            humi_max_text.Text = ""
        End If

        If dimension_select.Text = "2" Then
            lux_min_text.Enabled = True
            lux_max_text.Enabled = True
            tmp_min_text.Enabled = True
            tmp_max_text.Enabled = True
            press_min_text.Enabled = False
            press_min_text.Text = ""
            press_max_text.Enabled = False
            press_max_text.Text = ""
            humi_min_text.Enabled = False
            humi_min_text.Text = ""
            humi_max_text.Enabled = False
            humi_max_text.Text = ""
        End If

        If dimension_select.Text = "1" Then
            lux_min_text.Enabled = True
            lux_max_text.Enabled = True
            tmp_min_text.Enabled = False
            tmp_min_text.Text = ""
            tmp_max_text.Enabled = False
            tmp_max_text.Text = ""
            press_min_text.Enabled = False
            press_min_text.Text = ""
            press_max_text.Enabled = False
            press_max_text.Text = ""
            humi_min_text.Enabled = False
            humi_min_text.Text = ""
            humi_max_text.Enabled = False
            humi_max_text.Text = ""
        End If
        dimension_data_nums = dimension_select.Text
    End Sub

    Private Sub lux_min_text_TextChanged(sender As Object, e As EventArgs) Handles lux_min_text.TextChanged
        lux_min_str = lux_min_text.Text
    End Sub

    Private Sub lux_max_text_TextChanged(sender As Object, e As EventArgs) Handles lux_max_text.TextChanged
        lux_max_str = lux_max_text.Text
    End Sub

    Private Sub tmp_min_text_TextChanged(sender As Object, e As EventArgs) Handles tmp_min_text.TextChanged
        temp_min_str = tmp_min_text.Text
    End Sub

    Private Sub tmp_max_text_TextChanged(sender As Object, e As EventArgs) Handles tmp_max_text.TextChanged
        temp_max_str = tmp_max_text.Text
    End Sub

    Private Sub press_min_text_TextChanged(sender As Object, e As EventArgs) Handles press_min_text.TextChanged
        press_min_str = press_min_text.Text
    End Sub

    Private Sub press_max_text_TextChanged(sender As Object, e As EventArgs) Handles press_max_text.TextChanged
        press_max_str = press_max_text.Text
    End Sub

    Private Sub humi_min_text_TextChanged(sender As Object, e As EventArgs) Handles humi_min_text.TextChanged
        humi_min_str = humi_min_text.Text
    End Sub

    Private Sub humi_max_text_TextChanged(sender As Object, e As EventArgs) Handles humi_max_text.TextChanged
        humi_max_str = humi_max_text.Text
    End Sub

    Private Sub data_num_SelectedIndexChanged(sender As Object, e As EventArgs) Handles data_num.SelectedIndexChanged
        data_num_str = data_num.Text
    End Sub

    Private Sub collect_period_TextChanged(sender As Object, e As EventArgs) Handles collect_period.TextChanged
        If (collect_period.Text = "") Then
            collect_period_int = 1
        Else
            collect_period_int = CInt(collect_period.Text)
        End If
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        If RT_RevText.Text.Length <> recv_richtext_length Then
            recv_richtext_length = RT_RevText.Text.Length
            RT_RevText.ScrollToCaret()
        End If
    End Sub

End Class
