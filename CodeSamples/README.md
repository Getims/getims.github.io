# Code Samples

Здесь собраны исходные коды трёх Unity-проектов на C# и отдельный пример веб-игры на TypeScript. Три Unity-проекта построены на общем архитектурном каркасе, который я переиспользую от проекта к проекту: инфраструктура общая, отличается в основном геймплей. 

DungeonJourney — отдельный эксперимент с разработкой веб-игры и AI-assisted workflow.

**Стек Unity-проектов:** Unity, C#, Zenject, DOTween, Odin Inspector, Lean Pool, TextMeshPro.

**Стек DungeonJourney:** TypeScript, HTML5 Canvas 2D, DOM/CSS, Web Audio API, Playwright.

| Проект | Что смотреть | Играбельная версия |
|---|---|---|
| [ArmyClash Architecture](ArmyClash%20Architecture/) | каркас в «чистом» виде + система юнитов | — |
| [MergePuzzle](MergePuzzle/) | генерация и логика пазла, debug-модуль, UI-эффекты | [играть](https://getims.github.io/MergePuzzle/) |
| [JumpingCar](JumpingCar/) | гиперкежуал-аркада: фабрики, пулы, системы движения | [играть](https://getims.github.io/JumpingCar/) |
| [DungeonJourney AI Sample](DungeonJourney%20AI%20Sample/) | веб-игра для Poki (TypeScript): симуляция боя по хукам, гибридный Canvas/DOM UI, процедурный звук | [играть](https://getims.github.io/DungeonJourney/) |

---

## Общий каркас (во всех трёх проектах)

| Папка | Что внутри |
|---|---|
| `Infrastructure/StateMachines` | Типизированная машина состояний: `Enter<TState>()`, `Enter<TState, TPayload>()`, интерфейсы `IEnterState` / `IExitState` / `ITickableState`, фабрика состояний |
| `Runtime/Scenes` | Бутстрап: `GameCoreInstaller` / `GameSceneInstaller` (Zenject), состояния загрузки сцен и игровые состояния (Prepare → GamePlay → GameOver → Reset) |
| `Infrastructure/Events` | Типобезопасная шина событий `GameEventProvider`: подписка по типу события, события с параметром и без |
| `Infrastructure/Data`, `Services/SaveLoad` | Слой данных: `ADataService`, `GameDatabase`, сохранение через JSON или PlayerPrefs за общим интерфейсом `ISaveLoadService` |
| `Infrastructure/Data/Experimental` | `DataValue<TData, TValue>` – обёртка над полем сохранения: валидация, автосохранение, уведомление об изменении |
| `Infrastructure/Configs`, `Configs` | Конфиги на ScriptableObject с категориями (`ConfigCategoryAttribute`) и единым `ConfigsProvider` |
| `Editor/ConfigsEditor`, `Editor/DataEditor` | Редакторы на Odin: окно со всеми конфигами проекта (дерево, поиск, превью), редактор сохранений |
| `UI/Common` | Базовые классы UI: панели, попапы, контролы, счётчики, анимации кнопок |
| `Infrastructure/Utilities` | Утилиты: задержки, рандом и шаффл, форматирование денег, Unix-время, логирование |

**Принципы:** DI вместо синглтонов, зависимости через интерфейсы, геймплей не знает о UI (связь через события), данные и конфиги отделены от логики.

---

## ArmyClash Architecture

Авто-баттлер «команда против команды». Каркас в наиболее «чистом» виде – с него удобнее всего начинать просмотр.

- `Gameplay/Units/Systems` – юниты разделены на системы:
  - **глобальные** (`UnitsSystem`, `TargetSystem`, `MovementSystem`, `BattleSystem`) – обходят всех юнитов каждый кадр;
  - **локальные** (`UnitMoveSystem`, `UnitMoveSystemAI`, `UnitAttackSystem`, `UnitHealthSystem`) – поведение конкретного юнита; движение игрока и ИИ взаимозаменяемы через `IUnitMoveSystem`.
- `Gameplay/Factory` – фабрика юнитов с зонами спавна и пулом объектов.
- `Configs/Gameplay` – параметры юнитов (форма, размер, цвет, статы) собираются из отдельных конфигов.
- `Gameplay/Debug` – гизмо зон спавна и отладка юнитов.

## MergePuzzle

Sliding puzzle с «склеиванием» правильно поставленных частей.

- `Gameplay/Puzzle` – генерация пазла (`PuzzleGenerator`), сетка (`PuzzleGrid`), объединение частей в группы (`PuzzleGroupManager`), перемешивание (`PuzzleShuffler`).
- `Gameplay/Levels` – элементы уровня и сетка с пулами, подсказки.
- `Modules/DebugModule` – чит-консоль для тестировщиков: уровень, деньги, подсказки, FPS, сброс прогресса; читы добавляются реализацией `ICheat`.
- `Modules/UIAnimator` – последовательности UI-анимаций из простых шагов (move, scale, fade, rotate).
- `UI/Common/FlyIcons` – «летящие» иконки наград с пулом и настраиваемыми последовательностями.
- `UI/MainMenu` – меню с коллекциями, выбором уровня и навигацией.

## JumpingCar

Аркада: машинка прыгает по платформам и уворачивается от встречных машин.

- `Gameplay/Cars`, `Gameplay/Platforms`, `Gameplay/Background` – фабрики и пулы, отдельные системы движения для машин, платформ и фона, гравитация игрока (`GravitySystem`).
- `Runtime/UIAnimator` – тот же модуль UI-анимаций, что и в MergePuzzle.
- `Configs` – машины, платформы, фон, аудио.

---

## DungeonJourney AI Sample

Веб-игра для Poki.com на чистом TypeScript, без игровых движков. Auto-battler roguelike с мета-прогрессией, мерджем экипировки и сундуками. Разработка велась с использованием AI-агентов поэтапно, на основе формализованных правил, технического задания и архитектурного плана.

- **Гибридный рендер и игровое ядро (`Game/core/`)** – логическая сцена 1920×1080 с адаптивным letterbox; Canvas 2D для боя и частиц, DOM/CSS для меню, карт и инвентаря. Собственный игровой цикл с `timeScale`, машина экранов и типизированная шина событий (`events.ts`).
- **Симуляция боя и система хуков (`Game/battle/`)** – headless-симуляция боя с фиксированным seed генерирует лог событий, который воспроизводится презентером (`presenter.ts`) с управлением таймингами и ускорением ×3. Статы, перки и способности врагов используют единый data-driven механизм хуков (`on_before_hit`, `on_crit`, `on_kill` и др.).
- **Мета-прогрессия и сохранения (`Game/meta.ts`, `save.ts`)** – 7 слотов экипировки, крафт и объединение предметов 3→1, разбор на осколки, сундуки за ключи, продолжение прогресса после смерти. Версионируемые сохранения в `localStorage` с автобэкапом.
- **Процедурный звук и визуальные эффекты (`Game/synth.ts`, `fx.ts`)** – генерация звуков в реальном времени через Web Audio API без аудиоассетов; компрессор на мастере и защита от повторного воспроизведения. Пул на 400 частиц с программной отрисовкой спрайтов.
- **Сборка и тестирование (`Tools/`, `Config/`)** – кастомный бандлер собирает игру в единый HTML-файл с контролем размера (`build.mjs`); сквозные автотесты на Playwright (`test.mjs`), dev-панель и headless-прогон игрового баланса (`simRun`).
- **AI development workflow (`Docs/`, `rules/`, `Modules/`)** – разработка по этапам на основе GDD и технического плана; набор общих правил для AI-агентов, повторно используемые модули и инструкции по работе с проектом. Перед реализацией крупных систем агент предлагает варианты, а изменения выполняются в рамках текущего этапа.

---

Код приведён без ассетов и сцен, только скрипты – чтобы показать архитектуру и стиль.
