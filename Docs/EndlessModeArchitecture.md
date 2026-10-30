# Бесконечный режим: устройство кода

Схемы кода бесконечного режима. Сейчас здесь каркас — этап 2 плана «Бесконечный уровень»; боссы, множитель здоровья,
погода, окно режима и рекорд дописываются по ходу следующих этапов. Пути — от `Assets/_Project/Develop/`, имя файла
совпадает с именем класса.

Схемы нарисованы в Mermaid: их рисуют предпросмотр Markdown в VS Code (расширение Markdown Preview Mermaid Support) и
GitHub.

Термины:
- **Волна** — то, что в коде называется стейджем (`EnemiesWavesStageConfig`, `EnemiesWavesStage`). HUD пишет «Волна N».
- **Поток** — `EnemiesWaveConfig` внутри волны: одна дорога, интервал между врагами, группы врагов. У волны бесконечного
  режима по потоку на каждую дорогу уровня.

Цвета на схемах:

```mermaid
%%{init: {"theme": "base", "themeVariables": {"primaryColor": "#dbe9f9", "primaryTextColor": "#1b1b1b", "primaryBorderColor": "#5b8fd1", "lineColor": "#8b949e", "textColor": "#8b949e", "edgeLabelBackground": "#e8ecf1", "clusterBkg": "rgba(139,148,158,0.10)", "clusterBorder": "#8b949e", "titleColor": "#8b949e"}}}%%
flowchart LR
    classDef new fill:#d4edda,stroke:#2e7d32,color:#1b1b1b
    classDef changed fill:#fff3cd,stroke:#b7791f,color:#1b1b1b
    classDef config fill:#dbe9f9,stroke:#2f6db3,color:#1b1b1b
    classDef old fill:#ececec,stroke:#8a8a8a,color:#1b1b1b

    N["новый класс"]:::new ~~~ C["изменённый класс"]:::changed ~~~ K["конфиг или ассет"]:::config ~~~ O["был и не менялся"]:::old
```

## 1. Общая картина

```mermaid
%%{init: {"theme": "base", "themeVariables": {"primaryColor": "#dbe9f9", "primaryTextColor": "#1b1b1b", "primaryBorderColor": "#5b8fd1", "lineColor": "#8b949e", "textColor": "#8b949e", "edgeLabelBackground": "#e8ecf1", "clusterBkg": "rgba(139,148,158,0.10)", "clusterBorder": "#8b949e", "titleColor": "#8b949e"}}}%%
flowchart LR
    classDef new fill:#d4edda,stroke:#2e7d32,color:#1b1b1b
    classDef changed fill:#fff3cd,stroke:#b7791f,color:#1b1b1b
    classDef config fill:#dbe9f9,stroke:#2f6db3,color:#1b1b1b
    classDef old fill:#ececec,stroke:#8a8a8a,color:#1b1b1b

    subgraph MENU["Главное меню"]
        direction TB
        Card["PlayBoardPopupView<br/>карточка «Бесконечный»"]:::changed
        Board["PlayBoardPopupPresenter"]:::changed
    end

    Args["GameplayInputArgs<br/>ForEndless()"]:::changed

    subgraph REG["Регистрации сцены Gameplay"]
        direction TB
        Reg["GameplayContextRegistrations"]:::changed
        EReg["EndlessContextRegistrations"]:::new
    end

    subgraph BATTLE["Бой"]
        direction TB
        Mode["EndlessGameMode<br/>наследник GameModeBase"]:::new
        Cycle["StagesCycle"]:::changed
        Provider["StageProviderService"]:::changed
        Source["EndlessStagesSource"]:::new
        Gen["EndlessWavesGenerator"]:::new
        Stage["EnemiesWavesStage"]:::old
        Spawner["WavesSpawner"]:::old
    end

    Config[("EndlessModeConfig.asset")]:::config

    Card --> Board --> Args --> Reg --> EReg
    EReg -.->|создаёт| Source
    Mode --> Cycle --> Provider -->|Get N| Source --> Gen
    Config -.-> Gen
    Provider --> Stage --> Spawner
```

