using System;

namespace Labyrinth.Services.WorldBuilding
    {
    public readonly struct Setting<T> where T: Enum
        {
        private readonly SettingIndication _indication;
        private readonly T _setting;

        private Setting(SettingIndication indication, T value = default)
            {
            this._indication = indication;
            this._setting = value;
            }

        public static Setting<T> SettingUseDefault()
            {
            return new Setting<T>(SettingIndication.UseBreedDefault);
            }

        public static Setting<T> SettingNoBehaviour()
            {
            return new Setting<T>(SettingIndication.OverrideBreedWithNoBehaviour);
            }

        public static Setting<T> NewSetting(T overrideValue)
            {
            return new Setting<T>(SettingIndication.OverrideBreedWithSpecifiedBehaviour, overrideValue);
            }

        public bool UseBreedDefault => this._indication == SettingIndication.UseBreedDefault;

        public bool RemoveBreedBehaviour => this._indication == SettingIndication.OverrideBreedWithNoBehaviour;

        public bool UseSpecificBehaviour => this._indication == SettingIndication.OverrideBreedWithSpecifiedBehaviour;

        public T SpecificBehaviour
            {
            get
                {
                if (!this.UseSpecificBehaviour)
                    throw new InvalidOperationException();
                return this._setting;
                }
            }

        private enum SettingIndication
            {
            UseBreedDefault,
            OverrideBreedWithNoBehaviour,
            OverrideBreedWithSpecifiedBehaviour
            }
        }
    }
