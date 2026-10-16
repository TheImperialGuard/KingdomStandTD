using System;
using System.Collections.Generic;
using Assets._Project.Develop.Runtime.Configs.Encyclopedia;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Abilities;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Abilities.Enemies;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Characters;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Towers;
using Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage;
using Assets._Project.Develop.Runtime.Gameplay.Features.Raycast;
using Assets._Project.Develop.Runtime.UI.Core.Hotkeys;
using Assets._Project.Develop.Runtime.UI.Core.Popups;
using Assets._Project.Develop.Runtime.UI.Core.Views;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.Text;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.ProjectContext.Encyclopedia
{
    // Энциклопедия: по записи на разворот, закладки меняют раздел и возвращают на первый разворот, стрелки и клавиши ← →
    // листают. Без записи для открытия книга открывается на последнем разделе (Р-22), открытие по записи тоже запоминает
    // её раздел (Р-69). Esc и щелчок по затемнению закрывают книгу только отдельно от меню — в меню Esc ловит меню (Р-66).
    public class EncyclopediaPopupPresenter : PopupPresenterBase
    {
        private const int PagesPerSpread = 2;
        private const int PercentMultiplier = 100;
        private const string PercentSign = "%";
        private const string ZeroPercent = "0%";

        private readonly EncyclopediaPopupView _view;
        private readonly EncyclopediaModes _mode;
        private readonly EncyclopediaContents _contents;
        private readonly EncyclopediaConfig _encyclopediaConfig;
        private readonly TowersAbilitiesListConfig _towersAbilitiesListConfig;
        private readonly EncyclopediaSectionMemory _sectionMemory;
        private readonly HotkeysService _hotkeysService;
        private readonly ViewsFactory _viewsFactory;
        private readonly ScriptableObject _startRecord;

        private readonly List<IDisposable> _hotkeysDisposables = new();

        private EncyclopediaSections _section;
        private int _spread;

        public EncyclopediaPopupPresenter(
            ICoroutinesPerformer coroutinesPerformer,
            RayShooterService rayShooterService,
            EncyclopediaPopupView view,
            EncyclopediaModes mode,
            EncyclopediaContents contents,
            EncyclopediaConfig encyclopediaConfig,
            TowersAbilitiesListConfig towersAbilitiesListConfig,
            EncyclopediaSectionMemory sectionMemory,
            HotkeysService hotkeysService,
            ViewsFactory viewsFactory,
            ScriptableObject startRecord)
            : base(coroutinesPerformer, rayShooterService)
        {
            _view = view;
            _mode = mode;
            _contents = contents;
            _encyclopediaConfig = encyclopediaConfig;
            _towersAbilitiesListConfig = towersAbilitiesListConfig;
            _sectionMemory = sectionMemory;
            _hotkeysService = hotkeysService;
            _viewsFactory = viewsFactory;
            _startRecord = startRecord;
        }

        protected override PopupViewBase PopupView => _view;

        public override void Initialize()
        {
            base.Initialize();

            _view.SectionClicked += OnSectionClicked;
            _view.PreviousClicked += OnPreviousClicked;
            _view.NextClicked += OnNextClicked;

            _hotkeysDisposables.Add(_hotkeysService.PreviousRequested.Subscribe(OnPreviousClicked));
            _hotkeysDisposables.Add(_hotkeysService.NextRequested.Subscribe(OnNextClicked));

            if (_mode == EncyclopediaModes.Standalone)
            {
                _hotkeysDisposables.Add(_hotkeysService.CloseRequested.Subscribe(OnCloseRequested));
                _view.SetStandaloneLayout();
            }

            EncyclopediaPage startPage = GetStartPage();
            _sectionMemory.Remember(startPage.Section);
            ShowSpread(startPage.Section, startPage.Spread);
        }

        public override void Dispose()
        {
            base.Dispose();

            _view.SectionClicked -= OnSectionClicked;
            _view.PreviousClicked -= OnPreviousClicked;
            _view.NextClicked -= OnNextClicked;

            foreach (IDisposable disposable in _hotkeysDisposables)
                disposable.Dispose();

            _hotkeysDisposables.Clear();
        }

        // Место и масштаб книги в меню считает раскладка меню (Р-66).
        public void SetMenuLayout(float scale, float rightMargin) => _view.SetLayout(scale, rightMargin);

        private EncyclopediaPage GetStartPage()
        {
            switch (_startRecord)
            {
                case null:
                    return new EncyclopediaPage(_sectionMemory.LastSection, 0);

                case EncyclopediaNoteConfig note:
                    return _contents.FindPage(note);

                case ShootingTowerConfig tower:
                    return _contents.FindPage(tower);

                case CharacterConfig enemy:
                    return _contents.FindPage(enemy);

                default:
                    throw new ArgumentException($"Encyclopedia can not open a record of type {_startRecord.GetType()}");
            }
        }

        private void OnSectionClicked(EncyclopediaSections section)
        {
            _sectionMemory.Remember(section);
            ShowSpread(section, 0);
        }

        private void OnPreviousClicked()
        {
            if (_spread > 0)
                ShowSpread(_section, _spread - 1);
        }

        private void OnNextClicked()
        {
            if (_spread < GetLastSpread(_section))
                ShowSpread(_section, _spread + 1);
        }

        private void OnCloseRequested() => OnCloseRequest();

        private int GetLastSpread(EncyclopediaSections section) => _contents.GetRecordsCount(section) - 1;

        private void ShowSpread(EncyclopediaSections section, int spread)
        {
            _section = section;
            _spread = Mathf.Clamp(spread, 0, Mathf.Max(0, GetLastSpread(section)));

            int leftPageNumber = _spread * PagesPerSpread + 1;

            _view.SetSection(section);
            _view.SetPageNumbers(leftPageNumber, leftPageNumber + 1);
            _view.SetArrows(_spread > 0, _spread < GetLastSpread(section));

            ClearPages();

            if (_contents.GetRecordsCount(section) == 0)
                return;

            switch (section)
            {
                case EncyclopediaSections.Tutorial:
                    ShowNote(_contents.Notes[_spread]);
                    break;

                case EncyclopediaSections.Towers:
                    ShowTower(_contents.Towers[_spread]);
                    break;

                case EncyclopediaSections.Enemies:
                    ShowEnemy(_contents.Enemies[_spread]);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(section), section, null);
            }
        }

        private void ShowNote(EncyclopediaNoteConfig note)
        {
            ShowLeftPage(note.Picture, note.Title, string.Empty);
            _view.NotePage.SetText(note.Text);
        }

        private void ShowTower(ShootingTowerConfig tower)
        {
            ShowLeftPage(tower.Picture, NonBreakingSpace.BeforeTrailingNumber(tower.TowerName), tower.TowerDesc);

            EncyclopediaStats damageStat = tower.DamageType == DamageTypes.Magic
                ? EncyclopediaStats.MagicDamage
                : EncyclopediaStats.PhysicalDamage;

            AddStatTile(damageStat, DecimalText.Format(tower.AttackDamage));
            AddStatTile(EncyclopediaStats.AttacksPerSecond, DecimalText.Format(tower.AttackPerSecond));
            AddStatTile(EncyclopediaStats.AttackRange, DecimalText.Format(tower.AttackRange));

            bool hasAbilities = _contents.IsTopLevel(tower);
            _view.EntityPage.SetAbilitiesVisible(hasAbilities);

            if (hasAbilities == false)
                return;

            TowersAbilitiesConfig abilities = _towersAbilitiesListConfig.GetBy(tower.TowerType);
            AddTowerAbility(abilities.FirstAbilityGroup[0]);
            AddTowerAbility(abilities.SecondAbilityGroup[0]);
        }

        private void ShowEnemy(MeleeConfig enemy)
        {
            ShowLeftPage(enemy.Picture, enemy.CharacterName, string.Empty);

            if (enemy.IsBoss)
                AddTrait(_encyclopediaConfig.BossTrait);

            if (enemy.IsFlying)
                AddTrait(_encyclopediaConfig.FlyingTrait);

            _view.LeftPage.SetTraitsVisible(_view.LeftPage.TraitsList.Elements.Count > 0);

            AddStatTile(EncyclopediaStats.Health, DecimalText.Format(enemy.MaxHealth));
            AddResistanceTile(EncyclopediaStats.PhysicalResistance, enemy, DamageTypes.Physic);
            AddResistanceTile(EncyclopediaStats.MagicResistance, enemy, DamageTypes.Magic);
            AddStatTile(EncyclopediaStats.GoldOnDeath, enemy.GoldOnDeath.ToString());
            AddStatTile(EncyclopediaStats.CastleDamage, enemy.DamageOnFinishPath.ToString());

            bool hasAbility = enemy.Ability != null;
            _view.EntityPage.SetAbilitiesVisible(hasAbility);

            if (hasAbility)
                AddEnemyAbility(enemy.Ability);
        }

        private void ShowLeftPage(Sprite picture, string title, string description)
        {
            _view.LeftPage.SetPicture(picture);
            _view.LeftPage.SetTitle(title);
            _view.LeftPage.SetDescription(description);
            _view.LeftPage.SetTraitsVisible(false);
        }

        private void AddTrait(EncyclopediaTraitInfo traitInfo)
        {
            EncyclopediaTraitView trait = _viewsFactory.Create<EncyclopediaTraitView>(ViewIDs.EncyclopediaTrait);
            _view.LeftPage.TraitsList.AddElement(trait);
            trait.Set(traitInfo.Icon, traitInfo.Name, traitInfo.Description);
        }

        // Защита от типа урона, которой у врага нет, показывается нулевой и приглушённой.
        private void AddResistanceTile(EncyclopediaStats stat, MeleeConfig enemy, DamageTypes damageType)
        {
            int percent = enemy.DamageResistanceType == damageType
                ? Mathf.RoundToInt(enemy.DamageResistanceIndex * PercentMultiplier)
                : 0;

            if (percent == 0)
            {
                AddStatTile(stat, ZeroPercent, isMuted: true);
                return;
            }

            AddStatTile(stat, percent + PercentSign);
        }

        private void AddStatTile(EncyclopediaStats stat, string value, bool isMuted = false)
        {
            EncyclopediaStatInfo statInfo = _encyclopediaConfig.GetStatInfo(stat);
            EncyclopediaStatTileView tile = _viewsFactory.Create<EncyclopediaStatTileView>(ViewIDs.EncyclopediaStatTile);

            _view.EntityPage.StatTilesList.AddElement(tile);
            tile.Set(statInfo.Icon, value, statInfo.Caption, isMuted);
        }

        private void AddTowerAbility(AbilityConfig ability)
        {
            string description = ability.DescriptionByLevel.Count > 0 ? ability.DescriptionByLevel[0] : string.Empty;

            AddAbility(ability.Icon, ability.Name, description, EncyclopediaAbilityNumbers.Format(ability));
        }

        private void AddEnemyAbility(EnemyAbilityConfig ability)
            => AddAbility(ability.Icon, ability.Name, ability.Description, EncyclopediaAbilityNumbers.Format(ability));

        private void AddAbility(Sprite icon, string abilityName, string description, string numbers)
        {
            EncyclopediaAbilityView ability = _viewsFactory.Create<EncyclopediaAbilityView>(ViewIDs.EncyclopediaAbility);

            _view.EntityPage.AddAbility(ability);
            ability.Set(icon, abilityName, description, numbers);
        }

        private void ClearPages()
        {
            EncyclopediaTraitsListView traitsList = _view.LeftPage.TraitsList;
            EncyclopediaStatTilesListView statTilesList = _view.EntityPage.StatTilesList;

            ReleaseAll(traitsList.Elements, traitsList.RemoveElement);
            ReleaseAll(statTilesList.Elements, statTilesList.RemoveElement);
            ReleaseAll(_view.EntityPage.Abilities, _view.EntityPage.RemoveAbility);
        }

        private void ReleaseAll<TElement>(IReadOnlyList<TElement> elements, Action<TElement> remove)
            where TElement : MonoBehaviour, IView
        {
            foreach (TElement element in new List<TElement>(elements))
            {
                remove(element);
                _viewsFactory.Release(element);
            }
        }
    }
}
