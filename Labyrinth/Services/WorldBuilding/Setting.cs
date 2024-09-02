using System;
using JetBrains.Annotations;

namespace Labyrinth.Services.WorldBuilding
    {
    [PublicAPI]
    public readonly struct Setting<T> where T: Enum
        {
        private readonly SettingIndication _indication;
        private readonly T _setting;

        private Setting(T value)
            {
            this._indication = SettingIndication.OverrideBreedWithSpecifiedBehaviour;
            this._setting = value;
            }

        private Setting(SettingIndication indication)
            {
            if (indication != SettingIndication.UseBreedDefault && indication != SettingIndication.OverrideBreedWithNoBehaviour)
                throw new ArgumentOutOfRangeException(nameof(indication));
            this._indication = indication;
            this._setting = default!;
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
            return new Setting<T>(overrideValue);
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
