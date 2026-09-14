using Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage;
using Assets._Project.Develop.Runtime.UI.CommonViews;
using Assets._Project.Develop.Runtime.UI.Core.Popups;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.BottomInfoPopup
{
    public class BottomInfoPopupView : PopupViewBase
    {
        [SerializeField] private TitleWithTextView _titleWithText;

        public void SetTitle(string title) => _titleWithText.SetTitle(title);

        public void SetTowerDesc(string damage, DamageTypes damageType)
        {
            string damageTypeName = damageType == DamageTypes.Physic ? "Физический" : "Магический";

            string desc = $"АТК:   {damage}   [{damageTypeName}]";

            _titleWithText.SetText(desc);
        }

        public void SetEnemyDesc(string health, DamageTypes damageResistanceType)
        {
            string damageResistanceTypeName = damageResistanceType == DamageTypes.Physic ? "физ." : "маг.";

            if (damageResistanceType == DamageTypes.None)
                damageResistanceTypeName = "нет";

            string desc = $"ОЗ:   {health}   Защита от:   {damageResistanceTypeName}";

            _titleWithText.SetText(desc);
        }
    }
}
