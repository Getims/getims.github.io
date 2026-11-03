# Code Samples

Исходный код трёх Unity-проектов (C#). Все три построены на одном архитектурном каркасе, который я переиспользую от проекта к проекту: инфраструктура общая, отличается только геймплей.

**Стек:** Unity, C#, Zenject, DOTween, Odin Inspector, Lean Pool, TextMeshPro.

| Проект | Что смотреть | Играбельная версия |
|---|---|---|
| [ArmyClash Architecture](ArmyClash%20Architecture/) | каркас в «чистом» виде + система юнитов | — |
| [MergePuzzle](MergePuzzle/) | генерация и логика пазла, debug-модуль, UI-эффекты | [играть](https://getims.github.io/MergePuzzle/) |
| [JumpingCar](JumpingCar/) | гиперкежуал-аркада: фабрики, пулы, системы движения | [играть](https://getims.github.io/JumpingCar/) |

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

Код приведён без ассетов и сцен, только скрипты – чтобы показать архитектуру и стиль.
