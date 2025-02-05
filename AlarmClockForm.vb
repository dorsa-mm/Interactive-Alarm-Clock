Public Class AlarmClockForm

    ' Declare controls
    Private clockTab As TabControl
    Private clockPanel As TimePanel
    Private alarm1Panel As AlarmPanel
    Private alarm2Panel As AlarmPanel
    Private alarm3Panel As AlarmPanel
    Public clockTimer As Timer


    ' Form initialization
    Public Sub New()
        ' Initialize components
        Me.InitializeComponent()

        ' Set up the form properties
        Me.Text = "Alarm Clock - Dorsa Malekahmadi"
        Me.Size = New Drawing.Size(450, 600)

        ' Initialize tab control
        clockTab = New TabControl With {
            .Dock = DockStyle.Fill,
            .Size = New Drawing.Size(450, 600)
        }


        ' Initialize and start the clock timer
        clockTimer = New Timer()
        clockTimer.Interval = 1000 ' Update every second
        AddHandler clockTimer.Tick, AddressOf UpdateClock
        clockTimer.Start()


        ' Initialize clock panel and set size
        clockPanel = New TimePanel(Me) With {
            .Size = New Drawing.Size(350, 450)
        }

        ' Initialize alarm panels and set size
        alarm1Panel = New AlarmPanel(1) With {
            .Size = New Drawing.Size(350, 450)
        }
        alarm2Panel = New AlarmPanel(2) With {
            .Size = New Drawing.Size(350, 450)
        }
        alarm3Panel = New AlarmPanel(3) With {
            .Size = New Drawing.Size(350, 450)
        }

        ' Create and set up TabPages for clock and alarms
        Dim clockTabPage As New TabPage("Clock")
        clockTabPage.Controls.Add(clockPanel)

        Dim alarm1TabPage As New TabPage("Alarm 1")
        alarm1TabPage.Controls.Add(alarm1Panel)

        Dim alarm2TabPage As New TabPage("Alarm 2")
        alarm2TabPage.Controls.Add(alarm2Panel)

        Dim alarm3TabPage As New TabPage("Alarm 3")
        alarm3TabPage.Controls.Add(alarm3Panel)

        ' Add the TabPages to the TabControl
        clockTab.TabPages.Add(clockTabPage)
        clockTab.TabPages.Add(alarm1TabPage)
        clockTab.TabPages.Add(alarm2TabPage)
        clockTab.TabPages.Add(alarm3TabPage)

        ' Add controls to form
        Me.Controls.Add(clockTab)

        ' Set initial time
        UpdateClock(Nothing, Nothing)

        ' Sync the checkboxes between the clock and alarm panels
        AddHandler clockPanel.chkAlarm1.CheckedChanged, AddressOf SyncAlarm1Checkbox
        AddHandler clockPanel.chkAlarm2.CheckedChanged, AddressOf SyncAlarm2Checkbox
        AddHandler clockPanel.chkAlarm3.CheckedChanged, AddressOf SyncAlarm3Checkbox

        ' Sync from AlarmPanel to TimePanel
        AddHandler alarm1Panel.chkAlarmOn.CheckedChanged, AddressOf SyncAlarm1FromAlarmPanel
        AddHandler alarm2Panel.chkAlarmOn.CheckedChanged, AddressOf SyncAlarm2FromAlarmPanel
        AddHandler alarm3Panel.chkAlarmOn.CheckedChanged, AddressOf SyncAlarm3FromAlarmPanel
    End Sub


    ' Method to update the clock every second and check for alarms
    Private Sub UpdateClock(sender As Object, e As EventArgs)
        If clockPanel.isManualTimeSet Then
            ' If manual time is set, continue updating based on manual time
            clockPanel.manualTime = clockPanel.manualTime.AddSeconds(1)
            clockPanel.lblHour.Text = clockPanel.manualTime.ToString("hh")
            clockPanel.lblMinute.Text = clockPanel.manualTime.ToString("mm")
            If clockPanel.manualTime.Hour >= 12 Then
                clockPanel.rbtnPM.Checked = True
            Else
                clockPanel.rbtnAM.Checked = True
            End If
        Else
            ' If manual time is not set, use system's current time
            Dim currentTime As DateTime = DateTime.Now
            clockPanel.lblHour.Text = currentTime.ToString("hh")
            clockPanel.lblMinute.Text = currentTime.ToString("mm")
            If currentTime.Hour >= 12 Then
                clockPanel.rbtnPM.Checked = True
            Else
                clockPanel.rbtnAM.Checked = True
            End If
        End If

        ' Check each alarm to see if it should ring
        CheckAlarm(alarm1Panel, 1)
        CheckAlarm(alarm2Panel, 2)
        CheckAlarm(alarm3Panel, 3)
    End Sub



    ' Method to check if the alarm should ring
    Private Sub CheckAlarm(alarmPanel As AlarmPanel, alarmIndex As Integer)
        If alarmPanel.chkAlarmOn.Checked AndAlso Not alarmPanel.isAlarmHandled Then
            Dim alarmTime As String = alarmPanel.lblHour.Text & ":" & alarmPanel.lblMinute.Text & If(alarmPanel.rbtnAM.Checked, " AM", " PM")
            Dim currentTime As String = DateTime.Now.ToString("hh:mm tt")
            If alarmTime = currentTime Then
                clockTab.SelectedIndex = alarmIndex
                alarmPanel.StartAlarm()
            End If
        End If
    End Sub


    ' Synchronize the state of Alarms' checkboxes from ClockPanel to AlarmPanel
    Private Sub SyncAlarm1Checkbox(sender As Object, e As EventArgs)
        alarm1Panel.chkAlarmOn.Checked = clockPanel.chkAlarm1.Checked
    End Sub

    Private Sub SyncAlarm2Checkbox(sender As Object, e As EventArgs)
        alarm2Panel.chkAlarmOn.Checked = clockPanel.chkAlarm2.Checked
    End Sub

    Private Sub SyncAlarm3Checkbox(sender As Object, e As EventArgs)
        alarm3Panel.chkAlarmOn.Checked = clockPanel.chkAlarm3.Checked
    End Sub

    ' Reverse sync the state of Alarms' checkboxes from AlarmPanel to ClockPanel
    Private Sub SyncAlarm1FromAlarmPanel(sender As Object, e As EventArgs)
        clockPanel.chkAlarm1.Checked = alarm1Panel.chkAlarmOn.Checked
    End Sub

    Private Sub SyncAlarm2FromAlarmPanel(sender As Object, e As EventArgs)
        clockPanel.chkAlarm2.Checked = alarm2Panel.chkAlarmOn.Checked
    End Sub

    Private Sub SyncAlarm3FromAlarmPanel(sender As Object, e As EventArgs)
        clockPanel.chkAlarm3.Checked = alarm3Panel.chkAlarmOn.Checked
    End Sub
End Class
