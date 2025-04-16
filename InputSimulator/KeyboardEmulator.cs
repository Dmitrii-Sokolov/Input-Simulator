using System;
using System.Runtime.InteropServices;

namespace InputSimulator
{
  public static class KeyboardEmulator
  {
    private enum InputType : uint
    {
      InputMouse = 0,
      InputKeyboard = 1,
      InputHardware = 2
    }

    private enum KeyEvent : uint
    {
      None = 0,
      ExtendedKey = 0x0001,
      KeyUp = 0x0002,
      Unicode = 0x0004,
      Scancode = 0x0008
    }

    [StructLayout(LayoutKind.Sequential)]
    struct Input
    {
      public InputType type;
      public InputUnion union;
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct InputUnion
    {
      [FieldOffset(0)] public MouseInput mi;
      [FieldOffset(0)] public KeyboardInput ki;
      [FieldOffset(0)] public HardwareInput hi;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MouseInput
    {
      public int dx;
      public int dy;
      public uint mouseData;
      public uint dwFlags;
      public uint time;
      public IntPtr dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct KeyboardInput
    {
      public ushort virtualKeyCode;
      public ushort hardwareScanCode;
      public KeyEvent dwFlags;
      public uint time;
      public IntPtr dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct HardwareInput
    {
      public uint uMsg;
      public ushort wParamL;
      public ushort wParamH;
    }

    [DllImport("user32.dll", SetLastError = true)]
    static extern uint SendInput(uint nInputs, Input[] pInputs, int cbSize);

    public static void SendKey(ushort keyCoode)
    {
      var keyDown = CreateKeyInput(keyCoode, KeyEvent.None);
      var keyUp = CreateKeyInput(keyCoode, KeyEvent.KeyUp);
      var inputs = new[] { keyDown, keyUp };
      int size = Marshal.SizeOf(typeof(Input));
      SendInput((uint)inputs.Length, inputs, size);
    }

    private static Input CreateKeyInput(ushort keyCoode, KeyEvent keyEvent) => new Input
    {
      type = InputType.InputKeyboard,
      union = CreateKeyboardInput(keyCoode, keyEvent)
    };

    private static InputUnion CreateKeyboardInput(ushort keyCoode, KeyEvent keyEvent) => new InputUnion
    {
      ki = new KeyboardInput
      {
        virtualKeyCode = keyCoode,
        dwFlags = keyEvent
      }
    };
  }
}