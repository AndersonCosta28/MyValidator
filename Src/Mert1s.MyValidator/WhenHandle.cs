namespace Mert1s.MyValidator;

public abstract partial class ValidatorBuilder<T>
{
    protected sealed class WhenHandle
    {
        private readonly ValidatorBuilder<T> _parent;
        private readonly List<IValidationRule<T>> _wrappers;

        internal WhenHandle(ValidatorBuilder<T> parent, List<IValidationRule<T>> wrappers)
        {
            _parent = parent;
            _wrappers = wrappers;
        }

        public ValidatorBuilder<T> SetCascadeMode(CascadeMode mode)
        {
            foreach (var w in _wrappers)
            {
                w.CascadeMode = mode;
            }

            return _parent;
        }
    }
}