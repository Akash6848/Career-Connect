using CareerConnect.PostService.Enums;

namespace CareerConnect.PostService.Entities;

public class PostFiles
{
    public long Id { get; set; }

    public long PostId { get; set; }
    public Post Post { get; set; } = null!;

    public PostFileType Type { get; set; }
    public string Link { get; set; } = string.Empty;
}