Конфиг волны, который собирает генератор, — обычный `EnemiesWavesStageConfig`, тот же, что у стейджей кампании.
Поэтому `EnemiesWavesStage` и `WavesSpawner` бесконечный режим не меняет.

## 2. Регистрации: чем бесконечный уровень отличается от кампании

Отличий три: откуда взять `LevelConfig`, источник волн и списки для подготовки уровня. Всё остальное в сцене общее.

```mermaid
%%{init: {"theme": "base", "themeVariables": {"primaryColor": "#dbe9f9", "primaryTextColor": "#1b1b1b", "primaryBorderColor": "#5b8fd1", "lineColor": "#8b949e", "textColor": "#8b949e", "edgeLabelBackground": "#e8ecf1", "clusterBkg": "rgba(139,148,158,0.10)", "clusterBorder": "#8b949e", "titleColor": "#8b949e"}}}%%
flowchart TB
    classDef new fill:#d4edda,stroke:#2e7d32,color:#1b1b1b
    classDef changed fill:#fff3cd,stroke:#b7791f,color:#1b1b1b
    classDef config fill:#dbe9f9,stroke:#2f6db3,color:#1b1b1b
    classDef old fill:#ececec,stroke:#8a8a8a,color:#1b1b1b

    Process["GameplayContextRegistrations.Process"]:::changed

    Process --> LevelChoice{"GetLevelConfig"}
    LevelChoice -->|кампания| Campaign["LevelsListConfig.GetBy(N)"]:::old
    LevelChoice -->|бесконечный| Endless["EndlessModeConfig.Level<br/>EndlessLevel.asset"]:::config

    Process --> Branch{"IsEndless?"}
    Branch -->|да| ER
    Branch -->|нет| CR

    subgraph ER["EndlessContextRegistrations"]
        direction TB
        Gen["EndlessWavesGenerator"]:::new
        EndlessSource["EndlessStagesSource<br/>он же IStagesSource"]:::new
        EndlessContent["LevelStagesContent<br/>враги всех тиров, все дороги"]:::new
    end

    subgraph CR["Кампания"]
        direction TB
        FixedSource["FixedStagesSource<br/>он же IStagesSource"]:::new
        CampaignContent["LevelStagesContent.CollectFrom<br/>враги и дороги стейджей"]:::new
    end

    subgraph COMMON["Общие регистрации"]
        direction TB
        Provider["StageProviderService"]:::changed
        Prewarmer["EnemiesPrewarmer<br/>прогрев пула"]:::changed
        Preview["RoutesPreviewService<br/>стрелки дорог"]:::changed
        Modes["GameModesFactory<br/>по LevelConfig.GameMode"]:::changed
    end

    EndlessSource & FixedSource -->|IStagesSource| Provider
    EndlessContent & CampaignContent -->|Enemies| Prewarmer
    EndlessContent & CampaignContent -->|PathNumbers| Preview
    Modes -->|Basic| BasicMode["BasicGameMode"]:::changed
    Modes -->|Infinity| EndlessMode["EndlessGameMode"]:::new
```

## 3. Бой

### Иерархии

```mermaid
%%{init: {"theme": "base", "themeVariables": {"primaryColor": "#dbe9f9", "primaryTextColor": "#1b1b1b", "primaryBorderColor": "#5b8fd1", "lineColor": "#8b949e", "textColor": "#8b949e", "noteBkgColor": "#fff3cd", "noteTextColor": "#1b1b1b", "noteBorderColor": "#b7791f"}}}%%
classDiagram
    direction LR

    class IGameMode {
        <<interface>>
        End
        Start()
        Update(deltaTime)
    }

    class GameModeBase {
        <<abstract>>
        Start()
        Update(deltaTime)
        #OnUpdate(deltaTime)
        #ProcessEndGame(results)
    }

    class BasicGameMode
    class EndlessGameMode

    IGameMode <|.. GameModeBase
    GameModeBase <|-- BasicGameMode
    GameModeBase <|-- EndlessGameMode

    note for GameModeBase "окно старта, поражение по здоровью замка,<br/>остановка боя в конце"
    note for BasicGameMode "победа: последний стейдж, спавн окончен, врагов нет"
    note for EndlessGameMode "победы нет"
```

