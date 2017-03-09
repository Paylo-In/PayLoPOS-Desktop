
namespace PayLoPOS.Model
{
    class Global
    {
        public static UserData currentUser { get; set; }
        public static NewBill currentBill { get; set; } = new NewBill();
        public static bool isLogin { get; set; } = false;

        public static void updateBill(string reference, double amount, string mobile, string email, string name)
        {
            currentBill.amount = amount;
            currentBill.mobile = mobile;
            currentBill.email = email;
            currentBill.reference = reference;
            currentBill.name = name;
        }


    }
}
