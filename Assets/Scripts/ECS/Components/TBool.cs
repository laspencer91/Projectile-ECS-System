namespace ECS.Components
{
    public struct TBool
    {
        private readonly byte _value;
        public TBool(bool value) { _value = (byte)(value ? 1 : 0); }
        public static implicit operator TBool(bool value) { return new TBool(value); }
        public static implicit operator bool(TBool value) { return value._value != 0; }
    }
}