```mermaid
%%{init: {"theme": "base", "themeVariables": {"primaryColor": "#dbe9f9", "primaryTextColor": "#1b1b1b", "primaryBorderColor": "#5b8fd1", "lineColor": "#8b949e", "textColor": "#8b949e", "noteBkgColor": "#fff3cd", "noteTextColor": "#1b1b1b", "noteBorderColor": "#b7791f"}}}%%
classDiagram
    direction LR

    class StageProviderService {
        CurrentStageNumber
        NextStageConfig
        HasFiniteStages
        StagesCount
        HasNextStage()
        SwitchToNext()
    }

    class IStagesSource {
        <<interface>>
        IsFinite
        Count
        Has(N)
        Get(N)
    }

    class FixedStagesSource
    class EndlessStagesSource {
        TryGetWaveNumber(stream, N)
    }

    class EndlessWavesGenerator {
        PathNumbers
        Generate(N)
    }

    StageProviderService --> IStagesSource
    IStagesSource <|.. FixedStagesSource
    IStagesSource <|.. EndlessStagesSource
    EndlessStagesSource --> EndlessWavesGenerator

    note for FixedStagesSource "стейджи из LevelConfig кампании"
    note for EndlessStagesSource "генерирует по запросу и кеширует"
```

У бесконечного источника `Count` нет, поэтому HUD (`StagesStatusPresenter`) пишет «Волна N» без «/M».

### Жизнь одной волны

```mermaid
%%{init: {"theme": "base", "themeVariables": {"primaryColor": "#dbe9f9", "primaryTextColor": "#1b1b1b", "primaryBorderColor": "#5b8fd1", "lineColor": "#8b949e", "textColor": "#8b949e", "noteBkgColor": "#fff3cd", "noteTextColor": "#1b1b1b", "noteBorderColor": "#b7791f", "signalColor": "#8b949e", "signalTextColor": "#8b949e", "actorLineColor": "#8b949e", "labelBoxBkgColor": "#e8e1f7", "labelBoxBorderColor": "#6a4fb3", "labelTextColor": "#1b1b1b", "loopTextColor": "#8b949e", "sequenceNumberColor": "#ffffff"}}}%%
sequenceDiagram
    autonumber
    participant SC as StagesCycle
    participant SPS as StageProviderService
    participant SRC as EndlessStagesSource
    participant GEN as EndlessWavesGenerator
    participant ST as EnemiesWavesStage
    participant WS as WavesSpawner

    SC->>SPS: SwitchToNext
    SPS->>SRC: Get(N)
    alt волны N ещё нет в кеше
        SRC->>GEN: Generate(N)
        GEN-->>SRC: EnemiesWavesStageConfig
        Note over SRC: волна N в кеш,<br/>каждый её поток → N
    end
    SRC-->>SPS: конфиг волны N
    SPS->>ST: StagesFactory.Create, Start
    loop каждый поток волны
        ST->>WS: SpawnWave(поток)
    end
    Note over ST,WS: враги выходят по одному через интервал
    ST-->>SC: прошло TimeToSkipStage — окно вызова следующей волны
    ST-->>SC: прошло StageTime — следующая волна
```

Окна старта и вызова показывают состав следующей волны заранее (`NextStageConfig`), поэтому источник кеширует волну:
спавн должен получить тот же самый объект. По потоку этап 3 узнаёт номер волны заспавненного врага —
`TryGetWaveNumber`.

