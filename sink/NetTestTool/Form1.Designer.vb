<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class NetTestTool
    Inherits System.Windows.Forms.Form

    'Form 重写 Dispose，以清理组件列表。
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Windows 窗体设计器所必需的
    Private components As System.ComponentModel.IContainer

    '注意:  以下过程是 Windows 窗体设计器所必需的
    '可以使用 Windows 窗体设计器修改它。  
    '不要使用代码编辑器修改它。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.L_LocalIP = New System.Windows.Forms.Label()
        Me.TB_LocalIP = New System.Windows.Forms.TextBox()
        Me.L_LocalPort = New System.Windows.Forms.Label()
        Me.TB_Port = New System.Windows.Forms.TextBox()
        Me.ClearText = New System.Windows.Forms.Button()
        Me.StartStop = New System.Windows.Forms.Button()
        Me.SentButton = New System.Windows.Forms.Button()
        Me.CearSendZone = New System.Windows.Forms.Button()
        Me.L_RecZone = New System.Windows.Forms.Label()
        Me.CB_Client = New System.Windows.Forms.ComboBox()
        Me.start_time_label = New System.Windows.Forms.Label()
        Me.data_dimension = New System.Windows.Forms.Label()
        Me.dimension_select = New System.Windows.Forms.ComboBox()
        Me.lux_lable = New System.Windows.Forms.Label()
        Me.lux_min_text = New System.Windows.Forms.TextBox()
        Me.lux_max_text = New System.Windows.Forms.TextBox()
        Me.lux_mid = New System.Windows.Forms.Label()
        Me.tmp_label = New System.Windows.Forms.Label()
        Me.tmp_min_text = New System.Windows.Forms.TextBox()
        Me.tmp_mid = New System.Windows.Forms.Label()
        Me.tmp_max_text = New System.Windows.Forms.TextBox()
        Me.humi_lable = New System.Windows.Forms.Label()
        Me.humi_min_text = New System.Windows.Forms.TextBox()
        Me.humi_mid = New System.Windows.Forms.Label()
        Me.humi_max_text = New System.Windows.Forms.TextBox()
        Me.press_lable = New System.Windows.Forms.Label()
        Me.press_min_text = New System.Windows.Forms.TextBox()
        Me.press_mid = New System.Windows.Forms.Label()
        Me.press_max_text = New System.Windows.Forms.TextBox()
        Me.data_num_lable = New System.Windows.Forms.Label()
        Me.data_num = New System.Windows.Forms.ComboBox()
        Me.collect_period_label = New System.Windows.Forms.Label()
        Me.collect_period = New System.Windows.Forms.TextBox()
        Me.ms_label = New System.Windows.Forms.Label()
        Me.lux_l = New System.Windows.Forms.Label()
        Me.wendul = New System.Windows.Forms.Label()
        Me.pa_l = New System.Windows.Forms.Label()
        Me.shidul = New System.Windows.Forms.Label()
        Me.server_config = New System.Windows.Forms.GroupBox()
        Me.selsect_config = New System.Windows.Forms.GroupBox()
        Me.strong_select = New System.Windows.Forms.Label()
        Me.endtime_text = New System.Windows.Forms.TextBox()
        Me.end_time_label = New System.Windows.Forms.Label()
        Me.startime_text = New System.Windows.Forms.TextBox()
        Me.device_id = New System.Windows.Forms.TextBox()
        Me.device_id_label = New System.Windows.Forms.Label()
        Me.RT_RevText = New System.Windows.Forms.RichTextBox()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.server_config.SuspendLayout()
        Me.selsect_config.SuspendLayout()
        Me.SuspendLayout()
        '
        'L_LocalIP
        '
        Me.L_LocalIP.AutoSize = True
        Me.L_LocalIP.Font = New System.Drawing.Font("宋体", 10.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.L_LocalIP.Location = New System.Drawing.Point(6, 17)
        Me.L_LocalIP.Name = "L_LocalIP"
        Me.L_LocalIP.Size = New System.Drawing.Size(49, 14)
        Me.L_LocalIP.TabIndex = 2
        Me.L_LocalIP.Text = "本机IP"
        '
        'TB_LocalIP
        '
        Me.TB_LocalIP.Font = New System.Drawing.Font("宋体", 10.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.TB_LocalIP.Location = New System.Drawing.Point(76, 14)
        Me.TB_LocalIP.Name = "TB_LocalIP"
        Me.TB_LocalIP.Size = New System.Drawing.Size(147, 23)
        Me.TB_LocalIP.TabIndex = 3
        Me.TB_LocalIP.Text = "192.168.3.102"
        '
        'L_LocalPort
        '
        Me.L_LocalPort.AutoSize = True
        Me.L_LocalPort.Font = New System.Drawing.Font("宋体", 10.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.L_LocalPort.Location = New System.Drawing.Point(6, 52)
        Me.L_LocalPort.Name = "L_LocalPort"
        Me.L_LocalPort.Size = New System.Drawing.Size(49, 14)
        Me.L_LocalPort.TabIndex = 4
        Me.L_LocalPort.Text = "端口号"
        '
        'TB_Port
        '
        Me.TB_Port.Font = New System.Drawing.Font("宋体", 10.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.TB_Port.Location = New System.Drawing.Point(76, 49)
        Me.TB_Port.Name = "TB_Port"
        Me.TB_Port.Size = New System.Drawing.Size(147, 23)
        Me.TB_Port.TabIndex = 5
        Me.TB_Port.Text = "8989"
        '
        'ClearText
        '
        Me.ClearText.Font = New System.Drawing.Font("宋体", 10.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.ClearText.Location = New System.Drawing.Point(139, 85)
        Me.ClearText.Name = "ClearText"
        Me.ClearText.Size = New System.Drawing.Size(84, 30)
        Me.ClearText.TabIndex = 6
        Me.ClearText.Text = "清空显示"
        Me.ClearText.UseVisualStyleBackColor = True
        '
        'StartStop
        '
        Me.StartStop.Font = New System.Drawing.Font("宋体", 10.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.StartStop.Location = New System.Drawing.Point(6, 85)
        Me.StartStop.Name = "StartStop"
        Me.StartStop.Size = New System.Drawing.Size(84, 30)
        Me.StartStop.TabIndex = 7
        Me.StartStop.Text = "启动"
        Me.StartStop.UseVisualStyleBackColor = True
        '
        'SentButton
        '
        Me.SentButton.Font = New System.Drawing.Font("宋体", 10.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.SentButton.Location = New System.Drawing.Point(9, 330)
        Me.SentButton.Name = "SentButton"
        Me.SentButton.Size = New System.Drawing.Size(84, 30)
        Me.SentButton.TabIndex = 9
        Me.SentButton.Text = "查询"
        Me.SentButton.UseVisualStyleBackColor = True
        '
        'CearSendZone
        '
        Me.CearSendZone.Font = New System.Drawing.Font("宋体", 10.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.CearSendZone.Location = New System.Drawing.Point(138, 330)
        Me.CearSendZone.Name = "CearSendZone"
        Me.CearSendZone.Size = New System.Drawing.Size(84, 30)
        Me.CearSendZone.TabIndex = 10
        Me.CearSendZone.Text = "清空输入"
        Me.CearSendZone.UseVisualStyleBackColor = True
        '
        'L_RecZone
        '
        Me.L_RecZone.AutoSize = True
        Me.L_RecZone.Location = New System.Drawing.Point(237, 9)
        Me.L_RecZone.Name = "L_RecZone"
        Me.L_RecZone.Size = New System.Drawing.Size(41, 12)
        Me.L_RecZone.TabIndex = 12
        Me.L_RecZone.Text = "显示区"
        '
        'CB_Client
        '
        Me.CB_Client.Font = New System.Drawing.Font("宋体", 10.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.CB_Client.FormattingEnabled = True
        Me.CB_Client.Items.AddRange(New Object() {"ALL Client"})
        Me.CB_Client.Location = New System.Drawing.Point(65, 10)
        Me.CB_Client.Name = "CB_Client"
        Me.CB_Client.Size = New System.Drawing.Size(158, 22)
        Me.CB_Client.TabIndex = 14
        Me.CB_Client.Text = "ALL Client"
        '
        'start_time_label
        '
        Me.start_time_label.AutoSize = True
        Me.start_time_label.Font = New System.Drawing.Font("宋体", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.start_time_label.Location = New System.Drawing.Point(6, 41)
        Me.start_time_label.Name = "start_time_label"
        Me.start_time_label.Size = New System.Drawing.Size(53, 12)
        Me.start_time_label.TabIndex = 15
        Me.start_time_label.Text = "起始时间"
        '
        'data_dimension
        '
        Me.data_dimension.AutoSize = True
        Me.data_dimension.Font = New System.Drawing.Font("宋体", 10.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.data_dimension.Location = New System.Drawing.Point(6, 129)
        Me.data_dimension.Name = "data_dimension"
        Me.data_dimension.Size = New System.Drawing.Size(35, 14)
        Me.data_dimension.TabIndex = 16
        Me.data_dimension.Text = "维度"
        '
        'dimension_select
        '
        Me.dimension_select.Font = New System.Drawing.Font("宋体", 10.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.dimension_select.FormattingEnabled = True
        Me.dimension_select.Items.AddRange(New Object() {"4", "3", "2", "1"})
        Me.dimension_select.Location = New System.Drawing.Point(65, 126)
        Me.dimension_select.Name = "dimension_select"
        Me.dimension_select.Size = New System.Drawing.Size(158, 22)
        Me.dimension_select.TabIndex = 17
        Me.dimension_select.Text = "4"
        '
        'lux_lable
        '
        Me.lux_lable.AutoSize = True
        Me.lux_lable.Font = New System.Drawing.Font("宋体", 10.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.lux_lable.Location = New System.Drawing.Point(5, 227)
        Me.lux_lable.Name = "lux_lable"
        Me.lux_lable.Size = New System.Drawing.Size(49, 14)
        Me.lux_lable.TabIndex = 18
        Me.lux_lable.Text = "光照度"
        '
        'lux_min_text
        '
        Me.lux_min_text.Location = New System.Drawing.Point(64, 220)
        Me.lux_min_text.Name = "lux_min_text"
        Me.lux_min_text.Size = New System.Drawing.Size(50, 21)
        Me.lux_min_text.TabIndex = 19
        Me.lux_min_text.Text = "0"
        '
        'lux_max_text
        '
        Me.lux_max_text.Location = New System.Drawing.Point(145, 220)
        Me.lux_max_text.Name = "lux_max_text"
        Me.lux_max_text.Size = New System.Drawing.Size(50, 21)
        Me.lux_max_text.TabIndex = 20
        Me.lux_max_text.Text = "3000"
        '
        'lux_mid
        '
        Me.lux_mid.AutoSize = True
        Me.lux_mid.Font = New System.Drawing.Font("宋体", 10.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.lux_mid.Location = New System.Drawing.Point(120, 224)
        Me.lux_mid.Name = "lux_mid"
        Me.lux_mid.Size = New System.Drawing.Size(21, 14)
        Me.lux_mid.TabIndex = 21
        Me.lux_mid.Text = "至"
        '
        'tmp_label
        '
        Me.tmp_label.AutoSize = True
        Me.tmp_label.Font = New System.Drawing.Font("宋体", 10.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.tmp_label.Location = New System.Drawing.Point(12, 254)
        Me.tmp_label.Name = "tmp_label"
        Me.tmp_label.Size = New System.Drawing.Size(35, 14)
        Me.tmp_label.TabIndex = 22
        Me.tmp_label.Text = "温度"
        '
        'tmp_min_text
        '
        Me.tmp_min_text.Location = New System.Drawing.Point(64, 248)
        Me.tmp_min_text.Name = "tmp_min_text"
        Me.tmp_min_text.Size = New System.Drawing.Size(50, 21)
        Me.tmp_min_text.TabIndex = 23
        Me.tmp_min_text.Text = "1.0"
        '
        'tmp_mid
        '
        Me.tmp_mid.AutoSize = True
        Me.tmp_mid.Font = New System.Drawing.Font("宋体", 10.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.tmp_mid.Location = New System.Drawing.Point(121, 251)
        Me.tmp_mid.Name = "tmp_mid"
        Me.tmp_mid.Size = New System.Drawing.Size(21, 14)
        Me.tmp_mid.TabIndex = 24
        Me.tmp_mid.Text = "至"
        '
        'tmp_max_text
        '
        Me.tmp_max_text.Location = New System.Drawing.Point(145, 248)
        Me.tmp_max_text.Name = "tmp_max_text"
        Me.tmp_max_text.Size = New System.Drawing.Size(50, 21)
        Me.tmp_max_text.TabIndex = 25
        Me.tmp_max_text.Text = "30.0"
        '
        'humi_lable
        '
        Me.humi_lable.AutoSize = True
        Me.humi_lable.Font = New System.Drawing.Font("宋体", 10.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.humi_lable.Location = New System.Drawing.Point(6, 308)
        Me.humi_lable.Name = "humi_lable"
        Me.humi_lable.Size = New System.Drawing.Size(35, 14)
        Me.humi_lable.TabIndex = 26
        Me.humi_lable.Text = "湿度"
        '
        'humi_min_text
        '
        Me.humi_min_text.Location = New System.Drawing.Point(64, 304)
        Me.humi_min_text.Name = "humi_min_text"
        Me.humi_min_text.Size = New System.Drawing.Size(51, 21)
        Me.humi_min_text.TabIndex = 27
        Me.humi_min_text.Text = "0"
        '
        'humi_mid
        '
        Me.humi_mid.AutoSize = True
        Me.humi_mid.Font = New System.Drawing.Font("宋体", 10.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.humi_mid.Location = New System.Drawing.Point(121, 305)
        Me.humi_mid.Name = "humi_mid"
        Me.humi_mid.Size = New System.Drawing.Size(21, 14)
        Me.humi_mid.TabIndex = 28
        Me.humi_mid.Text = "至"
        '
        'humi_max_text
        '
        Me.humi_max_text.Location = New System.Drawing.Point(145, 304)
        Me.humi_max_text.Name = "humi_max_text"
        Me.humi_max_text.Size = New System.Drawing.Size(50, 21)
        Me.humi_max_text.TabIndex = 29
        Me.humi_max_text.Text = "100"
        '
        'press_lable
        '
        Me.press_lable.AutoSize = True
        Me.press_lable.Font = New System.Drawing.Font("宋体", 10.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.press_lable.Location = New System.Drawing.Point(5, 282)
        Me.press_lable.Name = "press_lable"
        Me.press_lable.Size = New System.Drawing.Size(49, 14)
        Me.press_lable.TabIndex = 30
        Me.press_lable.Text = "大气压"
        '
        'press_min_text
        '
        Me.press_min_text.Location = New System.Drawing.Point(64, 275)
        Me.press_min_text.Name = "press_min_text"
        Me.press_min_text.Size = New System.Drawing.Size(51, 21)
        Me.press_min_text.TabIndex = 31
        Me.press_min_text.Text = "90.000"
        '
        'press_mid
        '
        Me.press_mid.AutoSize = True
        Me.press_mid.Font = New System.Drawing.Font("宋体", 10.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.press_mid.Location = New System.Drawing.Point(121, 279)
        Me.press_mid.Name = "press_mid"
        Me.press_mid.Size = New System.Drawing.Size(21, 14)
        Me.press_mid.TabIndex = 32
        Me.press_mid.Text = "至"
        '
        'press_max_text
        '
        Me.press_max_text.Location = New System.Drawing.Point(145, 276)
        Me.press_max_text.Name = "press_max_text"
        Me.press_max_text.Size = New System.Drawing.Size(50, 21)
        Me.press_max_text.TabIndex = 33
        Me.press_max_text.Text = "110.000"
        '
        'data_num_lable
        '
        Me.data_num_lable.AutoSize = True
        Me.data_num_lable.Location = New System.Drawing.Point(6, 163)
        Me.data_num_lable.Name = "data_num_lable"
        Me.data_num_lable.Size = New System.Drawing.Size(53, 12)
        Me.data_num_lable.TabIndex = 34
        Me.data_num_lable.Text = "数据位数"
        '
        'data_num
        '
        Me.data_num.Font = New System.Drawing.Font("宋体", 10.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.data_num.FormattingEnabled = True
        Me.data_num.Items.AddRange(New Object() {"8", "16", "32", "64"})
        Me.data_num.Location = New System.Drawing.Point(66, 158)
        Me.data_num.Name = "data_num"
        Me.data_num.Size = New System.Drawing.Size(157, 22)
        Me.data_num.TabIndex = 35
        Me.data_num.Text = "16"
        '
        'collect_period_label
        '
        Me.collect_period_label.AutoSize = True
        Me.collect_period_label.Location = New System.Drawing.Point(6, 192)
        Me.collect_period_label.Name = "collect_period_label"
        Me.collect_period_label.Size = New System.Drawing.Size(53, 12)
        Me.collect_period_label.TabIndex = 36
        Me.collect_period_label.Text = "采集周期"
        '
        'collect_period
        '
        Me.collect_period.Font = New System.Drawing.Font("宋体", 10.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.collect_period.Location = New System.Drawing.Point(64, 189)
        Me.collect_period.Name = "collect_period"
        Me.collect_period.Size = New System.Drawing.Size(67, 23)
        Me.collect_period.TabIndex = 37
        Me.collect_period.Text = "500"
        '
        'ms_label
        '
        Me.ms_label.AutoSize = True
        Me.ms_label.Font = New System.Drawing.Font("宋体", 10.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.ms_label.Location = New System.Drawing.Point(157, 192)
        Me.ms_label.Name = "ms_label"
        Me.ms_label.Size = New System.Drawing.Size(21, 14)
        Me.ms_label.TabIndex = 38
        Me.ms_label.Text = "ms"
        '
        'lux_l
        '
        Me.lux_l.AutoSize = True
        Me.lux_l.Font = New System.Drawing.Font("宋体", 10.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.lux_l.Location = New System.Drawing.Point(199, 220)
        Me.lux_l.Name = "lux_l"
        Me.lux_l.Size = New System.Drawing.Size(28, 14)
        Me.lux_l.TabIndex = 39
        Me.lux_l.Text = "lux"
        '
        'wendul
        '
        Me.wendul.AutoSize = True
        Me.wendul.Font = New System.Drawing.Font("宋体", 10.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.wendul.Location = New System.Drawing.Point(201, 251)
        Me.wendul.Name = "wendul"
        Me.wendul.Size = New System.Drawing.Size(21, 14)
        Me.wendul.TabIndex = 40
        Me.wendul.Text = "℃"
        '
        'pa_l
        '
        Me.pa_l.AutoSize = True
        Me.pa_l.Font = New System.Drawing.Font("宋体", 10.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.pa_l.Location = New System.Drawing.Point(199, 282)
        Me.pa_l.Name = "pa_l"
        Me.pa_l.Size = New System.Drawing.Size(28, 14)
        Me.pa_l.TabIndex = 41
        Me.pa_l.Text = "KPa"
        '
        'shidul
        '
        Me.shidul.AutoSize = True
        Me.shidul.Font = New System.Drawing.Font("宋体", 10.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.shidul.Location = New System.Drawing.Point(201, 308)
        Me.shidul.Name = "shidul"
        Me.shidul.Size = New System.Drawing.Size(14, 14)
        Me.shidul.TabIndex = 42
        Me.shidul.Text = "%"
        '
        'server_config
        '
        Me.server_config.Controls.Add(Me.L_LocalIP)
        Me.server_config.Controls.Add(Me.TB_LocalIP)
        Me.server_config.Controls.Add(Me.L_LocalPort)
        Me.server_config.Controls.Add(Me.TB_Port)
        Me.server_config.Controls.Add(Me.StartStop)
        Me.server_config.Controls.Add(Me.ClearText)
        Me.server_config.Location = New System.Drawing.Point(4, 9)
        Me.server_config.Name = "server_config"
        Me.server_config.Size = New System.Drawing.Size(229, 121)
        Me.server_config.TabIndex = 43
        Me.server_config.TabStop = False
        '
        'selsect_config
        '
        Me.selsect_config.Controls.Add(Me.strong_select)
        Me.selsect_config.Controls.Add(Me.endtime_text)
        Me.selsect_config.Controls.Add(Me.end_time_label)
        Me.selsect_config.Controls.Add(Me.CB_Client)
        Me.selsect_config.Controls.Add(Me.startime_text)
        Me.selsect_config.Controls.Add(Me.device_id)
        Me.selsect_config.Controls.Add(Me.device_id_label)
        Me.selsect_config.Controls.Add(Me.start_time_label)
        Me.selsect_config.Controls.Add(Me.shidul)
        Me.selsect_config.Controls.Add(Me.CearSendZone)
        Me.selsect_config.Controls.Add(Me.data_dimension)
        Me.selsect_config.Controls.Add(Me.SentButton)
        Me.selsect_config.Controls.Add(Me.humi_max_text)
        Me.selsect_config.Controls.Add(Me.pa_l)
        Me.selsect_config.Controls.Add(Me.humi_mid)
        Me.selsect_config.Controls.Add(Me.dimension_select)
        Me.selsect_config.Controls.Add(Me.humi_min_text)
        Me.selsect_config.Controls.Add(Me.press_max_text)
        Me.selsect_config.Controls.Add(Me.humi_lable)
        Me.selsect_config.Controls.Add(Me.wendul)
        Me.selsect_config.Controls.Add(Me.press_mid)
        Me.selsect_config.Controls.Add(Me.data_num_lable)
        Me.selsect_config.Controls.Add(Me.press_min_text)
        Me.selsect_config.Controls.Add(Me.lux_l)
        Me.selsect_config.Controls.Add(Me.press_lable)
        Me.selsect_config.Controls.Add(Me.data_num)
        Me.selsect_config.Controls.Add(Me.ms_label)
        Me.selsect_config.Controls.Add(Me.collect_period_label)
        Me.selsect_config.Controls.Add(Me.collect_period)
        Me.selsect_config.Controls.Add(Me.lux_lable)
        Me.selsect_config.Controls.Add(Me.lux_min_text)
        Me.selsect_config.Controls.Add(Me.lux_mid)
        Me.selsect_config.Controls.Add(Me.tmp_max_text)
        Me.selsect_config.Controls.Add(Me.lux_max_text)
        Me.selsect_config.Controls.Add(Me.tmp_mid)
        Me.selsect_config.Controls.Add(Me.tmp_label)
        Me.selsect_config.Controls.Add(Me.tmp_min_text)
        Me.selsect_config.Location = New System.Drawing.Point(4, 130)
        Me.selsect_config.Name = "selsect_config"
        Me.selsect_config.Size = New System.Drawing.Size(229, 370)
        Me.selsect_config.TabIndex = 44
        Me.selsect_config.TabStop = False
        '
        'strong_select
        '
        Me.strong_select.AutoSize = True
        Me.strong_select.Location = New System.Drawing.Point(7, 17)
        Me.strong_select.Name = "strong_select"
        Me.strong_select.Size = New System.Drawing.Size(53, 12)
        Me.strong_select.TabIndex = 48
        Me.strong_select.Text = "存储节点"
        '
        'endtime_text
        '
        Me.endtime_text.Location = New System.Drawing.Point(66, 65)
        Me.endtime_text.Name = "endtime_text"
        Me.endtime_text.Size = New System.Drawing.Size(156, 21)
        Me.endtime_text.TabIndex = 47
        Me.endtime_text.Text = "2020-02-26 12:12:56"
        '
        'end_time_label
        '
        Me.end_time_label.AutoSize = True
        Me.end_time_label.Font = New System.Drawing.Font("宋体", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.end_time_label.Location = New System.Drawing.Point(6, 68)
        Me.end_time_label.Name = "end_time_label"
        Me.end_time_label.Size = New System.Drawing.Size(53, 12)
        Me.end_time_label.TabIndex = 46
        Me.end_time_label.Text = "结束时间"
        '
        'startime_text
        '
        Me.startime_text.Location = New System.Drawing.Point(66, 38)
        Me.startime_text.Name = "startime_text"
        Me.startime_text.Size = New System.Drawing.Size(156, 21)
        Me.startime_text.TabIndex = 45
        Me.startime_text.Text = "2020-01-06 11:12:56"
        '
        'device_id
        '
        Me.device_id.Font = New System.Drawing.Font("宋体", 10.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.device_id.Location = New System.Drawing.Point(66, 93)
        Me.device_id.Name = "device_id"
        Me.device_id.Size = New System.Drawing.Size(156, 23)
        Me.device_id.TabIndex = 44
        Me.device_id.Text = "8f654a2d1182"
        '
        'device_id_label
        '
        Me.device_id_label.AutoSize = True
        Me.device_id_label.Font = New System.Drawing.Font("宋体", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.device_id_label.Location = New System.Drawing.Point(6, 96)
        Me.device_id_label.Name = "device_id_label"
        Me.device_id_label.Size = New System.Drawing.Size(41, 12)
        Me.device_id_label.TabIndex = 43
        Me.device_id_label.Text = "设备ID"
        '
        'RT_RevText
        '
        Me.RT_RevText.AutoWordSelection = True
        Me.RT_RevText.BackColor = System.Drawing.SystemColors.ControlLightLight
        Me.RT_RevText.Location = New System.Drawing.Point(239, 25)
        Me.RT_RevText.Name = "RT_RevText"
        Me.RT_RevText.ReadOnly = True
        Me.RT_RevText.Size = New System.Drawing.Size(555, 478)
        Me.RT_RevText.TabIndex = 49
        Me.RT_RevText.Text = ""
        '
        'Timer1
        '
        '
        'NetTestTool
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(804, 514)
        Me.Controls.Add(Me.RT_RevText)
        Me.Controls.Add(Me.selsect_config)
        Me.Controls.Add(Me.server_config)
        Me.Controls.Add(Me.L_RecZone)
        Me.MinimumSize = New System.Drawing.Size(800, 553)
        Me.Name = "NetTestTool"
        Me.Text = "两层无线传感网络多维隐私数据查询终端"
        Me.server_config.ResumeLayout(False)
        Me.server_config.PerformLayout()
        Me.selsect_config.ResumeLayout(False)
        Me.selsect_config.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents L_LocalIP As System.Windows.Forms.Label
    Friend WithEvents TB_LocalIP As System.Windows.Forms.TextBox
    Friend WithEvents L_LocalPort As System.Windows.Forms.Label
    Friend WithEvents TB_Port As System.Windows.Forms.TextBox
    Friend WithEvents ClearText As System.Windows.Forms.Button
    Friend WithEvents StartStop As System.Windows.Forms.Button
    Friend WithEvents SentButton As System.Windows.Forms.Button
    Friend WithEvents CearSendZone As System.Windows.Forms.Button
    Friend WithEvents L_RecZone As System.Windows.Forms.Label
    Friend WithEvents CB_Client As System.Windows.Forms.ComboBox
    Friend WithEvents start_time_label As System.Windows.Forms.Label
    Friend WithEvents data_dimension As System.Windows.Forms.Label
    Friend WithEvents dimension_select As System.Windows.Forms.ComboBox
    Friend WithEvents lux_lable As System.Windows.Forms.Label
    Friend WithEvents lux_min_text As System.Windows.Forms.TextBox
    Friend WithEvents lux_max_text As System.Windows.Forms.TextBox
    Friend WithEvents lux_mid As System.Windows.Forms.Label
    Friend WithEvents tmp_label As System.Windows.Forms.Label
    Friend WithEvents tmp_min_text As System.Windows.Forms.TextBox
    Friend WithEvents tmp_mid As System.Windows.Forms.Label
    Friend WithEvents tmp_max_text As System.Windows.Forms.TextBox
    Friend WithEvents humi_lable As System.Windows.Forms.Label
    Friend WithEvents humi_min_text As System.Windows.Forms.TextBox
    Friend WithEvents humi_mid As System.Windows.Forms.Label
    Friend WithEvents humi_max_text As System.Windows.Forms.TextBox
    Friend WithEvents press_lable As System.Windows.Forms.Label
    Friend WithEvents press_min_text As System.Windows.Forms.TextBox
    Friend WithEvents press_mid As System.Windows.Forms.Label
    Friend WithEvents press_max_text As System.Windows.Forms.TextBox
    Friend WithEvents data_num_lable As System.Windows.Forms.Label
    Friend WithEvents data_num As System.Windows.Forms.ComboBox
    Friend WithEvents collect_period_label As System.Windows.Forms.Label
    Friend WithEvents collect_period As System.Windows.Forms.TextBox
    Friend WithEvents ms_label As System.Windows.Forms.Label
    Friend WithEvents lux_l As System.Windows.Forms.Label
    Friend WithEvents wendul As System.Windows.Forms.Label
    Friend WithEvents pa_l As System.Windows.Forms.Label
    Friend WithEvents shidul As System.Windows.Forms.Label
    Friend WithEvents server_config As System.Windows.Forms.GroupBox
    Friend WithEvents selsect_config As System.Windows.Forms.GroupBox
    Friend WithEvents device_id As System.Windows.Forms.TextBox
    Friend WithEvents device_id_label As System.Windows.Forms.Label
    Friend WithEvents startime_text As System.Windows.Forms.TextBox
    Friend WithEvents endtime_text As System.Windows.Forms.TextBox
    Friend WithEvents end_time_label As System.Windows.Forms.Label
    Friend WithEvents strong_select As System.Windows.Forms.Label
    Friend WithEvents RT_RevText As System.Windows.Forms.RichTextBox
    Friend WithEvents Timer1 As System.Windows.Forms.Timer

End Class
