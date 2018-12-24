using System;
using System.Windows;
using System.Windows.Media;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Speech.Synthesis;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Threading;
using System.Globalization;
using foxhole_artillery_calculator.classes;
using foxhole_artillery_calculator.screens;

namespace foxhole_artillery_calculator
{
    /// <summary>
    ///  
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Переменные и константы
        // Координаты
        private int enemyDistance = 0;
        private int enemyAzimuth = 0;
        private int friendlyDistance = 0;
        private int friendlyAzimuth = 0;
        private int targetDistance = 0;
        private int targetAzimuth = 0;
        // Количество введенных символов в текущее поле
        private int count = 0;
        // Состояние текущего ввода
        private Status currentStatus {
            get => _currentStatus;
            set
            {
                //Task.Run(() => Console.Beep(300, 200));
                _currentStatus = value;
            }
        }
        private Status _currentStatus;
        private enum Status {
            none,
            enemyDistance,
            enemyAzimuth,
            friendlyDistance,
            friendlyAzimuth
        };
        // Регулярка - только цифры
        private static readonly Regex regex = new Regex(@"^\d$");
        // Создадим клавиатурный хук
        KeyboardHook keyboardHook = new KeyboardHook();
        // Перемещение окна
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
        // Текущий режим окна
        private Mode currentMode;
        private enum Mode
        {
            FullScreen,
            Square
        };
        private static int screenWidth = (int)SystemParameters.PrimaryScreenWidth;
        private static int screenHeight = (int)SystemParameters.PrimaryScreenHeight;
        // Передний план
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetForegroundWindow(IntPtr hWnd);
        #endregion

        // Вход
        public MainWindow()
        {
            // Отлавливаем события клавиатурным хуком
            keyboardHook.KeyDown += new KeyboardHook.KeyboardHookCallback(KeyboardHook_KeyDown);
            // Установим клавиатурный хук
            keyboardHook.Install();

            // Инициализация
            InitializeComponent();

            // Обновим интерфейс
            UpdateInterface();
        }

        #region Интерфейс
        // Обновить интерфейс
        void UpdateInterface()
        {
            this.Dispatcher.Invoke(delegate
            {
                // Установлю значения меток из переменных
                enemyDistanceLBL.Content = enemyDistance;
                enemyAzimuthLBL.Content = enemyAzimuth;
                friendlyDistanceLBL.Content = friendlyDistance;
                friendlyAzimuthLBL.Content = friendlyAzimuth;

                // Посчитаю координаты цели
                Calculate();
                targetDistanceLBL.Content = targetDistance;
                targetAzimuthLBL.Content = targetAzimuth;

                // Установлю цвет полей в зависимости от текущего статуса
                enemyDistanceLBL.Background = new SolidColorBrush(Colors.Transparent);
                enemyAzimuthLBL.Background = new SolidColorBrush(Colors.Transparent);
                friendlyDistanceLBL.Background = new SolidColorBrush(Colors.Transparent);
                friendlyAzimuthLBL.Background = new SolidColorBrush(Colors.Transparent);
                switch (currentStatus)
                {
                    case Status.enemyDistance:
                        enemyDistanceLBL.Background = new SolidColorBrush(Colors.Lime);
                        break;
                    case Status.enemyAzimuth:
                        enemyAzimuthLBL.Background = new SolidColorBrush(Colors.Lime);
                        break;
                    case Status.friendlyDistance:
                        friendlyDistanceLBL.Background = new SolidColorBrush(Colors.Lime);
                        break;
                    case Status.friendlyAzimuth:
                        friendlyAzimuthLBL.Background = new SolidColorBrush(Colors.Lime);
                        break;
                    default:
                        break;
                }
            });
            return;
        }

        // Нажатие кнопки таблиц
        private void Button_Data(object sender, RoutedEventArgs e) => (new Data()).ShowDialog();

        // Нажатие кнопки информации о программе
        private void Button_Info(object sender, RoutedEventArgs e) => (new Info()).ShowDialog();

        // Нажатие кнопки свернуть/развернуть
        private void Button_Turn(object sender, RoutedEventArgs e) => Dispatcher.Invoke(() => body.Visibility = body.IsVisible ? Visibility.Collapsed : Visibility.Visible);

        // Нажатие кнопки выход
        private void Button_Exit(object sender, RoutedEventArgs e) => System.Windows.Application.Current.Shutdown();