### Конец игры — общий для обоих режимов

```mermaid
%%{init: {"theme": "base", "themeVariables": {"primaryColor": "#dbe9f9", "primaryTextColor": "#1b1b1b", "primaryBorderColor": "#5b8fd1", "lineColor": "#8b949e", "textColor": "#8b949e", "noteBkgColor": "#fff3cd", "noteTextColor": "#1b1b1b", "noteBorderColor": "#b7791f", "signalColor": "#8b949e", "signalTextColor": "#8b949e", "actorLineColor": "#8b949e", "labelBoxBkgColor": "#e8e1f7", "labelBoxBorderColor": "#6a4fb3", "labelTextColor": "#1b1b1b", "loopTextColor": "#8b949e", "sequenceNumberColor": "#ffffff"}}}%%
sequenceDiagram
    participant PH as PlayerHealth
    participant GM as GameModeBase
    participant SC as StagesCycle
    participant WS as WavesSpawner
    participant GC as GameplayCycle

    PH->>GM: здоровье замка ≤ 0
    GM->>SC: Stop
    Note right of SC: окно вызова закрыто,<br/>таймер волны погашен
    GM->>WS: Stop
    Note right of WS: недоспавненные потоки<br/>остановлены
    GM->>GC: End(Defeat)
    GC->>GC: музыка и окно поражения
```

Раньше таймер волны — корутина — после поражения доходил до конца и запускал следующую волну. Поражение срабатывает
один раз: после итога падение здоровья замка ничего не делает.

## 4. Генератор волн

`EndlessWavesGenerator` (`Runtime/Gameplay/Features/EndlessMode/`) превращает номер волны в `EnemiesWavesStageConfig`.
Состояния забега у него нет: всё берётся из конфига и номера волны. Поэтому волна N всегда одна и та же, а таблицу волн
в инспекторе строит тот же генератор.

### Шаги — на примере волны 12

Числа — из текущего чернового конфига; их же показывает таблица волн в инспекторе.

```mermaid
%%{init: {"theme": "base", "themeVariables": {"primaryColor": "#dbe9f9", "primaryTextColor": "#1b1b1b", "primaryBorderColor": "#5b8fd1", "lineColor": "#8b949e", "textColor": "#8b949e", "edgeLabelBackground": "#e8ecf1", "clusterBkg": "rgba(139,148,158,0.10)", "clusterBorder": "#8b949e", "titleColor": "#8b949e"}}}%%
flowchart TB
    classDef config fill:#dbe9f9,stroke:#2f6db3,color:#1b1b1b
    classDef old fill:#ececec,stroke:#8a8a8a,color:#1b1b1b
    classDef step fill:#e8e1f7,stroke:#6a4fb3,color:#1b1b1b
    classDef out fill:#d4edda,stroke:#2e7d32,color:#1b1b1b

    subgraph IN["EndlessModeConfig"]
        direction LR
        Seed["Seed"]:::config
        Count["EnemiesCount"]:::config
        Delay["DelayBetweenSpawns"]:::config
        Rest["RestTime"]:::config
        Tiers["Tiers<br/>Share · KindsPerWave<br/>Enemies с FirstWave"]:::config
    end

    Roads["Дороги уровня<br/>First, Second"]:::old

    S0["0 · Зерно волны<br/>Random(Seed × 100003 + N)"]:::step
    S1["1 · Числа из таблиц<br/>20,8 → 21 враг · интервал 1,2 с · передышка 12 с"]:::step
    S2a["2a · Открытые враги тиров<br/>FirstWave ≤ 12: слабых 4, средних 5, сильных 3"]:::step
    S2b["2b · Доли тиров<br/>0,55 · 0,32 · 0,13"]:::step
    S2c["2c · Делёж по наибольшему остатку<br/>квоты 11,55 · 6,72 · 2,73<br/>целые 11 · 6 · 2, ещё 2 — большим остаткам<br/>итог 11 · 7 · 3"]:::step
    S2d["2d · Виды внутри тира<br/>2 случайных вида из открытых,<br/>враги поровну, лишний — первому по жребию"]:::step
    S3["3 · По дорогам<br/>First 11 · Second 10"]:::step
    S4["4 · EnemiesWavesStageConfig<br/>поток на дорогу · спавн 12 с · волна 24 с"]:::out

    Seed --> S0
    Count & Delay & Rest --> S1
    Tiers --> S2a
    S0 --> S1 --> S2a --> S2b --> S2c --> S2d --> S3 --> S4
    Roads --> S3
```

