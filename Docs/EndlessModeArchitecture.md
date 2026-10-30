# Бесконечный режим: устройство кода

Схема кода, который добавил этап 2 плана «Бесконечный уровень» — каркас режима. Пути — от `Assets/_Project/Develop/`,
имя файла совпадает с именем класса. Боссов, множителя здоровья, погоды, окна режима и рекорда здесь ещё нет; куда они
пристроятся — в разделе 7.

Термины:
- **Волна** — то, что в коде называется стейджем (`EnemiesWavesStageConfig`, `EnemiesWavesStage`). HUD пишет «Волна N».
- **Поток** — `EnemiesWaveConfig` внутри волны: одна дорога, интервал между врагами, группы врагов. У волны бесконечного
  режима по потоку на каждую дорогу уровня.

## 1. Запуск: от карточки меню до сцены боя

```text
  ГЛАВНОЕ МЕНЮ · Runtime/UI/Meta/MainMenu/Levels/
┌──────────────────────────────────────────┐
│ PlayBoardPopupView                       │ ◄── щелчок по карточке «Бесконечный»
│   _endlessCard.Clicked → EndlessClicked  │
├──────────────────────────────────────────┤
│ PlayBoardPopupPresenter                  │
│   OnEndlessClicked → переход в Gameplay  │
└────────────────────┬─────────────────────┘
                     │ GameplayInputArgs.ForEndless()
                     ▼
  СЦЕНА GAMEPLAY · Runtime/Gameplay/Infrastructure/
┌──────────────────────────────────────────┐
│ GameplayInputArgs                        │   ForCampaign(N) — уровень N кампании
│   LevelKind  (GameplayLevelKinds)        │   ForEndless()   — бесконечный уровень
│   IsEndless                              │   «Заново» передаёт тот же объект
└────────────────────┬─────────────────────┘
                     ▼
             регистрации — раздел 2
```

## 2. Регистрации: чем бесконечный уровень отличается от кампании

Отличий всего три — откуда взять `LevelConfig`, источник волн и списки для подготовки уровня. Всё остальное в сцене
общее и не знает, какой уровень идёт.

```text
  GameplayContextRegistrations.Process · Runtime/Gameplay/Infrastructure/

  GetLevelConfig:   кампания     → LevelsListConfig.GetBy(N)
                    бесконечный  → EndlessModeConfig.Level  (EndlessLevel.asset)

                            ┌─────── IsEndless? ───────┐
                            │ да                       │ нет
                            ▼                          ▼
┌────────────────────────────────────┐  ┌────────────────────────────────────┐
│ EndlessContextRegistrations        │  │ кампания — прямо в                 │
│                                    │  │ GameplayContextRegistrations       │
│ EndlessWavesGenerator              │  │                                    │
│ EndlessStagesSource                │  │ FixedStagesSource                  │
│   он же IStagesSource              │  │   он же IStagesSource              │
│ LevelStagesContent:                │  │ LevelStagesContent.CollectFrom:    │
│   враги всех тиров,                │  │   враги и дороги                   │
│   все дороги уровня                │  │   из стейджей уровня               │
└─────────────────┬──────────────────┘  └─────────────────┬──────────────────┘
                  └──────────────────┬────────────────────┘
                                     ▼
┌────────────────────────────────────────────────────────────────────────────┐
│ общие регистрации                                                          │
│   StageProviderService    ← IStagesSource                                  │
│   EnemiesPrewarmer        ← LevelStagesContent.Enemies     (прогрев пула)  │
│   RoutesPreviewService    ← LevelStagesContent.PathNumbers (стрелки дорог) │
│   GameModesFactory        → режим по LevelConfig.GameMode: Basic / Infinity│
└────────────────────────────────────────────────────────────────────────────┘
```

## 3. Бой: кто кого вызывает

