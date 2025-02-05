Public Class TimePanel
    Inherits Panel

    ' TimePanel controls
    Public lblHour As DigitLED
    Public lblMinute As DigitLED
    Public lblColon As Label
    Public rbtnAM As RadioButton
    Public rbtnPM As RadioButton
    Public btnSet As Button
    Public btnReset As Button
    Public KeypadPanel As Panel
    Public btnOk As Button
    Public btnCancel As Button
    Public chkAlarm1 As CheckBox
    Public chkAlarm2 As CheckBox
    Public chkAlarm3 As CheckBox
    Public picAnimation As PictureBox
    Private digitButtons(9) As Button ' Array for digits
    Private originalTime As DateTime ' Store original time 
    Public manualTime As DateTime ' Store manually set time
    Public isManualTimeSet As Boolean = False ' Track if time is being set manually
    Private isSettingInProgress As Boolean = False ' Track if time is being set by the user
    Private enteredTime As String = "" ' Store entered time
    Private currentInputField As String = "" ' To track hour or minute input
    Private clockTimer As Timer
    Private alarmClockForm As AlarmClockForm

    ' Alarm times in string format
    Public alarmTime1 As String = ""
    Public alarmTime2 As String = ""
    Public alarmTime3 As String = ""



    ' Method to set the clockTimer manually from the AlarmClockForm
    Public Sub SetClockTimer(timer As Timer)
        alarmClockForm.clockTimer = timer
    End Sub


    ' Layout
    Public Sub New(alarmForm As AlarmClockForm)

        ' Store the reference to the main form
        alarmClockForm = alarmForm

        Me.Size = New Drawing.Size(500, 800)

        ' Initialize labels for hour and minute 
        lblHour = New DigitLED With {
            .MaxDigit = 12,
            .Size = New Size(45, 30),
            .Location = New Point(45, 15),
            .Font = New Font("Arial", 20, FontStyle.Bold)
        }
        lblMinute = New DigitLED With {
            .MaxDigit = 59,
            .Size = New Size(50, 30),
            .Location = New Point(103, 15),
            .Font = New Font("Arial", 20, FontStyle.Bold)
        }

        ' Initialize colon
        lblColon = New Label With {
            .Text = ":",
            .Font = New Font("Arial", 20, FontStyle.Bold),
            .Size = New Size(50, 30),
            .Location = New Point(85, 15)
        }

        ' Initialize AM/PM buttons
        rbtnAM = New RadioButton With {
            .Text = "AM",
            .Location = New Point(170, 40),
            .AutoSize = True
        }
        rbtnPM = New RadioButton With {
            .Text = "PM",
            .Location = New Point(170, 60),
            .AutoSize = True
        }

        ' Initialize Set and Reset buttons
        btnSet = New Button With {
            .Text = "Set",
            .Location = New Point(38, 50),
            .Size = New Drawing.Size(60, 25)
        }
        btnReset = New Button With {
            .Text = "Reset",
            .Location = New Point(98, 50),
            .Size = New Drawing.Size(60, 25)
        }

        ' Initialize PictureBox for animation
        picAnimation = New PictureBox With {
            .Location = New Point(10, 90),
            .Size = New Drawing.Size(95, 88),
            .Image = My.Resources.Train1,
            .SizeMode = PictureBoxSizeMode.StretchImage,
            .Enabled = False ' Disable it initially
        }


        ' Initialize checkboxes for the alarms
        chkAlarm1 = New CheckBox With {
            .Text = "Alarm 1 On",
            .Location = New Point(120, 100),
            .AutoSize = True
        }
        chkAlarm2 = New CheckBox With {
            .Text = "Alarm 2 On",
            .Location = New Point(120, 130),
            .AutoSize = True
        }
        chkAlarm3 = New CheckBox With {
            .Text = "Alarm 3 On",
            .Location = New Point(120, 160),
            .AutoSize = True
        }

        ' Initialize keypad panel
        KeypadPanel = New Panel With {
            .Size = New Drawing.Size(200, 200),
            .Location = New Point(25, 200),
            .Visible = False ' Hide initially
        }

        ' Add digit buttons (0-9) in a 3x4 grid layout
        Dim buttonSize As New Size(50, 30)
        For i As Integer = 1 To 9
            digitButtons(i) = New Button With {
                .Text = i.ToString(),
                .Size = buttonSize,
                .Location = New Point(((i - 1) Mod 3) * 50, ((i - 1) \ 3) * 40)
            }
            AddHandler digitButtons(i).Click, AddressOf DigitButton_Click
            KeypadPanel.Controls.Add(digitButtons(i))
        Next

        ' Add 0, Cancel, and OK buttons
        digitButtons(0) = New Button With {
            .Text = "0",
            .Size = buttonSize,
            .Location = New Point(50, 120) ' Centered
        }
        AddHandler digitButtons(0).Click, AddressOf DigitButton_Click
        KeypadPanel.Controls.Add(digitButtons(0))

        btnCancel = New Button With {
            .Text = "Cancel",
            .Size = buttonSize,
            .Location = New Point(0, 120) ' Leftmost button
        }
        AddHandler btnCancel.Click, AddressOf CancelButton_Click
        KeypadPanel.Controls.Add(btnCancel)

        btnOk = New Button With {
            .Text = "OK",
            .Size = buttonSize,
            .Location = New Point(100, 120) ' Rightmost button
        }
        AddHandler btnOk.Click, AddressOf OkButton_Click
        KeypadPanel.Controls.Add(btnOk)


        ' Add controls to the TimePanel
        Me.Controls.Add(lblHour)
        Me.Controls.Add(lblMinute)
        Me.Controls.Add(lblColon)
        Me.Controls.Add(rbtnAM)
        Me.Controls.Add(rbtnPM)
        Me.Controls.Add(btnSet)
        Me.Controls.Add(btnReset)
        Me.Controls.Add(KeypadPanel)
        Me.Controls.Add(chkAlarm1)
        Me.Controls.Add(chkAlarm2)
        Me.Controls.Add(chkAlarm3)
        Me.Controls.Add(picAnimation)

        ' Functionality for Set button
        AddHandler btnSet.Click, AddressOf SetButton_Click

        ' Functionality for Reset button
        AddHandler btnReset.Click, AddressOf ResetButton_Click


        ' Initialize and start  clock timer
        clockTimer = New Timer()
        AddHandler clockTimer.Tick, AddressOf UpdateClockTime
        clockTimer.Interval = 60000
        clockTimer.Start()

        ' Immediately update clock when the panel is created
        UpdateClockTime(Nothing, Nothing)
    End Sub



    ' Update the clock every second and check alarms
    Private Sub UpdateClockTime(sender As Object, e As EventArgs)
        If Not isSettingInProgress Then
            If isManualTimeSet Then
                manualTime = manualTime.AddMinutes(1)
                lblHour.Text = manualTime.ToString("hh")
                lblMinute.Text = manualTime.ToString("mm")
                If manualTime.Hour >= 12 Then
                    rbtnPM.Checked = True
                Else
                    rbtnAM.Checked = True
                End If
            Else
                Dim currentTime As DateTime = DateTime.Now
                lblHour.Text = currentTime.ToString("hh")
                lblMinute.Text = currentTime.ToString("mm")
                If currentTime.Hour >= 12 Then
                    rbtnPM.Checked = True
                Else
                    rbtnAM.Checked = True
                End If
            End If
        End If
    End Sub


    ' Set time 
    Private Sub SetButton_Click(sender As Object, e As EventArgs)
        originalTime = If(isManualTimeSet, manualTime, DateTime.Now)

        ' Check if alarmClockForm is not null before stopping the timer
        If alarmClockForm IsNot Nothing Then
            alarmClockForm.clockTimer.Stop()
        Else
            MessageBox.Show("AlarmClockForm reference is null.")
        End If

        isSettingInProgress = True
        lblHour.Text = "00"
        lblMinute.Text = "00"
        currentInputField = "Hour"
        enteredTime = ""
        EnableValidKeysForHour(True)
        KeypadPanel.Visible = True
    End Sub



    ' Reset the time
    Private Sub ResetButton_Click(sender As Object, e As EventArgs)
        isManualTimeSet = False
        isSettingInProgress = False

        Dim currentTime As DateTime = DateTime.Now
        lblHour.Text = currentTime.ToString("hh")
        lblMinute.Text = currentTime.ToString("mm")
        If currentTime.Hour >= 12 Then
            rbtnPM.Checked = True
        Else
            rbtnAM.Checked = True
        End If
    End Sub


    ' Handle digit button clicks
    Private Sub DigitButton_Click(sender As Object, e As EventArgs)
        Dim button As Button = DirectCast(sender, Button)
        Dim digit As String = button.Text

        If currentInputField = "Hour" Then
            If enteredTime.Length = 0 Then
                ' Set first hour digit
                enteredTime &= digit
                lblHour.Text = digit & "0"
                If digit = "1" Then
                    EnableValidKeysForHour(False) ' Enable 0-2 for second hour digit
                ElseIf digit = "0" Then
                    EnableValidKeysForHour(False) ' Enable 1-9 for second hour digit if first digit is 0
                Else
                    EnableValidKeysForMinute()
                    currentInputField = "Minute"
                End If
            Else
                ' Set second hour digit
                enteredTime &= digit
                lblHour.Text = enteredTime
                EnableValidKeysForMinute()
                currentInputField = "Minute"
                enteredTime = ""
            End If
        ElseIf currentInputField = "Minute" Then
            If enteredTime.Length = 0 Then
                ' Set first minute digit
                enteredTime &= digit
                lblMinute.Text = digit & "0"
                EnableValidKeysForMinuteSecondDigit() ' Enable valid keys for the second minute digit
            Else
                ' Set second minute digit
                enteredTime &= digit
                lblMinute.Text = enteredTime
                DisableAllDigitButtons() ' Disable all digit buttons after second minute digit is set
            End If
        End If
    End Sub


    ' Disable all digit buttons after four digits are entered
    Private Sub DisableAllDigitButtons()
        For i As Integer = 0 To 9
            digitButtons(i).Enabled = False
        Next
        btnOk.Enabled = True
        btnCancel.Enabled = True
    End Sub


    ' Enable valid keys for the first hour digit (1 or 0)
    Private Sub EnableValidKeysForHour(isFirstDigit As Boolean)
        ' Disable all keys
        For i As Integer = 0 To 9
            digitButtons(i).Enabled = False
        Next

        If isFirstDigit Then
            ' Enable only 1 and 0 for the first digit of hour
            digitButtons(1).Enabled = True
            digitButtons(0).Enabled = True
        Else
            ' If first digit is 1, enable 0-2 for second hour digit
            If lblHour.Text.StartsWith("1") Then
                digitButtons(0).Enabled = True
                digitButtons(1).Enabled = True
                digitButtons(2).Enabled = True
            ElseIf lblHour.Text.StartsWith("0") Then
                ' If first digit is 0, enable only 1-9 for second hour digit
                For i As Integer = 1 To 9
                    digitButtons(i).Enabled = True
                Next
                digitButtons(0).Enabled = False
            End If
        End If
    End Sub


    ' Enable valid keys for the first minute digit (0-5)
    Private Sub EnableValidKeysForMinute()
        For i As Integer = 0 To 9
            digitButtons(i).Enabled = False
        Next
        For i As Integer = 0 To 5
            digitButtons(i).Enabled = True
        Next
    End Sub


    ' Enable valid keys for the second minute digit (0-9)
    Private Sub EnableValidKeysForMinuteSecondDigit()
        For i As Integer = 0 To 9
            digitButtons(i).Enabled = True
        Next
    End Sub


    ' OK button 
    Private Sub OkButton_Click(sender As Object, e As EventArgs)
        Dim newHour As Integer = Integer.Parse(lblHour.Text)
        Dim newMinute As Integer = Integer.Parse(lblMinute.Text)

        ' Handle AM/PM conversion
        If rbtnPM.Checked AndAlso newHour < 12 Then
            newHour += 12
        ElseIf rbtnAM.Checked AndAlso newHour = 12 Then
            newHour = 0
        End If

        ' Set the manualTime to the new time
        manualTime = New DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, newHour, newMinute, 0)

        isManualTimeSet = True
        isSettingInProgress = False
        KeypadPanel.Visible = False
        alarmClockForm.clockTimer.Start()
    End Sub


    ' Cancel button logic
    Private Sub CancelButton_Click(sender As Object, e As EventArgs)
        ' Restore the original time that was saved before clicking "Set"
        lblHour.Text = originalTime.ToString("hh")
        lblMinute.Text = originalTime.ToString("mm")
        If originalTime.Hour >= 12 Then
            rbtnPM.Checked = True
        Else
            rbtnAM.Checked = True
        End If

        KeypadPanel.Visible = False
        isSettingInProgress = False
        alarmClockForm.clockTimer.Start()
    End Sub

End Class