- **Зерно** у каждой волны своё: состав волны не зависит от того, какие волны собирались раньше.
- **Тир без открытых врагов** получает долю 0, и она сама расходится по остальным тирам пропорционально.
- **Время волны:** спавн = (врагов на самой длинной дороге − 1) × интервал = (11 − 1) × 1,2 = 12 с. Окно вызова
  следующей волны (`TimeToSkipStage`) открывается после спавна — когда вышел последний враг. Вся волна (`StageTime`) —
  спавн плюс передышка: 12 + 12 = 24 с.

### Волна 12 по тирам и видам

```mermaid
%%{init: {"theme": "base", "themeVariables": {"pie1": "#7fb77e", "pie2": "#e0b44c", "pie3": "#d9737f", "pieTitleTextColor": "#8b949e", "pieLegendTextColor": "#8b949e", "pieSectionTextColor": "#1b1b1b", "pieStrokeColor": "#8b949e", "pieOuterStrokeColor": "#8b949e"}}}%%
pie showData
    title Волна 12 — 21 враг по тирам
    "Слабые: Skeleton 6, HumanRogue 5" : 11
    "Средние: SkeletonRogue 4, Orc 3" : 7
    "Сильные: SkeletonMage 1, Barbarian 2" : 3
```

### Волна 12 по дорогам

```mermaid
%%{init: {"theme": "base", "themeVariables": {"primaryColor": "#dbe9f9", "primaryTextColor": "#1b1b1b", "primaryBorderColor": "#5b8fd1", "lineColor": "#8b949e", "textColor": "#8b949e", "edgeLabelBackground": "#e8ecf1", "clusterBkg": "rgba(139,148,158,0.10)", "clusterBorder": "#8b949e", "titleColor": "#8b949e"}}}%%
flowchart LR
    classDef weak fill:#d4edda,stroke:#2e7d32,color:#1b1b1b
    classDef medium fill:#fff3cd,stroke:#b7791f,color:#1b1b1b
    classDef strong fill:#f8d7da,stroke:#a83240,color:#1b1b1b

    subgraph FIRST["Дорога First — 11 врагов"]
        direction LR
        F1["Skeleton 3"]:::weak --> F2["HumanRogue 3"]:::weak --> F3["SkeletonRogue 2"]:::medium --> F4["Orc 1"]:::medium --> F5["SkeletonMage 1"]:::strong --> F6["Barbarian 1"]:::strong
    end

    subgraph SECOND["Дорога Second — 10 врагов"]
        direction LR
        S1["Skeleton 3"]:::weak --> S2["HumanRogue 2"]:::weak --> S3["SkeletonRogue 2"]:::medium --> S4["Orc 2"]:::medium --> S5["Barbarian 1"]:::strong
    end
```

Стрелки — порядок выхода на дороге: сначала слабые, потом средние, потом сильные. Каждая группа волны делится между
дорогами поровну: Skeleton 6 — по 3, SkeletonRogue 4 — по 2. Лишние раздаются по очереди, и очередь идёт сквозь все
группы волны: лишний HumanRogue — на First, лишний Orc — уже на Second, единственный SkeletonMage — снова на First,
Barbarian 2 — по одному. Так лишние не копятся на первой дороге.