        // Нажатие на скриншот убирает его
        private void Screenshot_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) => Screenshot.Source = null;

        // Сохранение координат
        private void Label_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e == null)
                return;
            Task.Run(() =>
            {
                ContentControl s = ((ContentControl)sender);
                Dispatcher.Invoke(() => s.Content = (targetDistance != 0 || targetAzimuth != 0) ? ("" + targetDistance + Environment.NewLine + targetAzimuth) : ("") );
            });
        }
        // Нажатие кнопки озвучки координат
        private void Sound_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) => Say(targetDistance, targetAzimuth);
        #endregion

        #region Считаю координаты цели
        // Расчет координат цели
        void Calculate()
        {
            // Если дистанции не заполнены - не считаем.
            if (enemyDistance == 0 && friendlyDistance == 0)
            {
                targetDistance = 0;
                targetAzimuth = 0;
                return;
            }

            // Угол наводчика в радианах
            double AngleOfSpotter = DegToRad(Math.Abs(enemyAzimuth - friendlyAzimuth));
            // Дистанция от союзников до врага
            targetDistance = (int)Math.Round(Math.Sqrt(Math.Pow(x: enemyDistance, y: 2) + Math.Pow(x: friendlyDistance, y: 2) - (2 * enemyDistance * friendlyDistance * Math.Cos(d: AngleOfSpotter))));

            // Если азимуты не заполнены, не считаем азимут.
            if (enemyAzimuth == 0 && friendlyAzimuth == 0)
            {
                targetAzimuth = 0;
                return;
            }
            // Угол союзников
            double friendlyDegree = Math.Round(RadToDeg(Math.Asin((enemyDistance * Math.Sin(AngleOfSpotter)) / targetDistance)));
            // Азимут от союзников до врага
            targetAzimuth = (int)Math.Round(enemyAzimuth > friendlyAzimuth ? (friendlyAzimuth + 180) - friendlyDegree : (friendlyAzimuth + 180) + friendlyDegree);
            targetAzimuth = (targetAzimuth > 360) ? (targetAzimuth - 360) : (targetAzimuth < 0 ? targetAzimuth + 360 : targetAzimuth); /// TODO: Тут точно все верно, а то прилетал отрицательный азимут?
        }
        double DegToRad(int deg) => (Math.PI * deg) / 180;
        double RadToDeg(double rad) => (rad * 180) / Math.PI;
        #endregion

        #region Чтение клавиатуры
        // Передадим нажатую клавишу клавиатуры
        private void KeyboardHook_KeyDown(KeyboardHook.VKeys VKey) => KeyPressedAsync(VKey);
        private async void KeyPressedAsync(KeyboardHook.VKeys VKey) => await Task.Run(() => KeyPressed(VKey));
        private void KeyPressed(KeyboardHook.VKeys VKey)
        {            
            switch (VKey)
            {
                // Если нажали на минус и хотим установить позицию врага
                case KeyboardHook.VKeys.SUBTRACT:
                    count = 0;
                    enemyAzimuth = 0;
                    if (currentStatus == Status.enemyDistance)
                    {
                        currentStatus = Status.enemyAzimuth;
                    }
                    else
                    {
                        currentStatus = Status.enemyDistance;
                        enemyDistance = 0;
                    }
                    UpdateInterface();
                    return;
                // Если нажали на плюс и хотим установить позицию союзника
                case KeyboardHook.VKeys.ADD:
                    count = 0;
                    friendlyAzimuth = 0;
                    if (currentStatus == Status.friendlyDistance)
                    {
                        currentStatus = Status.friendlyAzimuth;
                    }
                    else
                    {
                        currentStatus = Status.friendlyDistance;
                        friendlyDistance = 0;
                    }
                    UpdateInterface();
                    return;
                // Если нажали на принтскрин и заскринили текущие координаты
                case KeyboardHook.VKeys.SNAPSHOT:
                    CaptureScreen();
                    return;
                // Если нажали на умножение - меняем разрешение окна фоксхола на квадратное
                case KeyboardHook.VKeys.MULTIPLY:
                    ChangeResolution();
                    return;
                // Если нажали на точку - свернуть/развернуть окно
                case KeyboardHook.VKeys.DECIMAL:
                    Button_Turn(null, null);
                    return;
                default:
                    if (currentStatus == Status.none)
                        return;
                    break;
            }

            string value = VKey.ToString().ToLower();

            // Если нажали не на Num -, Num +, PrtScr и не на Num 0-9 
            // или текущий ввод не активен - прерываем любые вводы и завершаем процедуру
            if (value.Length != 7 || !regex.IsMatch(value.Substring(6, 1)))
            {
                count = 0;
                switch (currentStatus)
                {
                    case Status.enemyDistance:
                        currentStatus = Status.enemyAzimuth;
                        break;
                    case Status.friendlyDistance:
                        currentStatus = Status.friendlyAzimuth;
                        break;
                    default:
                        currentStatus = Status.none;
                        break;
                }
                UpdateInterface();
                return;
            }

            // Если нажали Num 0 - 9
            if (Int32.TryParse(value.Substring(6, 1), out int digit))
            {
                AddDigit(digit);
                UpdateInterface();
            }
            return;
        }

        // Добавим цифру к текущему полю
        void AddDigit(int digit)
        {
            count++;
            switch (currentStatus)
            {
                case Status.enemyDistance:
                    enemyDistance *= 10;
                    enemyDistance += digit;
                    // Если уже ввели 3 цифры в текущее поле 
                    // (таким образом учитываем и лидирующие нули)
                    // Или если ввели 2 цифры и лидирующая цифра больше 1
                    if (count == 3 || (count == 2 && enemyDistance > 9 && Math.Truncate((double)enemyDistance/10) > 1))
                    {
                        count = 0;
                        currentStatus = Status.enemyAzimuth;
                    }
                    break;
                case Status.enemyAzimuth:
                    enemyAzimuth *= 10;
                    enemyAzimuth += digit;
                    // Если уже ввели 3 цифры в текущее поле 
                    // (таким образом учитываем и лидирующие нули)
                    // Или если ввели 2 цифры и лидирующая цифра больше 3
                    if (count == 3 || (count == 2 && enemyAzimuth > 9 && Math.Truncate((double)enemyAzimuth / 10) > 3))
                    {
                        count = 0;
                        currentStatus = Status.none;
                    }
                    break;
                case Status.friendlyDistance:
                    friendlyDistance *= 10;
                    friendlyDistance += digit;
                    // Если уже ввели 3 цифры в текущее поле 
                    // (таким образом учитываем и лидирующие нули)
                    // Или если ввели 2 цифры и лидирующая цифра больше 1
                    if (count == 3 || (count == 2 && friendlyDistance > 9 && Math.Truncate((double)friendlyDistance / 10) > 1))
                    {
                        count = 0;
                        currentStatus = Status.friendlyAzimuth;
                    }
                    break;
                case Status.friendlyAzimuth:
                    friendlyAzimuth *= 10;
                    friendlyAzimuth += digit;
                    // Если уже ввели 3 цифры в текущее поле 
                    // (таким образом учитываем и лидирующие нули)
                    // Или если ввели 2 цифры и лидирующая цифра больше 3
                    if (count == 3 || (count == 2 && friendlyAzimuth > 9 && Math.Truncate((double)friendlyAzimuth / 10) > 3))
                    {
                        count = 0;
                        currentStatus = Status.none;
                    }
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Сделать скриншот
        private void CaptureScreen()
        {
            Task.Run(() =>
            {
                int w = 100;
                int h = 50;
                var pos = System.Windows.Forms.Cursor.Position;
                Bitmap result = new Bitmap(w, h);

                try
                {
                    using (Graphics g = Graphics.FromImage(result))
                    {
                        Rectangle rectangle = new Rectangle(0, 0, w, h);
                        g.CopyFromScreen(pos.X-w/2, pos.Y+10, 0, 0, rectangle.Size, CopyPixelOperation.SourceCopy);
                    }
                }
                catch
                {
                    return;
                }

                this.Dispatcher.Invoke(delegate
                {
                    using (MemoryStream memory = new MemoryStream())
                    {
                        result.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                        memory.Position = 0;
                        BitmapImage bitmapimage = new BitmapImage();
                        bitmapimage.BeginInit();
                        bitmapimage.StreamSource = memory;
                        bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapimage.EndInit();

                        Screenshot.Source = bitmapimage;
                    }
                });
            });
        }
        #endregion

        #region Сменить разрешение экрана
        void ChangeResolution()
        {
            Task.Run(() =>
            {
                Process[] processes = Process.GetProcesses();
                foreach (var process in processes)
                {
                    if (!process.ProcessName.StartsWith("War") || process.MainWindowHandle == IntPtr.Zero)
                        continue;
                    IntPtr handle = process.MainWindowHandle;

                    int X;
                    int Y;
                    int nWidth;
                    int nHeight;

                    switch (currentMode)
                    {
                        case Mode.Square:
                            currentMode = Mode.FullScreen;
                            nWidth = screenWidth;
                            nHeight = screenHeight;
                            X = 0;
                            Y = 0;
                            break;
                        case Mode.FullScreen:
                        default:
                            currentMode = Mode.Square;
                            nWidth = screenHeight * 4 / 3;
                            nHeight = screenHeight;
                            X = (screenWidth - screenHeight * 4 / 3) / 2;
                            Y = 0;
                            break;
                    }
                    MoveWindow(hWnd: handle, X: X, Y: Y, nWidth: nWidth, nHeight: nHeight, bRepaint: true);
                    SetForegroundWindow(handle);
                }
            });
        }
        #endregion

        #region Синтез речи
        void Say(int sayDistance, int sayAzimuth = 0)
        {
            if (sayDistance == 0)
                return;
            Task.Run(() =>
            {
                try { 
                    // Initialize a new instance of the speech synthesizer.  
                    using (SpeechSynthesizer synth = new SpeechSynthesizer())
                    {
                        // Configure the synthesizer to send output to the default audio device.  
                        synth.SetOutputToDefaultAudioDevice();

                        synth.SelectVoiceByHints(VoiceGender.Neutral, VoiceAge.NotSet, 0, CultureInfo.GetCultureInfo("en-US"));

                        // Speak a phrase.  
                        synth.Speak(String.Format("Distance: {0}, azimuth: {1}", sayDistance, sayAzimuth));
                    }
                } finally
                {

                }
            });
        }
        #endregion
    
    }
}
