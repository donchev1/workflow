namespace Organiser.Data.EnumType
{
    public abstract class TypeSafeEnum
    {
        private readonly string Name;

        private readonly int Value;

        public TypeSafeEnum(string name, int value)
        {
            Name = name;
            Value = value;
        }

        public override string ToString()
        {
            return this.Name;
        }

        public static implicit operator string(TypeSafeEnum num)
        {
            return num.Name;
        }

        public static implicit operator int(TypeSafeEnum num)
        {
            return num.Value;
        }
    }
}
