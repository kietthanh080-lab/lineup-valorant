using System;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Windows.Interop;

namespace ValoLineupViewer.Services
{
    public class HotkeyManager : IDisposable
    {
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public const uint MOD_NONE = 0x0000;
        public const uint MOD_ALT = 0x0001;
        public const uint MOD_CONTROL = 0x0002;
        public const uint MOD_SHIFT = 0x0004;
        public const uint MOD_WIN = 0x0008;

        public const int WM_HOTKEY = 0x0312;

        private IntPtr _hWnd;
        private HwndSource? _source;
        private int _currentId = 9000;
        private bool _isDisposed = false;

        public event Action? OnToggleHotkeyPressed;

        public void Initialize(IntPtr windowHandle)
        {
            _hWnd = windowHandle;
            _source = HwndSource.FromHwnd(_hWnd);
            _source?.AddHook(HwndHook);

            // Đăng ký phím tắt theo yêu cầu: Phím H để Ẩn/Hiện HUD
            RegisterKey(MOD_NONE, (uint)KeyInterop.VirtualKeyFromKey(Key.H));
            
            // Phím dự phòng: Alt + H
            RegisterKey(MOD_ALT, (uint)KeyInterop.VirtualKeyFromKey(Key.H));
        }

        private void RegisterKey(uint modifier, uint virtualKey)
        {
            if (_hWnd != IntPtr.Zero)
            {
                _currentId++;
                RegisterHotKey(_hWnd, _currentId, modifier, virtualKey);
            }
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_HOTKEY)
            {
                OnToggleHotkeyPressed?.Invoke();
                handled = true;
            }
            return IntPtr.Zero;
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                if (_source != null)
                {
                    _source.RemoveHook(HwndHook);
                    _source = null;
                }

                for (int id = 9001; id <= _currentId; id++)
                {
                    UnregisterHotKey(_hWnd, id);
                }

                _isDisposed = true;
            }
        }
    }
}
