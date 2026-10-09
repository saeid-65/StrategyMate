using System.Text.Json;
using System.Text.Json.Serialization;

namespace StrategyMate.Helpers
{
    /// <summary>
    /// JSON converter that converts floating-point numbers to <see cref="long"/> (with rounding).
    /// </summary>
    public class DoubleToLongConverter : JsonConverter<long>
    {
        public override long Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                // If the value is already an integer, return it directly.
                if (reader.TryGetInt64(out long longValue))
                {
                    return longValue;
                }

                // Otherwise, read it as a double and round it.
                if (reader.TryGetDouble(out double doubleValue))
                {
                    return (long)Math.Round(doubleValue);
                }
            }

            throw new JsonException($"Cannot convert value to long: {reader.TokenType}");
        }

        public override void Write(Utf8JsonWriter writer, long value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value);
        }
    }
}
