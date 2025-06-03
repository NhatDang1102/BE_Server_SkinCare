//dto request cho blog
using Contract.DTOs;

public class CreateUpdateBlogDto
{
    public string Title { get; set; }
    public string Content { get; set; }
    public Guid? ProductId { get; set; }
    public string ExternalProductLink { get; set; }
}

//dto response blog
public class BlogDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public Guid? ProductId { get; set; }
    public string ExternalProductLink { get; set; }
    public ProductDto Product { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

// Contract/DTOs/CreateUpdateBlogCommentDto.cs
public class CreateUpdateBlogCommentDto
{
    public Guid BlogId { get; set; }
    public string CommentText { get; set; }
}

// Contract/DTOs/BlogCommentDto.cs
public class BlogCommentDto
{
    public Guid Id { get; set; }
    public Guid BlogId { get; set; }
    public Guid UserId { get; set; }
    public string CommentText { get; set; }
    public DateTime? CreatedAt { get; set; }

    // Thông tin user comment
    public Guid? User_Id { get; set; }
    public string User_Name { get; set; }
    public string User_Avatar { get; set; }
}
