namespace TopGear.Application.DTOs.EmailDTO;

public class SendEmailDTO
{
    public List<string> Recipients { get; set; } = [];
    public string Subject { get; set; } = null!;
    public string Body { get;set;  } = null!;
    public bool IsHtml { get; set; } = false;
}