```text
GameplayBootstrap.Update
└─ GameplayCycle.Update
   └─ IGameMode.Update                                     Runtime/Gameplay/GameMode/
      │    GameModeBase — общее: окно старта, поражение по здоровью замка, остановка в конце
      │    ├─ BasicGameMode    + условие победы (кампания)
      │    └─ EndlessGameMode    победы нет
      │
      └─ StagesCycle.Update                                Runtime/Gameplay/Features/LevelStages/
         └─ StageProviderService ── Has(N), Get(N) ──► IStagesSource
            │                                          ├─ FixedStagesSource    список стейджей уровня
            │                                          └─ EndlessStagesSource  генератор + кеш (раздел 5)
            │ конфиг волны N
            ▼
            EnemiesWavesStage — таймер StageTime; окно вызова — когда прошло TimeToSkipStage
            │ на старте волны, каждый поток
            ▼
            WavesSpawner.SpawnWave(поток) ─► EnemiesFactory ─► враг выходит на дорогу
```

Конец игры — общий для обоих режимов (исправление бага из пункта доски `results`):

```text
здоровье замка ≤ 0
└─ GameModeBase.ProcessEndGame
   ├─ StagesCycle.Stop()    окно вызова закрывается, таймер волны гаснет — следующая волна не начнётся
   ├─ WavesSpawner.Stop()   недоспавненные потоки останавливаются
   └─ End(Defeat) ─► GameplayCycle ─► музыка и окно поражения
```

## 4. Генератор волн

`EndlessWavesGenerator` (`Runtime/Gameplay/Features/EndlessMode/`) превращает номер волны в обычный
`EnemiesWavesStageConfig` — тот же конфиг, что у стейджей кампании, поэтому дальше его играет существующий конвейер.
Состояния забега у генератора нет: всё берётся из конфига и номера волны. Отсюда два следствия — волна N всегда одна и
та же, и тот же генератор строит таблицу волн в инспекторе.

### Что он читает

```text
  Runtime/Configs/Gameplay/Levels/Endless/

  EndlessModeConfig                 ассет Resources/Configs/Gameplay/Levels/Endless/EndlessModeConfig.asset
  ├─ Level                LevelConfig уровня режима (EndlessLevel.asset)
  ├─ Seed                 зерно случайности
  ├─ EnemiesCount         WaveValueTable — число врагов волны
  ├─ Tiers[]              EndlessTier — в порядке выхода на дорогу: слабые, средние, сильные
  │  ├─ Name
  │  ├─ Enemies[]         EndlessTierEnemy: EnemyConfig + FirstWave (с какой волны враг открыт)
  │  ├─ Share             WaveValueTable — доля тира в волне
  │  └─ KindsPerWave      сколько разных видов тира в одной волне
  ├─ DelayBetweenSpawns   WaveValueTable — интервал между врагами одной дороги
  └─ RestTime             WaveValueTable — передышка после выхода последнего врага

  + номера дорог уровня (RoadPath.Number у префаба) — у EndlessLevel это First и Second
```

### Шаги — на примере волны 12

Числа — из текущего чернового конфига; их же показывает таблица волн в инспекторе.

