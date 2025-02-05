# Alarm Clock Application

## Overview
This project is an interactive Visual Basic **Alarm Clock application**. The alarm clock includes a digital time display and supports three independent alarms. The user can interact with the application through a **keypad interface**, allowing them to set and reset alarms. The project incorporates sound effects and animations to indicate when an alarm goes off.

## Features
### User Interface
- The application consists of **four tabs**:
  1. **Clock Tab** - Displays the current system time.
  2. **Alarm 1 Tab** - Allows the user to set Alarm 1.
  3. **Alarm 2 Tab** - Allows the user to set Alarm 2.
  4. **Alarm 3 Tab** - Allows the user to set Alarm 3.
- Each alarm panel contains:
  - A **time display** in AM/PM format.
  - A **checkbox** to enable or disable the alarm.
  - A **dropdown list** to select a sound effect.
  - A **visual animation** that activates when the alarm rings.

### Clock and Alarm Functionality
- The **clock panel** automatically syncs with the system time.
- The user can **set an alarm** by pressing the "Set" button, which opens a **numeric keypad**.
- The user can **cancel a new time entry**, which restores the previous time setting.
- A **reset button** returns an alarm to its default state (**00:00 AM**).
- The alarms will **trigger the selected sound effect** and an animation at the designated time.
- Clicking on the alarm's animation **stops the alarm sound**.

### Keypad Input System
- The **keypad only accepts valid time inputs**.
- The application **disables invalid buttons dynamically** as the user enters digits.
- Time is entered **digit by digit from left to right**.
- The keypad disappears once the user either confirms or cancels their input.

### Synchronization Features
- The **main clock panel includes checkboxes** to enable or disable alarms across all tabs.
- These checkboxes are **synchronized** with the individual alarm panels, meaning that enabling or disabling an alarm from the clock panel will reflect in the corresponding alarm tab.

## Implementation Details
This project is implemented using **Visual Basic (.NET)** in **Visual Studio**. 

### Object-Oriented Programming (OOP) Approach
The project is structured using **custom classes**:
1. **DigitLED Class**
   - A custom control derived from the **Label** control.
   - Each digit of the clock and alarms is an instance of this class.
   - Includes a `MaxDigit` property to restrict digit values based on position.
2. **TimePanel Class**
   - Represents the time display and control panel for both the clock and alarms.
   - Contains four `DigitLED` instances, a colon (`:`), AM/PM radio buttons, and **Set** and **Reset** buttons.
   - Allows users to modify time using the keypad.
3. **AlarmPanel Class**
   - Used to create each of the three alarm tabs.
   - Includes a `TimePanel`, a dropdown list for selecting alarm sounds, and a **checkbox** for enabling or disabling the alarm.
   - The alarm’s animation is handled within this class.

### Technical Components
- **`TabControl`**: Used for switching between the clock and alarm panels.
- **`Timer`**: Updates the clock display every minute and checks if any alarms need to ring.
- **`Panel` for Keypad**: The keypad is contained within a panel and is dynamically shown or hidden.
- **`Array of Buttons` for Keypad Input**: Instead of individually handling events for each button, an array is used to apply a common event handler.
- **`Event Handling for Alarms`**:
  - The application monitors all three alarms.
  - If an alarm is enabled and the current time matches the alarm time, the alarm sound and animation trigger.
  - The user can stop the alarm by clicking the animated button.
