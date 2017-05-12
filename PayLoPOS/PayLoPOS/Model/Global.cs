
using EzetapApi;

namespace PayLoPOS.Model
{
    class Global
    {
        public static UserData currentUser { get; set; }
        public static NewBill currentBill { get; set; } = new NewBill();
        public static bool isLogin { get; set; } = false;
        public static int networkError { get; set; }

        public static EzeApi api;

        //-- Set application mode Live for true or Test for false
        public static bool isApplicationLive { get; set; } = true;

        public static void updateBill(string reference, double amount, string mobile, string email, string name)
        {
            currentBill.amount = amount;
            currentBill.mobile = mobile;
            currentBill.email = email;
            currentBill.reference = reference;
            currentBill.name = name;
        }

        [System.Runtime.InteropServices.DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int Description, int ReservedValue);

        public static bool isInternetConnected()
        {
            int desc;
            bool isConnected = InternetGetConnectedState(out desc, 0);
            if(isConnected == false)
            {
                networkError = desc;
            }
            return isConnected;
        }

    }
}
