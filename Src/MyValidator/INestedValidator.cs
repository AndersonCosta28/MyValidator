﻿namespace MyValidator;
internal interface INestedValidator
{
    internal List<ValidationResult> Validate(object instance);
}
