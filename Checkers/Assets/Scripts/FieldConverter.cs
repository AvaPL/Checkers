public class FieldConverter
{
    public static int alphabeticToNumeric(char character)
    {
        return (int) char.GetNumericValue((char) (character - ('A' - '1')));
    }
}
