namespace ShoppingApi.OutputModel
{
    public static class GenerateRandmVAlues
    {
         public static string RAndomNumber()
        {
            int output;
            Random random = new Random();
            var numbers = "1234567890";
            var numberOfLength = 4;
            var codeChars = new char[numberOfLength];

            for (var i = 0; i < numberOfLength; i++ )
            {
                var Items = random.Next(numbers.Length - 1);

                codeChars[i] = numbers[Items];
            }

            var code = new string(codeChars);
            return code;
        }
    }
}