```text
EndlessWavesGenerator.Generate(12)
│
├─ 0. Случайность волны
│     new System.Random(Seed × 100003 + N)
│     у каждой волны своё зерно, поэтому её состав не зависит от того, какие волны собирались раньше
│
├─ 1. Числа волны из таблиц — WaveValueTable.GetValue(12)
│     EnemiesCount        20,8 → округление → 21 враг
│     DelayBetweenSpawns  1,2 с
│     RestTime            12 с
│
├─ 2. ComposeGroups — состав по тирам
│  │
│  ├─ a. GetOpenEnemies — враги тира, у которых FirstWave ≤ 12
│  │     слабые: 4 из 4 · средние: 5 из 5 · сильные: 3 из 3
│  │
│  ├─ b. доли тиров — Share.GetValue(12)
│  │     0,55 · 0,32 · 0,13
│  │     у тира, где ещё никто не открыт, доля 0 — она сама расходится по остальным пропорционально
│  │
│  ├─ c. DistributeByLargestRemainder — делёж 21 врага так, чтобы сумма сошлась
│  │     квоты 21 × доля / сумма долей   11,55 · 6,72 · 2,73
│  │     целые части                     11 · 6 · 2        вместе 19, осталось 2
│  │     по одному — большим остаткам    сильным (0,73), средним (0,72)
│  │     итог                            11 · 7 · 3
│  │
│  └─ d. AddTierGroups — внутри тира KindsPerWave = 2 случайных вида из открытых,
│        враги тира между ними поровну, лишний — первому по жребию
│        слабые 11    Skeleton 6, HumanRogue 5
│        средние 7    SkeletonRogue 4, Orc 3
│        сильные 3    SkeletonMage 1, Barbarian 2
│
├─ 3. SplitByPaths — каждая группа поровну на все дороги; лишние раздаются по очереди,
│     и очередь идёт сквозь все группы волны, чтобы лишние не копились на первой дороге
│     (таблица ниже)
│
└─ 4. Сборка EnemiesWavesStageConfig
      на каждую дорогу — поток EnemiesWaveConfig: группы в порядке тиров, полоса Center, старт без задержки
      спавн           = (врагов на самой длинной дороге − 1) × интервал   (11 − 1) × 1,2 = 12 с
      TimeToSkipStage = спавн                                             окно вызова — когда вышел последний враг
      StageTime       = спавн + передышка                                 12 + 12 = 24 с
```

Раскладка волны 12 по дорогам (шаг 3):

| Дорога | Skeleton | HumanRogue | SkeletonRogue | Orc | SkeletonMage | Barbarian | Всего |
|---|---|---|---|---|---|---|---|
| First | 3 | 3 | 2 | 1 | 1 | 1 | **11** |
| Second | 3 | 2 | 2 | 2 | — | 1 | **10** |

Очередь лишних: HumanRogue 5 — по 2, лишний на First; Orc 3 — по 1, лишний уже на Second; SkeletonMage 1 — снова First.
На дороге враги идут в порядке групп: сначала слабые, потом средние, потом сильные.

### Таблица значений по волнам — WaveValueTable

Все числа режима, которые меняются от волны к волне, — такие таблицы: ключи «волна → значение» плюс режим хвоста.

```text
  до первого ключа        значение первого ключа
  между ключами           линейно
  после последнего        TailMode:  Hold      держать значение
                                     Linear    + TailStep за каждую волну
                                     Compound  × (1 + TailStep) за каждую волну — сложный процент
```

Пример — `EnemiesCount` с ключами 1 → 6, 10 → 20, 20 → 24:

| Волна | 1 | 5 | 10 | 12 | 20 | 30 |
|---|---|---|---|---|---|---|
| Значение | 6 | 12,2 | 20 | 20,8 | 24 | Hold: 24 · Linear, шаг 1: 34 · Compound, шаг 0,05: ≈ 39,1 |

Пустая таблица — ошибка конфига: инспектор показывает её, а в игре `GetValue` бросает исключение, а не отдаёт ноль.

## 5. Источник волн — EndlessStagesSource

```text
StageProviderService — NextStageConfig (окна старта и вызова), SwitchToNext (запуск волны)
│ Get(N)
▼
EndlessStagesSource                                       Runtime/Gameplay/Features/EndlessMode/
├─ волна N уже в кеше?  → тот же самый объект конфига
└─ нет → EndlessWavesGenerator.Generate(N) → в кеш
         и для каждого потока запомнить: поток → N        TryGetWaveNumber(поток) — для этапа 3
```

Зачем кеш: окна старта и вызова читают следующую волну заранее, а спавн должен получить тот же объект — по потоку
этап 3 узнаёт номер волны заспавненного врага и применяет её множитель здоровья.

`IStagesSource` — общий интерфейс двух источников: `IsFinite`, `Count`, `Has(N)`, `Get(N)`. У бесконечного `Count` нет,
поэтому HUD (`StagesStatusPresenter`) пишет «Волна N» без «/M».