### Волны 1–25

```mermaid
%%{init: {"theme": "base", "themeVariables": {"xyChart": {"backgroundColor": "transparent", "titleColor": "#8b949e", "xAxisLabelColor": "#8b949e", "xAxisTitleColor": "#8b949e", "xAxisTickColor": "#8b949e", "xAxisLineColor": "#8b949e", "yAxisLabelColor": "#8b949e", "yAxisTitleColor": "#8b949e", "yAxisTickColor": "#8b949e", "yAxisLineColor": "#8b949e", "plotColorPalette": "#5b8fd1"}}}}%%
xychart-beta
    title "Врагов в волне"
    x-axis "Волна" ["1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "25"]
    y-axis "Врагов" 0 --> 26
    bar [6, 8, 9, 11, 12, 14, 15, 17, 18, 20, 20, 21, 21, 22, 22, 22, 23, 23, 24, 24, 24, 24, 24, 24, 24]
```

| Волна | 1 | 5 | 10 | 15 | 20 | 30 |
|---|---|---|---|---|---|---|
| Слабые | 6 | 10 | 12 | 10 | 8 | 8 |
| Средние | 0 | 2 | 6 | 8 | 10 | 10 |
| Сильные | 0 | 0 | 2 | 4 | 6 | 6 |

Рост 1–10 идёт за счёт числа врагов, на 10–20 сильные тиры вытесняют слабых, после 20 держится плато, а виды внутри
тиров меняются от волны к волне.

### Таблица значений по волнам — WaveValueTable

Все числа режима, которые меняются от волны к волне, — такие таблицы: ключи «волна → значение» плюс режим хвоста.

```mermaid
%%{init: {"theme": "base", "themeVariables": {"primaryColor": "#dbe9f9", "primaryTextColor": "#1b1b1b", "primaryBorderColor": "#5b8fd1", "lineColor": "#8b949e", "textColor": "#8b949e", "edgeLabelBackground": "#e8ecf1", "clusterBkg": "rgba(139,148,158,0.10)", "clusterBorder": "#8b949e", "titleColor": "#8b949e"}}}%%
flowchart LR
    classDef config fill:#dbe9f9,stroke:#2f6db3,color:#1b1b1b
    classDef step fill:#e8e1f7,stroke:#6a4fb3,color:#1b1b1b

    Wave(["волна N"]) --> Where{"где N<br/>относительно ключей?"}
    Where -->|до первого| First["значение первого ключа"]:::step
    Where -->|между двумя| Between["линейно между ними"]:::step
    Where -->|после последнего| Tail{"TailMode"}
    Tail -->|Hold| Hold["значение последнего ключа"]:::config
    Tail -->|Linear| Linear["+ TailStep за каждую волну"]:::config
    Tail -->|Compound| Compound["× (1 + TailStep) за каждую волну"]:::config
```

Пример — `EnemiesCount` с ключами 1 → 6, 10 → 20, 20 → 24:

| Волна | 1 | 5 | 10 | 12 | 20 | 30 |
|---|---|---|---|---|---|---|
| Значение | 6 | 12,2 | 20 | 20,8 | 24 | Hold: 24 · Linear, шаг 1: 34 · Compound, шаг 0,05: ≈ 39,1 |

Пустая таблица — ошибка конфига: инспектор показывает её, а в игре `GetValue` бросает исключение, а не отдаёт ноль.

### Что генератор читает

