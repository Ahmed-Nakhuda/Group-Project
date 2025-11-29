namespace SSD_Lab1
{
    public class AppSettings
    {
        public ConnectionStrings ConnectionStrings { get; set; }
        public Secrets Secrets { get; set; }
        public EmailSettings Email { get; set; }
    }

    public class ConnectionStrings
    {
        public string DefaultConnection { get; set; }
    }

    public class Secrets
    {
        public string SupervisorPassword { get; set; }
        public string EmployeePassword { get; set; }
    }

    public class EmailSettings
    {
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}