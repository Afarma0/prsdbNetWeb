namespace prsdbNetWeb.Models
{
    public class RequestCreate
    {
        //4 strings 1 int 1 dateonly

        public int UserId { get; set; }

        public string Description { get; set; }
        public string Justification { get; set; }
  
        public string DeliveryMode { get; set; }

        public DateOnly DateNeeded { get; set;  }

    }
}