```mermaid
%%{init: {"theme": "base", "themeVariables": {"primaryColor": "#dbe9f9", "primaryTextColor": "#1b1b1b", "primaryBorderColor": "#5b8fd1", "lineColor": "#8b949e", "textColor": "#8b949e", "noteBkgColor": "#fff3cd", "noteTextColor": "#1b1b1b", "noteBorderColor": "#b7791f"}}}%%
classDiagram
    direction LR

    class EndlessModeConfig {
        Level
        Description
        Seed
        EnemiesCount
        Tiers
        DelayBetweenSpawns
        RestTime
    }

    class EndlessTier {
        Name
        Enemies
        Share
        KindsPerWave
    }

    class EndlessTierEnemy {
        EnemyConfig
        FirstWave
    }

    class WaveValueTable {
        Keys
        TailMode
        TailStep
        GetValue(N)
    }

    class WaveValueKey {
        Wave
        Value
    }

    EndlessModeConfig "1" *-- "many" EndlessTier
    EndlessTier "1" *-- "many" EndlessTierEnemy
    EndlessModeConfig ..> WaveValueTable : EnemiesCount, DelayBetweenSpawns, RestTime
    EndlessTier ..> WaveValueTable : Share
    WaveValueTable "1" *-- "many" WaveValueKey

    note for EndlessModeConfig "Resources/Configs/Gameplay/Levels/Endless/EndlessModeConfig.asset"
```

Классы конфигов лежат в `Runtime/Configs/Gameplay/Levels/Endless/`. Тиры идут в порядке выхода на дорогу: слабые,
средние, сильные.

## 5. Инспектор конфига — только в редакторе

```mermaid
%%{init: {"theme": "base", "themeVariables": {"primaryColor": "#dbe9f9", "primaryTextColor": "#1b1b1b", "primaryBorderColor": "#5b8fd1", "lineColor": "#8b949e", "textColor": "#8b949e", "edgeLabelBackground": "#e8ecf1", "clusterBkg": "rgba(139,148,158,0.10)", "clusterBorder": "#8b949e", "titleColor": "#8b949e"}}}%%
flowchart LR
    classDef new fill:#d4edda,stroke:#2e7d32,color:#1b1b1b
    classDef changed fill:#fff3cd,stroke:#b7791f,color:#1b1b1b
    classDef old fill:#ececec,stroke:#8a8a8a,color:#1b1b1b

    Editor["EndlessModeConfigEditor<br/>инспектор EndlessModeConfig"]:::new
    Validator["EndlessModeConfigValidator<br/>замечания конфига"]:::new
    Gen["EndlessWavesGenerator<br/>тот же, что в игре"]:::new
    Summary["EndlessWaveSummary<br/>строка таблицы волн"]:::new
    Prefab[("префаб уровня режима<br/>его дороги")]:::old
    LevelValidator["LevelValidator<br/>не проверяет стейджи у Infinity"]:::changed

    Editor -->|1| Validator
    Editor -->|2 — если ошибок нет| Gen
    Prefab -.-> Gen
    Gen --> Summary
```

- **Замечания:** пустые таблицы, ключи не по возрастанию, нет уровня, префаба или дорог — ошибки, и тогда таблица волн
  не строится. Режим уровня не `Infinity`, стейджи у уровня, пустой тир или элемент, босс в тире — предупреждения.
- **Таблица:** волна, врагов, по тирам, состав (имена ассетов в порядке тиров), спавн, длительность. Число волн в
  таблице (по умолчанию 40) хранится в `EditorPrefs`, а не в ассете.

## 6. Куда пристраиваются следующие этапы

