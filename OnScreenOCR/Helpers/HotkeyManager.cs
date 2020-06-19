using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace OnScreenOCR.Helpers
{
    public class HotkeyManager
    {
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public event EventHandler HotKeyPressed;

        private const int HotkeyId = 9000;

        private HwndSource _source;
        private IntPtr _windowHandle;

        private uint _vkKey;


        public void CreateHotkey(Modifiers modifier, uint vkKey)
        {
            _vkKey = vkKey;
            _windowHandle = new WindowInteropHelper(Application.Current.MainWindow ?? throw new InvalidOperationException("Main window couldn't be found")).Handle;
            
            _source = HwndSource.FromHwnd(_windowHandle);
            _source?.AddHook(HwndHook);
            
            RegisterHotKey(_windowHandle, HotkeyId, (uint) modifier, _vkKey);
        }

        public void DeleteHotkeys()
        {
            _source.RemoveHook(HwndHook);
            UnregisterHotKey(_windowHandle, HotkeyId);
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int wmHotkey = 0x0312;
            switch (msg)
            {
                case wmHotkey:
                    switch (wParam.ToInt32())
                    {
                        case HotkeyId:
                            var vkey = (((int)lParam >> 16) & 0xFFFF);
                            if (vkey == _vkKey)
                            {
                                OnHotKeyPressed();
                            }
                            handled = true;
                            break;
                    }
                    break;
            }
            return IntPtr.Zero;
        }


        protected virtual void OnHotKeyPressed()
        {
            HotKeyPressed?.Invoke(this, EventArgs.Empty);
        }
    }



    [Flags]
    public enum Modifiers
    {
        ModNone = 0x0000,
        ModAlt = 0x0001,
        ModControl = 0x0002, 
        ModShift = 0x0004, 
        ModWindows = 0x0008 
    }
}
