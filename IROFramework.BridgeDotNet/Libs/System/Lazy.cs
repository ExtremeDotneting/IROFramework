namespace System
{
     public class Lazy<T>
     {
        T _value;
        Func<T> _valueFactory;

        public bool IsValueCreated { get; private set; }

        public Lazy(Func<T> valueFactory, bool isThreadSafe=true)          
        {
            if (valueFactory == null)
                throw new ArgumentNullException(nameof(valueFactory), "Lazy<T> exception.");
            _valueFactory = valueFactory;
        }

        public override string ToString()
        {
            return Value.ToString();
        } 
 
        public T Value
        {
            get
            {
                if (!IsValueCreated)
                {
                    _value = _valueFactory();
                }
                return _value;
            }
        }
        
    }
}
