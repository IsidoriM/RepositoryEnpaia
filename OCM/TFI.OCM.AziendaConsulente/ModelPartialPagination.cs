using System.Collections;

namespace OCM.TFI.OCM.AziendaConsulente
{
    public class ModelPartialPagination
    {
        public IEnumerable Lista { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public int TotalPage { get; set; }
        public string Controller { get; set; }
        public string EndPoint { get; set; }
        public bool IsPartial { get; set; }
        public RefreshPartial Partial { get; set; } = new RefreshPartial();

        public ModelPartialPagination(IEnumerable lista, int pageSize, int pageNumber, int totalPage, string controller,
            string endPoint, bool isPartial, string idPartial, string idForm)
        {
            Lista = lista;
            PageSize = pageSize;
            PageNumber = pageNumber;
            TotalPage = totalPage;
            Controller = controller;
            EndPoint = endPoint;
            IsPartial = isPartial;
            Partial.IdPartial = idPartial;
            Partial.IdForm = idForm;
        }
    }

    public class RefreshPartial
    {
        public string IdPartial { get; set; }
        public string IdForm { get; set; }
    }
}

