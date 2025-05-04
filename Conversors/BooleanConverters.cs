// Conversors/BooleanConverters.cs (crie este arquivo)
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DentalClinicAPI.Conversors
{
    public class BoolToIntConverter : ValueConverter<bool, int>
    {
        public BoolToIntConverter()
            : base(
                v => v ? 1 : 0,
                v => v == 1)
        {
        }
    }

    public class NullableBoolToIntConverter : ValueConverter<bool?, int?>
    {
        public NullableBoolToIntConverter()
            : base(
                v => v.HasValue ? (v.Value ? 1 : 0) : null,
                v => v.HasValue ? (v == 1) : null)
        {
        }
    }
}