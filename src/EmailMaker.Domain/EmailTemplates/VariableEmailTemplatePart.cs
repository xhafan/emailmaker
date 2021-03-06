﻿using EmailMaker.Domain.EmailTemplates.VariableTypes;

namespace EmailMaker.Domain.EmailTemplates
{
    public class VariableEmailTemplatePart : EmailTemplatePart
    {
        public virtual VariableType VariableType { get; protected set; }
        public virtual string Value { get; protected set; }

        protected VariableEmailTemplatePart() {}

        public VariableEmailTemplatePart(string value)
        {
            _SetValue(value);
            VariableType = VariableType.InputText;
        }

        public virtual void SetValue(string value)
        {
            _SetValue(value);
        }

        private void _SetValue(string value)
        {
            Value = value ?? string.Empty;
        }

    }
}