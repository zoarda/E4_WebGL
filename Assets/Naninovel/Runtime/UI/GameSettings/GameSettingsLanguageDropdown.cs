using System.Collections.Generic;
using System.Linq;

namespace Naninovel.UI
{
    public class GameSettingsLanguageDropdown : ScriptableDropdown
    {
        private IReadOnlyList<string> availableLocales;
        private ILocalizationManager localizationManager;
        private ICommunityLocalization communityLocalization;
        private ITextManager textManager;

        protected override void Awake ()
        {
            base.Awake();

            textManager = Engine.GetService<ITextManager>();
            localizationManager = Engine.GetService<ILocalizationManager>();
            communityLocalization = Engine.GetService<ICommunityLocalization>();
            localizationManager.OnLocaleChanged += HandleLocaleChanged;
            availableLocales = GetAvailableLocales();
            InitializeOptions();
        }

        protected virtual string[] GetAvailableLocales ()
        {
            if (communityLocalization.Active) return new[] { communityLocalization.Author };
            return localizationManager.GetAvailableLocales().ToArray();
        }

        protected virtual void InitializeOptions ()
        {
            UIComponent.ClearOptions();
            UIComponent.AddOptions(availableLocales.Select(GetLabelForLocale).ToList());
            UpdateSelectedLocale(localizationManager.SelectedLocale);
        }

        protected virtual void UpdateSelectedLocale (string locale)
        {
            UIComponent.value = availableLocales.IndexOf(locale);
            UIComponent.RefreshShownValue();
        }

        protected virtual string GetLabelForLocale (string locale)
        {
            if (communityLocalization.Active) return locale;
            var localized = textManager.GetRecordValue(locale, Languages.ManagedTextCategory);
            if (!string.IsNullOrWhiteSpace(localized)) return localized;
            return Languages.GetNameByTag(locale);
        }

        protected override void OnValueChanged (int value)
        {
            var selectedLocale = availableLocales[value];
            localizationManager.SelectLocaleAsync(selectedLocale);
        }

        protected virtual void HandleLocaleChanged (string locale)
        {
            InitializeOptions();
        }
    }
}
