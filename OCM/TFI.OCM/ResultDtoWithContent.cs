using TFI.OCM;

namespace OCM.TFI.OCM;

public class ResultDtoWithContent<T> : ResultDto
{
    public T Content { get; set; }
    public ResultDtoWithContent(bool result) : base (result)
    {
    }
    public ResultDtoWithContent(T content) : base(true)
    {
        Content = content;
    }
    public ResultDtoWithContent(bool isSuccessfull, T content) : base(isSuccessfull)
    {
        Content = content;
    }
    public ResultDtoWithContent(bool isSuccessfull, string message) : base(isSuccessfull, message) { }
    public ResultDtoWithContent(bool isSuccessfull, string message, T content) : base(isSuccessfull, message)
    {
        Content = content;
    }
}