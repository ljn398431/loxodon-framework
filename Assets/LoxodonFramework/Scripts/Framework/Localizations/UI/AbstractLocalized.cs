﻿using Loxodon.Framework.Observables;
using Loxodon.Log;
using System;
using UnityEngine;

namespace Loxodon.Framework.Localizations
{
    public abstract class AbstractLocalized<T> : MonoBehaviour where T : Component
    {
        private static readonly ILog log = LogManager.GetLogger("AbstractLocalized");

        [SerializeField]
        private string key;

        protected T target;
        protected IObservableProperty value;

        protected virtual void OnKeyChanged()
        {
            if (this.value != null)
                this.value.ValueChanged -= OnValueChanged;

            if (!this.enabled || this.target == null || string.IsNullOrEmpty(key))
                return;

            Localization localization = Localization.Current;
            this.value = localization.Get<IObservableProperty>(key);
            if (this.value == null)
            {
                if (log.IsErrorEnabled)
                    log.ErrorFormat("There is an invalid localization key \"{0}\" on the {1} object named \"{2}\".", key, typeof(T).Name, this.name);
                return;
            }

            this.value.ValueChanged += OnValueChanged;
            this.OnValueChanged(this.value, EventArgs.Empty);
        }

        public string Key
        {
            get { return this.key; }
            set
            {
                if (string.IsNullOrEmpty(value) || value.Equals(this.key))
                    return;

                this.key = value;
                this.OnKeyChanged();
            }
        }

        protected virtual void OnEnable()
        {
            this.target = this.GetComponent<T>();
            if (this.target == null)
                return;

            this.OnKeyChanged();
        }

        protected virtual void OnDisable()
        {
            if (this.value != null)
                this.value.ValueChanged -= OnValueChanged;
        }

        protected abstract void OnValueChanged(object sender, EventArgs e);
    }
}
