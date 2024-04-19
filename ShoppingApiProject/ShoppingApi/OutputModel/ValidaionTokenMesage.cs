namespace ShoppingApi.OutputModel
{
    public class ValidaionTokenMesage
    {
        public string UserID { get; set; }
        public Task<string> Token { get; set; }
        public string Message { get; set; }
        public string UniqueCode { get; set; }
    }
}