```mermaid
%%{init: {"theme": "base", "themeVariables": {"primaryColor": "#dbe9f9", "primaryTextColor": "#1b1b1b", "primaryBorderColor": "#5b8fd1", "lineColor": "#8b949e", "textColor": "#8b949e", "edgeLabelBackground": "#e8ecf1", "clusterBkg": "rgba(139,148,158,0.10)", "clusterBorder": "#8b949e", "titleColor": "#8b949e"}}}%%
flowchart LR
    classDef new fill:#d4edda,stroke:#2e7d32,color:#1b1b1b
    classDef changed fill:#fff3cd,stroke:#b7791f,color:#1b1b1b
    classDef old fill:#ececec,stroke:#8a8a8a,color:#1b1b1b
    classDef future fill:#f6dff3,stroke:#9c3f91,color:#1b1b1b,stroke-dasharray:5 3

    Bosses["Этап 3 · босс каждые 10 волн"]:::future --> Gen["EndlessWavesGenerator"]:::new
    Spawner["WavesSpawner"]:::old -.->|событие спавна| Health["Этап 3 · множитель здоровья"]:::future
    Health -->|номер волны врага| Source["EndlessStagesSource.TryGetWaveNumber"]:::new
    Mode["EndlessGameMode"]:::new -.-> Skip["Этап 3 · запрет досрочного вызова"]:::future
    Skip --> Cycle["StagesCycle"]:::changed
    Provider["StageProviderService.CurrentStageNumber"]:::changed -.->|начало волны| Weather["Этап 4 · погода"]:::future
    Record["Этап 5 · рекорд и окно итога"]:::future --> GameplayCycle["GameplayCycle"]:::old
```

## 7. Файлы

```mermaid
%%{init: {"theme": "base", "themeVariables": {"primaryColor": "#dbe9f9", "primaryTextColor": "#1b1b1b", "primaryBorderColor": "#5b8fd1", "lineColor": "#8b949e", "textColor": "#8b949e"}}}%%
mindmap
  root((Бесконечный режим))
    Runtime/Configs/Gameplay/Levels/Endless
      EndlessModeConfig
      EndlessTier
      EndlessTierEnemy
      WaveValueTable
      WaveValueKey
      WaveValueTailModes
    Runtime/Gameplay/Features/EndlessMode
      EndlessWavesGenerator
      EndlessStagesSource
    Runtime/Gameplay/Features/LevelStages
      IStagesSource
      FixedStagesSource
      LevelStagesContent
    Runtime/Gameplay/GameMode
      GameModeBase
      EndlessGameMode
    Runtime/Gameplay/Infrastructure
      EndlessContextRegistrations
      GameplayLevelKinds
    Editor/EndlessMode
      EndlessModeConfigEditor
      EndlessModeConfigValidator
      EndlessWaveSummary
```

Новые файлы — на схеме выше. Изменённые:

| Папка | Файл | Что изменилось |
|---|---|---|
| `Runtime/Configs/Gameplay/Levels/Stages/` | `EnemiesWavesStageConfig`, `EnemiesWaveConfig`, `WaveEnemyGroup` | конструкторы для сборки в коде |
| `Runtime/Gameplay/Features/LevelStages/` | `StageProviderService`, `StagesCycle` | работают через источник; `Stop` гасит волну |
| `Runtime/Gameplay/GameMode/` | `BasicGameMode`, `GameModesFactory` | наследник `GameModeBase`; ветка `Infinity` |
| `Runtime/Gameplay/Infrastructure/` | `GameplayInputArgs`, `GameplayContextRegistrations`, `GameplayBootstrap` | кампания или бесконечный; развилка регистраций |
| `Runtime/Gameplay/Features/Enemies/` | `EnemiesPrewarmer` | прогрев по готовому списку врагов |
| `Runtime/Gameplay/Features/RoutesPreview/` | `RoutesPreviewService` | стрелки по готовому списку дорог |
| `Runtime/UI/Gameplay/` | `StagesStatusPresenter`, `PausePopupPresenter`, `DefeatPopupPresenter` | «Волна N»; «Заново» — тот же уровень |
| `Runtime/UI/Meta/` | `PlayBoardPopupView`, `PlayBoardPopupPresenter`, `MainMenuPresentersFactory`, `LevelTilePresenter` | щелчок по карточке → бой |
| `Runtime/Utilities/ConfigsManagment/` | `ResourcesConfigsLoader` | путь к `EndlessModeConfig` |
| `Editor/LevelEditor/` | `LevelValidator`, `PlaytestSession` | уровень `Infinity` без стейджей; `ForCampaign` |
