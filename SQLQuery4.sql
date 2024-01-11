Select * From CommentLikes
Select * From ReportComments
Select * From ReportPosts
Select * From Comments
Select * From PostSuggestions
Select * From Users
Select Content From Posts

Delete From CommentLikes
Delete From ReportComments
Delete From ReportPosts
Delete From CommentAttachments
Delete From Comments
Delete From Attachments
Delete From PostSuggestions
Delete From Posts
Delete From Users Where UserId = 18

UPDATE Users
SET IsEmailConfirmed = 1
WHERE UserId = 8;
