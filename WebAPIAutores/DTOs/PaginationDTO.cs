namespace WebAPIAutores.DTOs
{
    public class PaginationDTO
    {

        public int Pagina { get; set; } = 1;
        private int recordsByPage = 10;
        private readonly int countLimitByPage = 50;

        public int RecordsByPage
        {
            get { return recordsByPage; }
            set { recordsByPage = (value > countLimitByPage) ? countLimitByPage : value; }
        }

    }
}