## 6. Инспектор конфига — только в редакторе

```text
  Editor/EndlessMode/

EndlessModeConfigEditor          инспектор EndlessModeConfig
├─ EndlessModeConfigValidator    замечания: пустые таблицы, ключи не по порядку, нет уровня или дорог, босс в тире
├─ EndlessWavesGenerator         тот же класс, что в игре, по дорогам префаба уровня режима
└─ EndlessWaveSummary            строка таблицы: волна, врагов, по тирам, состав, спавн, длительность
```

Число волн в таблице (по умолчанию 40) хранится в `EditorPrefs`, а не в ассете. `LevelValidator` редактора уровней не
проверяет стейджи у уровня с режимом `Infinity`.

## 7. Куда пристраиваются следующие этапы

| Этап | Что | Где |
|---|---|---|
| 3 | Боссы каждые 10 волн | `EndlessWavesGenerator` — боссовая волна; поля боссов в `EndlessModeConfig` |
| 3 | Множитель здоровья | событие спавна в `WavesSpawner` → `EndlessStagesSource.TryGetWaveNumber` → множитель волны |
| 3 | Запрет досрочного вызова | условие от `EndlessGameMode`, которое проверяет `StagesCycle` |
| 4 | Погода | смена по началу волны — `StageProviderService.CurrentStageNumber` |
| 5 | Рекорд и окно итога | `GameplayCycle` по типу режима; рекорд — `Meta/Features/Endless/` |

## 8. Все файлы этапа

| Папка | Файл | Что делает |
|---|---|---|
| `Runtime/Configs/Gameplay/Levels/Endless/` | `EndlessModeConfig` | правила режима |
| | `EndlessTier`, `EndlessTierEnemy` | тир врагов и враг тира с волной появления |
| | `WaveValueTable`, `WaveValueKey`, `WaveValueTailModes` | таблица значений по волнам |
| `Runtime/Configs/Gameplay/Levels/Stages/` | `EnemiesWavesStageConfig`, `EnemiesWaveConfig`, `WaveEnemyGroup` | + конструкторы для сборки в коде |
| `Runtime/Gameplay/Features/EndlessMode/` | `EndlessWavesGenerator` | номер волны → конфиг волны |
| | `EndlessStagesSource` | бесконечный источник волн с кешем |
| `Runtime/Gameplay/Features/LevelStages/` | `IStagesSource`, `FixedStagesSource` | источник стейджей и его вариант для кампании |
| | `LevelStagesContent` | враги и дороги уровня для прогрева и превью |
| | `StageProviderService`, `StagesCycle` | работают через источник; `Stop` гасит волну |
| `Runtime/Gameplay/GameMode/` | `GameModeBase` | общее у режимов, остановка боя в конце |
| | `BasicGameMode`, `EndlessGameMode` | кампания с победой и бесконечный без неё |
| | `GameModesFactory` | + ветка `Infinity` |
| `Runtime/Gameplay/Infrastructure/` | `GameplayInputArgs`, `GameplayLevelKinds` | аргумент сцены: кампания или бесконечный |
| | `EndlessContextRegistrations` | регистрации только бесконечного уровня |
| | `GameplayContextRegistrations` | развилка «бесконечный / кампания» |
| `Runtime/Gameplay/Features/Enemies/` | `EnemiesPrewarmer` | прогрев по готовому списку врагов |
| `Runtime/Gameplay/Features/RoutesPreview/` | `RoutesPreviewService` | стрелки по готовому списку дорог |
| `Runtime/UI/Gameplay/StagesStatus/` | `StagesStatusPresenter` | «Волна N» или «Волна N/M» |
| `Runtime/UI/Meta/MainMenu/Levels/` | `PlayBoardPopupView`, `PlayBoardPopupPresenter` | щелчок по карточке → бой |
| `Editor/EndlessMode/` | `EndlessModeConfigEditor`, `EndlessModeConfigValidator`, `EndlessWaveSummary` | инспектор и таблица волн |
