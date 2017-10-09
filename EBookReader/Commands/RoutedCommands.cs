using System.Windows.Input;

namespace EBookReader.Commands
{
    public static class RoutedCommands
    {
        public static readonly RoutedUICommand FileOpen = new RoutedUICommand(
            "FileOpen",
            "FileOpen",
            typeof(RoutedCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.O, ModifierKeys.Control)
            }
        );

        public static readonly RoutedUICommand Mark = new RoutedUICommand(
            "Mark",
            "Mark",
            typeof(RoutedCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.M, ModifierKeys.Control)
            }
        );

        public static readonly RoutedUICommand Unmark = new RoutedUICommand(
            "Unmark",
            "Unmark",
            typeof(RoutedCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.U, ModifierKeys.Control)
            }
        );

        public static readonly RoutedUICommand NightMode = new RoutedUICommand(
            "NightMode",
            "NightMode",
            typeof(RoutedCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.F11)
            }
        );

        public static readonly RoutedUICommand FullScreen = new RoutedUICommand(
            "FullScreen",
            "FullScreen",
            typeof(RoutedCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.F12)
            }
        );

        public static readonly RoutedUICommand ShowBooks = new RoutedUICommand(
            "ShowBooks",
            "ShowBooks",
            typeof(RoutedCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.B, ModifierKeys.Control)
            }
        );
    }
}