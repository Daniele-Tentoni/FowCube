namespace FowCube.Utils
{
    public static class Consts
    {
        public static readonly string CREATECARDMESSAGE = "CreateCardToCollection";
        public static readonly string ADDCARDMESSAGE = "AddCardToCollection";

        public static string GetCardsPageTitle(string coll_name) => $"Cards of {coll_name}";
    }
}
