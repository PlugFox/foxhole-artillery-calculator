Foxhole
---
# Калькулятор артиллериста

![](screenshots/5.png)

## Управление / Controls

### Хоткеи по умолчанию / Default Hotkeys

```
Num -
 Инициация ввода координат противника  
 
Num + 
 Инициация ввода координат союзника  
 
Num 0..9 
 Ввод координат  
 
Num * 
 Ресайз окна  
 
Num . 
 Свернуть/развернуть калькулятор  
 
PrtScr 
 Скриншот координат бинокля   
```

+ Квадратики справа нужны чтоб сохранять координаты точек опорных позиций.  
+ При наведении на орудие - выскакивает подсказка о его дальности.  
+ Ввод координат выполняется последовательно, с помощью нампада (сначало дистанция, затем азимут).  
+ Если дистанция или угол не трехзначные, может быть удобно вводить с лидирующими нулями, но можно и нажать любую кнопку после ввода.
+ Также, если дистанция начинается с цифры большей 1 или азимут с цифры больше 3 - будет инициирован двухзначный ввод.
+ При нажатии на результирующие координаты - будет произведена попытка их озвучки синтетическим движком в en-US.   
+ Если на данный момент кальк не нужен - можно "свернуть" в полосочку по стрелочкам в шапочке, чтоб не мешался.  
+ Подсветка возможности ведения ответного огня  
+ В шапке есть кнопка для открытия дополнительной полезной информации.

---

## Конфигурация / Configuration

С версии 2.0 калькулятор поддерживает настройку через конфигурационный файл `config.yaml`. При первом запуске файл создается автоматически со значениями по умолчанию.

**Расположение файла конфигурации / Configuration file location:**
- Windows: `%APPDATA%\FoxholeArtilleryCalculator\config.yaml`
- Полный путь отображается при ошибке загрузки конфигурации

### Настройка хоткеев / Hotkey Configuration

В файле `config.yaml` можно изменить хоткеи для различных действий:

```yaml
hotkeys:
  enemy_coordinates: SUBTRACT      # Ввод координат противника (по умолчанию Num -)
  friendly_coordinates: ADD        # Ввод координат союзника (по умолчанию Num +)
  screenshot: SNAPSHOT             # Снимок экрана (по умолчанию PrtScr)
  change_resolution: MULTIPLY      # Смена разрешения (по умолчанию Num *)
  toggle_window: DECIMAL           # Свернуть/развернуть (по умолчанию Num .)
```

**Доступные клавиши / Available keys:**
- Клавиши Numpad: `NUMPAD0-NUMPAD9`, `ADD`, `SUBTRACT`, `MULTIPLY`, `DIVIDE`, `DECIMAL`
- Функциональные клавиши: `F1-F24`
- Специальные: `SNAPSHOT` (PrtScr), `INSERT`, `DELETE`, `HOME`, `END`, `PRIOR` (PageUp), `NEXT` (PageDown)
- Буквы: `KEY_A` через `KEY_Z`
- Цифры: `KEY_0` через `KEY_9`
- Стрелки: `UP`, `DOWN`, `LEFT`, `RIGHT`
- Другие: `SPACE`, `RETURN` (Enter), `ESCAPE`, `TAB`, `BACK` (Backspace)

Полный список доступных клавиш смотрите в файле `classes/KeyboardHook.cs`

### Настройка интерфейса / UI Configuration

```yaml
ui:
  active_field_color: Lime         # Цвет активного поля ввода
  in_range_color: "#7F00FF00"      # Цвет для артиллерии в радиусе действия
  out_of_range_color: "#7FFF0000"  # Цвет для артиллерии вне радиуса
  enable_sound: true               # Включить озвучку координат
  voice_culture: en-US             # Язык озвучки (en-US, ru-RU и др.)
```

### Настройка дальности артиллерии / Artillery Range Configuration

```yaml
artillery:
  mortar:
    min_range: 44
    max_range: 66
  field_artillery:
    min_range: 74
    max_range: 151
  howitzer:
    min_range: 59
    max_range: 166
  gunship:
    min_range: 49
    max_range: 101
```

### Общие настройки / General Settings

```yaml
general:
  beep_on_field_change: false      # Звуковой сигнал при смене поля ввода
  screenshot_width: 100            # Ширина снимка экрана в пикселях
  screenshot_height: 50            # Высота снимка экрана в пикселях
```

---

## Создание релиза / Creating a Release

Для создания нового релиза используйте GitHub Actions:

1. Перейдите во вкладку **Actions** в репозитории
2. Выберите workflow **"Create Release"**
3. Нажмите **"Run workflow"**
4. Введите версию в формате `X.Y.Z` (например, `1.0.0`)
5. Workflow автоматически:
   - Создаст git тэг `vX.Y.Z`
   - Обновит версию в `AssemblyInfo.cs`
   - Соберёт Windows executable
   - Создаст GitHub release с исполняемым файлом

To create a new release using GitHub Actions:

1. Go to the **Actions** tab in the repository
2. Select the **"Create Release"** workflow
3. Click **"Run workflow"**
4. Enter version in format `X.Y.Z` (e.g., `1.0.0`)
5. The workflow will automatically:
   - Create git tag `vX.Y.Z`
   - Update version in `AssemblyInfo.cs`
   - Build Windows executable
   - Create GitHub release with the executable file

---

## CI/CD / Непрерывная интеграция

Репозиторий использует автоматические проверки кода при каждом Pull Request и коммите в main:

### Linux Code Checks (быстрая проверка)
- Проверка синтаксиса YAML файлов
- Валидация структуры файлов проекта
- Проверка базового синтаксиса C#
- Проверка конфигурации NuGet пакетов
- Проверка документации

### Windows Build Verification (проверка компиляции)
- Сборка Debug и Release конфигураций
- Проверка NuGet зависимостей
- Проверка выходных файлов (exe, dll)
- Артефакты сборки доступны в Actions

The repository uses automated code checks for every Pull Request and commit to main:

### Linux Code Checks (fast validation)
- YAML syntax validation
- Project file structure verification
- Basic C# syntax checking
- NuGet packages configuration check
- Documentation validation

### Windows Build Verification (compilation check)
- Build Debug and Release configurations
- Verify NuGet dependencies
- Check build output files (exe, dll)
- Build artifacts available in Actions