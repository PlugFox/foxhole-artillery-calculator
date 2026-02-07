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
using System.Threading.Tasks;
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
        private double targetDistance = 0;
        private double targetAzimuth = 0;
        // Количество введенных символов в текущее поле
        private int count = 0;
        // Состояние текущего ввода
        private Status CurrentStatus {
            get => _CurrentStatus;
            set
            {
                if (config.General.BeepOnFieldChange)
                {
                    Task.Run(() => Console.Beep(300, 200));
                }
                _CurrentStatus = value;
            }
        }
        private Status _CurrentStatus;
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
        // Конфигурация
        private AppConfiguration config;
        // Хоткеи из конфигурации
        private KeyboardHook.VKeys enemyCoordinatesHotkey;
        private KeyboardHook.VKeys friendlyCoordinatesHotkey;
        private KeyboardHook.VKeys screenshotHotkey;
        private KeyboardHook.VKeys changeResolutionHotkey;
        private KeyboardHook.VKeys toggleWindowHotkey;
        #endregion

        // Вход
        public MainWindow()
        {
            // Загрузим конфигурацию
            config = ConfigurationManager.Instance;
            
            // Загрузим хоткеи из конфигурации
            try
            {
                enemyCoordinatesHotkey = ConfigurationManager.ParseHotkey(config.Hotkeys.EnemyCoordinates);
                friendlyCoordinatesHotkey = ConfigurationManager.ParseHotkey(config.Hotkeys.FriendlyCoordinates);
                screenshotHotkey = ConfigurationManager.ParseHotkey(config.Hotkeys.Screenshot);
                changeResolutionHotkey = ConfigurationManager.ParseHotkey(config.Hotkeys.ChangeResolution);
                toggleWindowHotkey = ConfigurationManager.ParseHotkey(config.Hotkeys.ToggleWindow);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки хоткеев из конфигурации: {ex.Message}\n\nИспользуются значения по умолчанию.", "Ошибка конфигурации", MessageBoxButton.OK, MessageBoxImage.Warning);
                // Fallback to defaults
                enemyCoordinatesHotkey = KeyboardHook.VKeys.SUBTRACT;
                friendlyCoordinatesHotkey = KeyboardHook.VKeys.ADD;
                screenshotHotkey = KeyboardHook.VKeys.SNAPSHOT;
                changeResolutionHotkey = KeyboardHook.VKeys.MULTIPLY;
                toggleWindowHotkey = KeyboardHook.VKeys.DECIMAL;
            }

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
                System.Windows.Media.Color activeColor;
                try
                {
                    activeColor = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(config.UI.ActiveFieldColor);
                }
                catch
                {
                    activeColor = Colors.Lime;
                }
                
                enemyDistanceLBL.Background = new SolidColorBrush(Colors.Transparent);
                enemyAzimuthLBL.Background = new SolidColorBrush(Colors.Transparent);
                friendlyDistanceLBL.Background = new SolidColorBrush(Colors.Transparent);
                friendlyAzimuthLBL.Background = new SolidColorBrush(Colors.Transparent);
                switch (CurrentStatus)
                {
                    case Status.enemyDistance:
                        enemyDistanceLBL.Background = new SolidColorBrush(activeColor);
                        break;
                    case Status.enemyAzimuth:
                        enemyAzimuthLBL.Background = new SolidColorBrush(activeColor);
                        break;
                    case Status.friendlyDistance:
                        friendlyDistanceLBL.Background = new SolidColorBrush(activeColor);
                        break;
                    case Status.friendlyAzimuth:
                        friendlyAzimuthLBL.Background = new SolidColorBrush(activeColor);
                        break;
                    default:
                        break;
                }

                // Подкрашу иконки в зависимости от выставленной дальности
                System.Windows.Media.Color colorGreen;
                System.Windows.Media.Color colorRed;
                try
                {
                    colorGreen = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(config.UI.InRangeColor);
                    colorRed = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(config.UI.OutOfRangeColor);
                }
                catch
                {
                    colorGreen = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#7F00FF00");
                    colorRed = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#7FFF0000");
                }
                
                // Мортира
                if (config.Artillery.Mortar.MinRange < targetDistance && targetDistance < config.Artillery.Mortar.MaxRange)
                {
                    Mortar.Background = new SolidColorBrush(colorGreen);
                } else
                {
                    Mortar.Background = new SolidColorBrush(colorRed);
                }
                // Артилерия
                if (config.Artillery.FieldArtillery.MinRange < targetDistance && targetDistance < config.Artillery.FieldArtillery.MaxRange)
                {
                    FieldArtillery.Background = new SolidColorBrush(colorGreen);
                }
                else
                {
                    FieldArtillery.Background = new SolidColorBrush(colorRed);
                }
                // Хова с учетом разброса
                if (config.Artillery.Howitzer.MinRange < targetDistance && targetDistance < config.Artillery.Howitzer.MaxRange)
                {
                    Howitzer.Background = new SolidColorBrush(colorGreen);
                }
                else
                {
                    Howitzer.Background = new SolidColorBrush(colorRed);
                }
                // Ганбоат
                if (config.Artillery.Gunship.MinRange < targetDistance && targetDistance < config.Artillery.Gunship.MaxRange)
                {
                    Gunship.Background = new SolidColorBrush(colorGreen);
                }
                else
                {
                    Gunship.Background = new SolidColorBrush(colorRed);
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
        private void Sound_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) => Say((int)targetDistance, (int)targetAzimuth);
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

            // Если не заполнена дистанция до союзного орудия - предполагаем, что стоим перед артой.
            int _friendlyDistance;
            int _friendlyAzimuth;
            if (friendlyDistance == 0)
            {
                _friendlyDistance = 1;
                _friendlyAzimuth = (enemyAzimuth > 180) ? 180 + enemyAzimuth : 180 - enemyAzimuth;
            } else
            {
                _friendlyDistance = friendlyDistance;
                _friendlyAzimuth = friendlyAzimuth;
            }

            // Угол наводчика в радианах
            double AngleOfSpotter = DegToRad(Math.Abs(enemyAzimuth - _friendlyAzimuth));
            // Дистанция от союзников до врага
            targetDistance = Math.Sqrt(Math.Pow(x: enemyDistance, y: 2) + Math.Pow(x: _friendlyDistance, y: 2) - (2 * enemyDistance * _friendlyDistance * Math.Cos(d: AngleOfSpotter)));

            // Если азимуты не заполнены, не считаем азимут.
            if (enemyAzimuth == 0 && _friendlyAzimuth == 0)
            {
                targetAzimuth = 0;
                return;
            }

            // v1.1 begin
            // Угол союзников
            //double friendlyDegree = Math.Round(RadToDeg(Math.Asin((enemyDistance * Math.Sin(AngleOfSpotter)) / targetDistance)));
            // Азимут от союзников до врага
            //targetAzimuth = (int)Math.Round(enemyAzimuth > friendlyAzimuth ? (friendlyAzimuth + 180) - friendlyDegree : (friendlyAzimuth + 180) + friendlyDegree);
            //targetAzimuth = (targetAzimuth > 360) ? (targetAzimuth - 360) : (targetAzimuth < 0 ? targetAzimuth + 360 : targetAzimuth); /// TODO: Тут точно все верно, а то прилетал отрицательный азимут?
            // v1.1 end

            // v1.3 begin
            double step = RadToDeg(Math.Acos((Math.Pow(_friendlyDistance, 2) + Math.Pow(targetDistance, 2) - Math.Pow(enemyDistance, 2)) / (2 * _friendlyDistance * targetDistance)));

            if (ConvertAngle(RadToDeg(AngleOfSpotter)) > 180)
                targetAzimuth = (enemyAzimuth > _friendlyAzimuth) ? _friendlyAzimuth + 180 + step : _friendlyAzimuth + 180 - step;
            else
                targetAzimuth = (enemyAzimuth > _friendlyAzimuth) ? _friendlyAzimuth + 180 - step : _friendlyAzimuth + 180 + step;

            targetAzimuth = ConvertAngle(Math.Round(targetAzimuth, 0));
            targetDistance = Math.Round(targetDistance, 0);
            // v1.3 end
        }
        double DegToRad(int deg) => (Math.PI * deg) / 180;
        double RadToDeg(double rad) => (rad * 180) / Math.PI;
        double ConvertAngle(double deg) => ((deg >= 360) ? deg - 360 : deg);
        #endregion

        #region Чтение клавиатуры
        // Передадим нажатую клавишу клавиатуры
        private void KeyboardHook_KeyDown(KeyboardHook.VKeys VKey) => KeyPressedAsync(VKey);
        private async void KeyPressedAsync(KeyboardHook.VKeys VKey) => await Task.Run(() => KeyPressed(VKey));
        private void KeyPressed(KeyboardHook.VKeys VKey)
        {            
            // Если нажали на хоткей врага
            if (VKey == enemyCoordinatesHotkey)
            {
                count = 0;
                enemyAzimuth = 0;
                if (CurrentStatus == Status.enemyDistance)
                {
                    CurrentStatus = Status.enemyAzimuth;
                }
                else
                {
                    CurrentStatus = Status.enemyDistance;
                    enemyDistance = 0;
                }
                UpdateInterface();
                return;
            }
            
            // Если нажали на хоткей союзника
            if (VKey == friendlyCoordinatesHotkey)
            {
                count = 0;
                friendlyAzimuth = 0;
                if (CurrentStatus == Status.friendlyDistance)
                {
                    CurrentStatus = Status.friendlyAzimuth;
                }
                else
                {
                    CurrentStatus = Status.friendlyDistance;
                    friendlyDistance = 0;
                }
                UpdateInterface();
                return;
            }
            
            // Если нажали на хоткей скриншота
            if (VKey == screenshotHotkey)
            {
                CaptureScreen();
                return;
            }
            
            // Если нажали на хоткей смены разрешения
            if (VKey == changeResolutionHotkey)
            {
                ChangeResolution();
                return;
            }
            
            // Если нажали на хоткей сворачивания окна
            if (VKey == toggleWindowHotkey)
            {
                Button_Turn(null, null);
                return;
            }
            
            // Если текущий статус не активен, выходим
            if (CurrentStatus == Status.none)
                return;

            string value = VKey.ToString().ToLower();

            // Если нажали не на Num 0-9 
            // или текущий ввод не активен - прерываем любые вводы и завершаем процедуру
            if (value.Length != 7 || !regex.IsMatch(value.Substring(6, 1)))
            {
                count = 0;
                switch (CurrentStatus)
                {
                    case Status.enemyDistance:
                        CurrentStatus = Status.enemyAzimuth;
                        break;
                    case Status.friendlyDistance:
                        CurrentStatus = Status.friendlyAzimuth;
                        break;
                    default:
                        CurrentStatus = Status.none;
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
            switch (CurrentStatus)
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
                        CurrentStatus = Status.enemyAzimuth;
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
                        CurrentStatus = Status.none;
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
                        CurrentStatus = Status.friendlyAzimuth;
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
                        CurrentStatus = Status.none;
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
                int w = config.General.ScreenshotWidth;
                int h = config.General.ScreenshotHeight;
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
            if (sayDistance == 0 || !config.UI.EnableSound)
                return;
            Task.Run(() =>
            {
                try { 
                    // Initialize a new instance of the speech synthesizer.  
                    using (SpeechSynthesizer synth = new SpeechSynthesizer())
                    {
                        // Configure the synthesizer to send output to the default audio device.  
                        synth.SetOutputToDefaultAudioDevice();

                        try
                        {
                            synth.SelectVoiceByHints(VoiceGender.Neutral, VoiceAge.NotSet, 0, CultureInfo.GetCultureInfo(config.UI.VoiceCulture));
                        }
                        catch
                        {
                            // Fallback to en-US if configured culture is not available
                            synth.SelectVoiceByHints(VoiceGender.Neutral, VoiceAge.NotSet, 0, CultureInfo.GetCultureInfo("en-US"));
                        }

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
